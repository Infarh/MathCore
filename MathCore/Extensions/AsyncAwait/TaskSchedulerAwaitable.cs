#nullable enable
using System.Runtime.CompilerServices;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    public readonly ref struct TaskSchedulerAwaitable
    {
        private readonly TaskScheduler _Scheduler;
        private readonly Task? _Task;

        public TaskSchedulerAwaitable(TaskScheduler Scheduler, Task? Task = null)
        {
            _Scheduler = Scheduler;
            _Task = Task;
        }

        public TaskSchedulerAwaiter GetAwaiter() => new(_Scheduler, _Task);

        public readonly struct TaskSchedulerAwaiter : ICriticalNotifyCompletion, INotifyCompletion
        {
            private readonly Task? _Task;
            private readonly TaskScheduler _Scheduler;

            public bool IsCompleted => _Task?.IsCompleted ?? false;

            public TaskSchedulerAwaiter(TaskScheduler Scheduler, Task? Task = null)
            {
                _Task = Task;
                _Scheduler = Scheduler;
            }

            public void OnCompleted([NotNull] Action continuation) => 
                Task.Factory.StartNew(continuation, CancellationToken.None, TaskCreationOptions.None, _Scheduler);

            public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

            // ReSharper disable once AsyncConverter.AsyncWait
            public void GetResult() => _Task?.Wait();
        }
    }

    public readonly ref struct TaskSchedulerAwaitable<T>
    {
        private readonly Task<T> _Task;
        private readonly TaskScheduler _Scheduler;

        public TaskSchedulerAwaitable(TaskScheduler Scheduler, Task<T> Task)
        {
            _Task = Task;
            _Scheduler = Scheduler;
        }

        public TaskSchedulerAwaiter<T> GetAwaiter() => new(_Scheduler, _Task);

        public readonly struct TaskSchedulerAwaiter<TResult> : INotifyCompletion
        {
            private readonly TaskScheduler _Scheduler;
            private readonly Task<TResult> _Task;

            public bool IsCompleted => _Task.IsCompleted;

            public TaskSchedulerAwaiter(TaskScheduler Scheduler, Task<TResult> Task)
            {
                _Scheduler = Scheduler;
                _Task = Task;
            }

            public void OnCompleted([NotNull] Action continuation) => 
                Task.Factory.StartNew(continuation, CancellationToken.None, TaskCreationOptions.None, _Scheduler);

            // ReSharper disable once AsyncConverter.AsyncWait
            public TResult GetResult() => _Task.Result;
        }
    }
}