using System.Runtime.CompilerServices;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
#pragma warning disable CA1822 // Mark members as static
    public class TaskSchedulerAwaiter : INotifyCompletion
    {
        private readonly TaskScheduler _Scheduler;

        public bool IsCompleted => false;

        public TaskSchedulerAwaiter(TaskScheduler Scheduler) => _Scheduler = Scheduler;

        public void OnCompleted([NotNull] Action continuation) => Task.Factory.StartNew(continuation, CancellationToken.None, TaskCreationOptions.None, _Scheduler);

        public void GetResult() { }

        [NotNull] public TaskSchedulerAwaiter GetAwaiter() => this;
    }
#pragma warning restore CA1822 // Mark members as static
}