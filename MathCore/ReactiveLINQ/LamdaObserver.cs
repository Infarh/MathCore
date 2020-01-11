using MathCore.Annotations;

namespace System.Linq.Reactive
{
    public class LamdaObserver<T> : SimpleObserverEx<T>
    {
        private readonly Action<T> _OnNext;
        private readonly Action _OnCompleted;
        private readonly Action _OnReset;
        private readonly Action<Exception> _OnError;

        public LamdaObserver
        (
            [NotNull]IObservable<T> Observable,
            [CanBeNull]Action<T> OnNext = null,
            [CanBeNull]Action OnCompleted = null,
            [CanBeNull]Action OnReset = null,
            [CanBeNull]Action<Exception> OnError = null
        ) : base(Observable)
        {
            _OnNext = OnNext;
            _OnCompleted = OnCompleted;
            _OnReset = OnReset;
            _OnError = OnError;
        }

        public override void OnNext(T item)
        {
            base.OnNext(item);
            _OnNext?.Invoke(item);
        }

        public override void OnCompleted()
        {
            base.OnCompleted();
            _OnCompleted?.Invoke();
        }

        public override void OnReset()
        {
            base.OnReset();
            _OnReset?.Invoke();
        }

        public override void OnError(Exception error)
        {
            base.OnError(error);
            _OnError?.Invoke(error);
        }
    }
}