#nullable enable

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

internal sealed class SelectLambdaObservableEx<TItem, TValue> : SimpleObservableEx<TValue>
{
    private readonly SelectLambdaObserverEx<TItem, TValue> _Observer;

    public SelectLambdaObservableEx(IObservable<TItem> observable, Func<TItem, TValue> Selector) => _Observer = new SelectLambdaObserverEx<TItem, TValue>(observable, this, Selector);

    protected override void Dispose(bool Disposing)
    {
        base.Dispose(Disposing);
        _Observer.Dispose();
    }
}