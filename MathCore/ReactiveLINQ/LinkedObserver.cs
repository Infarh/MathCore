// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive
{
    internal sealed class LinkedObserver<T> : SimpleObserverEx<T>
    {
        private readonly SimpleObservableEx<T> _Destination;

        public LinkedObserver(IObservable<T> source, SimpleObservableEx<T> destination) : base(source) => _Destination = destination;

        public override void OnNext(T item)
        {
            base.OnNext(item);
            _Destination?.OnNext(item);
        }

        public override void OnCompleted()
        {
            base.OnCompleted();
            _Destination?.OnCompleted();
        }

        public override void OnReset()
        {
            base.OnReset();
            _Destination?.OnReset();
        }

        public override void OnError(Exception error)
        {
            base.OnError(error);
            _Destination?.OnError(error);
        }
    }
}