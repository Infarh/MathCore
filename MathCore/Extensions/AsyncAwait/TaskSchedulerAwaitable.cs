#nullable enable
using System.Runtime.CompilerServices;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

public readonly ref struct TaskSchedulerAwaitable(TaskScheduler Scheduler, Task? Task = null)
{
    private readonly Task? _Task = Task;

    public TaskSchedulerAwaiter GetAwaiter() => new(Scheduler, _Task);

    public readonly struct TaskSchedulerAwaiter(TaskScheduler Scheduler, Task? task = null) : ICriticalNotifyCompletion, INotifyCompletion
    {
        public bool IsCompleted => task?.IsCompleted ?? false;

        public void OnCompleted(Action continuation) => 
            Task.Factory.StartNew(
                continuation,
                CancellationToken.None, 
                TaskCreationOptions.None,
                Scheduler);

        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

        // ReSharper disable once AsyncConverter.AsyncWait
        public void GetResult() => task?.Wait();
    }
}

public readonly ref struct TaskSchedulerAwaitable<T>(TaskScheduler Scheduler, Task<T> task)
{
    public TaskSchedulerAwaiter<T> GetAwaiter() => new(Scheduler, task);

    public readonly struct TaskSchedulerAwaiter<TResult>(TaskScheduler Scheduler, Task<TResult> task) : INotifyCompletion
    {
        public bool IsCompleted => task.IsCompleted;

        public void OnCompleted(Action continuation) => 
            Task.Factory.StartNew(
                action: continuation,
                cancellationToken: CancellationToken.None,
                creationOptions: TaskCreationOptions.None,
                scheduler: Scheduler);

        // ReSharper disable once AsyncConverter.AsyncWait
        public TResult GetResult() => task.Result;
    }
}