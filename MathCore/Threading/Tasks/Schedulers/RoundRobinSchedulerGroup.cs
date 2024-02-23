using System.Collections.ObjectModel;

namespace MathCore.Threading.Tasks.Schedulers;

/// <summary>Enables the creation of a group of schedulers that support round-robin scheduling for fairness.</summary>
public sealed class RoundRobinSchedulerGroup
{
    private readonly List<RoundRobinTaskSchedulerQueue> _Queues = [];

    private int _NextQueue;

    /// <summary>Creates a new scheduler as part of this group.</summary>
    /// <returns>The new scheduler.</returns>
    public TaskScheduler CreateScheduler()
    {
        var created_queue = new RoundRobinTaskSchedulerQueue(this);
        lock (_Queues) 
            _Queues.Add(created_queue);
        return created_queue;
    }

    /// <summary>Gets a collection of all schedulers in this group.</summary>
    public ReadOnlyCollection<TaskScheduler> Schedulers
    {
        get { lock (_Queues) return new(_Queues.Cast<TaskScheduler>().ToArray()); }
    }

    /// <summary>Removes a scheduler from the group.</summary>
    /// <param name="queue">The scheduler to be removed.</param>
    private void RemoveQueue_NeedsLock(RoundRobinTaskSchedulerQueue queue)
    {
        var index = _Queues.IndexOf(queue);
        if (_NextQueue >= index) _NextQueue--;
        _Queues.RemoveAt(index);
    }

    /// <summary>Notifies the ThreadPool that there's a new item to be executed.</summary>
    private void NotifyNewWorkItem() =>
        // Queue a processing delegate to the ThreadPool
        ThreadPool.UnsafeQueueUserWorkItem(_ =>
        {
            Task                         target_task           = null;
            RoundRobinTaskSchedulerQueue queue_for_target_task = null;
            lock (_Queues)
            {
                // Determine the order in which we'll search the schedulers for work
                var search_order = Enumerable.Range(_NextQueue, _Queues.Count - _NextQueue).Concat(Enumerable.Range(0, _NextQueue));

                // Look for the next item to process
                foreach (var i in search_order)
                {
                    queue_for_target_task = _Queues[i];
                    var items = queue_for_target_task.WorkItems;
                    if (items.Count == 0) continue;
                    target_task = items.Dequeue();
                    _NextQueue  = i;
                    if (queue_for_target_task.Disposed && items.Count == 0) 
                        RemoveQueue_NeedsLock(queue_for_target_task);
                    break;
                }
                _NextQueue = (_NextQueue + 1) % _Queues.Count;
            }

            // If we found an item, run it
            if (target_task != null) queue_for_target_task.RunQueuedTask(target_task);
        }, null);

    /// <summary>A scheduler that participates in round-robin scheduling.</summary>
    private sealed class RoundRobinTaskSchedulerQueue : TaskScheduler, IDisposable
    {
        internal RoundRobinTaskSchedulerQueue(RoundRobinSchedulerGroup pool) => _Pool = pool;

        private readonly RoundRobinSchedulerGroup _Pool;
        internal readonly Queue<Task> WorkItems = [];
        internal bool Disposed;

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            object obj        = _Pool._Queues;
            var    lock_taken = false;
            try
            {
                Monitor.TryEnter(obj, ref lock_taken);
                return lock_taken ? WorkItems.ToArray() : throw new NotSupportedException();
            }
            finally
            {
                if (lock_taken) Monitor.Exit(obj);
            }
        }

        protected override void QueueTask(Task task)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().Name);
            lock (_Pool._Queues) WorkItems.Enqueue(task);
            _Pool.NotifyNewWorkItem();
        }

        protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) => TryExecuteTask(task);

        internal void RunQueuedTask(Task task) => TryExecuteTask(task);

        void IDisposable.Dispose()
        {
            if (Disposed) return;
            lock (_Pool._Queues)
            {
                if (WorkItems.Count == 0) 
                    _Pool.RemoveQueue_NeedsLock(this);
                Disposed = true;
            }
        }
    }
}