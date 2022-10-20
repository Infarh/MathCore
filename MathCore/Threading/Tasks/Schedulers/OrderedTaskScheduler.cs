namespace MathCore.Threading.Tasks.Schedulers;

/// <summary>Планировщик, обеспечивающий одновременное выполнение только одной задачи</summary>
public sealed class OrderedTaskScheduler : LimitedConcurrencyLevelTaskScheduler
{
    public OrderedTaskScheduler() : base(1) { }
}