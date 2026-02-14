// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers;

/// <summary>Предоставляет планировщик задач с поддержкой переприоритизации ранее поставленных задач</summary>
/// <example>
/// <code>
/// var scheduler = new ReprioritizableTaskScheduler();
///
/// var t1 = Task.Factory.StartNew(() => { /* будет выполнено позже */ },
///     CancellationToken.None, TaskCreationOptions.None, scheduler);
///
/// var t2 = Task.Factory.StartNew(() => { /* будет выполнено раньше */ },
///     CancellationToken.None, TaskCreationOptions.None, scheduler);
///
/// scheduler.Prioritize(t2);
///
/// Task.WaitAll(t1, t2);
/// </code>
/// </example>
public sealed class ReprioritizableTaskScheduler : TaskScheduler
{
    private readonly LinkedList<Task> _Tasks = new(); // protected by lock(_tasks)

    /// <summary>Ставит задачу в очередь планировщика</summary>
    /// <param name="task">Задача для постановки в очередь</param>
    protected override void QueueTask(Task task)
    {
        // Сохраняем задачу и уведомляем ThreadPool о поступлении работы
        lock (_Tasks) _Tasks.AddLast(task);
        ThreadPool.UnsafeQueueUserWorkItem(ProcessNextQueuedItem, null);
    }

    /// <summary>Переприоритизирует ранее поставленную задачу в начало очереди</summary>
    /// <param name="task">Задача для переприоритизации</param>
    /// <returns>Признак успешного перемещения задачи в начало очереди</returns>
    public bool Prioritize(Task task)
    {
        lock (_Tasks)
        {
            var node = _Tasks.Find(task);
            if (node is null) return false;
            _Tasks.Remove(node);
            _Tasks.AddFirst(node);
            return true;
        }
    }

    /// <summary>Переприоритизирует ранее поставленную задачу в конец очереди</summary>
    /// <param name="task">Задача для переприоритизации</param>
    /// <returns>Признак успешного перемещения задачи в конец очереди</returns>
    public bool Deprioritize(Task task)
    {
        lock (_Tasks)
        {
            var node = _Tasks.Find(task);
            if (node is null) return false;
            _Tasks.Remove(node);
            _Tasks.AddLast(node);
            return true;
        }
    }

    /// <summary>Удаляет ранее поставленную задачу из планировщика</summary>
    /// <param name="task">Задача для удаления</param>
    /// <returns>Признак успешного удаления задачи из планировщика</returns>
    protected override bool TryDequeue(Task task)
    {
        lock (_Tasks) return _Tasks.Remove(task);
    }

    /// <summary>Забирает и выполняет следующий элемент очереди</summary>
    private void ProcessNextQueuedItem(object? _)
    {
        Task task;
        lock (_Tasks)
        {
            if (_Tasks.Count == 0) return;
            task = _Tasks.First!.Value;
            _Tasks.RemoveFirst();
        }
        TryExecuteTask(task);
    }

    /// <summary>Выполняет указанную задачу встроенно</summary>
    /// <param name="task">Задача для выполнения</param>
    /// <param name="TaskWasPreviouslyQueued">Признак предварительной постановки задачи в очередь</param>
    /// <returns>Признак возможности встроенного выполнения</returns>
    protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) => TryExecuteTask(task);

    /// <summary>Возвращает все задачи, находящиеся в очереди планировщика</summary>
    /// <returns>Перечисление задач, находящихся в очереди планировщика</returns>
    protected override IEnumerable<Task> GetScheduledTasks()
    {
        var lock_taken = false;
        try
        {
            Monitor.TryEnter(_Tasks, ref lock_taken);
            return lock_taken ? _Tasks.ToArray() : throw new NotSupportedException();
        }
        finally
        {
            if (lock_taken) Monitor.Exit(_Tasks);
        }
    }
}