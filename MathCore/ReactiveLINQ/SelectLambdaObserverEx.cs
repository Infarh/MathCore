#nullable enable


// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

internal sealed class SelectLambdaObserverEx<TItem, TValue> : LambdaObserver<TItem>
{
    public SelectLambdaObserverEx
    (
        IObservable<TItem> Source,
        SimpleObservableEx<TValue> Destination,
        Func<TItem, TValue> Converter
    ) : base
    (
        Source,
        t => Destination.OnNext(Converter(t)),
        Destination.OnCompleted,
        Destination.OnReset,
        Destination.OnError
    )
    {
    }
}