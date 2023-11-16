using MathCore.Annotations;

namespace MathCore.Threading.Tasks.Schedulers;

public class SynchronizationContextScheduler : TaskScheduler
{
    public static SynchronizationContextScheduler CurrentContext => new(SynchronizationContext.Current);

    private readonly SynchronizationContext _Context;
    private readonly SendOrPostCallback _Execute;

    public SynchronizationContextScheduler(SynchronizationContext Context)
    {
        _Context = Context;
        _Execute = Execute;
    }

    protected override IEnumerable<Task> GetScheduledTasks() => Enumerable.Empty<Task>();

    protected override void QueueTask(Task task) => _Context.Send(_Execute, task);

    private void Execute([NotNull] object p) => TryExecuteTask((Task)p);

    protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) => TryExecuteTask(task);
}