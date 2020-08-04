using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MathCore.Threading.Tasks.Schedulers
{
    /// <summary>Provides a task scheduler that targets the I/O ThreadPool.</summary>
    public sealed class IOTaskScheduler : TaskScheduler, IDisposable
    {
        /// <summary>Represents a task queued to the I/O pool.</summary>
        private unsafe class WorkItem
        {
            internal IOTaskScheduler Scheduler;
            internal NativeOverlapped* _pNOlap;
            internal Task Task;

            internal void Callback(uint ErrorCode, uint NumBytes, NativeOverlapped* pNOlap)
            {
                // Execute the task
                Scheduler.TryExecuteTask(Task);

                // Put this item back into the pool for someone else to use
                var pool = Scheduler._AvailableWorkItems;
                if (pool != null) pool.PutObject(this);
                else Overlapped.Free(pNOlap);
            }
        }

        // A pool of available WorkItem instances that can be used to schedule tasks
        private ObjectPool<WorkItem> _AvailableWorkItems;

        /// <summary>Initializes a new instance of the IOTaskScheduler class.</summary>
        public unsafe IOTaskScheduler() =>
            // Configure the object pool of work items
            _AvailableWorkItems = new ObjectPool<WorkItem>(() =>
            {
                var wi = new WorkItem { Scheduler = this };
                wi._pNOlap = new Overlapped().UnsafePack(wi.Callback, null);
                return wi;
            }, new ConcurrentStack<WorkItem>());

        /// <summary>Queues a task to the scheduler for execution on the I/O ThreadPool.</summary>
        /// <param name="task">The Task to queue.</param>
        protected override unsafe void QueueTask(Task task)
        {
            var pool = _AvailableWorkItems;
            if (pool == null) throw new ObjectDisposedException(GetType().Name);
            var wi = pool.GetObject();
            wi.Task = task;
            ThreadPool.UnsafeQueueNativeOverlapped(wi._pNOlap);
        }

        /// <summary>Executes a task on the current thread.</summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="TaskWasPreviouslyQueued">Ignored.</param>
        /// <returns>Whether the task could be executed.</returns>
        protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) => TryExecuteTask(task);

        /// <summary>Disposes of resources used by the scheduler.</summary>
        public unsafe void Dispose()
        {
            var pool = _AvailableWorkItems;
            _AvailableWorkItems = null;
            var work_items = pool.ToArrayAndClear();
            foreach (var item in work_items) Overlapped.Free(item._pNOlap);
            // NOTE: A window exists where some number of NativeOverlapped ptrs could
            // be leaked, if the call to Dispose races with work items completing.
        }

        /// <summary>Gets an enumerable of tasks queued to the scheduler.</summary>
        /// <returns>An enumerable of tasks queued to the scheduler.</returns>
        /// <remarks>This implementation will always return an empty enumerable.</remarks>
        protected override IEnumerable<Task> GetScheduledTasks() => Enumerable.Empty<Task>();
    }
}