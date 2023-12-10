#nullable enable
using System.Runtime.CompilerServices;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

public readonly ref struct YieldAwaitableThreadPool(bool LockContext, bool LongRunning = false)
{
    public Awaiter GetAwaiter() => new(LockContext, LongRunning);

    public readonly struct Awaiter(bool LockContext, bool LongRunning = false) : ICriticalNotifyCompletion, IEquatable<Awaiter>
    {
        private readonly bool _LockContext = LockContext;
        private static readonly WaitCallback __WaitCallbackRunAction = RunAction;
        private static readonly SendOrPostCallback __SendOrPostCallbackRunAction = RunAction;

        public bool IsCompleted => false;

        public void GetResult() { }

        private static void RunAction(object? State) => ((Action)State!).Invoke();

        public void OnCompleted(Action Continuation) => QueueContinuation(Continuation, true, _LockContext, LongRunning);

        public void UnsafeOnCompleted(Action Continuation) => QueueContinuation(Continuation, false, _LockContext, LongRunning);

        private static void QueueContinuation(Action Continuation, bool FlowContext, bool LockContext, bool LongRunning)
        {
            if (Continuation is null) throw new ArgumentNullException(nameof(Continuation));

            var context = SynchronizationContext.Current;
            if (LockContext && context != null && context.GetType() != typeof(SynchronizationContext))
                context.Post(__SendOrPostCallbackRunAction, Continuation);
            else
            {
                var scheduler = TaskScheduler.Current;
                if (!LockContext || scheduler == TaskScheduler.Default)
                    if (LongRunning)
                        Task.Factory.StartNew(
                            Continuation,
                            default,
                            TaskCreationOptions.LongRunning,
                            TaskScheduler.Default);
                    else if (FlowContext)
                        ThreadPool.QueueUserWorkItem(__WaitCallbackRunAction, Continuation);
                    else
                        ThreadPool.UnsafeQueueUserWorkItem(__WaitCallbackRunAction, Continuation);
                else
                    Task.Factory.StartNew(
                        Continuation,
                        default,
                        LongRunning 
                            ? TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning 
                            : TaskCreationOptions.PreferFairness, 
                        scheduler);
            }
        }

        public override bool Equals(object? obj) => obj is Awaiter awaiter && awaiter._LockContext == _LockContext;

        public override int GetHashCode() => _LockContext.GetHashCode();

        public static bool operator ==(Awaiter left, Awaiter right) => left.Equals(right);

        public static bool operator !=(Awaiter left, Awaiter right) => !(left == right);

        public bool Equals(Awaiter other) => other._LockContext == _LockContext;
    }
}