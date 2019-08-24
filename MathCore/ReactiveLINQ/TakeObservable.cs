namespace System.Linq.Reactive
{
    /// <summary>Наблюдаемый объект с указанным числом генерации событий</summary>
    /// <typeparam name="T">Тип объектов последовательности</typeparam>
    sealed class TakeObservable<T> : SimpleObservableEx<T>
    {
        /// <summary>Исходный наблюдатель</summary>
        private readonly IObserver<T> _Observer;

        /// <summary>Наблюдаемый объект с указанным числом генерации событий</summary>
        /// <param name="observable">Исходный наблюдаемый объект</param>
        /// <param name="Count">Количество извлекаемых событий</param>
        public TakeObservable(IObservable<T> observable, int Count)
        {
            _Observer = new TakeObserver<T>(observable, Count);
        }
    }
}