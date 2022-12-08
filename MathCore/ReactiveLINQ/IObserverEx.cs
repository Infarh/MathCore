#nullable enable
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

/// <summary>Интерфейс наблюдателя</summary>
/// <typeparam name="T">Тип объектов последовательности событий</typeparam>
public interface IObserverEx<T> : IObserver<T>, IDisposable
{
    /// <summary>Событие появления следующего объекта последовательности</summary>
    event Action<T>? Next;

    /// <summary>Событие завершения последовательности</summary>
    event Action? Completed;

    /// <summary>Событие сброса последовательности</summary>
    event Action? Reset;

    /// <summary>Событие появления исключения</summary>
    event Action<Exception>? Error;

    /// <summary>Метод генерации события сброса последовательности</summary>
    void OnReset();
}