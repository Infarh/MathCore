using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    public readonly ref struct YieldAwaitableThread
    {
        public Awaiter GetAwaiter() => new();

        public readonly struct Awaiter : ICriticalNotifyCompletion
        {
            private static readonly ParameterizedThreadStart __ThreadAction = action => ((Action)action)();

            public bool IsCompleted => false;

            public void GetResult() { }

            public void OnCompleted(Action Continuation) => new Thread(__ThreadAction).Start(Continuation);

            public void UnsafeOnCompleted(Action Continuation) => new Thread(__ThreadAction).Start(Continuation);
        }
    }
}