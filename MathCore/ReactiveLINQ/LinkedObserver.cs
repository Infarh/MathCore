#nullable enable
// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

internal sealed class LinkedObserver<T>(IObservable<T> source, SimpleObservableEx<T>? destination) : SimpleObserverEx<T>(source)
{
    public override void OnNext(T item)
    {
        base.OnNext(item);
        destination?.OnNext(item);
    }

    public override void OnCompleted()
    {
        base.OnCompleted();
        destination?.OnCompleted();
    }

    public override void OnReset()
    {
        base.OnReset();
        destination?.OnReset();
    }

    public override void OnError(Exception error)
    {
        base.OnError(error);
        destination?.OnError(error);
    }
}