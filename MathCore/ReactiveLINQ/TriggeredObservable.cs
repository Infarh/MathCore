// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive
{
    /// <summary>Управляемый наблюдаемый объект</summary>
    /// <typeparam name="T">Тип объекта последовательности</typeparam>
    internal sealed class TriggeredObservable<T> : SimpleObservableEx<T>
    {
        /// <summary>Наблюдатель</summary>
        private readonly IObserver<T> _Observer;

        /// <summary>Признак разрешения потока элементов последовательности</summary>
        public bool State { get; set; }

        /// <summary>Управляемый наблюдаемый объект</summary>
        /// <param name="observable">Наблюдаемый объект</param>
        /// <param name="InitialState">Исходное состояние</param>
        public TriggeredObservable(IObservable<T> observable, bool InitialState = true)
        {
            _Observer = new LinkedObserver<T>(observable, this);
            State = InitialState;
        }

        /// <summary>Если состояние <see langword="true"/>, то значения пропускаются в выходную последовательность</summary>
        /// <param name="item">Объект события</param>
        public override void OnNext(T item) { if(State) base.OnNext(item); }

        /// <summary>Если состояние <see langword="true"/>, то генерирует событие завершения последовательности</summary>
        public override void OnCompleted() { if(State) base.OnCompleted(); }

        /// <summary>Если состояние <see langword="true"/>, то генерирует событие сброса последовательности</summary>
        public override void OnReset() { if(State) base.OnReset(); }

        /// <summary>Если состояние <see langword="true"/>, то генерирует событие возникновения ошибки</summary>
        /// <param name="error">Возникшее исключение</param>
        public override void OnError(Exception error) { if(State) base.OnError(error); }

        /// <inheritdoc />
        protected override void Dispose(bool Disposing)
        {
            base.Dispose(Disposing);
            (_Observer as IDisposable)?.Dispose();
        }
    }
}