#nullable enable
using System.Runtime.CompilerServices;

// ReSharper disable UnusedParameter.Global
// ReSharper disable InvalidXmlDocComment

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

// ReSharper disable once UnusedType.Global
public static class YieldAwaiterExtensions
{
    /// <summary>Продолжить в пуле потоков</summary>
    /// <param name="LockContext">Если истина, то продолжение будет выполнено в том же потоке, если ложь - то в пуле потоков</param>
    public static YieldAwaitableThreadPool ConfigureAwait(this YieldAwaitable _, bool LockContext) => new(LockContext);

    /// <summary>Продолжить в пуле потоков</summary>
    /// <param name="LockContext">Если истина, то продолжение будет выполнено в том же потоке, если ложь - то в пуле потоков</param>
    /// <param name="LongRunning">Длительное выполнение задачи</param>
    public static YieldAwaitableThreadPool ConfigureAwait(this YieldAwaitable _, bool LockContext, bool LongRunning) => new(LockContext, LongRunning);

    /// <summary>Продолжить в пуле потоков</summary>
    public static YieldAwaitableThreadPool ConfigureAwaitLongRunning(this YieldAwaitable _, bool LockContext) => new(LockContext, true);

    /// <summary>Продолжить в новом потоке</summary>
    public static YieldAwaitableThread ConfigureAwaitThread(this YieldAwaitable _) => new();

    /// <summary>Выполнить продолжение в указанном планировщике</summary>
    /// <param name="Scheduler">Планировщик, в котором требуется выполнить продолжение</param>
    public static TaskSchedulerAwaitable ConfigureAwait(this YieldAwaitable _, TaskScheduler Scheduler) => new(Scheduler);

    /// <summary>Выполнить продолжение в указанном контексте синхронизации</summary>
    /// <param name="context">Контекст синхронизации, в котором требуется выполнить продолжение</param>
    public static SynchronizationContextAwaitable ConfigureAwait(this YieldAwaitable _, SynchronizationContext context) => new(context);
}