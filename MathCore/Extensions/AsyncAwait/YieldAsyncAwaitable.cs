using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct YieldAsyncAwaitable
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
#pragma warning disable CA1822 // Mark members as static
        public YieldAsyncAwaiter GetAwaiter() => new YieldAsyncAwaiter();
#pragma warning restore CA1822 // Mark members as static

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        //[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true, Synchronization = true)]
        // ReSharper disable once RedundantExtendsListEntry
#pragma warning disable CA1815 // Override equals and operator equals on value types
        public struct YieldAsyncAwaiter : ICriticalNotifyCompletion, INotifyCompletion
#pragma warning restore CA1815 // Override equals and operator equals on value types
        {
            [NotNull] private static readonly WaitCallback __WaitCallbackRunAction = RunAction;
            [NotNull] private static readonly SendOrPostCallback __SendOrPostCallbackRunAction = RunAction;

            public bool IsCompleted => false;

            private static void RunAction([NotNull] object state) => ((Action)state)();

            [SecurityCritical]
            private static void QueueContinuation([NotNull] Action continuation, bool FlowContext)
            {
                if (continuation is null) throw new ArgumentNullException(nameof(continuation));

                if (FlowContext)
                    ThreadPool.QueueUserWorkItem(__WaitCallbackRunAction, continuation);
                else
                    ThreadPool.UnsafeQueueUserWorkItem(__WaitCallbackRunAction, continuation);
            }

            [SecuritySafeCritical]
            public void OnCompleted([NotNull] Action continuation) => QueueContinuation(continuation, true);

            [SecurityCritical]
            public void UnsafeOnCompleted([NotNull] Action continuation) => QueueContinuation(continuation, false);

#pragma warning disable CA1822 // Mark members as static
            public void GetResult() { }
#pragma warning restore CA1822 // Mark members as static
        }
    }
}