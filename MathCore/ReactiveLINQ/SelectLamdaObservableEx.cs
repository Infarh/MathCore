namespace System.Linq.Reactive
{
    sealed class SelectLamdaObservableEx<T, Q> : SimpleObservableEx<Q>
    {
        private readonly SelectLamdaObserverEx<T, Q> _Observer;

        public SelectLamdaObservableEx(IObservable<T> observable, Func<T, Q> Selector) => _Observer = new SelectLamdaObserverEx<T, Q>(observable, this, Selector);

        public override void Dispose()
        {
            base.Dispose();
            ((IDisposable)_Observer).Dispose();
        }
    }
}