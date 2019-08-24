using System.Collections.Generic;

namespace System.Linq.Reactive
{
    internal sealed class ObservableCollectionEnumerator<T> : IObservable<T>
    {
        private readonly IEnumerable<T> _Collection;

        public ObservableCollectionEnumerator(IEnumerable<T> collection) => _Collection = collection;

        public IDisposable Subscribe(IObserver<T> observer)
        {
            (observer as IObserverEx<T>)?.OnReset();
            try
            {
                _Collection.OnComplite(observer.OnCompleted).Foreach(observer.OnNext);
            } catch(Exception e)
            {
                observer.OnError(e);
            }
            return new List<IObserver<T>>().AddObserver(observer);
        }
    }
}