using System.Diagnostics.Contracts;

namespace System.Linq.Reactive
{
    /// <summary>Класс контрактов для интерфейса <see cref="IObservableEx&lt;T&gt;"/></summary>
    /// <typeparam name="T">Тип объектов последовательности событий</typeparam>
    [ContractClassFor(typeof(IObservableEx<>))]
    internal sealed class ObservableExContract<T> : IObservableEx<T>
    {
        public IDisposable Subscribe(IObserverEx<T> observer) => Subscribe((IObserver<T>)observer);

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Contract.Requires(observer != null);
            Contract.Ensures(Contract.Result<IDisposable>() != null);
            throw new NotSupportedException();
        }
    }
}