using System.Runtime.CompilerServices;

using MathCore.Annotations;

namespace System.Threading.Tasks
{
    public readonly ref struct SynchronizationContextAwaitable
    {
        internal static readonly SendOrPostCallback StartAction = action => ((Action)action)();
        private readonly SynchronizationContext _Context;
        private readonly Task _Task;

        public SynchronizationContextAwaitable(SynchronizationContext Context, [CanBeNull] Task Task = null)
        {
            _Context = Context;
            _Task = Task;
        }

        public SynchronizationContextAwaiter GetAwaiter() => new SynchronizationContextAwaiter(_Context, _Task);

        public readonly struct SynchronizationContextAwaiter : ICriticalNotifyCompletion, INotifyCompletion
        {

            private readonly SynchronizationContext _Context;
            private readonly Task _Task;

            public bool IsCompleted => _Task?.IsCompleted ?? false;

            public SynchronizationContextAwaiter(SynchronizationContext Context, [CanBeNull] Task Task = null)
            {
                _Context = Context;
                _Task = Task;
            }

            public void OnCompleted([NotNull] Action continuation) => _Context.Post(StartAction, continuation);

            public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

            public void GetResult() => _Task?.Wait();
        }
    }

    public readonly ref struct SynchronizationContextAwaitable<T>
    {
        private readonly SynchronizationContext _Context;
        private readonly Task<T> _Task;

        public SynchronizationContextAwaitable(SynchronizationContext Context, [CanBeNull] Task<T> Task = null)
        {
            _Context = Context;
            _Task = Task;
        }

        public SynchronizationContextAwaiter GetAwaiter() => new SynchronizationContextAwaiter(_Context, _Task);

        public readonly struct SynchronizationContextAwaiter : ICriticalNotifyCompletion, INotifyCompletion
        {
            private readonly SynchronizationContext _Context;
            private readonly Task<T> _Task;

            public bool IsCompleted => _Task.IsCompleted;

            public SynchronizationContextAwaiter(SynchronizationContext Context, [CanBeNull] Task<T> Task = null)
            {
                _Context = Context;
                _Task = Task;
            }

            public void OnCompleted([NotNull] Action continuation) => _Context.Post(SynchronizationContextAwaitable.StartAction, continuation);

            public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

            public T GetResult() => _Task.Result;
        }
    }
}