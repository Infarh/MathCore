using System.Runtime.CompilerServices;
// ReSharper disable UnusedParameter.Global
// ReSharper disable InvalidXmlDocComment

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    // ReSharper disable once UnusedType.Global
    public static class YieldAwaiterExtensions
    {
        /// <summary>Продолжить в пуле потоков</summary>
        /// <param name="LockContext">Если истина, то продолжение будет выполнено в том же потоке, если ложь - то в пуле потоков</param>
        public static YieldAwaitableThreadPool ConfigureAwait(this YieldAwaitable _, bool LockContext) => new(LockContext);

        /// <summary>Продолжить в новом потоке</summary>
        public static YieldAwaitableThread ConfigureAwaitLongRunning(this YieldAwaitable _) => new();

        /// <summary>Выполнить продолжение в указанном планировщике</summary>
        /// <param name="Scheduler">Планировщик, в котором требуется выполнить продолжение</param>
        public static TaskSchedulerAwaitable ConfigureAwait(this YieldAwaitable _, TaskScheduler Scheduler) => new(Scheduler);

        /// <summary>Выполнить продолжение в указанном контексте синхронизации</summary>
        /// <param name="context">Контекст синхронизации, в котором требуется выполнить продолжение</param>
        public static SynchronizationContextAwaitable ConfigureAwait(this YieldAwaitable _, SynchronizationContext context) => new(context);
    }
}