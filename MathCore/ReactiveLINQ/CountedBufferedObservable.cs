#nullable enable
// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

internal class CountedBufferedObservable<T>(
    IObservable<T> ObservableObject,
    int BufferLength,
    int BufferPeriod = 0,
    int BufferPhase = 0) : BufferedObservable<T>(ObservableObject, BufferPeriod / BufferLength + 1, BufferLength)
{
    private volatile int _Phase;

    protected override void OnNext(T value)
    {
        T[]? r;
        lock (_SyncRoot)
        {
            if (Interlocked.Increment(ref _Phase) % BufferPeriod == BufferPhase) AddBuffer();
            r = AddValue(value);
        }
        if (r != null) OnNext(r);
    }
}