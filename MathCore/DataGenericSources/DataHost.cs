#nullable enable
using System.Linq.Reactive;

namespace MathCore.DataGenericSources;

public abstract class DataHost : IDisposable
{
    /* -------------------------------------------------------------------------------- */

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) { }

    /* -------------------------------------------------------------------------------- */
}

public abstract class DataHost<T> : DataHost, IObservable<T>
{
    /* -------------------------------------------------------------------------------- */


    /* -------------------------------------------------------------------------------- */

    public event EventHandler? Updated;

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

    private readonly SimpleObservableEx<T> _ObservableObject = new();

    public IDisposable Subscribe(IObserver<T> observer) => _ObservableObject.Subscribe(observer);

    #endregion

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing) return;
        _ObservableObject.Dispose();
    }

    /* -------------------------------------------------------------------------------- */
}