using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MathCore.Threading.Tasks.Schedulers
{
    /// <summary>Provides a work-stealing scheduler.</summary>
    public class WorkStealingTaskScheduler : TaskScheduler, IDisposable
    {
        private readonly int _ConcurrencyLevel;
        private readonly Queue<Task> _Queue = new Queue<Task>();
        private WorkStealingQueue<Task>[] _WsQueues = new WorkStealingQueue<Task>[Environment.ProcessorCount];
        private readonly Lazy<Thread[]> _Threads;
        private int _ThreadsWaiting;
        private bool _Shutdown;
        [ThreadStatic]
        private static WorkStealingQueue<Task> __Wsq;

        /// <summary>Initializes a new instance of the WorkStealingTaskScheduler class.</summary>
        /// <remarks>This constructors defaults to using twice as many threads as there are processors.</remarks>
        public WorkStealingTaskScheduler() : this(Environment.ProcessorCount * 2) { }

        /// <summary>Initializes a new instance of the WorkStealingTaskScheduler class.</summary>
        /// <param name="ConcurrencyLevel">The number of threads to use in the scheduler.</param>
        public WorkStealingTaskScheduler(int ConcurrencyLevel)
        {
            // Store the concurrency level
            if (ConcurrencyLevel <= 0) throw new ArgumentOutOfRangeException(nameof(ConcurrencyLevel));
            _ConcurrencyLevel = ConcurrencyLevel;

            // Set up threads
            _Threads = new Lazy<Thread[]>(() =>
            {
                var threads = new Thread[_ConcurrencyLevel];
                for (var i = 0; i < threads.Length; i++)
                {
                    threads[i] = new Thread(DispatchLoop) { IsBackground = true };
                    threads[i].Start();
                }
                return threads;
            });
        }

        /// <summary>Queues a task to the scheduler.</summary>
        /// <param name="task">The task to be scheduled.</param>
        protected override void QueueTask(Task task)
        {
            // Make sure the pool is started, e.g. that all threads have been created.
            _ = _Threads.Value;

            // If the task is marked as long-running, give it its own dedicated thread
            // rather than queueing it.
            if ((task.CreationOptions & TaskCreationOptions.LongRunning) != 0)
                new Thread(state => TryExecuteTask((Task)state)) { IsBackground = true }.Start(task);
            else
            {
                // Otherwise, insert the work item into a queue, possibly waking a thread.
                // If there's a local queue and the task does not prefer to be in the global queue,
                // add it to the local queue.
                var wsq = __Wsq;
                if (wsq != null && ((task.CreationOptions & TaskCreationOptions.PreferFairness) == 0))
                {
                    // Add to the local queue and notify any waiting threads that work is available.
                    // Races may occur which result in missed event notifications, but they're benign in that
                    // this thread will eventually pick up the work item anyway, as will other threads when another
                    // work item notification is received.
                    wsq.LocalPush(task);
                    if (_ThreadsWaiting == 0) return;
                    // OK to read lock-free.
                    lock (_Queue)
                        Monitor.Pulse(_Queue);
                }
                // Otherwise, add the work item to the global queue
                else
                    lock (_Queue)
                    {
                        _Queue.Enqueue(task);
                        if (_ThreadsWaiting > 0) Monitor.Pulse(_Queue);
                    }
            }
        }

        /// <summary>Executes a task on the current thread.</summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="TaskWasPreviouslyQueued">Ignored.</param>
        /// <returns>Whether the task could be executed.</returns>
        protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) => TryExecuteTask(task);

        // // Optional replacement: Instead of always trying to execute the task (which could
        // // benignly leave a task in the queue that's already been executed), we
        // // can search the current work-stealing queue and remove the task,
        // // executing it inline only if it's found.
        // WorkStealingQueue<Task> wsq = _wsq;
        // return wsq != null && wsq.TryFindAndPop(task) && TryExecuteTask(task);
        /// <summary>Gets the maximum concurrency level supported by this scheduler.</summary>
        public override int MaximumConcurrencyLevel => _ConcurrencyLevel;

        /// <summary>Gets all of the tasks currently scheduled to this scheduler.</summary>
        /// <returns>An enumerable containing all of the scheduled tasks.</returns>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            // Keep track of all of the tasks we find
            var tasks = new List<Task>();

            // Get all of the global tasks.  We use TryEnter so as not to hang
            // a debugger if the lock is held by a frozen thread.
            var lock_taken = false;
            try
            {
                Monitor.TryEnter(_Queue, ref lock_taken);
                if (lock_taken) tasks.AddRange(_Queue.ToArray());
                else throw new NotSupportedException();
            }
            finally
            {
                if (lock_taken) Monitor.Exit(_Queue);
            }

            // Now get all of the tasks from the work-stealing queues
            var queues = _WsQueues;
            for (var i = 0; i < queues.Length; i++)
            {
                var wsq = queues[i];
                if (wsq != null) tasks.AddRange(wsq.ToArray());
            }

            // Return to the debugger all of the collected task instances
            return tasks;
        }

        /// <summary>Adds a work-stealing queue to the set of queues.</summary>
        /// <param name="wsq">The queue to be added.</param>
        private void AddWsq(WorkStealingQueue<Task> wsq)
        {
            lock (_WsQueues)
            {
                // Find the next open slot in the array. If we find one,
                // store the queue and we're done.
                int i;
                for (i = 0; i < _WsQueues.Length; i++)
                    if (_WsQueues[i] is null)
                    {
                        _WsQueues[i] = wsq;
                        return;
                    }

                // We couldn't find an open slot, so double the length 
                // of the array by creating a new one, copying over,
                // and storing the new one. Here, i == _wsQueues.Length.
                var queues = new WorkStealingQueue<Task>[i * 2];
                Array.Copy(_WsQueues, queues, i);
                queues[i] = wsq;
                _WsQueues = queues;
            }
        }

        /// <summary>Remove a work-stealing queue from the set of queues.</summary>
        /// <param name="wsq">The work-stealing queue to remove.</param>
        private void RemoveWsq(WorkStealingQueue<Task> wsq)
        {
            lock (_WsQueues)
                // Find the queue, and if/when we find it, null out its array slot
                for (var i = 0; i < _WsQueues.Length; i++)
                    if (_WsQueues[i] == wsq)
                        _WsQueues[i] = null;
        }

        /// <summary>
        /// The dispatch loop run by each thread in the scheduler.
        /// </summary>
        private void DispatchLoop()
        {
            // Create a new queue for this thread, store it in TLS for later retrieval,
            // and add it to the set of queues for this scheduler.
            var wsq = new WorkStealingQueue<Task>();
            __Wsq = wsq;
            AddWsq(wsq);

            try
            {
                // Until there's no more work to do...
                while (true)
                {
                    Task wi = null;

                    // Search order: (1) local WSQ, (2) global Q, (3) steals from other queues.
                    if (!wsq.LocalPop(ref wi))
                    {
                        // We weren't able to get a task from the local WSQ
                        var searched_for_steals = false;
                        while (true)
                        {
                            lock (_Queue)
                            {
                                // If shutdown was requested, exit the thread.
                                if (_Shutdown)
                                    return;

                                // (2) try the global queue.
                                if (_Queue.Count != 0)
                                {
                                    // We found a work item! Grab it ...
                                    wi = _Queue.Dequeue();
                                    break;
                                }

                                if (searched_for_steals)
                                {
                                    // Note that we're not waiting for work, and then wait
                                    _ThreadsWaiting++;
                                    try { Monitor.Wait(_Queue); } finally { _ThreadsWaiting--; }

                                    // If we were signaled due to shutdown, exit the thread.
                                    if (_Shutdown)
                                        return;

                                    searched_for_steals = false;
                                    continue;
                                }
                            }

                            // (3) try to steal.
                            var ws_queues = _WsQueues;
                            int i;
                            for (i = 0; i < ws_queues.Length; i++)
                            {
                                var q = ws_queues[i];
                                if (q != null && q != wsq && q.TrySteal(ref wi)) break;
                            }

                            if (i != ws_queues.Length) break;

                            searched_for_steals = true;
                        }
                    }

                    // ...and Invoke it.
                    TryExecuteTask(wi);
                }
            }
            finally
            {
                RemoveWsq(wsq);
            }
        }

        /// <summary>Signal the scheduler to shutdown and wait for all threads to finish.</summary>
        public void Dispose()
        {
            _Shutdown = true;
            if (_Queue is null || !_Threads.IsValueCreated) return;
            var threads = _Threads.Value;
            lock (_Queue) Monitor.PulseAll(_Queue);
            foreach (var thread in threads)
                thread.Join();
        }
    }
}