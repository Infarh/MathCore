#nullable enable
using System.Runtime.CompilerServices;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    public readonly ref struct YieldAwaitableThreadPool
    {
        private readonly bool _LockContext;

        public YieldAwaitableThreadPool(in bool LockContext) => _LockContext = LockContext;

        public Awaiter GetAwaiter() => new Awaiter(_LockContext);

        [Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Вложенные типы не должны быть видимыми")]
        public readonly struct Awaiter : ICriticalNotifyCompletion, IEquatable<Awaiter>
        {
            private readonly bool _LockContext;
            private static readonly WaitCallback __WaitCallbackRunAction = RunAction;
            private static readonly SendOrPostCallback __SendOrPostCallbackRunAction = RunAction;

            public Awaiter(in bool LockContext) => _LockContext = LockContext;

            [Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Пометьте члены как статические")]
            public bool IsCompleted => false;

            [Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Пометьте члены как статические")]
            public void GetResult() { }

            private static void RunAction(object? State) => ((Action)State!).Invoke();

            public void OnCompleted(Action Continuation) => QueueContinuation(Continuation, true, _LockContext);

            public void UnsafeOnCompleted(Action Continuation) => QueueContinuation(Continuation, false, _LockContext);

            private static void QueueContinuation(Action Continuation, bool FlowContext, bool LockContext)
            {
                if (Continuation is null) throw new ArgumentNullException(nameof(Continuation));

                var context = SynchronizationContext.Current;
                if (LockContext && context != null && context.GetType() != typeof(SynchronizationContext))
                    context.Post(__SendOrPostCallbackRunAction, Continuation);
                else
                {
                    var scheduler = TaskScheduler.Current;
                    if (scheduler == TaskScheduler.Default)
                    {
                        if (FlowContext)
                            ThreadPool.QueueUserWorkItem(__WaitCallbackRunAction, Continuation);
                        else
                            ThreadPool.UnsafeQueueUserWorkItem(__WaitCallbackRunAction, Continuation);
                    }
                    else
                        Task.Factory.StartNew(Continuation, default, TaskCreationOptions.PreferFairness, scheduler);
                }
            }

            public override bool Equals(object? obj) => obj is Awaiter awaiter && awaiter._LockContext == _LockContext;

            public override int GetHashCode() => _LockContext.GetHashCode();

            public static bool operator ==(Awaiter left, Awaiter right) => left.Equals(right);

            public static bool operator !=(Awaiter left, Awaiter right) => !(left == right);

            public bool Equals(Awaiter other) => other._LockContext == _LockContext;
        }
    }
}