// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

/// <summary>Признак приостановки выполнения работы</summary>
/// <param name="source"></param>
public readonly ref struct PauseToken(PauseTokenSource source)
{
    private static readonly Task __CompletedTask = Task.FromResult(true);

    public bool IsPaused => source is { IsPaused: true };

    public Task WaitWhilePausedAsync() => IsPaused ? source.WaitWhilePausedAsync() : __CompletedTask;
}