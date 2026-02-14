using System.Diagnostics;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers;

/// <summary>Предоставляет согласованные планировщики задач для параллельного и эксклюзивного выполнения</summary>
/// <example>
/// <code>
/// var interleave = new ConcurrentExclusiveInterleave();
///
/// var exclusive = Task.Factory.StartNew(
///     () =>
///     {
///         // код, который должен выполняться строго один за раз
///     },
///     CancellationToken.None,
///     TaskCreationOptions.None,
///     interleave.ExclusiveTaskScheduler);
///
/// var concurrent = Enumerable.Range(0, 4)
///     .Select(i => Task.Factory.StartNew(
///         () =>
///         {
///             // задачи этого типа могут выполняться параллельно
///         },
///         CancellationToken.None,
///         TaskCreationOptions.None,
///         interleave.ConcurrentTaskScheduler))
///     .ToArray();
///
/// Task.WaitAll([.. concurrent, exclusive]);
/// </code>
/// </example>
[DebuggerDisplay("ConcurrentTasksWaiting={ConcurrentTaskCount}, ExclusiveTasksWaiting={ExclusiveTaskCount}")]
[DebuggerTypeProxy(typeof(ConcurrentExclusiveInterleaveDebugView))]
public sealed class ConcurrentExclusiveInterleave
{
    /// <summary>Отладочное представление для <see cref="ConcurrentExclusiveInterleave"/></summary>
    /// <param name="Interleave">Экземпляр, для которого формируется отладочное представление</param>
    /// <remarks>
    /// Инициализирует новый экземпляр отладочного представления
    /// </remarks>
    private class ConcurrentExclusiveInterleaveDebugView(ConcurrentExclusiveInterleave Interleave)
    {
        /// <summary>Экземпляр, для которого формируется отладочное представление</summary>
        private readonly ConcurrentExclusiveInterleave _Interleave = Interleave ?? throw new ArgumentNullException(nameof(Interleave));

        public IEnumerable<Task> ExclusiveTasksWaiting => _Interleave._ExclusiveTaskScheduler.Tasks;

        /// <summary>Задачи, ожидающие запуска в параллельном планировщике</summary>
        public IEnumerable<Task> ConcurrentTasksWaiting => _Interleave._ConcurrentTaskScheduler.Tasks;

        /// <summary>Текущая задача обработки очередей (для отладки)</summary>
        public Task? InterleaveTask => _Interleave._TaskExecuting;
    }

    /// <summary>Синхронизирует всю активность в этом типе и созданных им планировщиках</summary>
#if NET9_0_OR_GREATER
    private readonly Lock _InternalLock;
#else
    private readonly object _InternalLock;
#endif

    /// <summary>Параметры параллельного выполнения для фоновой задачи обработки и циклов</summary>
    private readonly ParallelOptions _ParallelOptions;

    /// <summary>Планировщик для «читателей», которые могут выполняться одновременно</summary>
    private readonly ConcurrentExclusiveTaskScheduler _ConcurrentTaskScheduler;

    /// <summary>Планировщик для «писателей», которые должны выполняться эксклюзивно</summary>
    private readonly ConcurrentExclusiveTaskScheduler _ExclusiveTaskScheduler;

    /// <summary>Задача, выполняющая обработку очередей (если запущена)</summary>
    private Task? _TaskExecuting;

    /// <summary>Нужно ли при эксклюзивной обработке учитывать дочерние задачи</summary>
    private readonly bool _ExclusiveProcessingIncludesChildren;

    /// <summary>Инициализирует новый экземпляр <see cref="ConcurrentExclusiveInterleave"/></summary>
    public ConcurrentExclusiveInterleave() : this(TaskScheduler.Current) { }

    /// <summary>Инициализирует новый экземпляр <see cref="ConcurrentExclusiveInterleave"/></summary>
    /// <param name="ExclusiveProcessingIncludesChildren">Признак того, что эксклюзивная обработка должна учитывать дочерние задачи</param>
    public ConcurrentExclusiveInterleave(bool ExclusiveProcessingIncludesChildren)
        : this(TaskScheduler.Current, ExclusiveProcessingIncludesChildren) { }

    /// <summary>Инициализирует новый экземпляр <see cref="ConcurrentExclusiveInterleave"/></summary>
    /// <param name="TargetScheduler">Целевой планировщик, на котором будет выполняться обработчик очередей</param>
    /// <param name="ExclusiveProcessingIncludesChildren">Признак того, что эксклюзивная обработка должна учитывать дочерние задачи</param>
    /// <exception cref="ArgumentNullException">Если <paramref name="TargetScheduler"/> равен <see langword="null"/></exception>
    public ConcurrentExclusiveInterleave(TaskScheduler TargetScheduler, bool ExclusiveProcessingIncludesChildren = false)
    {
        // Создаём состояние интерлива
        _ParallelOptions = new() { TaskScheduler = TargetScheduler.NotNull() };
        _InternalLock = new();
        _ExclusiveProcessingIncludesChildren = ExclusiveProcessingIncludesChildren;
        _ConcurrentTaskScheduler = new(this, [], TargetScheduler.MaximumConcurrencyLevel);
        _ExclusiveTaskScheduler = new(this, [], 1);
    }

    /// <summary>Планировщик, позволяющий выполнять задачи параллельно (возможны одновременные «читатели»)</summary>
    public TaskScheduler ConcurrentTaskScheduler => _ConcurrentTaskScheduler;

    /// <summary>Планировщик, требующий эксклюзивного выполнения задачи (один «писатель» и без «читателей»)</summary>
    public TaskScheduler ExclusiveTaskScheduler => _ExclusiveTaskScheduler;

    /// <summary>Количество задач, ожидающих эксклюзивного выполнения</summary>
    private int ExclusiveTaskCount { get { lock (_InternalLock) return _ExclusiveTaskScheduler.Tasks.Count; } }
    /// <summary>Количество задач, ожидающих параллельного выполнения</summary>
    private int ConcurrentTaskCount { get { lock (_InternalLock) return _ConcurrentTaskScheduler.Tasks.Count; } }

    /// <summary>Уведомляет интерлив о поступлении новой работы</summary>
    /// <remarks>Метод должен вызываться только при удержании внутренней блокировки</remarks>
    private void NotifyOfNewWork()
    {
        // Если обработчик уже запущен — выходим
        if (_TaskExecuting != null) return;

        // Иначе запускаем обработчик. Сначала сохраняем ссылку на задачу, затем стартуем,
        // чтобы присваивание произошло до начала выполнения тела
        _TaskExecuting = new(ConcurrentExclusiveInterleaveProcessor, CancellationToken.None, TaskCreationOptions.None);
        _TaskExecuting.Start(_ParallelOptions.TaskScheduler.NotNull("Не задан планировщик задач"));
    }

    /// <summary>Тело обработчика очередей, выполняемое в единственном экземпляре задачи</summary>
    /// <remarks>Вынесено в отдельный метод для улучшения отображения в окне Parallel Tasks</remarks>
    private void ConcurrentExclusiveInterleaveProcessor()
    {
        // Работаем, пока есть задачи для обработки
        var run_tasks = true;
        var cleanup_on_exit = true;
        while (run_tasks)
            try
            {
                // Обрабатываем все ожидающие эксклюзивные задачи
                foreach (var task in GetExclusiveTasks())
                {
                    _ExclusiveTaskScheduler.ExecuteTask(task);

                    // Выполнение не означает завершение: у задачи могут быть дочерние задачи,
                    // которые завершатся позже асинхронно. Если нужно учитывать дочерние задачи
                    // и задача ещё не завершена — выходим, оставляя обработчик «запущенным».
                    // Когда задача завершится, вернёмся и продолжим обработку.
                    // Важно: дочерние задачи не должны планироваться в этот же интерлив, иначе будет deadlock
                    if (!_ExclusiveProcessingIncludesChildren || task.IsCompleted) continue;
                    cleanup_on_exit = false;
                    task.ContinueWith(_ => ConcurrentExclusiveInterleaveProcessor(), _ParallelOptions.TaskScheduler.NotNull("Не задан планировщик задач"));
                    return;
                }

                // Обрабатываем параллельные задачи до момента появления эксклюзивных
                Parallel.ForEach(GetConcurrentTasksUntilExclusiveExists(), _ParallelOptions,
                    ExecuteConcurrentTask);
            }
            finally
            {
                if (cleanup_on_exit)
                    lock (_InternalLock) // Если задач не осталось — завершаем работу, иначе идём на следующий цикл
                        if (_ConcurrentTaskScheduler.Tasks.Count == 0 && _ExclusiveTaskScheduler.Tasks.Count == 0)
                        {
                            _TaskExecuting = null;
                            run_tasks = false;
                        }
            }
    }

    /// <summary>Выполняет параллельную задачу</summary>
    /// <param name="task">Задача для выполнения</param>
    /// <remarks>Вынесено в отдельный метод для улучшения отображения в окне Parallel Tasks</remarks>
    private void ExecuteConcurrentTask(Task task) => _ConcurrentTaskScheduler.ExecuteTask(task);

    /// <summary>
    /// Перечисление, выдающее ожидающие параллельные задачи по одной до те,
    /// пока не закончатся параллельные задачи или не появятся эксклюзивные</summary>
    private IEnumerable<Task> GetConcurrentTasksUntilExclusiveExists()
    {
        while (true)
        {
            Task? found_task = null;
            lock (_InternalLock)
                if (_ExclusiveTaskScheduler.Tasks.Count == 0 && _ConcurrentTaskScheduler.Tasks.Count > 0)
                    found_task = _ConcurrentTaskScheduler.Tasks.Dequeue();

            if (found_task is null) yield break;
            yield return found_task;
        }
    }

    /// <summary>Перечисление, выдающее все ожидающие эксклюзивные задачи по одной</summary>
    private IEnumerable<Task> GetExclusiveTasks()
    {
        while (true)
        {
            Task? found_task = null;
            lock (_InternalLock)
                if (_ExclusiveTaskScheduler.Tasks.Count > 0)
                    found_task = _ExclusiveTaskScheduler.Tasks.Dequeue();

            if (found_task is null) yield break;
            yield return found_task;
        }
    }

    /// <summary>Планировщик-адаптер для постановки задач в интерлив и выполнения по запросу обработчика интерлива</summary>
    private class ConcurrentExclusiveTaskScheduler : TaskScheduler
    {
        /// <summary>Родительский интерлив</summary>
        private readonly ConcurrentExclusiveInterleave _Interleave;

        /// <summary>Максимальный уровень параллелизма для планировщика</summary>
        private readonly int _MaximumConcurrencyLevel;

        /// <summary>Признак того, что текущий поток выполняет задачу в рамках этого планировщика</summary>
        private readonly ThreadLocal<bool> _ProcessingTaskOnCurrentThread = new();

        /// <summary>Инициализирует новый экземпляр планировщика</summary>
        /// <param name="interleave">Родительский интерлив</param>
        /// <param name="tasks">Очередь, в которую будут помещаться задачи</param>
        /// <param name="MaximumConcurrencyLevel">Максимальный уровень параллелизма</param>
        internal ConcurrentExclusiveTaskScheduler(ConcurrentExclusiveInterleave interleave, Queue<Task> tasks, int MaximumConcurrencyLevel)
        {
            _Interleave = interleave.NotNull();
            Tasks = tasks.NotNull();
            _MaximumConcurrencyLevel = MaximumConcurrencyLevel;
        }

        /// <summary>Максимальный уровень параллелизма, поддерживаемый планировщиком</summary>
        public override int MaximumConcurrencyLevel => _MaximumConcurrencyLevel;

        /// <summary>Очередь задач планировщика</summary>
        internal Queue<Task> Tasks { get; }

        /// <summary>Ставит задачу в очередь планировщика</summary>
        /// <param name="task">Задача для постановки в очередь</param>
        protected override void QueueTask(Task task)
        {
            lock (_Interleave._InternalLock)
            {
                Tasks.Enqueue(task);
                _Interleave.NotifyOfNewWork();
            }
        }

        /// <summary>Выполняет задачу в рамках данного планировщика</summary>
        /// <param name="task">Задача для выполнения</param>
        internal void ExecuteTask(Task task)
        {
            var processing_task_on_current_thread = _ProcessingTaskOnCurrentThread.Value;
            if (!processing_task_on_current_thread) _ProcessingTaskOnCurrentThread.Value = true;
            TryExecuteTask(task);
            if (!processing_task_on_current_thread) _ProcessingTaskOnCurrentThread.Value = false;
        }

        /// <summary>Пытается выполнить задачу синхронно в текущем потоке, если это допустимо</summary>
        /// <param name="task">Задача для выполнения</param>
        /// <param name="TaskWasPreviouslyQueued">Признак того, что задача ранее ставилась в очередь</param>
        /// <returns>Истина, если задачу удалось выполнить встроенно</returns>
        protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued)
        {
            if (!_ProcessingTaskOnCurrentThread.Value) return false;
            var t = new Task<bool>(state => TryExecuteTask((Task)state!), task);
            t.RunSynchronously(_Interleave._ParallelOptions.TaskScheduler.NotNull("Не задан планировщик задач"));
            return t.Result;
        }

        /// <summary>Возвращает задачи, находящиеся в очереди (для отладки)</summary>
        /// <returns>Перечисление задач, ожидающих выполнения</returns>
        protected override IEnumerable<Task> GetScheduledTasks() => Tasks;
    }
}