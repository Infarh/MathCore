using System.Collections.Generic;
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

/// <summary>Простейший наблюдаемый объект</summary><typeparam name="T">Тип объектов событий</typeparam>
public class SimpleObservableEx<T> : IObservableEx<T>, IObserver<T>, IDisposable
{
    /// <summary>Список объектов наблюдателей</summary>
    protected readonly List<IObserver<T>> _Observers = new();

    /// <summary>Метод генерации следующего события</summary>
    /// <param name="item">Объект события</param>
    public virtual void OnNext(T item) => _Observers.ForEach(o => OnNext(o, item));

    protected virtual void OnNext([NotNull] IObserver<T> observer, T item) => observer.OnNext(item);

    /// <summary>Метод генерации события завершения последовательности</summary>
    public virtual void OnCompleted() => _Observers.ForEach(OnCompleted);

    protected virtual void OnCompleted([NotNull] IObserver<T> observer) => observer.OnCompleted();

    /// <summary>Метод генерации события сброса последовательности</summary>
    public virtual void OnReset() => _Observers.OfType<IObserverEx<T>>().Foreach(OnReset);

    protected virtual void OnReset([NotNull] IObserverEx<T> observer) => observer.OnReset();

    /// <summary>Метод генерации события возникновения ошибки</summary>
    /// <param name="error">Возникшее исключение</param>
    public virtual void OnError(Exception error) => _Observers.ForEach(o => OnError(o, error));

    /// <summary>Генерация события возникновения исключения</summary>
    /// <param name="observer">Наблюдаемый объект, в котором возникло исключение</param>
    /// <param name="error">Возникшее исключение</param>
    protected virtual void OnError([NotNull] IObserver<T> observer, [NotNull] Exception error) => observer.OnError(error);

    /// <inheritdoc />
    [NotNull]
    public virtual IDisposable Subscribe([NotNull] IObserverEx<T> observer) => Subscribe((IObserver<T>)observer);

    /// <inheritdoc />
    public virtual IDisposable Subscribe(IObserver<T> observer) => _Observers.AddObserver(observer);

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Освобождение ресурсов</summary>
    /// <param name="Disposing">Если <see langword="true"/>, то выполнить освобождение управляемых ресурсов</param>
    protected virtual void Dispose(bool Disposing) { }
}