// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers;

/// <summary>Планировщик с собственным пулом потоков</summary>
/// <example>
/// <code>
/// var scheduler = new WorkStealingTaskScheduler(4);
/// var task = new Task(() => Console.WriteLine("Hello from task"));
/// task.Start(scheduler);
/// task.Wait();
/// scheduler.Dispose();
/// </code>
/// </example>
public class WorkStealingTaskScheduler : TaskScheduler, IDisposable
{
    [ThreadStatic] private static WorkStealingQueue<Task>? __ThreadTaskQueue;

    private readonly int _ConcurrencyLevel;
    private readonly Queue<Task> _Queue = [];
    private WorkStealingQueue<Task>[] _TaskQueues = new WorkStealingQueue<Task>[Environment.ProcessorCount];
    private readonly Lazy<Thread[]> _Threads;
    private int _ThreadsWaiting;
    private bool _Shutdown;

    /// <summary>Инициализация нового планировщика с числом потоков, равным удвоенному числу процессоров в системе</summary>
    public WorkStealingTaskScheduler() : this(Environment.ProcessorCount * 2) { }

    /// <summary>Инициализация нового планировщика с указанным числом потоков</summary>
    /// <param name="ConcurrencyLevel">Число потоков, доступных планировщику</param>
    public WorkStealingTaskScheduler(int ConcurrencyLevel)
    {
        // Сохранение уровня параллелизма
        if (ConcurrencyLevel <= 0) throw new ArgumentOutOfRangeException(nameof(ConcurrencyLevel));
        _ConcurrencyLevel = ConcurrencyLevel;

        // Инициализация потоков
        _Threads = new(() =>
        {
            var threads = new Thread[_ConcurrencyLevel];
            for (var i = 0; i < threads.Length; i++)
            {
                threads[i] = new(DispatchLoop) { IsBackground = true };
                threads[i].Start();
            }
            return threads;
        });
    }

    /// <summary>Добавление задачи в очередь к планировщику</summary>
    protected override void QueueTask(Task task)
    {
        // Убедиться, что пул запущен, например, что все потоки были созданы
        _ = _Threads.Value;

        // Если задача отмечена как долгоживущая, дать ей собственный выделенный поток
        // вместо добавления в очередь
        if ((task.CreationOptions & TaskCreationOptions.LongRunning) != 0)
            new Thread(state => TryExecuteTask((Task)state!)) { IsBackground = true }.Start(task);
        else
        {
            // Иначе добавить рабочую задачу в очередь, возможно разбудив поток
            // Если есть локальная очередь и задача не предпочитает быть в глобальной очереди,
            // добавить её в локальную очередь
            var wsq = __ThreadTaskQueue;
            if (wsq != null && ((task.CreationOptions & TaskCreationOptions.PreferFairness) == 0))
            {
                // Добавить в локальную очередь и уведомить ожидающие потоки о доступности работы
                // Могут возникнуть условия гонки, которые приведут к пропущенным уведомлениям,
                // но они безвредны, так как этот поток в конечном итоге получит рабочую задачу,
                // как и другие потоки при получении другого уведомления о задаче
                wsq.LocalPush(task);
                if (_ThreadsWaiting == 0) return;
                // Безопасное чтение без блокировки
                lock (_Queue)
                    Monitor.Pulse(_Queue);
            }
            // Иначе добавить рабочую задачу в глобальную очередь
            else
                lock (_Queue)
                {
                    _Queue.Enqueue(task);
                    if (_ThreadsWaiting > 0) Monitor.Pulse(_Queue);
                }
        }
    }

    /// <summary>Выполнить задачу в текущем потоке</summary>
    /// <param name="task">Задача для выполнения</param>
    /// <param name="TaskWasPreviouslyQueued">Игнорируется</param>
    /// <returns>Можно ли выполнить задачу</returns>
    protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) => TryExecuteTask(task);

    // // Альтернативная реализация: вместо всегда попытки выполнить задачу (что может оставить
    // // задачу в очереди, которая уже была выполнена), мы можем
    // // поискать задачу в текущей очереди с кражей работ и удалить её,
    // // выполняя её встроенно только если она найдена
    // WorkStealingQueue<Task> queue = __ThreadTaskQueue;
    // return queue != null && queue.TryFindAndPop(task) && TryExecuteTask(task);

    /// <summary>Получить максимальный уровень параллелизма, поддерживаемый этим планировщиком</summary>
    public override int MaximumConcurrencyLevel => _ConcurrencyLevel;

    /// <summary>Получить все задачи, текущего запланированные для этого планировщика</summary>
    /// <returns>Перечисление содержащее все запланированные задачи</returns>
    protected override IEnumerable<Task> GetScheduledTasks()
    {
        // Отслеживать все найденные задачи
        var tasks = new List<Task>();

        // Получить все глобальные задачи. Используем TryEnter чтобы не зависнуть
        // в отладчике если блокировка удерживается замороженным потоком
        var lock_taken = false;
        try
        {
            Monitor.TryEnter(_Queue, ref lock_taken);
            if (lock_taken) tasks.AddRange(_Queue.ToArray());
            else throw new NotSupportedException();
        }
        finally
        {
            if (lock_taken) Monitor.Exit(_Queue);
        }

        // Теперь получить все задачи из очередей с кражей работ
        var queues = _TaskQueues;
        for (var i = 0; i < queues.Length; i++)
        {
            var wsq = queues[i];
            if (wsq != null) tasks.AddRange(wsq.ToArray());
        }

        // Вернуть отладчику все собранные экземпляры задач
        return tasks;
    }

    /// <summary>Добавить очередь с кражей работ в набор очередей</summary>
    /// <param name="queue">Очередь для добавления</param>
    private void AddQueue(WorkStealingQueue<Task> queue)
    {
        lock (_TaskQueues)
        {
            // Найти следующий свободный слот в массиве. Если найдём,
            // сохраним очередь и готово
            int i;
            for (i = 0; i < _TaskQueues.Length; i++)
                if (_TaskQueues[i] is null)
                {
                    _TaskQueues[i] = queue;
                    return;
                }

            // Не удалось найти свободный слот, поэтому удвоим длину массива
            // создав новый, скопировав данные
            // и сохранив новый. Здесь i == _TaskQueues.Length
            var queues = new WorkStealingQueue<Task>[i * 2];
            Array.Copy(_TaskQueues, queues, i);
            queues[i] = queue;
            _TaskQueues = queues;
        }
    }

    /// <summary>Удалить очередь с кражей работ из набора очередей</summary>
    /// <param name="wsq">Очередь с кражей работ для удаления</param>
    private void RemoveWsq(WorkStealingQueue<Task> wsq)
    {
        lock (_TaskQueues)
            // Найти очередь и если найдём, то обнулить её слот в массиве
            for (var i = 0; i < _TaskQueues.Length; i++)
                if (_TaskQueues[i] == wsq)
                    _TaskQueues[i] = null!;
    }

    /// <summary>Цикл диспетчера, выполняемый каждым потоком планировщика</summary>
    private void DispatchLoop()
    {
        // Создать новую очередь для этого потока, сохранить её в TLS для последующего получения,
        // и добавить её в набор очередей для этого планировщика
        var queue = new WorkStealingQueue<Task>();
        __ThreadTaskQueue = queue;
        AddQueue(queue);

        try
        {
            // Пока есть работа для выполнения...
            while (true)
            {
                // Порядок поиска: (1) локальная очередь с кражей, (2) глобальная очередь, (3) кража из других очередей
                if (!queue.LocalPop(out var task))
                {
                    // Не удалось получить задачу из локальной очереди с кражей
                    var searched_for_steals = false;
                    while (true)
                    {
                        lock (_Queue)
                        {
                            // Если запрошено завершение, выйти из потока
                            if (_Shutdown)
                                return;

                            // (2) попробовать глобальную очередь
                            if (_Queue.Count != 0)
                            {
                                // Нашли рабочую задачу! Возьмём её...
                                task = _Queue.Dequeue();
                                break;
                            }

                            if (searched_for_steals)
                            {
                                // Отметить что мы не ждём работу, а потом ждём
                                _ThreadsWaiting++;
                                try { Monitor.Wait(_Queue); } finally { _ThreadsWaiting--; }

                                // Если мы получили сигнал завершения, выйти из потока
                                if (_Shutdown)
                                    return;

                                searched_for_steals = false;
                                continue;
                            }
                        }

                        // (3) попробовать украсть
                        var ws_queues = _TaskQueues;
                        int i;
                        for (i = 0; i < ws_queues.Length; i++)
                        {
                            var q = ws_queues[i];
                            if (q != null && q != queue && q.TrySteal(out task)) break;
                        }

                        if (i != ws_queues.Length) break;

                        searched_for_steals = true;
                    }
                }

                // ...и выполнить её
                TryExecuteTask(task!);
            }
        }
        finally
        {
            RemoveWsq(queue);
        }
    }

    /// <summary>Сигнализировать планировщику о завершении и дождаться окончания всех потоков</summary>
    public void Dispose()
    {
        _Shutdown = true;
        if (_Queue is null || !_Threads.IsValueCreated) return;
        var threads = _Threads.Value;
        lock (_Queue) Monitor.PulseAll(_Queue);
        foreach (var thread in threads)
            thread.Join();
    }
}