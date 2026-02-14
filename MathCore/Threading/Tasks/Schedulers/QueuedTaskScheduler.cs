using System.Collections.Concurrent;
using System.Diagnostics;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers;

/// <summary>Планировщик обеспечивает постановку задач в очередь с приоритетами</summary>
/// <remarks>
/// Планировщик поддерживает два режима работы:
/// 1) запуск задач на собственной группе потоков;
/// 2) упаковка задач и их выполнение поверх заданного <see cref="TaskScheduler"/> с ограничением параллелизма
/// </remarks>
/// <example>
/// <code>
/// using var scheduler = new QueuedTaskScheduler(ThreadCount: 2, ThreadName: "QTS");
///
/// var high_priority = scheduler.ActivateNewQueue(priority: -1);
/// var normal       = scheduler.ActivateNewQueue(priority: 0);
///
/// var t1 = Task.Factory.StartNew(() => { /* срочно */ },
///     CancellationToken.None, TaskCreationOptions.None, high_priority);
///
/// var t2 = Task.Factory.StartNew(() => { /* обычно */ },
///     CancellationToken.None, TaskCreationOptions.None, normal);
///
/// Task.WaitAll(t1, t2);
/// </code>
/// </example>
[DebuggerTypeProxy(typeof(QueuedTaskSchedulerDebugView))]
[DebuggerDisplay("Id={Id}, Queues={DebugQueueCount}, ScheduledTasks = {DebugTaskCount}")]
public sealed class QueuedTaskScheduler : TaskScheduler, IDisposable
{
    /// <summary>Отладочное представление</summary>
    /// <remarks>Инициализация отладочного представления для планировщика</remarks>
    /// <param name="scheduler">Рассматриваемый планировщик</param>
    private class QueuedTaskSchedulerDebugView(QueuedTaskScheduler scheduler)
    {
        /// <summary>Экземпляр планировщика</summary>
        private readonly QueuedTaskScheduler _Scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));

        /// <summary>Извлечение всех задач, запланированных непосредственно в планировщике</summary>
        public IEnumerable<Task> ScheduledTasks
        {
            get
            {
                var tasks = _Scheduler._TargetScheduler is null
                    ? (IEnumerable<Task>)_Scheduler._BlockingTaskQueue!
                    : _Scheduler._NonThreadSafeTaskQueue!;
                return [.. tasks.Where(t => t != null)];
            }
        }

        /// <summary>Все очереди задач</summary>
        public IEnumerable<TaskScheduler> Queues => _Scheduler._QueueGroups.SelectMany(group => group.Value);
    }

    /// <summary>
    /// Отсортированный список циклических списков.<br/>
    /// Задачи с малыми приоритетами являются предпочтительными.<br/>
    /// Группы приоритетов являются циклическими в пределах одного уровня приоритета.
    /// </summary>
    private readonly SortedList<int, QueueGroup> _QueueGroups = [];

    /// <summary>Система отмены задач в случае вызова метода <see cref="Dispose"/></summary>
    private readonly CancellationTokenSource _DisposeCancellation = new();

    /// <summary>
    /// Максимально допустимый уровень конкурентности для текущего планировщика.<br/>
    /// Если используются вручную создаваемые потоки, то данное поле отображает число создаваемых потоков.
    /// </summary>
    private readonly int _ConcurrencyLevel;

    /// <summary>Признак того, что текущий поток обрабатывает задачи этого планировщика</summary>
    private static readonly ThreadLocal<bool> __TaskProcessingThread = new();

    // ***
    // *** Режим выполнения поверх целевого планировщика
    // ***

    /// <summary>Целевой планировщик, поверх которого запускается обработка</summary>
    private readonly TaskScheduler? _TargetScheduler;

    /// <summary>Очередь задач для режима выполнения поверх целевого планировщика</summary>
    private readonly Queue<Task?>? _NonThreadSafeTaskQueue;

    /// <summary>Число задач, поставленных в очередь или выполняющихся в режиме поверх целевого планировщика</summary>
    private int _DelegatesQueuedOrRunning;

    // ***
    // *** Режим выполнения на собственных потоках
    // ***

    /// <summary>Блокирующая коллекция задач для выполнения на собственных потоках</summary>
    private readonly BlockingCollection<Task?>? _BlockingTaskQueue;

    // ***

    /// <summary>Инициализирует новый экземпляр <see cref="QueuedTaskScheduler"/></summary>
    public QueuedTaskScheduler() : this(Default) { }

    /// <summary>Инициализирует новый экземпляр <see cref="QueuedTaskScheduler"/></summary>
    /// <param name="TargetScheduler">Целевой планировщик, в который будет направляться работа</param>
    /// <param name="MaxConcurrencyLevel">Максимальный уровень параллелизма для обработчика</param>
    /// <exception cref="ArgumentNullException">Если <paramref name="TargetScheduler"/> равен <see langword="null"/></exception>
    /// <exception cref="ArgumentOutOfRangeException">Если <paramref name="MaxConcurrencyLevel"/> меньше 0</exception>
    public QueuedTaskScheduler(TaskScheduler TargetScheduler, int MaxConcurrencyLevel = 0)
    {
        // Проверка аргументов
        if (MaxConcurrencyLevel < 0) throw new ArgumentOutOfRangeException(nameof(MaxConcurrencyLevel));

        // Инициализируем только поля для режима выполнения поверх целевого планировщика
        _TargetScheduler = TargetScheduler ?? throw new ArgumentNullException(nameof(TargetScheduler));
        _NonThreadSafeTaskQueue = [];

        // If 0, use the number of logical processors.  But make sure whatever value we pick
        // is not greater than the degree of parallelism allowed by the underlying scheduler.
        _ConcurrencyLevel = MaxConcurrencyLevel != 0 ? MaxConcurrencyLevel : Environment.ProcessorCount;
        if (TargetScheduler.MaximumConcurrencyLevel > 0 && TargetScheduler.MaximumConcurrencyLevel < _ConcurrencyLevel)
            _ConcurrencyLevel = TargetScheduler.MaximumConcurrencyLevel;
    }

    /// <summary>Initializes the scheduler.</summary>
    /// <param name="ThreadCount">The number of threads to create and use for processing work items.</param>
    public QueuedTaskScheduler(int ThreadCount) : this(ThreadCount, string.Empty) { }

    /// <summary>Initializes the scheduler.</summary>
    /// <param name="ThreadCount">The number of threads to create and use for processing work items.</param>
    /// <param name="ThreadName">The name to use for each of the created threads.</param>
    /// <param name="UseForegroundThreads">A Boolean value that indicates whether to use foreground threads instead of background.</param>
    /// <param name="ThreadPriority">The priority to assign to each thread.</param>
    /// <param name="ThreadApartmentState">The apartment state to use for each thread.</param>
    /// <param name="ThreadMaxStackSize">The stack size to use for each thread.</param>
    /// <param name="ThreadInit">An initialization routine to run on each thread.</param>
    /// <param name="ThreadFinally">A finalization routine to run on each thread.</param>
    public QueuedTaskScheduler(
        int ThreadCount,
        string ThreadName = "",
        bool UseForegroundThreads = false,
        ThreadPriority ThreadPriority = ThreadPriority.Normal,
        ApartmentState ThreadApartmentState = ApartmentState.MTA,
        int ThreadMaxStackSize = 0,
        Action? ThreadInit = null,
        Action? ThreadFinally = null)
    {
        _ConcurrencyLevel = ThreadCount switch
        {
            > 0 => ThreadCount,
            0 => Environment.ProcessorCount,
            _ => throw new ArgumentOutOfRangeException(nameof(ThreadCount))
        };

        //// Validates arguments (some validation is left up to the Thread type itself).
        //// If the thread count is 0, default to the number of logical processors.
        //if (ThreadCount < 0) throw new ArgumentOutOfRangeException(nameof(ThreadCount));

        //_ConcurrencyLevel = ThreadCount == 0 
        //    ? Environment.ProcessorCount 
        //    : ThreadCount;

        // Инициализируем очередь задач
        _BlockingTaskQueue = [];

        // Создаём потоки
        var threads = new Thread[ThreadCount];
        for (var i = 0; i < ThreadCount; i++)
        {
            threads[i] = new(() => ThreadBasedDispatchLoop(ThreadInit, ThreadFinally), ThreadMaxStackSize)
            {
                Priority = ThreadPriority,
                IsBackground = !UseForegroundThreads,
            };
            if (ThreadName != null) threads[i].Name = $"{ThreadName} ({i})";
#pragma warning disable CA1416
            threads[i].SetApartmentState(ThreadApartmentState);
#pragma warning restore CA1416
        }

        // Запускаем потоки
        foreach (var thread in threads)
            thread.Start();
    }

    /// <summary>Цикл диспетчеризации, выполняемый каждым потоком планировщика</summary>
    /// <param name="ThreadInit">Инициализация, выполняемая при старте потока</param>
    /// <param name="ThreadFinally">Завершение, выполняемое перед остановкой потока</param>
    private void ThreadBasedDispatchLoop(Action? ThreadInit, Action? ThreadFinally)
    {
        __TaskProcessingThread.Value = true;
        ThreadInit?.Invoke();
        try
        {
            // If a thread abort occurs, we'll try to reset it and continue running.
            while (true)
                try
                {
                    // Для каждой задачи, поставленной в очередь, пытаемся выполнить её
                    foreach (var task in _BlockingTaskQueue!.GetConsumingEnumerable(_DisposeCancellation.Token))
                        // Если задача не null — это задача, поставленная непосредственно в планировщик
                        if (task != null)
                            TryExecuteTask(task);
                        // Если задача null — это маркер задач подочередей; тогда выбираем следующую задачу
                        // по приоритету и справедливости
                        else
                        {
                            // Находим следующую задачу по правилам упорядочивания
                            Task? target_task;
                            QueuedTaskSchedulerQueue? queue_for_target_task;
                            lock (_QueueGroups)
                                FindNextTask_NeedsLock(out target_task, out queue_for_target_task);

                            // Если нашли — выполняем
                            if (target_task != null)
                                queue_for_target_task!.ExecuteTask(target_task);
                        }
                }
                catch (ThreadAbortException)
                {
                    // Если вызвана отмена работы потока в ходе завершения работы системы, или выгрузки домена
                    // Если получена отмена потока из-за завершения процесса/выгрузки домена — пропускаем,
                    // иначе сбрасываем abort и продолжаем работу
#if NET5_0_OR_GREATER
                    throw;
#else
                    if (!Environment.HasShutdownStarted && !AppDomain.CurrentDomain.IsFinalizingForUnload())
                        Thread.ResetAbort();
#endif
                }
        }
        catch (OperationCanceledException) { }
        finally
        {
            // Run a cleanup routine if there was one
            ThreadFinally?.Invoke();
            __TaskProcessingThread.Value = false;
        }
    }

    /// <summary>Количество активированных очередей</summary>
    private int DebugQueueCount => _QueueGroups.Sum(group => group.Value.Count);

    /// <summary>Количество запланированных задач (для отладки)</summary>
    private int DebugTaskCount => (_TargetScheduler is null ? (IEnumerable<Task?>)_BlockingTaskQueue! : _NonThreadSafeTaskQueue!).Count(t => t != null);

    /// <summary>Ищет следующую задачу для выполнения с учётом приоритетов и справедливости</summary>
    /// <param name="TargetTask">Найденная задача или <see langword="null"/>, если задач нет</param>
    /// <param name="QueueForTargetTask">
    /// Планировщик, связанный с найденной задачей
    /// Из-за проверок безопасности внутри TPL этот планировщик должен использоваться для запуска
    /// </param>
    private void FindNextTask_NeedsLock(out Task? TargetTask, out QueuedTaskSchedulerQueue? QueueForTargetTask)
    {
        TargetTask = null;
        QueueForTargetTask = null;

        // Проходим группы очередей в порядке приоритета
        foreach (var (_, queues) in _QueueGroups)
            // Внутри группы применяем round-robin: следующий поиск стартует со следующей позиции
            foreach (var i in queues.CreateSearchOrder())
            {
                QueueForTargetTask = queues[i];
                var items = QueueForTargetTask.WorkItems;
                if (items.Count == 0) continue;
                TargetTask = items.Dequeue();
                if (QueueForTargetTask.Disposed && items.Count == 0)
                    RemoveQueue_NeedsLock(QueueForTargetTask);
                queues.NextQueueIndex = (queues.NextQueueIndex + 1) % queues.Count;
                return;
            }
    }

    /// <summary>Queues a task to the scheduler.</summary>
    /// <param name="task">The task to be queued.</param>
    protected override void QueueTask(Task? task)
    {
        // If we've been disposed, no one should be queueing
        if (_DisposeCancellation.IsCancellationRequested)
            throw new ObjectDisposedException(GetType().Name);

        // If the target scheduler is null (meaning we're using our own threads),
        // add the task to the blocking queue
        if (_TargetScheduler == null)
            _BlockingTaskQueue!.Add(task);
        // Otherwise, add the task to the non-blocking queue,
        // and if there isn't already an executing processing task,
        // start one up
        else
        {
            // Queue the task and check whether we should launch a processing
            // task (noting it if we do, so that other threads don't result
            // in queueing up too many).
            var launch_task = false;
            lock (_NonThreadSafeTaskQueue!)
            {
                _NonThreadSafeTaskQueue.Enqueue(task);
                if (_DelegatesQueuedOrRunning < _ConcurrencyLevel)
                {
                    _DelegatesQueuedOrRunning++;
                    launch_task = true;
                }
            }

            // If necessary, start processing asynchronously
            if (launch_task)
                Task.Factory.StartNew(ProcessPrioritizedAndBatchedTasks, CancellationToken.None, TaskCreationOptions.None, _TargetScheduler);
        }
    }

    /// <summary>
    /// Process tasks one at a time in the best order.<br/>
    /// This should be run in a Task generated by QueueTask.<br/>
    /// It's been separated out into its own method to show up better in Parallel Tasks.
    /// </summary>
    private void ProcessPrioritizedAndBatchedTasks()
    {
        var continue_processing = true;
        while (!_DisposeCancellation.IsCancellationRequested && continue_processing)
            try
            {
                // Отмечаем, что текущий поток обрабатывает задачи
                __TaskProcessingThread.Value = true;

                // Until there are no more tasks to process
                while (!_DisposeCancellation.IsCancellationRequested)
                {
                    // Берём следующую задачу; если задач больше нет — завершаем
                    Task? target_task;
                    lock (_NonThreadSafeTaskQueue!)
                    {
                        if (_NonThreadSafeTaskQueue.Count == 0) break;
                        target_task = _NonThreadSafeTaskQueue.Dequeue();
                    }

                    // Если задача null — это маркер round-robin очередей; тогда выбираем настоящую задачу
                    QueuedTaskSchedulerQueue? queue_for_target_task = null;
                    if (target_task is null)
                        lock (_QueueGroups)
                            FindNextTask_NeedsLock(out target_task, out queue_for_target_task);

                    // Если задача найдена — выполняем; если она из round-robin очереди, используем связанный планировщик
                    if (target_task is null) continue;
                    if (queue_for_target_task is null)
                        TryExecuteTask(target_task);
                    else
                        queue_for_target_task.ExecuteTask(target_task);
                }
            }
            finally
            {
                // Проверяем, действительно ли больше нет работы; если нет — уменьшаем число обработчиков
                lock (_NonThreadSafeTaskQueue!)
                    if (_NonThreadSafeTaskQueue.Count == 0)
                    {
                        _DelegatesQueuedOrRunning--;
                        continue_processing = false;
                        __TaskProcessingThread.Value = false;
                    }
            }
    }

    /// <summary>Уведомляет планировщик о появлении работы в одной из round-robin очередей</summary>
    private void NotifyNewWorkItem() => QueueTask(null);

    /// <summary>Пытается выполнить задачу синхронно в текущем потоке</summary>
    /// <param name="task">Задача для выполнения</param>
    /// <param name="TaskWasPreviouslyQueued">Признак предварительной постановки в очередь</param>
    /// <returns>Истина, если удалось выполнить задачу встроенно</returns>
    protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) =>
        // Если текущий поток уже обрабатывает задачи, разрешаем inlining
        __TaskProcessingThread.Value && TryExecuteTask(task);

    /// <summary>Возвращает задачи, запланированные непосредственно в этот планировщик</summary>
    /// <returns>Перечисление задач, поставленных в этот планировщик</returns>
    /// <remarks>
    /// Задачи подочередей сюда не входят и извлекаются отладчиком отдельно
    /// </remarks>
    protected override IEnumerable<Task> GetScheduledTasks()
    {
        // Если работаем на собственных потоках — берём задачи из блокирующей очереди
        if (_TargetScheduler is null)
            // Фильтруем null, которые выступают маркерами задач подочередей
            return _BlockingTaskQueue!.Where(t => t != null).ToArray()!;

        // Иначе берём из неблокирующей очереди
        return _NonThreadSafeTaskQueue!.Where(t => t != null).ToArray()!;
    }

    /// <summary>Максимальный уровень параллелизма при обработке задач</summary>
    public override int MaximumConcurrencyLevel => _ConcurrencyLevel;

    /// <summary>Инициирует остановку планировщика</summary>
    public void Dispose() => _DisposeCancellation.Cancel();

    /// <summary>Создаёт и активирует новую очередь планирования для данного планировщика</summary>
    /// <returns>Созданная и активированная очередь с приоритетом 0</returns>
    public TaskScheduler ActivateNewQueue() => ActivateNewQueue(0);

    /// <summary>Создаёт и активирует новую очередь планирования для данного планировщика</summary>
    /// <param name="priority">Приоритет очереди (меньше — выше приоритет)</param>
    /// <returns>Созданная и активированная очередь с указанным приоритетом</returns>
    public TaskScheduler ActivateNewQueue(int priority)
    {
        // Создаём очередь
        var created_queue = new QueuedTaskSchedulerQueue(priority, this);

        // Добавляем очередь в группу по приоритету
        lock (_QueueGroups)
        {
            if (!_QueueGroups.TryGetValue(priority, out var list))
            {
                list = new();
                _QueueGroups.Add(priority, list);
            }
            list.Add(created_queue);
        }

        // Возвращаем созданную очередь
        return created_queue;
    }

    /// <summary>Удаляет очередь из группы (метод требует удержания блокировки)</summary>
    /// <param name="queue">Очередь для удаления</param>
    private void RemoveQueue_NeedsLock(QueuedTaskSchedulerQueue queue)
    {
        // Находим группу и индекс очереди внутри группы
        var queue_group = _QueueGroups[queue.Priority];
        var index = queue_group.IndexOf(queue);

        // Корректируем индекс начала round-robin обхода
        if (queue_group.NextQueueIndex >= index) queue_group.NextQueueIndex--;

        // Удаляем
        queue_group.RemoveAt(index);
    }

    /// <summary>Группа очередей одного уровня приоритета</summary>
    private class QueueGroup : List<QueuedTaskSchedulerQueue>
    {
        /// <summary>Стартовый индекс для следующего round-robin обхода</summary>
        public int NextQueueIndex;

        /// <summary>Формирует порядок обхода очередей в группе</summary>
        /// <returns>Последовательность индексов очередей</returns>
        public IEnumerable<int> CreateSearchOrder()
        {
            for (var i = NextQueueIndex; i < Count; i++) yield return i;
            for (var i = 0; i < NextQueueIndex; i++) yield return i;
        }
    }

    /// <summary>Очередь планирования, связанная с <see cref="QueuedTaskScheduler"/></summary>
    [DebuggerDisplay("QueuePriority = {Priority}, WaitingTasks = {WaitingTasks}")]
    [DebuggerTypeProxy(typeof(QueuedTaskSchedulerQueueDebugView))]
    private sealed class QueuedTaskSchedulerQueue : TaskScheduler, IDisposable
    {
        /// <summary>Отладочное представление очереди</summary>
        /// <remarks>Инициализирует новое отладочное представление</remarks>
        /// <param name="queue">Очередь, для которой строится представление</param>
        /// <exception cref="ArgumentNullException">Если <paramref name="queue"/> равен <see langword="null"/></exception>
        private sealed class QueuedTaskSchedulerQueueDebugView(QueuedTaskScheduler.QueuedTaskSchedulerQueue queue)
        {
            /// <summary>Экземпляр очереди</summary>
            private readonly QueuedTaskSchedulerQueue _Queue = queue ?? throw new ArgumentNullException(nameof(queue));

            /// <summary>Приоритет очереди в связанном планировщике</summary>
            public int Priority => _Queue.Priority;

            /// <summary>Идентификатор планировщика</summary>
            public int Id => _Queue.Id;

            /// <summary>Задачи, запланированные в данной очереди</summary>
            public IEnumerable<Task> ScheduledTasks => _Queue.GetScheduledTasks();

            /// <summary>Связанный планировщик</summary>
            public QueuedTaskScheduler AssociatedScheduler => _Queue._Pool;
        }

        /// <summary>Родительский планировщик</summary>
        private readonly QueuedTaskScheduler _Pool;
        /// <summary>Очередь задач</summary>
        internal readonly Queue<Task> WorkItems;
        /// <summary>Признак освобождения очереди</summary>
        internal bool Disposed;
        /// <summary>Приоритет очереди</summary>
        internal int Priority;

        /// <summary>Инициализирует новую очередь</summary>
        /// <param name="priority">Приоритет очереди</param>
        /// <param name="pool">Связанный планировщик</param>
        internal QueuedTaskSchedulerQueue(int priority, QueuedTaskScheduler pool)
        {
            Priority = priority;
            _Pool = pool;
            WorkItems = [];
        }

        /// <summary>Количество задач, ожидающих выполнения</summary>
        internal int WaitingTasks => WorkItems.Count;

        /// <summary>Возвращает задачи, запланированные в данную очередь</summary>
        /// <returns>Перечисление задач очереди</returns>
        protected override IEnumerable<Task> GetScheduledTasks() => [.. WorkItems];

        /// <summary>Ставит задачу в очередь</summary>
        /// <param name="task">Задача для постановки</param>
        /// <exception cref="ObjectDisposedException">Если очередь уже освобождена</exception>
        protected override void QueueTask(Task task)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().Name);

            // Ставим задачу в локальную очередь и уведомляем родителя о появлении работы
            lock (_Pool._QueueGroups) WorkItems.Enqueue(task);
            _Pool.NotifyNewWorkItem();
        }

        /// <summary>Пытается выполнить задачу синхронно в текущем потоке</summary>
        /// <param name="task">Задача для выполнения</param>
        /// <param name="TaskWasPreviouslyQueued">Признак предварительной постановки в очередь</param>
        /// <returns>Истина, если задачу удалось выполнить встроенно</returns>
        protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) =>
            // Если текущий поток обрабатывает задачи планировщика — разрешаем inlining
            __TaskProcessingThread.Value && TryExecuteTask(task);

        /// <summary>Выполняет задачу</summary>
        /// <param name="task">Задача для выполнения</param>
        internal void ExecuteTask(Task task) => TryExecuteTask(task);

        /// <summary>Максимальный уровень параллелизма при обработке задач</summary>
        /// <returns>
        /// В данном случае соответствует значению родительского планировщика
        /// </returns>
        public override int MaximumConcurrencyLevel => _Pool.MaximumConcurrencyLevel;

        /// <summary>Помечает очередь на удаление из планировщика после опустошения</summary>
        public void Dispose()
        {
            if (Disposed) return;
            lock (_Pool._QueueGroups)
                // Удаляем очередь только если она пуста
                // Если в ней есть задачи — помечаем как disposed, а удаление выполнит родитель,
                // когда счётчик задач дойдёт до 0
                if (WorkItems.Count == 0)
                    _Pool.RemoveQueue_NeedsLock(this);
            Disposed = true;
        }
    }
}