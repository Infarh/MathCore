#nullable enable

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

internal sealed class WhereLambdaObserverEx<T>(
    IObservable<T> Source,
    SimpleObservableEx<T> Destination,
    Func<T, bool> WhereSelector)
    : LambdaObserver<T>(Source, t => { if (WhereSelector(t)) Destination.OnNext(t); }, Destination.OnCompleted, Destination.OnReset, Destination.OnError);