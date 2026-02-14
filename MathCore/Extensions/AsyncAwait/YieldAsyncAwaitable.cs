using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

[StructLayout(LayoutKind.Sequential, Size = 1)]
/// <summary>Структура, представляющая awaitable для планирования продолжения через пул потоков</summary>
public readonly struct YieldAsyncAwaitable
{
    /// <summary>Возвращает awaiter для этого awaitable</summary>
    /// <returns>Экземпляр `YieldAsyncAwaiter` для ожидания</returns>
    public readonly YieldAsyncAwaiter GetAwaiter() => new();

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    //[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true, Synchronization = true)]
    // ReSharper disable once RedundantExtendsListEntry
    /// <summary>Awaiter для `YieldAsyncAwaitable`, планирующий продолжение в пуле потоков</summary>
    public readonly struct YieldAsyncAwaiter : ICriticalNotifyCompletion, INotifyCompletion
    {
        private static readonly WaitCallback __WaitCallbackRunAction = RunAction;
        // ReSharper disable once UnusedMember.Local
        //private static readonly SendOrPostCallback __SendOrPostCallbackRunAction = RunAction;

        /// <summary>Всегда возвращает false, чтобы заставить асинхронный метод отложить продолжение</summary>
        public readonly bool IsCompleted => false;

        private static void RunAction(object? action) => ((Action)action!)();

        [SecurityCritical]
        private static void QueueContinuation(Action continuation, bool FlowContext)
        {
            if (continuation is null) throw new ArgumentNullException(nameof(continuation));

            if (FlowContext)
                ThreadPool.QueueUserWorkItem(__WaitCallbackRunAction, continuation);
            else
                ThreadPool.UnsafeQueueUserWorkItem(__WaitCallbackRunAction, continuation);
        }

        /// <summary>Планирует продолжение с сохранением текущего контекста исполнения</summary>
        /// <param name="continuation">Делегат, представляющий продолжение</param>
        /// <exception cref="ArgumentNullException">Если `continuation` равен null</exception>
        [SecuritySafeCritical]
        public void OnCompleted(Action continuation) => QueueContinuation(continuation, true);

        /// <summary>Планирует продолжение без сохранения контекста исполнения</summary>
        /// <param name="continuation">Делегат, представляющий продолжение</param>
        /// <exception cref="ArgumentNullException">Если `continuation` равен null</exception>
        [SecurityCritical]
        public void UnsafeOnCompleted(Action continuation) => QueueContinuation(continuation, false);

        /// <summary>Завершает awaiter без возвращаемого значения</summary>
        public void GetResult() { }
    }
}