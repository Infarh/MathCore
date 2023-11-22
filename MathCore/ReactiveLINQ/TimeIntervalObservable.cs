#nullable enable
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

public class TimeIntervalObservable : SimpleObservableEx<TimeSpan>
{
    private readonly TimeSpan _Interval;
    private readonly bool _Async;
    private volatile bool _Work;
    private readonly object _SyncObject = new();
    private Thread? _Thread;

    public TimeIntervalObservable(TimeSpan interval, bool Start = false, bool Async = false)
    {
        _Interval = interval;
        _Async    = Async;
        if(Start) this.Start();
    }

    protected override void OnReset(IObserverEx<TimeSpan> observer) => throw new NotSupportedException();

    public void Start()
    {
        // ReSharper disable once InconsistentlySynchronizedField
        if(_Work) return;
        lock (_SyncObject)
        {
            if(_Work) return;
            _Work = true;
            var need_reset = _Thread != null;
            _Thread = _Async
                ? new Thread(AsyncThreadMethod) { IsBackground = true }
                : new Thread(SyncThreadMethod) { IsBackground  = true };
            if(need_reset) base.OnReset();
            _Thread.Start();
        }
    }

    public void Stop()
    {
        // ReSharper disable once InconsistentlySynchronizedField
        if(!_Work) return;
        lock (_SyncObject)
        {
            if(!_Work) return;
            _Work = false;
            if(!_Thread.Join(_Interval.Milliseconds))
                _Thread.Interrupt();
        }
        foreach(var observer in _Observers.ToArray())
            observer.OnCompleted();
    }

    public override void OnNext(TimeSpan item) => throw new NotSupportedException();

    private void SyncThreadMethod()
    {
        while(_Work)
        {
            var t = DateTime.Now.TimeOfDay;
            base.OnNext(t);
            Thread.Sleep(_Interval);
        }
    }

    private void AsyncThreadMethod()
    {
        Action<TimeSpan> next = base.OnNext;
        while(_Work)
        {
            var t = DateTime.Now.TimeOfDay;
            next.BeginInvoke(t, null, null);
            Thread.Sleep(_Interval);
        }
    }

    /// <inheritdoc />
    protected override async void Dispose(bool Disposing)
    {
        base.Dispose(Disposing);
        await Task.Run(Stop).ConfigureAwait(false);
    }
}