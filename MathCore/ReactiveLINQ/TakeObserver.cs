// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive
{
    internal sealed class TakeObserver<T> : SimpleObserverEx<T>
    {
        private readonly int _Count;
        private int _Position;

        public TakeObserver(IObservable<T> observer, int Count) : base(observer) => _Count = Count;

        public override void OnNext(T item)
        {
            if(_Position >= _Count) return;
            base.OnNext(item);
            if(_Position == _Count - 1) OnCompleted();
            _Position++;
        }

        public override void OnCompleted()
        {
            if(_Position >= _Count) return;
            base.OnCompleted();
        }

        public override void OnReset()
        {
            base.OnReset();
            _Position = 0;
        }

        public override void OnError(Exception error)
        {
            if(_Position >= _Count) return;
            base.OnError(error);
        }
    }
}