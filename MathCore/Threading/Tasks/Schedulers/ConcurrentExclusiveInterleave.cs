#nullable enable
using System.Diagnostics;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers;

/// <summary>Provides concurrent and exclusive task schedulers that coordinate.</summary>
[DebuggerDisplay("ConcurrentTasksWaiting={ConcurrentTaskCount}, ExclusiveTasksWaiting={ExclusiveTaskCount}")]
[DebuggerTypeProxy(typeof(ConcurrentExclusiveInterleaveDebugView))]
public sealed class ConcurrentExclusiveInterleave
{
    /// <summary>Provides a debug view for ConcurrentExclusiveInterleave.</summary>
    /// <remarks>Initializes the debug view.</remarks>
    /// <param name="Interleave">The interleave being debugged.</param>
    private class ConcurrentExclusiveInterleaveDebugView(ConcurrentExclusiveInterleave Interleave)
    {
        /// <summary>The interleave being debugged.</summary>
        private readonly ConcurrentExclusiveInterleave _Interleave = Interleave ?? throw new ArgumentNullException(nameof(Interleave));

        public IEnumerable<Task> ExclusiveTasksWaiting => _Interleave._ExclusiveTaskScheduler.Tasks;

        /// <summary>Gets the number of tasks waiting to run concurrently.</summary>
        public IEnumerable<Task> ConcurrentTasksWaiting => _Interleave._ConcurrentTaskScheduler.Tasks;

        /// <summary>Gets a description of the processing task for debugging purposes.</summary>
        public Task? InterleaveTask => _Interleave._TaskExecuting;
    }

    /// <summary>Synchronizes all activity in this type and its generated schedulers.</summary>
    private readonly object _InternalLock;

    /// <summary>The parallel options used by the asynchronous task and parallel loops.</summary>
    private readonly ParallelOptions _ParallelOptions;

    /// <summary>The scheduler used to queue and execute "reader" tasks that may run concurrently with other readers.</summary>
    private readonly ConcurrentExclusiveTaskScheduler _ConcurrentTaskScheduler;

    /// <summary>The scheduler used to queue and execute "writer" tasks that must run exclusively while no other tasks for this interleave are running.</summary>
    private readonly ConcurrentExclusiveTaskScheduler _ExclusiveTaskScheduler;

    /// <summary>Whether this interleave has queued its processing task.</summary>
    private Task? _TaskExecuting;

    /// <summary>Whether the exclusive processing of a task should include all of its children as well.</summary>
    private readonly bool _ExclusiveProcessingIncludesChildren;

    /// <summary>Initialies the ConcurrentExclusiveInterleave.</summary>
    public ConcurrentExclusiveInterleave() : this(TaskScheduler.Current) { }

    /// <summary>Initialies the ConcurrentExclusiveInterleave.</summary>
    /// <param name="ExclusiveProcessingIncludesChildren">Whether the exclusive processing of a task should include all of its children as well.</param>
    public ConcurrentExclusiveInterleave(bool ExclusiveProcessingIncludesChildren) 
        : this(TaskScheduler.Current, ExclusiveProcessingIncludesChildren) { }

    /// <summary>Initialies the ConcurrentExclusiveInterleave.</summary>
    /// <param name="TargetScheduler">The target scheduler on which this interleave should execute.</param>
    /// <param name="ExclusiveProcessingIncludesChildren">Whether the exclusive processing of a task should include all of its children as well.</param>
    public ConcurrentExclusiveInterleave(TaskScheduler TargetScheduler, bool ExclusiveProcessingIncludesChildren = false)
    {
        // A scheduler must be provided
        if (TargetScheduler is null) throw new ArgumentNullException(nameof(TargetScheduler));

        // Create the state for this interleave
        _InternalLock                        = new object();
        _ExclusiveProcessingIncludesChildren = ExclusiveProcessingIncludesChildren;
        _ParallelOptions                     = new ParallelOptions { TaskScheduler = TargetScheduler };
        _ConcurrentTaskScheduler             = new ConcurrentExclusiveTaskScheduler(this, new Queue<Task>(), TargetScheduler.MaximumConcurrencyLevel);
        _ExclusiveTaskScheduler              = new ConcurrentExclusiveTaskScheduler(this, new Queue<Task>(), 1);
    }

    /// <summary>
    /// Gets a TaskScheduler that can be used to schedule tasks to this interleave
    /// that may run concurrently with other tasks on this interleave.
    /// </summary>
    public TaskScheduler ConcurrentTaskScheduler => _ConcurrentTaskScheduler;

    /// <summary>
    /// Gets a TaskScheduler that can be used to schedule tasks to this interleave
    /// that must run exclusively with regards to other tasks on this interleave.
    /// </summary>
    public TaskScheduler ExclusiveTaskScheduler => _ExclusiveTaskScheduler;

    /// <summary>Gets the number of tasks waiting to run exclusively.</summary>
    private int ExclusiveTaskCount { get { lock (_InternalLock) return _ExclusiveTaskScheduler.Tasks.Count; } }
    /// <summary>Gets the number of tasks waiting to run concurrently.</summary>
    private int ConcurrentTaskCount { get { lock (_InternalLock) return _ConcurrentTaskScheduler.Tasks.Count; } }

    /// <summary>Notifies the interleave that new work has arrived to be processed.</summary>
    /// <remarks>Must only be called while holding the lock.</remarks>
    private void NotifyOfNewWork()
    {
        // If a task is already running, bail.  
        if (_TaskExecuting != null) return;

        // Otherwise, run the processor. Store the task and then start it to ensure that 
        // the assignment happens before the body of the task runs.
        _TaskExecuting = new Task(ConcurrentExclusiveInterleaveProcessor, CancellationToken.None, TaskCreationOptions.None);
        _TaskExecuting.Start(_ParallelOptions.TaskScheduler);
    }

    /// <summary>The body of the async processor to be run in a Task.  Only one should be running at a time.</summary>
    /// <remarks>This has been separated out into its own method to improve the Parallel Tasks window experience.</remarks>
    private void ConcurrentExclusiveInterleaveProcessor()
    {
        // Run while there are more tasks to be processed.  We assume that the first time through,
        // there are tasks.  If they aren't, worst case is we try to process and find none.
        var run_tasks       = true;
        var cleanup_on_exit = true;
        while (run_tasks)
            try
            {
                // Process all waiting exclusive tasks
                foreach (var task in GetExclusiveTasks())
                {
                    _ExclusiveTaskScheduler.ExecuteTask(task);

                    // Just because we executed the task doesn't mean it's "complete",
                    // if it has child tasks that have not yet completed
                    // and will complete later asynchronously.  To account for this, 
                    // if a task isn't yet completed, leave the interleave processor 
                    // but leave it still in a running state.  When the task completes,
                    // we'll come back in and keep going.  Note that the children
                    // must not be scheduled to this interleave, or this will deadlock.
                    if (!_ExclusiveProcessingIncludesChildren || task.IsCompleted) continue;
                    cleanup_on_exit = false;
                    task.ContinueWith(_ => ConcurrentExclusiveInterleaveProcessor(), _ParallelOptions.TaskScheduler);
                    return;
                }

                // Process all waiting concurrent tasks *until* any exclusive tasks show up, in which
                // case we want to switch over to processing those (by looping around again).
                Parallel.ForEach(GetConcurrentTasksUntilExclusiveExists(), _ParallelOptions,
                    ExecuteConcurrentTask);
            }
            finally
            {
                if (cleanup_on_exit)
                    lock (_InternalLock) // If there are no tasks remaining, we're done. If there are, loop around and go again.
                        if (_ConcurrentTaskScheduler.Tasks.Count == 0 && _ExclusiveTaskScheduler.Tasks.Count == 0)
                        {
                            _TaskExecuting = null;
                            run_tasks      = false;
                        }
            }
    }

    /// <summary>Runs a concurrent task.</summary>
    /// <param name="task">The task to execute.</param>
    /// <remarks>This has been separated out into its own method to improve the Parallel Tasks window experience.</remarks>
    private void ExecuteConcurrentTask(Task task) => _ConcurrentTaskScheduler.ExecuteTask(task);

    /// <summary>
    /// Gets an enumerable that yields waiting concurrent tasks one at a time until
    /// either there are no more concurrent tasks or there are any exclusive tasks.
    /// </summary>
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

    /// <summary>
    /// Gets an enumerable that yields all of the exclusive tasks one at a time.
    /// </summary>
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

    /// <summary>
    /// A scheduler shim used to queue tasks to the interleave and execute those tasks on request of the interleave.
    /// </summary>
    private class ConcurrentExclusiveTaskScheduler : TaskScheduler
    {
        /// <summary>The parent interleave.</summary>
        private readonly ConcurrentExclusiveInterleave _Interleave;
        /// <summary>The maximum concurrency level for the scheduler.</summary>
        private readonly int _MaximumConcurrencyLevel;
        /// <summary>Whether a Task is currently being processed on this thread.</summary>
        private readonly ThreadLocal<bool> _ProcessingTaskOnCurrentThread = new();

        /// <summary>Initializes the scheduler.</summary>
        /// <param name="interleave">The parent interleave.</param>
        /// <param name="tasks">The queue to store queued tasks into.</param>
        internal ConcurrentExclusiveTaskScheduler(ConcurrentExclusiveInterleave interleave, Queue<Task> tasks, int MaximumConcurrencyLevel)
        {
            _Interleave              = interleave ?? throw new ArgumentNullException(nameof(interleave));
            Tasks                    = tasks ?? throw new ArgumentNullException(nameof(tasks));
            _MaximumConcurrencyLevel = MaximumConcurrencyLevel;
        }

        /// <summary>Gets the maximum concurrency level this scheduler is able to support.</summary>
        public override int MaximumConcurrencyLevel => _MaximumConcurrencyLevel;

        /// <summary>Gets the queue of tasks for this scheduler.</summary>
        internal Queue<Task> Tasks { get; }

        /// <summary>Queues a task to the scheduler.</summary>
        /// <param name="task">The task to be queued.</param>
        protected override void QueueTask(Task task)
        {
            lock (_Interleave._InternalLock)
            {
                Tasks.Enqueue(task);
                _Interleave.NotifyOfNewWork();
            }
        }

        /// <summary>Executes a task on this scheduler.</summary>
        /// <param name="task">The task to be executed.</param>
        internal void ExecuteTask(Task task)
        {
            var processing_task_on_current_thread                                        = _ProcessingTaskOnCurrentThread.Value;
            if (!processing_task_on_current_thread) _ProcessingTaskOnCurrentThread.Value = true;
            TryExecuteTask(task);
            if (!processing_task_on_current_thread) _ProcessingTaskOnCurrentThread.Value = false;
        }

        /// <summary>Tries to execute the task synchronously on this scheduler.</summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="TaskWasPreviouslyQueued">Whether the task was previously queued to the scheduler.</param>
        /// <returns>true if the task could be executed; otherwise, false.</returns>
        protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued)
        {
            if (!_ProcessingTaskOnCurrentThread.Value) return false;
            var t = new Task<bool>(state => TryExecuteTask((Task)state), task);
            t.RunSynchronously(_Interleave._ParallelOptions.TaskScheduler);
            return t.Result;
        }

        /// <summary>Gets for debugging purposes the tasks scheduled to this scheduler.</summary>
        /// <returns>An enumerable of the tasks queued.</returns>
        protected override IEnumerable<Task> GetScheduledTasks() => Tasks;
    }
}