#nullable enable
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

public readonly ref struct SynchronizationContextAwaitable
{
    internal static readonly SendOrPostCallback StartAction = action => ((Action)action)();
    private readonly SynchronizationContext _Context;
    private readonly Task? _Task;

    public SynchronizationContextAwaitable(SynchronizationContext Context, Task? Task = null)
    {
        _Context = Context;
        _Task    = Task;
    }

    public SynchronizationContextAwaiter GetAwaiter() => new(_Context, _Task);

    public readonly struct SynchronizationContextAwaiter : ICriticalNotifyCompletion, INotifyCompletion
    {

        private readonly SynchronizationContext _Context;
        private readonly Task? _Task;

        public bool IsCompleted => _Task?.IsCompleted ?? false;

        public SynchronizationContextAwaiter(SynchronizationContext Context, Task? Task = null)
        {
            _Context = Context;
            _Task    = Task;
        }

        public void OnCompleted(Action continuation) => _Context.Post(StartAction, continuation);

        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

        public void GetResult() => _Task?.Wait();
    }
}

public readonly ref struct SynchronizationContextAwaitable<T>
{
    private readonly SynchronizationContext _Context;
    private readonly Task<T>? _Task;

    public SynchronizationContextAwaitable(SynchronizationContext Context, Task<T>? Task = null)
    {
        _Context = Context;
        _Task    = Task;
    }

    public SynchronizationContextAwaiter GetAwaiter() => new(_Context, _Task);

    public readonly struct SynchronizationContextAwaiter : ICriticalNotifyCompletion, INotifyCompletion
    {
        private readonly SynchronizationContext _Context;
        private readonly Task<T>? _Task;

        public bool IsCompleted => (_Task?.IsCompleted).GetValueOrDefault();

        public SynchronizationContextAwaiter(SynchronizationContext Context, Task<T>? Task = null)
        {
            _Context = Context;
            _Task    = Task;
        }

        public void OnCompleted(Action continuation) => _Context.Post(SynchronizationContextAwaitable.StartAction, continuation);

        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

        public T GetResult() => (_Task ?? throw new InvalidOperationException()).Result;
    }
}