#nullable enable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct YieldAsyncAwaitable
{
    public YieldAsyncAwaiter GetAwaiter() => new();

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    //[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true, Synchronization = true)]
    public struct YieldAsyncAwaiter : ICriticalNotifyCompletion, INotifyCompletion
    {
        private static readonly WaitCallback __WaitCallbackRunAction = RunAction;
        private static readonly SendOrPostCallback __SendOrPostCallbackRunAction = RunAction;

        public bool IsCompleted => false;

        private static void RunAction(object action) => ((Action)action)();

        [SecurityCritical]
        private static void QueueContinuation(Action continuation, bool FlowContext)
        {
            if (continuation is null) throw new ArgumentNullException(nameof(continuation));

            if (FlowContext)
                ThreadPool.QueueUserWorkItem(__WaitCallbackRunAction, continuation);
            else
                ThreadPool.UnsafeQueueUserWorkItem(__WaitCallbackRunAction, continuation);
        }

        [SecuritySafeCritical]
        public void OnCompleted(Action continuation) => QueueContinuation(continuation, true);

        [SecurityCritical]
        public void UnsafeOnCompleted(Action continuation) => QueueContinuation(continuation, false);

        public void GetResult() { }
    }
}