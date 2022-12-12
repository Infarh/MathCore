#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers;

/// <summary>Планировщик выполняет задачи синхронно в текущем потоке</summary>
public sealed class CurrentThreadTaskScheduler : TaskScheduler
{
    /// <summary>Максимальное число параллельно выполняемых задач в данном планировщике всегда равно 1</summary>
    public override int MaximumConcurrencyLevel => 1;

    /// <summary>Планирует выполнение задачи в текущем потоке</summary>
    /// <param name="task">Задача, которую необходимо выполнить</param>
    protected override void QueueTask(Task task) => TryExecuteTask(task);

    /// <summary>Выполняет задачу синхронно в текущем потоке</summary>
    /// <param name="task">Задача, которую необходимо выполнить</param>
    /// <param name="TaskWasPreviouslyQueued">Задача была изначально в очереди данного планировщике</param>
    /// <returns>Задача была успешно выполнена</returns>
    protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) => TryExecuteTask(task);

    /// <summary>Перечень задач, которые должны запланированы на выполнение в текущем планировщике</summary>
    /// <returns>Всегда возвращает пустое перечисление - задачи выполняются немедленно без планирования</returns>
    protected override IEnumerable<Task> GetScheduledTasks() => Enumerable.Empty<Task>();
}