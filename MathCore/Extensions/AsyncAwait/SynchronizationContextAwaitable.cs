#nullable enable
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

public readonly ref struct SynchronizationContextAwaitable(SynchronizationContext Context, Task? Task = null)
{
    internal static readonly SendOrPostCallback StartAction = action => ((Action)action)();
    private readonly Task? _Task = Task;

    public SynchronizationContextAwaiter GetAwaiter() => new(Context, _Task);

    public readonly struct SynchronizationContextAwaiter(SynchronizationContext Context, Task? Task = null) : ICriticalNotifyCompletion, INotifyCompletion
    {

        private readonly SynchronizationContext _Context = Context;
        private readonly Task? _Task = Task;

        public bool IsCompleted => _Task?.IsCompleted ?? false;

        public void OnCompleted(Action continuation) => _Context.Post(StartAction, continuation);

        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

        public void GetResult() => _Task?.Wait();
    }
}

public readonly ref struct SynchronizationContextAwaitable<T>(SynchronizationContext Context, Task<T>? Task = null)
{
    private readonly Task<T>? _Task = Task;

    public SynchronizationContextAwaiter GetAwaiter() => new(Context, _Task);

    public readonly struct SynchronizationContextAwaiter(SynchronizationContext Context, Task<T>? Task = null) : ICriticalNotifyCompletion, INotifyCompletion
    {
        private readonly SynchronizationContext _Context = Context;
        private readonly Task<T>? _Task = Task;

        public bool IsCompleted => (_Task?.IsCompleted).GetValueOrDefault();

        public void OnCompleted(Action continuation) => _Context.Post(SynchronizationContextAwaitable.StartAction, continuation);

        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

        public T GetResult() => (_Task ?? throw new InvalidOperationException()).Result;
    }
}