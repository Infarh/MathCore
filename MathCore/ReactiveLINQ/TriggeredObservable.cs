namespace System.Linq.Reactive
{
    /// <summary>Управляемый наблюдаемый объект</summary>
    /// <typeparam name="T">Тип объекта последовательности</typeparam>
    sealed class TriggeredObservable<T> : SimpleObservableEx<T>
    {
        /// <summary>Наблюдатель</summary>
        private readonly IObserver<T> _Observer;

        /// <summary>Признак разрешения генерации событий</summary>
        public bool Open { get; set; }

        /// <summary>Управляемый наблюдаемый объект</summary>
        /// <param name="observable">Наблюдаемый объект</param>
        /// <param name="IsOpen">Исходное состояние</param>
        public TriggeredObservable(IObservable<T> observable, bool IsOpen = true)
        {
            _Observer = new LinkedObserver<T>(observable, this);
            Open = IsOpen;
        }

        public override void OnNext(T item) { if(Open) base.OnNext(item); }

        public override void OnCompleted() { if(Open) base.OnCompleted(); }

        public override void OnReset() { if(Open) base.OnReset(); }

        public override void OnError(Exception error) { if(Open) base.OnError(error); }

        public override void Dispose()
        {
            base.Dispose();
            (_Observer as IDisposable)?.Dispose();
        }
    }
}