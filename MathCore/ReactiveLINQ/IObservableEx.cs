using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive
{
    /// <summary>Обозреваемый объект</summary>
    /// <typeparam name="T">Тип объектов последовательности событий</typeparam>
    public interface IObservableEx<T> : IObservable<T>
    {
        /// <summary>Метод получения наблюдателя</summary>
        /// <param name="observer">Наблюдатель объекта</param>
        /// <returns>Объект, реализующий возможность разрушения связи с наблюдаемым объектом</returns>
        [NotNull] IDisposable Subscribe([NotNull] IObserverEx<T> observer);
    }
}