// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

/// <summary>Система управления приостановки асинхронной операции</summary>
[Copyright("Stephen Toub", url = "https://devblogs.microsoft.com/pfxteam/cooperatively-pausing-async-methods/")]
public class PauseTokenSource
{
    private volatile TaskCompletionSource<bool> _Paused;

    public bool IsPaused
    {
        get => _Paused != null;
        set
        {
            if (value)
                Interlocked.CompareExchange(ref _Paused, new(), null);
            else
                while (true)
                {
                    var tcs = _Paused;
                    if (tcs is null) return;
                    if (Interlocked.CompareExchange(ref _Paused, null, tcs) != tcs) continue;
                    tcs.SetResult(true);
                    return;
                }
        }
    }

    public PauseToken Token => new(this);

    public Task WaitWhilePausedAsync() => _Paused.Task;
}