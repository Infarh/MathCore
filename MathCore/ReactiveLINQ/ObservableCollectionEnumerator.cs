#nullable enable
// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

internal sealed class ObservableCollectionEnumerator<T>(IEnumerable<T> collection) : IObservable<T>
{
    public IDisposable Subscribe(IObserver<T> observer)
    {
        (observer as IObserverEx<T>)?.OnReset();
        try
        {
            collection.OnComplete(observer.OnCompleted).Foreach(observer.OnNext);
        } catch(Exception e)
        {
            observer.OnError(e);
        }
        return new List<IObserver<T>>().AddObserver(observer);
    }
}