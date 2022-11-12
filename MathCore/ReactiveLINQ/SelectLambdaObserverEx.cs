using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

internal sealed class SelectLambdaObserverEx<TItem, TValue> : LambdaObserver<TItem>
{
    public SelectLambdaObserverEx
    (
        [NotNull]IObservable<TItem> Source,
        [NotNull]SimpleObservableEx<TValue> Destination,
        [NotNull]Func<TItem, TValue> Converter
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