#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MathCore.Extensions.AsyncAwait;

public readonly ref struct PerformActionAwaitable(Action Method, Task Task, bool LockContext = true)
{
    private readonly Task _Task = Task;

    public Awaiter GetAwaiter() => new(Method, _Task, LockContext);

    public readonly struct Awaiter(Action Method, Task Task, bool LockContext) : ICriticalNotifyCompletion
    {
        private readonly Action _Method = Method;
        private readonly Task _Task = Task;
        private readonly bool _LockContext = LockContext;
        private static readonly WaitCallback __WaitCallbackRunAction = RunAction;
        private static readonly SendOrPostCallback __SendOrPostCallbackRunAction = RunAction;

        public bool IsCompleted => _Task.IsCompleted;

        public void GetResult() => _Task.Wait();

        private static void RunAction(object? State) => ((Action)State!).Invoke();

        public void OnCompleted(Action Continuation) => QueueContinuation(Continuation, _Method, true, _LockContext);

        public void UnsafeOnCompleted(Action Continuation) => QueueContinuation(Continuation, _Method, false, _LockContext);

        private static void QueueContinuation(Action Continuation, Action Method, bool FlowContext, bool LockContext)
        {
            if (Continuation is null) throw new ArgumentNullException(nameof(Continuation));

            var context = SynchronizationContext.Current;
            if (LockContext && context != null && context.GetType() != typeof(SynchronizationContext))
            {
                context.Post(__SendOrPostCallbackRunAction, Method);
                context.Post(__SendOrPostCallbackRunAction, Continuation);
            }
            else
            {
                var scheduler = TaskScheduler.Current;
                if (scheduler == TaskScheduler.Default)
                {
                    if (FlowContext)
                    {
                        ThreadPool.QueueUserWorkItem(__WaitCallbackRunAction, Method);
                        ThreadPool.QueueUserWorkItem(__WaitCallbackRunAction, Continuation);
                    }
                    else
                    {
                        ThreadPool.UnsafeQueueUserWorkItem(__WaitCallbackRunAction, Method);
                        ThreadPool.UnsafeQueueUserWorkItem(__WaitCallbackRunAction, Continuation);
                    }
                }
                else
                {
                    Task.Factory.StartNew(Method, default, TaskCreationOptions.PreferFairness, scheduler);
                    Task.Factory.StartNew(Continuation, default, TaskCreationOptions.PreferFairness, scheduler);
                }
            }
        }

        public override bool Equals(object? obj) => obj is Awaiter awaiter && Equals(awaiter);

        public override int GetHashCode() => _LockContext.GetHashCode();

        public static bool operator ==(Awaiter left, Awaiter right) => left.Equals(right);

        public static bool operator !=(Awaiter left, Awaiter right) => !(left == right);

        public bool Equals(Awaiter other) => other._LockContext == _LockContext && other._Method == _Method;
    }
}

public readonly ref struct PerformActionAwaitable<T>(Action Method, Task<T> task, bool LockContext = true)
{
    public Awaiter GetAwaiter() => new(Method, task, LockContext);

    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public readonly struct Awaiter(Action Method, Task<T> task, bool LockContext) : ICriticalNotifyCompletion
    {
        private readonly Action _Method = Method;
        private readonly bool _LockContext = LockContext;
        private static readonly WaitCallback __WaitCallbackRunAction = RunAction;
        private static readonly SendOrPostCallback __SendOrPostCallbackRunAction = RunAction;

        public bool IsCompleted => task.IsCompleted;

        public T GetResult() => task.Result;

        private static void RunAction(object? State) => ((Action)State!).Invoke();

        public void OnCompleted(Action Continuation) => QueueContinuation(Continuation, _Method, true, _LockContext);

        public void UnsafeOnCompleted(Action Continuation) => QueueContinuation(Continuation, _Method, false, _LockContext);

        private static void QueueContinuation(Action Continuation, Action Method, bool FlowContext, bool LockContext)
        {
            if (Continuation is null) throw new ArgumentNullException(nameof(Continuation));

            var context = SynchronizationContext.Current;
            if (LockContext && context != null && context.GetType() != typeof(SynchronizationContext))
            {
                context.Post(__SendOrPostCallbackRunAction, Method);
                context.Post(__SendOrPostCallbackRunAction, Continuation);
            }
            else
            {
                var scheduler = TaskScheduler.Current;
                if (scheduler == TaskScheduler.Default)
                {
                    if (FlowContext)
                    {
                        ThreadPool.QueueUserWorkItem(__WaitCallbackRunAction, Method);
                        ThreadPool.QueueUserWorkItem(__WaitCallbackRunAction, Continuation);
                    }
                    else
                    {
                        ThreadPool.UnsafeQueueUserWorkItem(__WaitCallbackRunAction, Method);
                        ThreadPool.UnsafeQueueUserWorkItem(__WaitCallbackRunAction, Continuation);
                    }
                }
                else
                {
                    Task.Factory.StartNew(Method, default, TaskCreationOptions.PreferFairness, scheduler);
                    Task.Factory.StartNew(Continuation, default, TaskCreationOptions.PreferFairness, scheduler);
                }
            }
        }

        public override bool Equals(object? obj) => obj is Awaiter awaiter && Equals(awaiter);

        public override int GetHashCode() => _LockContext.GetHashCode();

        public static bool operator ==(Awaiter left, Awaiter right) => left.Equals(right);

        public static bool operator !=(Awaiter left, Awaiter right) => !(left == right);

        public bool Equals(Awaiter other) => other._LockContext == _LockContext && other._Method == _Method;
    }
}