using System.Runtime.CompilerServices;
using MathCore.Annotations;

namespace System.Threading.Tasks
{
    public readonly ref struct SynchronizationContextAwaitable
    {
        private readonly SynchronizationContext _Context;

        public SynchronizationContextAwaitable(SynchronizationContext Context) => _Context = Context;

        public SynchronizationContextAwaiter GetAwaiter() => new SynchronizationContextAwaiter(_Context);

        public readonly struct SynchronizationContextAwaiter : ICriticalNotifyCompletion, INotifyCompletion
        {
            private static readonly SendOrPostCallback __StartAction = action => ((Action)action)();

            private readonly SynchronizationContext _Context;

            public bool IsCompleted => false;

            public SynchronizationContextAwaiter(SynchronizationContext Context, Task Task = null) => _Context = Context;

            public void OnCompleted([NotNull] Action continuation) => _Context.Post(__StartAction, continuation);

            public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

            public void GetResult() { }
        }
    }
}