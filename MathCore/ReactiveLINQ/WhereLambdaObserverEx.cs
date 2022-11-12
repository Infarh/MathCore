using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

internal sealed class WhereLambdaObserverEx<T> : LambdaObserver<T>
{
    public WhereLambdaObserverEx
    (
        [NotNull]IObservable<T> Source,
        [NotNull]SimpleObservableEx<T> Destination,
        [NotNull]Func<T, bool> WhereSelector
    ) : base
    (
        Source,
        t => { if(WhereSelector(t)) Destination.OnNext(t); },
        Destination.OnCompleted,
        Destination.OnReset,
        Destination.OnError
    )
    {
    }
}