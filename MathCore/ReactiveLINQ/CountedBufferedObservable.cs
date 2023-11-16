#nullable enable
// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

internal class CountedBufferedObservable<T> : BufferedObservable<T>
{
    private volatile int _Phase;
    private readonly int _BufferPeriod;
    private readonly int _BufferPhase;

    public CountedBufferedObservable(
        IObservable<T> ObservableObject,
        int BufferLength,
        int BufferPeriod = 0, 
        int BufferPhase = 0) 
        : base(ObservableObject, BufferPeriod / BufferLength + 1, BufferLength)
    {
        _BufferPeriod = BufferPeriod;
        _BufferPhase  = BufferPhase;
    }
    protected override void OnNext(T value)
    {
        T[]? r;
        lock (_SyncRoot)
        {
            if (Interlocked.Increment(ref _Phase) % _BufferPeriod == _BufferPhase) AddBuffer();
            r = AddValue(value);
        }
        if (r != null) OnNext(r);
    }
}