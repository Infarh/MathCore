using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers
{
    /// <summary>Provides a task scheduler that supports reprioritizing previously queued tasks.</summary>
    public sealed class ReprioritizableTaskScheduler : TaskScheduler
    {
        private readonly LinkedList<Task> _Tasks = new(); // protected by lock(_tasks)

        /// <summary>Queues a task to the scheduler.</summary>
        /// <param name="task">The task to be queued.</param>
        protected override void QueueTask(Task task)
        {
            // Store the task, and notify the ThreadPool of work to be processed
            lock (_Tasks) _Tasks.AddLast(task);
            ThreadPool.UnsafeQueueUserWorkItem(ProcessNextQueuedItem, null);
        }

        /// <summary>Reprioritizes a previously queued task to the front of the queue.</summary>
        /// <param name="task">The task to be reprioritized.</param>
        /// <returns>Whether the task could be found and moved to the front of the queue.</returns>
        public bool Prioritize(Task task)
        {
            lock (_Tasks)
            {
                var node = _Tasks.Find(task);
                if (node is null) return false;
                _Tasks.Remove(node);
                _Tasks.AddFirst(node);
                return true;
            }
        }

        /// <summary>Reprioritizes a previously queued task to the back of the queue.</summary>
        /// <param name="task">The task to be reprioritized.</param>
        /// <returns>Whether the task could be found and moved to the back of the queue.</returns>
        public bool Deprioritize(Task task)
        {
            lock (_Tasks)
            {
                var node = _Tasks.Find(task);
                if (node is null) return false;
                _Tasks.Remove(node);
                _Tasks.AddLast(node);
                return true;
            }
        }

        /// <summary>Removes a previously queued item from the scheduler.</summary>
        /// <param name="task">The task to be removed.</param>
        /// <returns>Whether the task could be removed from the scheduler.</returns>
        protected override bool TryDequeue(Task task)
        {
            lock (_Tasks) return _Tasks.Remove(task);
        }

        /// <summary>Picks up and executes the next item in the queue.</summary>
        /// <param name="Ignored">Ignored.</param>
        private void ProcessNextQueuedItem(object Ignored)
        {
            Task task;
            lock (_Tasks)
            {
                if (_Tasks.Count == 0) return;
                task = _Tasks.First.Value;
                _Tasks.RemoveFirst();
            }
            TryExecuteTask(task);
        }

        /// <summary>Executes the specified task inline.</summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="TaskWasPreviouslyQueued">Whether the task was previously queued.</param>
        /// <returns>Whether the task could be executed inline.</returns>
        protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) => TryExecuteTask(task);

        /// <summary>Gets all of the tasks currently queued to the scheduler.</summary>
        /// <returns>An enumerable of the tasks currently queued to the scheduler.</returns>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            var lock_taken = false;
            try
            {
                Monitor.TryEnter(_Tasks, ref lock_taken);
                return lock_taken ? _Tasks.ToArray() : throw new NotSupportedException();
            }
            finally
            {
                if (lock_taken) Monitor.Exit(_Tasks);
            }
        }
    }
}
