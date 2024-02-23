#nullable enable
using System.Collections.Concurrent;
using System.Diagnostics;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers;

/// <summary>Планировщик обеспечивает постановку задач в очередь с приоритетами</summary>
[DebuggerTypeProxy(typeof(QueuedTaskSchedulerDebugView))]
[DebuggerDisplay("Id={Id}, Queues={DebugQueueCount}, ScheduledTasks = {DebugTaskCount}")]
public sealed class QueuedTaskScheduler : TaskScheduler, IDisposable
{
    /// <summary>Отладочное представление</summary>
    /// <remarks>Инициализация отладочного представления для планировщика</remarks>
    /// <param name="scheduler">Рассматриваемый планировщик</param>
    private class QueuedTaskSchedulerDebugView(QueuedTaskScheduler scheduler)
    {
        /// <summary>The scheduler.</summary>
        private readonly QueuedTaskScheduler _Scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));

        /// <summary>Извлечение всех задач, запланированных непосредственно в планировщике</summary>
        public IEnumerable<Task> ScheduledTasks
        {
            get
            {
                var tasks = _Scheduler._TargetScheduler is null
                    ? (IEnumerable<Task>)_Scheduler._BlockingTaskQueue! 
                    : _Scheduler._NonThreadSafeTaskQueue!;
                return tasks.Where(t => t != null).ToArray();
            }
        }

        /// <summary>Все очереди задач</summary>
        public IEnumerable<TaskScheduler> Queues => _Scheduler._QueueGroups.SelectMany(group => group.Value);
    }

    /// <summary>
    /// Отсортированный список циклических списков.
    /// Задачи с малыми приоритетами являются предпочтительными.
    /// Группы приоритетов являются циклическими в пределах одного уровня приоритета.
    /// </summary>
    private readonly SortedList<int, QueueGroup> _QueueGroups = [];

    /// <summary>Система отмены задач в случае вызова метода <see cref="Dispose"/></summary>
    private readonly CancellationTokenSource _DisposeCancellation = new();

    /// <summary>
    /// Максимально допустимый уровень конкурентности для текущего планировщика.
    /// Если используются вручную создаваемые потоки, то данное поле отображает число создаваемых потоков.
    /// </summary>
    private readonly int _ConcurrencyLevel;

    /// <summary>Whether we're processing tasks on the current thread.</summary>
    private static readonly ThreadLocal<bool> __TaskProcessingThread = new();

    // ***
    // *** For when using a target scheduler
    // ***

    /// <summary>The scheduler onto which actual work is scheduled.</summary>
    private readonly TaskScheduler? _TargetScheduler;

    /// <summary>The queue of tasks to process when using an underlying target scheduler.</summary>
    private readonly Queue<Task?>? _NonThreadSafeTaskQueue;

    /// <summary>The number of Tasks that have been queued or that are running whiel using an underlying scheduler.</summary>
    private int _DelegatesQueuedOrRunning;

    // ***
    // *** For when using our own threads
    // ***

    /// <summary>The collection of tasks to be executed on our custom threads.</summary>
    private readonly BlockingCollection<Task?>? _BlockingTaskQueue;

    // ***

    /// <summary>Initializes the scheduler.</summary>
    public QueuedTaskScheduler() : this(Default) { }

    /// <summary>Initializes the scheduler.</summary>
    /// <param name="TargetScheduler">The target underlying scheduler onto which this sceduler's work is queued.</param>
    /// <param name="MaxConcurrencyLevel">The maximum degree of concurrency allowed for this scheduler's work.</param>
    public QueuedTaskScheduler(TaskScheduler TargetScheduler, int MaxConcurrencyLevel = 0)
    {
        // Validate arguments
        if (MaxConcurrencyLevel < 0) throw new ArgumentOutOfRangeException(nameof(MaxConcurrencyLevel));

        // Initialize only those fields relevant to use an underlying scheduler.  We don't
        // initialize the fields relevant to using our own custom threads.
        _TargetScheduler        = TargetScheduler ?? throw new ArgumentNullException(nameof(TargetScheduler));
        _NonThreadSafeTaskQueue = [];

        // If 0, use the number of logical processors.  But make sure whatever value we pick
        // is not greater than the degree of parallelism allowed by the underlying scheduler.
        _ConcurrencyLevel = MaxConcurrencyLevel != 0 ? MaxConcurrencyLevel : Environment.ProcessorCount;
        if (TargetScheduler.MaximumConcurrencyLevel > 0 && TargetScheduler.MaximumConcurrencyLevel < _ConcurrencyLevel)
            _ConcurrencyLevel = TargetScheduler.MaximumConcurrencyLevel;
    }

    /// <summary>Initializes the scheduler.</summary>
    /// <param name="ThreadCount">The number of threads to create and use for processing work items.</param>
    public QueuedTaskScheduler(int ThreadCount) : this(ThreadCount, string.Empty) { }

    /// <summary>Initializes the scheduler.</summary>
    /// <param name="ThreadCount">The number of threads to create and use for processing work items.</param>
    /// <param name="ThreadName">The name to use for each of the created threads.</param>
    /// <param name="UseForegroundThreads">A Boolean value that indicates whether to use foreground threads instead of background.</param>
    /// <param name="ThreadPriority">The priority to assign to each thread.</param>
    /// <param name="ThreadApartmentState">The apartment state to use for each thread.</param>
    /// <param name="ThreadMaxStackSize">The stack size to use for each thread.</param>
    /// <param name="ThreadInit">An initialization routine to run on each thread.</param>
    /// <param name="ThreadFinally">A finalization routine to run on each thread.</param>
    public QueuedTaskScheduler(
        int ThreadCount,
        string ThreadName = "",
        bool UseForegroundThreads = false,
        ThreadPriority ThreadPriority = ThreadPriority.Normal,
        ApartmentState ThreadApartmentState = ApartmentState.MTA,
        int ThreadMaxStackSize = 0,
        Action? ThreadInit = null,
        Action? ThreadFinally = null)
    {
        _ConcurrencyLevel = ThreadCount switch
        {
            > 0 => ThreadCount,
            0   => Environment.ProcessorCount,
            _   => throw new ArgumentOutOfRangeException(nameof(ThreadCount))
        };

        //// Validates arguments (some validation is left up to the Thread type itself).
        //// If the thread count is 0, default to the number of logical processors.
        //if (ThreadCount < 0) throw new ArgumentOutOfRangeException(nameof(ThreadCount));

        //_ConcurrencyLevel = ThreadCount == 0 
        //    ? Environment.ProcessorCount 
        //    : ThreadCount;

        // Initialize the queue used for storing tasks
        _BlockingTaskQueue = [];

        // Create all of the threads
        var threads = new Thread[ThreadCount];
        for (var i = 0; i < ThreadCount; i++)
        {
            threads[i] = new(() => ThreadBasedDispatchLoop(ThreadInit, ThreadFinally), ThreadMaxStackSize)
            {
                Priority     = ThreadPriority,
                IsBackground = !UseForegroundThreads,
            };
            if (ThreadName != null) threads[i].Name = $"{ThreadName} ({i})";
#pragma warning disable CA1416
            threads[i].SetApartmentState(ThreadApartmentState);
#pragma warning restore CA1416
        }

        // Start all of the threads
        foreach (var thread in threads)
            thread.Start();
    }

    /// <summary>The dispatch loop run by all threads in this scheduler.</summary>
    /// <param name="ThreadInit">An initialization routine to run when the thread begins.</param>
    /// <param name="ThreadFinally">A finalization routine to run before the thread ends.</param>
    private void ThreadBasedDispatchLoop(Action? ThreadInit, Action? ThreadFinally)
    {
        __TaskProcessingThread.Value = true;
        ThreadInit?.Invoke();
        try
        {
            // If a thread abort occurs, we'll try to reset it and continue running.
            while (true)
                try
                {
                    // For each task queued to the scheduler, try to execute it.
                    foreach (var task in _BlockingTaskQueue!.GetConsumingEnumerable(_DisposeCancellation.Token))
                        // If the task is not null, that means it was queued to this scheduler directly.
                        // Run it.
                        if (task != null)
                            TryExecuteTask(task);
                        // If the task is null, that means it's just a placeholder for a task
                        // queued to one of the subschedulers.  Find the next task based on
                        // priority and fairness and run it.
                        else
                        {
                            // Find the next task based on our ordering rules...
                            Task?                    target_task;
                            QueuedTaskSchedulerQueue queue_for_target_task;
                            lock (_QueueGroups)
                                FindNextTask_NeedsLock(out target_task, out queue_for_target_task);

                            // ... and if we found one, run it
                            if (target_task != null)
                                queue_for_target_task.ExecuteTask(target_task);
                        }
                }
                catch (ThreadAbortException)
                {
                    // Если вызвана отмена работы потока в ходе завершения работы системы, или выгрузки домена
                    // If we received a thread abort, and that thread abort was due to shutting down
                    // or unloading, let it pass through.  Otherwise, reset the abort so we can
                    // continue processing work items.
#if NET5_0_OR_GREATER
                    throw;
#else
                    if (!Environment.HasShutdownStarted && !AppDomain.CurrentDomain.IsFinalizingForUnload())
                        Thread.ResetAbort();
#endif
                }
        }
        catch (OperationCanceledException) { }
        finally
        {
            // Run a cleanup routine if there was one
            ThreadFinally?.Invoke();
            __TaskProcessingThread.Value = false;
        }
    }

    /// <summary>Gets the number of queues currently activated.</summary>
    private int DebugQueueCount => _QueueGroups.Sum(group => group.Value.Count);

    /// <summary>Gets the number of tasks currently scheduled.</summary>
    private int DebugTaskCount => (_TargetScheduler is null ? (IEnumerable<Task?>?)_BlockingTaskQueue : _NonThreadSafeTaskQueue).Count(t => t != null);

    /// <summary>Find the next task that should be executed, based on priorities and fairness and the like.</summary>
    /// <param name="TargetTask">The found task, or null if none was found.</param>
    /// <param name="QueueForTargetTask">
    /// The scheduler associated with the found task.  Due to security checks inside of TPL,  
    /// this scheduler needs to be used to execute that task.
    /// </param>
    private void FindNextTask_NeedsLock(out Task? TargetTask, out QueuedTaskSchedulerQueue? QueueForTargetTask)
    {
        TargetTask         = null;
        QueueForTargetTask = null;

        // Look through each of our queue groups in sorted order.
        // This ordering is based on the priority of the queues.
        foreach (var (_, queues) in _QueueGroups)
            // Within each group, iterate through the queues in a round-robin
            // fashion.  Every time we iterate again and successfully find a task, 
            // we'll start in the next location in the group.
            foreach (var i in queues.CreateSearchOrder())
            {
                QueueForTargetTask = queues[i];
                var items = QueueForTargetTask.WorkItems;
                if (items.Count == 0) continue;
                TargetTask = items.Dequeue();
                if (QueueForTargetTask.Disposed && items.Count == 0)
                    RemoveQueue_NeedsLock(QueueForTargetTask);
                queues.NextQueueIndex = (queues.NextQueueIndex + 1) % queues.Count;
                return;
            }
    }

    /// <summary>Queues a task to the scheduler.</summary>
    /// <param name="task">The task to be queued.</param>
    protected override void QueueTask(Task? task)
    {
        // If we've been disposed, no one should be queueing
        if (_DisposeCancellation.IsCancellationRequested)
            throw new ObjectDisposedException(GetType().Name);

        // If the target scheduler is null (meaning we're using our own threads),
        // add the task to the blocking queue
        if (_TargetScheduler == null)
            _BlockingTaskQueue!.Add(task);
        // Otherwise, add the task to the non-blocking queue,
        // and if there isn't already an executing processing task,
        // start one up
        else
        {
            // Queue the task and check whether we should launch a processing
            // task (noting it if we do, so that other threads don't result
            // in queueing up too many).
            var launch_task = false;
            lock (_NonThreadSafeTaskQueue!)
            {
                _NonThreadSafeTaskQueue.Enqueue(task);
                if (_DelegatesQueuedOrRunning < _ConcurrencyLevel)
                {
                    _DelegatesQueuedOrRunning++;
                    launch_task = true;
                }
            }

            // If necessary, start processing asynchronously
            if (launch_task)
                Task.Factory.StartNew(ProcessPrioritizedAndBatchedTasks, CancellationToken.None, TaskCreationOptions.None, _TargetScheduler);
        }
    }

    /// <summary>
    /// Process tasks one at a time in the best order.  
    /// This should be run in a Task generated by QueueTask.
    /// It's been separated out into its own method to show up better in Parallel Tasks.
    /// </summary>
    private void ProcessPrioritizedAndBatchedTasks()
    {
        var continue_processing = true;
        while (!_DisposeCancellation.IsCancellationRequested && continue_processing)
            try
            {
                // Note that we're processing tasks on this thread
                __TaskProcessingThread.Value = true;

                // Until there are no more tasks to process
                while (!_DisposeCancellation.IsCancellationRequested)
                {
                    // Try to get the next task.  If there aren't any more, we're done.
                    Task? target_task;
                    lock (_NonThreadSafeTaskQueue!)
                    {
                        if (_NonThreadSafeTaskQueue.Count == 0) break;
                        target_task = _NonThreadSafeTaskQueue.Dequeue();
                    }

                    // If the task is null, it's a placeholder for a task in the round-robin queues.
                    // Find the next one that should be processed.
                    QueuedTaskSchedulerQueue? queue_for_target_task = null;
                    if (target_task is null)
                        lock (_QueueGroups)
                            FindNextTask_NeedsLock(out target_task, out queue_for_target_task);

                    // Now if we finally have a task, run it.  If the task
                    // was associated with one of the round-robin schedulers, we need to use it
                    // as a thunk to execute its task.
                    if (target_task is null) continue;
                    if (queue_for_target_task is null)
                        TryExecuteTask(target_task);
                    else
                        queue_for_target_task.ExecuteTask(target_task);
                }
            }
            finally
            {
                // Now that we think we're done, verify that there really is
                // no more work to do.  If there's not, highlight
                // that we're now less parallel than we were a moment ago.
                lock (_NonThreadSafeTaskQueue!)
                    if (_NonThreadSafeTaskQueue.Count == 0)
                    {
                        _DelegatesQueuedOrRunning--;
                        continue_processing          = false;
                        __TaskProcessingThread.Value = false;
                    }
            }
    }

    /// <summary>Notifies the pool that there's a new item to be executed in one of the round-robin queues.</summary>
    private void NotifyNewWorkItem() => QueueTask(null);

    /// <summary>Tries to execute a task synchronously on the current thread.</summary>
    /// <param name="task">The task to execute.</param>
    /// <param name="TaskWasPreviouslyQueued">Whether the task was previously queued.</param>
    /// <returns>true if the task was executed; otherwise, false.</returns>
    protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) =>
        // If we're already running tasks on this threads, enable inlining
        __TaskProcessingThread.Value && TryExecuteTask(task);

    /// <summary>Gets the tasks scheduled to this scheduler.</summary>
    /// <returns>An enumerable of all tasks queued to this scheduler.</returns>
    /// <remarks>This does not include the tasks on sub-schedulers.  Those will be retrieved by the debugger separately.</remarks>
    protected override IEnumerable<Task> GetScheduledTasks()
    {
        // If we're running on our own threads, get the tasks from the blocking queue...
        if (_TargetScheduler is null)
            // Get all of the tasks, filtering out nulls, which are just placeholders
            // for tasks in other sub-schedulers
            return _BlockingTaskQueue!.Where(t => t != null).ToArray()!;
        // otherwise get them from the non-blocking queue...

        return _NonThreadSafeTaskQueue!.Where(t => t != null).ToArray()!;
    }

    /// <summary>Gets the maximum concurrency level to use when processing tasks.</summary>
    public override int MaximumConcurrencyLevel => _ConcurrencyLevel;

    /// <summary>Initiates shutdown of the scheduler.</summary>
    public void Dispose() => _DisposeCancellation.Cancel();

    /// <summary>Creates and activates a new scheduling queue for this scheduler.</summary>
    /// <returns>The newly created and activated queue at priority 0.</returns>
    public TaskScheduler ActivateNewQueue() => ActivateNewQueue(0);

    /// <summary>Creates and activates a new scheduling queue for this scheduler.</summary>
    /// <param name="priority">The priority level for the new queue.</param>
    /// <returns>The newly created and activated queue at the specified priority.</returns>
    public TaskScheduler ActivateNewQueue(int priority)
    {
        // Create the queue
        var created_queue = new QueuedTaskSchedulerQueue(priority, this);

        // Add the queue to the appropriate queue group based on priority
        lock (_QueueGroups)
        {
            if (!_QueueGroups.TryGetValue(priority, out var list))
            {
                list = new();
                _QueueGroups.Add(priority, list);
            }
            list.Add(created_queue);
        }

        // Hand the new queue back
        return created_queue;
    }

    /// <summary>Removes a scheduler from the group.</summary>
    /// <param name="queue">The scheduler to be removed.</param>
    private void RemoveQueue_NeedsLock(QueuedTaskSchedulerQueue queue)
    {
        // Find the group that contains the queue and the queue's index within the group
        var queue_group = _QueueGroups[queue.Priority];
        var index       = queue_group.IndexOf(queue);

        // We're about to remove the queue, so adjust the index of the next
        // round-robin starting location if it'll be affected by the removal
        if (queue_group.NextQueueIndex >= index) queue_group.NextQueueIndex--;

        // Remove it
        queue_group.RemoveAt(index);
    }

    /// <summary>A group of queues a the same priority level.</summary>
    private class QueueGroup : List<QueuedTaskSchedulerQueue>
    {
        /// <summary>The starting index for the next round-robin traversal.</summary>
        public int NextQueueIndex;

        /// <summary>Creates a search order through this group.</summary>
        /// <returns>An enumerable of indices for this group.</returns>
        public IEnumerable<int> CreateSearchOrder()
        {
            for (var i = NextQueueIndex; i < Count; i++) yield return i;
            for (var i = 0; i < NextQueueIndex; i++) yield return i;
        }
    }

    /// <summary>Provides a scheduling queue associatd with a QueuedTaskScheduler.</summary>
    [DebuggerDisplay("QueuePriority = {Priority}, WaitingTasks = {WaitingTasks}")]
    [DebuggerTypeProxy(typeof(QueuedTaskSchedulerQueueDebugView))]
    private sealed class QueuedTaskSchedulerQueue : TaskScheduler, IDisposable
    {
        /// <summary>A debug view for the queue.</summary>
        private sealed class QueuedTaskSchedulerQueueDebugView
        {
            /// <summary>The queue.</summary>
            private readonly QueuedTaskSchedulerQueue _Queue;

            /// <summary>Initializes the debug view.</summary>
            /// <param name="queue">The queue to be debugged.</param>
            public QueuedTaskSchedulerQueueDebugView(QueuedTaskSchedulerQueue queue) => _Queue = queue ?? throw new ArgumentNullException(nameof(queue));

            /// <summary>Gets the priority of this queue in its associated scheduler.</summary>
            public int Priority => _Queue.Priority;

            /// <summary>Gets the ID of this scheduler.</summary>
            public int Id => _Queue.Id;

            /// <summary>Gets all of the tasks scheduled to this queue.</summary>
            public IEnumerable<Task> ScheduledTasks => _Queue.GetScheduledTasks();

            /// <summary>Gets the QueuedTaskScheduler with which this queue is associated.</summary>
            public QueuedTaskScheduler AssociatedScheduler => _Queue._Pool;
        }

        /// <summary>The scheduler with which this pool is associated.</summary>
        private readonly QueuedTaskScheduler _Pool;
        /// <summary>The work items stored in this queue.</summary>
        internal readonly Queue<Task> WorkItems;
        /// <summary>Whether this queue has been disposed.</summary>
        internal bool Disposed;
        /// <summary>Gets the priority for this queue.</summary>
        internal int Priority;

        /// <summary>Initializes the queue.</summary>
        /// <param name="priority">The priority associated with this queue.</param>
        /// <param name="pool">The scheduler with which this queue is associated.</param>
        internal QueuedTaskSchedulerQueue(int priority, QueuedTaskScheduler pool)
        {
            Priority  = priority;
            _Pool     = pool;
            WorkItems = [];
        }

        /// <summary>Gets the number of tasks waiting in this scheduler.</summary>
        internal int WaitingTasks => WorkItems.Count;

        /// <summary>Gets the tasks scheduled to this scheduler.</summary>
        /// <returns>An enumerable of all tasks queued to this scheduler.</returns>
        protected override IEnumerable<Task> GetScheduledTasks() => WorkItems.ToList();

        /// <summary>Queues a task to the scheduler.</summary>
        /// <param name="task">The task to be queued.</param>
        protected override void QueueTask(Task task)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().Name);

            // Queue up the task locally to this queue, and then notify
            // the parent scheduler that there's work available
            lock (_Pool._QueueGroups) WorkItems.Enqueue(task);
            _Pool.NotifyNewWorkItem();
        }

        /// <summary>Tries to execute a task synchronously on the current thread.</summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="TaskWasPreviouslyQueued">Whether the task was previously queued.</param>
        /// <returns>true if the task was executed; otherwise, false.</returns>
        protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) =>
            // If we're using our own threads and if this is being called from one of them,
            // or if we're currently processing another task on this thread, try running it inline.
            __TaskProcessingThread.Value && TryExecuteTask(task);

        /// <summary>Runs the specified ask.</summary>
        /// <param name="task">The task to execute.</param>
        internal void ExecuteTask(Task task) => TryExecuteTask(task);

        /// <summary>Gets the maximum concurrency level to use when processing tasks.</summary>
        public override int MaximumConcurrencyLevel => _Pool.MaximumConcurrencyLevel;

        /// <summary>Signals that the queue should be removed from the scheduler as soon as the queue is empty.</summary>
        public void Dispose()
        {
            if (Disposed) return;
            lock (_Pool._QueueGroups)
                // We only remove the queue if it's empty.  If it's not empty,
                // we still mark it as disposed, and the associated QueuedTaskScheduler
                // will remove the queue when its count hits 0 and its _disposed is true.
                if (WorkItems.Count == 0)
                    _Pool.RemoveQueue_NeedsLock(this);
            Disposed = true;
        }
    }
}