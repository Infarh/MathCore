#nullable enable
using System.Diagnostics;

namespace MathCore;

public class TimerAsync
{
    private readonly int _Timeout;
    private readonly Lazy<Stopwatch> _Timer = new(Stopwatch.StartNew);

    public TimerAsync(int Timeout) => _Timeout = Timeout;

    private async Task<int> WaitAsync()
    {
        var timer   = _Timer.Value;
        var elapsed = timer.ElapsedMilliseconds;
        var delay   = Math.Max(0, (int)(_Timeout - elapsed));
        if (delay > 0)
            await Task.Delay(delay).ConfigureAwait(false);
        return delay;
    }
}