namespace System.Linq.Reactive
{
    internal sealed class WhereLamdaObservableEx<T> : SimpleObservableEx<T>
    {
        private readonly IObserver<T> _Observer;

        public WhereLamdaObservableEx(IObservable<T> observable, Func<T, bool> WhereSelector) => _Observer = new WhereLamdaObserverEx<T>(observable, this, WhereSelector);
    }
}