using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeProtected.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

/// <summary>Простейший наблюдатель</summary>
/// <typeparam name="T">Объект события</typeparam>
public class SimpleObserverEx<T> : IObserverEx<T>
{
    /// <summary>События появления следующего объекта в последовательности</summary>
    public event Action<T> Next;
    /// <summary>Событие завершения последовательности</summary>
    public event Action Completed;
    /// <summary>Событие сброса последовательности</summary>
    public event Action Reset;
    /// <summary>Событие появления исключения</summary>
    public event Action<Exception> Error;

    private readonly IDisposable _Unsubscriber;

    /// <summary>Тэг наблюдателя</summary>
    public object Tag { get; set; }

    /// <summary>Инициализация нового простейшего наблюдателя</summary>
    public SimpleObserverEx([NotNull] IObservable<T> observable) => _Unsubscriber = observable.Subscribe(this);

    /// <summary>Метод генерации события появления следующего объекта</summary>
    /// <param name="item">Следующий объект в последовательности</param>
    public virtual void OnNext(T item) => Next?.Invoke(item);

    /// <summary>Метод генерации события исключительной ситуации</summary>
    /// <param name="error">Исключительная ситуация</param>
    public virtual void OnError(Exception error) => Error?.Invoke(error);

    /// <summary>Метод генерации события завершения последовательности</summary>
    public virtual void OnCompleted() => Completed?.Invoke();

    /// <summary>Метод генерации события сброса последовательности</summary>
    public virtual void OnReset() => Reset?.Invoke();

    /// <summary>Освобождение ресурсов и отписка от наблюдаемых объектов</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Освобождение ресурсов и отписка от наблюдаемых объектов</summary>
    /// <param name="Disposing">Если истина, то выполнить освобождение ресурсов и отписку</param>
    protected virtual void Dispose(bool Disposing)
    {
        if (!Disposing) return;
        _Unsubscriber.Dispose();
    }
}