using System.Collections.Concurrent;
// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers;

/// <summary>Provides a scheduler that uses STA threads.</summary>
public sealed class StaTaskScheduler : TaskScheduler, IDisposable
{
    /// <summary>Stores the queued tasks to be executed by our pool of STA threads.</summary>
    private BlockingCollection<Task> _Tasks;
    /// <summary>The STA threads used by the scheduler.</summary>
    private readonly List<Thread> _Threads;

    /// <summary>Initializes a new instance of the StaTaskScheduler class with the specified concurrency level.</summary>
    /// <param name="NumberOfThreads">The number of threads that should be created and used by this scheduler.</param>
    public StaTaskScheduler(int NumberOfThreads)
    {
        // Validate arguments
        if (NumberOfThreads < 1) throw new ArgumentOutOfRangeException(nameof(NumberOfThreads));

        // Initialize the tasks collection
        _Tasks = new BlockingCollection<Task>();

        // Create the threads to be used by this scheduler
        _Threads = Enumerable.Range(0, NumberOfThreads).Select(_ =>
        {
            var thread = new Thread(
                () =>
                {
                    // Continually get the next task and try to execute it.
                    // This will continue until the scheduler is disposed and no more tasks remain.
                    foreach (var t in _Tasks.GetConsumingEnumerable())
                    {
                        TryExecuteTask(t);
                    }
                }) {IsBackground = true};
#pragma warning disable CA1416
            thread.SetApartmentState(ApartmentState.STA);
#pragma warning restore CA1416
            return thread;
        }).ToList();

        // Start all of the threads
        _Threads.ForEach(t => t.Start());
    }

    /// <summary>Queues a Task to be executed by this scheduler.</summary>
    /// <param name="task">The task to be executed.</param>
    protected override void QueueTask(Task task) =>
        // Push it into the blocking collection of tasks
        _Tasks.Add(task);

    /// <summary>Provides a list of the scheduled tasks for the debugger to consume.</summary>
    /// <returns>An enumerable of all tasks currently scheduled.</returns>
    protected override IEnumerable<Task> GetScheduledTasks() =>
        // Serialize the contents of the blocking collection of tasks for the debugger
        _Tasks.ToArray();

    /// <summary>Determines whether a Task may be inlined.</summary>
    /// <param name="task">The task to be executed.</param>
    /// <param name="TaskWasPreviouslyQueued">Whether the task was previously queued.</param>
    /// <returns>true if the task was successfully inlined; otherwise, false.</returns>
    protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) =>
        // Try to inline if the current thread is STA
        Thread.CurrentThread.GetApartmentState() == ApartmentState.STA &&
        TryExecuteTask(task);

    /// <summary>Gets the maximum concurrency level supported by this scheduler.</summary>
    public override int MaximumConcurrencyLevel => _Threads.Count;

    /// <summary>
    /// Cleans up the scheduler by indicating that no more tasks will be queued.
    /// This method blocks until all threads successfully shutdown.
    /// </summary>
    public void Dispose()
    {
        if (_Tasks is null) return;
        // Indicate that no new tasks will be coming in
        _Tasks.CompleteAdding();

        // Wait for all threads to finish processing tasks
        foreach (var thread in _Threads) 
            thread.Join();

        // Cleanup
        _Tasks.Dispose();
        _Tasks = null;
    }
}