﻿#nullable enable
// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers;

/// <summary>Планировщик обеспечивает создание отдельного потока на каждую выполняемою задачу</summary>
public class ThreadPerTaskScheduler(Action<Thread>? ThreadInitializer = null) : TaskScheduler
{
    /// <summary>Получить перечень запланированных задач в данном планировщике (всегда возвращает пустое перечисление)</summary>
    /// <remarks>Данный планировщик не задерживает задачи, а исполняет их по мере поступления</remarks>
    protected override IEnumerable<Task> GetScheduledTasks() => [];

    /// <summary>Запускает новый поток для выполнения задачи</summary>
    protected override void QueueTask(Task task)
    {
        //var thread = new Thread(() => TryExecuteTask(task)) {IsBackground = true};
        var thread = new Thread(p => ((ThreadPerTaskScheduler)p).TryExecuteTask(task)) { IsBackground = true };
        ThreadInitializer?.Invoke(thread);
        thread.Start(this);
    }

    /// <summary>Выполняет задачу в текущем потоке</summary>
    /// <param name="task">Выполняемая задача</param>
    /// <param name="TaskWasPreviouslyQueued">Параметр игнорируется</param>
    /// <returns>Истина, если задача выполнена успешно</returns>
    protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) => TryExecuteTask(task);
}