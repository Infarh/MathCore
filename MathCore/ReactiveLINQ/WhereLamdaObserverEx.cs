using MathCore.Annotations;

namespace System.Linq.Reactive
{
    internal sealed class WhereLamdaObserverEx<T> : LamdaObserver<T>
    {
        public WhereLamdaObserverEx
            (
            [NotNull]IObservable<T> Source,
            [NotNull]SimpleObservableEx<T> Destination,
            [NotNull]Func<T, bool> WhereSelector
            ) : base
                (
                Source,
                t => { if(WhereSelector(t)) Destination.OnNext(t); },
                Destination.OnCompleted,
                Destination.OnReset,
                Destination.OnError
                )
        {
        }
    }
}