using MathCore.Annotations;

namespace System.Linq.Reactive
{
    internal sealed class SelectLamdaObserverEx<T, Q> : LamdaObserver<T>
    {
        public SelectLamdaObserverEx
        (
            [NotNull]IObservable<T> Source,
            [NotNull]SimpleObservableEx<Q> Destination,
            [NotNull]Func<T, Q> Converter
        ) : base
        (
            Source,
            t => Destination.OnNext(Converter(t)),
            Destination.OnCompleted,
            Destination.OnReset,
            Destination.OnError
        )
        {
        }
    }
}