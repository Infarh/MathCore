#nullable enable
#nullable enable

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

public class LambdaObserver<T>(
    IObservable<T> Observable,
    Action<T>? OnNext = null,
    Action? OnCompleted = null,
    Action? OnReset = null,
    Action<Exception>? OnError = null
    ) : SimpleObserverEx<T>(Observable)
{
    private readonly Action<T>? _OnNext = OnNext;
    private readonly Action? _OnCompleted = OnCompleted;
    private readonly Action? _OnReset = OnReset;
    private readonly Action<Exception>? _OnError = OnError;

    public override void OnNext(T item)
    {
        base.OnNext(item);
        _OnNext?.Invoke(item);
    }

    public override void OnCompleted()
    {
        base.OnCompleted();
        _OnCompleted?.Invoke();
    }

    public override void OnReset()
    {
        base.OnReset();
        _OnReset?.Invoke();
    }

    public override void OnError(Exception error)
    {
        base.OnError(error);
        _OnError?.Invoke(error);
    }
}