using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers;

/// <summary>Планировщик, способный выполнять не более указанного числа задач параллельно</summary>
public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
{
    /// <summary>Флаг, определяющий что текущий поток осуществляет выполнение задачи</summary>
    [ThreadStatic]
    private static bool __CurrentThreadIsProcessingItems;

    /// <summary>Список задач, требующих исполнения</summary>
    private readonly LinkedList<Task> _Tasks = new(); // protected by lock(_tasks)

    /// <summary>Максимальный уровень параллелизма, определённый для данного планировщика</summary>
    private readonly int _MaximumConcurrencyLevel;

    /// <summary>Определяет число задач, выполняемых в текущий момент планировщиком</summary>
    private int _DelegatesQueuedOrRunning;

    /// <summary>Максимальный уровень параллелизма, поддерживаемый планировщиком</summary>
    public sealed override int MaximumConcurrencyLevel => _MaximumConcurrencyLevel;

    /// <summary>Инициализация нового экземпляра планировщика</summary>
    /// <param name="MaximumConcurrencyLevel">Максимальное число одновременно выполняемых задач</param>
    public LimitedConcurrencyLevelTaskScheduler(int MaximumConcurrencyLevel)
    {
        if (MaximumConcurrencyLevel < 1) throw new ArgumentOutOfRangeException(nameof(MaximumConcurrencyLevel));
        _MaximumConcurrencyLevel = MaximumConcurrencyLevel;
    }

    /// <summary>Добавление задачи в очередь на исполнение</summary>
    /// <param name="task">Задача, требующая выполнения в планировщиком</param>
    protected sealed override void QueueTask(Task task)
    {
        // Добавление задачи в список задач для последующего исполнения.
        // Если в данный момент недостаточно делегатов для исполнения ещё одной задачи, то число делегатов инкрементируется
        lock (_Tasks)
        {
            _Tasks.AddLast(task);
            if (_DelegatesQueuedOrRunning >= _MaximumConcurrencyLevel) return;
            _DelegatesQueuedOrRunning++;
            NotifyThreadPoolOfPendingWork();
        }
    }

    /// <summary>Информирование пула потоков о том, что поступила новая задача на исполнение</summary>
    private void NotifyThreadPoolOfPendingWork() =>
        ThreadPool.UnsafeQueueUserWorkItem(
            _ =>
            {
                // Фиксируем то, что текущий поток обрабатывает задачу
                // Это необходимо для повторного использования потоков для исполнения новых задач
                __CurrentThreadIsProcessingItems = true;
                try
                {
                    // Обработка всех доступных задач из очереди
                    while (true)
                    {
                        Task task;
                        lock (_Tasks)
                        {
                            // Когда очередь пуста, освобождаем текущий поток-обработчик
                            if (_Tasks.Count == 0)
                            {
                                _DelegatesQueuedOrRunning--;
                                break;
                            }

                            // Извлечение очередной задачи из очереди
                            task = _Tasks.First.Value;
                            _Tasks.RemoveFirst();
                        }

                        // Выполнение извлечённой из очереди задачи
                        TryExecuteTask(task);
                    }
                }
                // При завершении работы обработчик снимает флаг активности для своего потока
                finally
                {
                    __CurrentThreadIsProcessingItems = false;
                }
            }, null);

    /// <summary>Запуск обработки очередной задачи в текущем потоке</summary>
    /// <param name="task">Задача, требующая выполнения</param>
    /// <param name="TaskWasPreviouslyQueued">
    /// Значение, указывающее, была ли задача ранее поставлена в очередь.
    /// Если этот параметр True, то задача, возможно, ранее была в очереди (по расписанию);
    /// Если False, то задача, не стояла в очереди, и её требуется выполнить без постановки в очередь.
    /// </param>
    /// <returns>Ы</returns>
    protected sealed override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued)
    {
        // Если текущий поток не занят выполнением задач в планировщике, то задачу выполнить нельзя - возвращаем ложь
        if (!__CurrentThreadIsProcessingItems) return false;

        // Если задача была ранее добавлена в очередь, то извлекаем её оттуда
        return (!TaskWasPreviouslyQueued || TryDequeue(task)) && TryExecuteTask(task);

        //return TaskWasPreviouslyQueued 
        //    ? TryDequeue(task) && TryExecuteTask(task) // и пытаемся запустить
        //    : TryExecuteTask(task);
    }

    /// <summary>Попытка удалить задачу из очереди</summary>
    /// <param name="task">Удаляемая задача</param>
    /// <returns>Истина, если задача была успешно удалена из очереди</returns>
    protected sealed override bool TryDequeue(Task task)
    {
        lock (_Tasks) return _Tasks.Remove(task);
    }

    /// <summary>Получить перечисление запланированных задач в планировщике</summary>
    /// <returns>Перечисление запланированных на текущий момент задач</returns>
    protected sealed override IEnumerable<Task> GetScheduledTasks()
    {
        var lock_taken = false;
        try
        {
            Monitor.TryEnter(_Tasks, ref lock_taken);
            return lock_taken ? _Tasks : throw new NotSupportedException();
        }
        finally
        {
            if (lock_taken) Monitor.Exit(_Tasks);
        }
    }
}