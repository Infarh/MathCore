using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers
{
    /// <summary>Provides a task scheduler that targets a specific SynchronizationContext.</summary>
    public sealed class SynchronizationContextTaskScheduler : TaskScheduler
    {
        /// <summary>The queue of tasks to execute, maintained for debugging purposes.</summary>
        private readonly ConcurrentQueue<Task> _Tasks;
        /// <summary>The target context under which to execute the queued tasks.</summary>
        private readonly SynchronizationContext _Context;

        /// <summary>Initializes an instance of the SynchronizationContextTaskScheduler class.</summary>
        public SynchronizationContextTaskScheduler() : this(SynchronizationContext.Current) { }

        /// <summary>
        /// Initializes an instance of the SynchronizationContextTaskScheduler class
        /// with the specified SynchronizationContext.
        /// </summary>
        /// <param name="context">The SynchronizationContext under which to execute tasks.</param>
        public SynchronizationContextTaskScheduler(SynchronizationContext context)
        {
            _Context = context ?? throw new ArgumentNullException(nameof(context));
            _Tasks = new ConcurrentQueue<Task>();
        }

        /// <summary>Queues a task to the scheduler for execution on the I/O ThreadPool.</summary>
        /// <param name="task">The Task to queue.</param>
        protected override void QueueTask(Task task)
        {
            _Tasks.Enqueue(task);
            _Context.Post(delegate
            {
                if (_Tasks.TryDequeue(out var next_task)) TryExecuteTask(next_task);
            }, null);
        }

        /// <summary>Tries to execute a task on the current thread.</summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="TaskWasPreviouslyQueued">Ignored.</param>
        /// <returns>Whether the task could be executed.</returns>
        protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) => _Context == SynchronizationContext.Current && TryExecuteTask(task);

        /// <summary>Gets an enumerable of tasks queued to the scheduler.</summary>
        /// <returns>An enumerable of tasks queued to the scheduler.</returns>
        protected override IEnumerable<Task> GetScheduledTasks() => _Tasks.ToArray();

        /// <summary>Gets the maximum concurrency level supported by this scheduler.</summary>
        public override int MaximumConcurrencyLevel => 1;
    }
}
