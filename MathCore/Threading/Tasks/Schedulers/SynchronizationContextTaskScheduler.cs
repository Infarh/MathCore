using System.Collections.Concurrent;
// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers;

/// <summary>Планировщик задач, выполняющий задачи в указанном SynchronizationContext</summary>
/// <remarks>Все задачи выполняются последовательно в контексте синхронизации</remarks>
/// <example>
/// <code>
/// var scheduler = new SynchronizationContextTaskScheduler();
/// var task_factory = new TaskFactory(scheduler);
/// var task = task_factory.StartNew(() => DoWork());
/// </code>
/// </example>
/// <remarks>
/// Инициализирует экземпляр планировщика с указанным SynchronizationContext
/// </remarks>
/// <param name="context">Контекст синхронизации для выполнения задач</param>
/// <exception cref="ArgumentNullException">Возникает, если context равен null</exception>
/// <remarks>Переданный контекст используется для выполнения всех задач планировщика</remarks>
/// <example>
/// <code>
/// var context = new SynchronizationContext();
/// var scheduler = new SynchronizationContextTaskScheduler(context);
/// </code>
/// </example>
public sealed class SynchronizationContextTaskScheduler(SynchronizationContext context) : TaskScheduler
{
    /// <summary>Очередь задач для выполнения, сохраняемая для отладки</summary>
    private readonly ConcurrentQueue<Task> _Tasks = new();
    /// <summary>Целевой контекст, в котором выполняются задачи</summary>
    private readonly SynchronizationContext _Context = context.NotNull();

    /// <summary>Инициализирует экземпляр планировщика для текущего SynchronizationContext</summary>
    /// <remarks>Если текущий контекст отсутствует, создается новый SynchronizationContext</remarks>
    /// <example>
    /// <code>
    /// var scheduler = new SynchronizationContextTaskScheduler();
    /// </code>
    /// </example>
    public SynchronizationContextTaskScheduler() : this(SynchronizationContext.Current ?? new()) { }

    /// <summary>Помещает задачу в очередь планировщика для выполнения в контексте синхронизации</summary>
    /// <param name="task">Задача для постановки в очередь</param>
    protected override void QueueTask(Task task)
    {
        _Tasks.Enqueue(task);
        _Context.Post(delegate
        {
            if (_Tasks.TryDequeue(out var next_task)) TryExecuteTask(next_task);
        }, null);
    }

    /// <summary>Пытается выполнить задачу в текущем потоке</summary>
    /// <param name="task">Задача для выполнения</param>
    /// <param name="TaskWasPreviouslyQueued">Признак прежней постановки в очередь</param>
    /// <returns>Признак успешного выполнения задачи</returns>
    protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) => _Context == SynchronizationContext.Current && TryExecuteTask(task);

    /// <summary>Возвращает перечисление задач, поставленных в очередь планировщика</summary>
    /// <returns>Перечисление задач, поставленных в очередь</returns>
    protected override IEnumerable<Task> GetScheduledTasks() => [.. _Tasks];

    /// <summary>Возвращает максимальный уровень параллелизма, поддерживаемый планировщиком</summary>
    public override int MaximumConcurrencyLevel => 1;
}