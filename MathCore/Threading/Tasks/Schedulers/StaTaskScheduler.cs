using System.Collections.Concurrent;
// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers;

/// <summary>Предоставляет планировщик, использующий STA-потоки</summary>
/// <remarks>Экземпляр управляет пулом STA-потоков и выполняет задачи через очередь</remarks>
/// <example>
/// <code>
/// using var scheduler = new StaTaskScheduler(2);
/// var task_factory = new TaskFactory(scheduler);
/// var result_task = task_factory.StartNew(() => 42);
/// var result = result_task.Result;
/// </code>
/// </example>
public sealed class StaTaskScheduler : TaskScheduler, IDisposable
{
    /// <summary>Хранит очередь задач, выполняемых пулом STA-потоков</summary>
    private BlockingCollection<Task> _Tasks;
    /// <summary>STA-потоки, используемые планировщиком</summary>
    private readonly List<Thread> _Threads;

    /// <summary>Инициализирует экземпляр планировщика с заданным уровнем параллелизма</summary>
    /// <param name="NumberOfThreads">Количество потоков, создаваемых планировщиком</param>
    /// <exception cref="ArgumentOutOfRangeException">Количество потоков меньше единицы</exception>
    /// <example>
    /// <code>
    /// using var scheduler = new StaTaskScheduler(1);
    /// var task_factory = new TaskFactory(scheduler);
    /// var task = task_factory.StartNew(() => "ok");
    /// var result = task.Result;
    /// </code>
    /// </example>
    public StaTaskScheduler(int NumberOfThreads)
    {
        // Проверка аргументов
        if (NumberOfThreads < 1) throw new ArgumentOutOfRangeException(nameof(NumberOfThreads));

        // Инициализация коллекции задач
        _Tasks = [];

        // Создание потоков для планировщика
        _Threads = Enumerable.Range(0, NumberOfThreads).Select(_ =>
        {
            var thread = new Thread(
                () =>
                {
                    // Постоянно получает следующую задачу и пытается её выполнить
                    // Это продолжается, пока планировщик не будет освобождён и задачи не закончатся
                    foreach (var t in _Tasks.GetConsumingEnumerable())
                    {
                        TryExecuteTask(t);
                    }
                })
            { IsBackground = true };
#pragma warning disable CA1416
            thread.SetApartmentState(ApartmentState.STA);
#pragma warning restore CA1416
            return thread;
        }).ToList();

        // Запуск всех потоков
        _Threads.ForEach(t => t.Start());
    }

    /// <summary>Ставит задачу в очередь на выполнение этим планировщиком</summary>
    /// <param name="task">Задача для выполнения</param>
    protected override void QueueTask(Task task) =>
        // Помещает задачу в блокирующую коллекцию
        _Tasks.Add(task);

    /// <summary>Возвращает список запланированных задач для отладчика</summary>
    /// <returns>Перечисление всех запланированных задач</returns>
    protected override IEnumerable<Task> GetScheduledTasks() =>
        // Сериализует содержимое блокирующей коллекции для отладчика
        [.. _Tasks];

    /// <summary>Определяет, может ли задача быть выполнена встроенно</summary>
    /// <param name="task">Задача для выполнения</param>
    /// <param name="TaskWasPreviouslyQueued">Признак того, что задача уже была в очереди</param>
    /// <returns>true, если задача успешно выполнена встроенно; иначе false</returns>
    protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) =>
        // Встраивает выполнение, если текущий поток STA
        Thread.CurrentThread.GetApartmentState() == ApartmentState.STA &&
        TryExecuteTask(task);

    /// <summary>Возвращает максимальный уровень параллелизма, поддерживаемый планировщиком</summary>
    public override int MaximumConcurrencyLevel => _Threads.Count;

    /// <summary>Освобождает ресурсы планировщика и завершает обработку задач</summary>
    /// <remarks>Блокирует поток вызывающего кода до завершения всех рабочих потоков</remarks>
    /// <example>
    /// <code>
    /// using var scheduler = new StaTaskScheduler(2);
    /// var task_factory = new TaskFactory(scheduler);
    /// var task = task_factory.StartNew(() => 1);
    /// task.Wait();
    /// </code>
    /// </example>
    public void Dispose()
    {
        if (_Tasks is null) return;
        // Сигнализирует, что новых задач больше не будет
        _Tasks.CompleteAdding();

        // Ожидает завершения всех потоков
        foreach (var thread in _Threads)
            thread.Join();

        // Очистка ресурсов
        _Tasks.Dispose();
        _Tasks = null!;
    }
}