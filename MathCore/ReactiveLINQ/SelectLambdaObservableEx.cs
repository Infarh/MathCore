using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

internal sealed class SelectLambdaObservableEx<TItem, TValue> : SimpleObservableEx<TValue>
{
    private readonly SelectLambdaObserverEx<TItem, TValue> _Observer;

    public SelectLambdaObservableEx([NotNull] IObservable<TItem> observable, [NotNull] Func<TItem, TValue> Selector) => _Observer = new SelectLambdaObserverEx<TItem, TValue>(observable, this, Selector);

    protected override void Dispose(bool Disposing)
    {
        base.Dispose(Disposing);
        ((IDisposable)_Observer).Dispose();
    }
}