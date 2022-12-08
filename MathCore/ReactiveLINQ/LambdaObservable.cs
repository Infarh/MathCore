#nullable enable

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

/// <summary>Наблюдаемый объект с методами обработки событий, задаваемыми лямбда-синтаксисом</summary>
/// <typeparam name="T"></typeparam>
public class LambdaObservable<T> : SimpleObservableEx<T>
{
    /// <summary>Присоединённый наблюдатель</summary>
    private readonly LinkedObserver<T>? _Observer;

    /// <summary>Действие обработки следующего объекта наблюдения</summary>
    private readonly Action<IObserver<T>, T>? _OnNext;

    /// <summary>Действие обработки завершения процесса наблюдения</summary>
    private readonly Action<IObserver<T>>? _OnCompleted;

    /// <summary>Действие обработки сброса состояния наблюдаемого объекта</summary>
    private readonly Action<IObserverEx<T>>? _OnReset;

    private readonly Action<IObserver<T>, Exception>? _OnError;

    public LambdaObservable
    (
        IObservable<T>? observable = null,
        Action<IObserver<T>, T>? OnNext = null,
        Action<IObserver<T>>? OnCompleted = null,
        Action<IObserverEx<T>>? OnReset = null,
        Action<IObserver<T>, Exception>? OnError = null
    )
    {
        if (observable != null)
            _Observer = new LinkedObserver<T>(observable, this);
        _OnNext      = OnNext;
        _OnCompleted = OnCompleted;
        _OnReset     = OnReset;
        _OnError     = OnError;
    }

    /// <inheritdoc />
    protected override void OnNext(IObserver<T> observer, T item) => (_OnNext ?? base.OnNext).Invoke(observer, item);

    /// <inheritdoc />
    protected override void OnCompleted(IObserver<T> observer) => (_OnCompleted ?? base.OnCompleted).Invoke(observer);

    /// <inheritdoc />
    protected override void OnReset(IObserverEx<T> observer) => (_OnReset ?? base.OnReset).Invoke(observer);

    /// <inheritdoc />
    protected override void OnError(IObserver<T> observer, Exception error) => (_OnError ?? base.OnError).Invoke(observer, error);

    /// <inheritdoc />
    protected override void Dispose(bool Disposing)
    {
        base.Dispose(Disposing);
        _Observer?.Dispose();
    }
}