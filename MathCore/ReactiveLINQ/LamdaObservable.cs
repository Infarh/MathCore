using MathCore.Annotations;

namespace System.Linq.Reactive
{
    /// <summary>НАблюдаемый объект с методами обработки событий, задаваемыми лямбда-синтексисом</summary>
    /// <typeparam name="T"></typeparam>
    public class LamdaObservable<T> : SimpleObservableEx<T>
    {
        /// <summary>Присоединённый наблюдатель</summary>
        private readonly LinkedObserver<T> _Observer;
        /// <summary>Действие обработки следующего объекта наблюдения</summary>
        private readonly Action<IObserver<T>, T> _OnNext;
        /// <summary>Действие обработки завершения процесса наблюдения</summary>
        private readonly Action<IObserver<T>> _OnCompleted;
        /// <summary>Действие обработки сброса состояния наблюдаемого объекта</summary>
        private readonly Action<IObserverEx<T>> _OnReset;
        private readonly Action<IObserver<T>, Exception> _OnError;

        public LamdaObservable
        (
            [CanBeNull]IObservable<T> observable = null,
            [CanBeNull]Action<IObserver<T>, T> OnNext = null,
            [CanBeNull]Action<IObserver<T>> OnCompleted = null,
            [CanBeNull]Action<IObserverEx<T>> OnReset = null,
            [CanBeNull]Action<IObserver<T>, Exception> OnError = null
        )
        {
            if(observable != null)
                _Observer = new LinkedObserver<T>(observable, this);
            _OnNext = OnNext;
            _OnCompleted = OnCompleted;
            _OnReset = OnReset;
            _OnError = OnError;
        }

        protected override void OnNext(IObserver<T> observer, T item) => (_OnNext ?? base.OnNext).Invoke(observer, item);
        protected override void OnCompleted(IObserver<T> observer) => (_OnCompleted ?? base.OnCompleted).Invoke(observer);
        protected override void OnReset(IObserverEx<T> observer) => (_OnReset ?? base.OnReset).Invoke(observer);
        protected override void OnError(IObserver<T> observer, Exception error) => (_OnError ?? base.OnError).Invoke(observer, error);

        public override void Dispose()
        {
            base.Dispose();
            (_Observer as IDisposable)?.Dispose();
        }
    }
}