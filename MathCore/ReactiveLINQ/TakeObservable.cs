#nullable enable
// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

/// <summary>Наблюдаемый объект с указанным числом генерации событий</summary>
/// <typeparam name="T">Тип объектов последовательности</typeparam>
/// <remarks>Наблюдаемый объект с указанным числом генерации событий</remarks>
/// <param name="observable">Исходный наблюдаемый объект</param>
/// <param name="Count">Количество извлекаемых событий</param>
internal sealed class TakeObservable<T>(IObservable<T> observable, int Count) : SimpleObservableEx<T>
{
    /// <summary>Исходный наблюдатель</summary>
    // ReSharper disable once NotAccessedField.Local
    private readonly IObserver<T> _Observer = new TakeObserver<T>(observable, Count);
}