// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

internal sealed class AsyncPatternObservable<T> : SimpleObservableEx<T>
{
    private readonly IAsyncResult _AsyncResult = null!;
    private T _Result = default;

    public AsyncPatternObservable
    (
        Func<AsyncCallback, object, IAsyncResult> BeginInvoke,
        Func<IAsyncResult, T> EndInvoke
    ) => _AsyncResult = BeginInvoke(CallBack, EndInvoke);

    private void CallBack(IAsyncResult result)
    {
        _Result = ((Func<IAsyncResult, T>?)result.AsyncState)(result);
        OnNext(_Result);
    }

    public override IDisposable Subscribe(IObserver<T> observer)
    {
        var result = base.Subscribe(observer);
        if (_AsyncResult.IsCompleted) observer.OnNext(_Result);
        return result;
    }
}