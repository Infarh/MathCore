using System;
using System.Linq.Reactive;

namespace MathCore.DataGenericSouces
{
    public abstract class DataHost : IDisposable
    {
        /* -------------------------------------------------------------------------------- */

        public virtual void Dispose() { }

        /* -------------------------------------------------------------------------------- */
    }

    public abstract class DataHost<T> : DataHost, IObservable<T>
    {
        /* -------------------------------------------------------------------------------- */


        /* -------------------------------------------------------------------------------- */

        public event EventHandler Updated;

        protected virtual void OnUpdated(EventArgs args)
        {
            Updated.Start(this, args);
            _ObservableObject.OnNext(args is EventArgs<T> event_args ? event_args.Argument : default);
        }

        /* -------------------------------------------------------------------------------- */

        protected DataHost() { }

        /* -------------------------------------------------------------------------------- */

        public virtual void Update() => OnUpdated(EventArgs.Empty);


        /* -------------------------------------------------------------------------------- */

        #region Implementation of IObservable<T>

        private readonly SimpleObservableEx<T> _ObservableObject = new SimpleObservableEx<T>();

        public IDisposable Subscribe(IObserver<T> observer) => _ObservableObject.Subscribe(observer);

        #endregion

        /* -------------------------------------------------------------------------------- */
    }
}