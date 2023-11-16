#nullable enable
// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

internal sealed class TakeObserver<T>(IObservable<T> observer, int Count) : SimpleObserverEx<T>(observer)
{
    private int _Position;

    public override void OnNext(T item)
    {
        if(_Position >= Count) return;
        base.OnNext(item);
        if(_Position == Count - 1) OnCompleted();
        _Position++;
    }

    public override void OnCompleted()
    {
        if(_Position >= Count) return;
        base.OnCompleted();
    }

    public override void OnReset()
    {
        base.OnReset();
        _Position = 0;
    }

    public override void OnError(Exception error)
    {
        if(_Position >= Count) return;
        base.OnError(error);
    }
}