using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathCore.Annotations;

// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers
{
    /// <summary>Provides a task scheduler that runs tasks on the current thread.</summary>
    public sealed class CurrentThreadTaskScheduler : TaskScheduler
    {
        /// <summary>Runs the provided Task synchronously on the current thread.</summary>
        /// <param name="task">The task to be executed.</param>
        protected override void QueueTask(Task task) => TryExecuteTask(task);

        /// <summary>Runs the provided Task synchronously on the current thread.</summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="TaskWasPreviouslyQueued">Whether the Task was previously queued to the scheduler.</param>
        /// <returns>True if the Task was successfully executed; otherwise, false.</returns>
        protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) => TryExecuteTask(task);

        /// <summary>Gets the Tasks currently scheduled to this scheduler.</summary>
        /// <returns>An empty enumerable, as Tasks are never queued, only executed.</returns>
        [NotNull]
        protected override IEnumerable<Task> GetScheduledTasks() => Enumerable.Empty<Task>();

        /// <summary>Gets the maximum degree of parallelism for this scheduler.</summary>
        public override int MaximumConcurrencyLevel => 1;
    }
}
