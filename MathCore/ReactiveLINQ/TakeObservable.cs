﻿// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

/// <summary>Наблюдаемый объект с указанным числом генерации событий</summary>
/// <typeparam name="T">Тип объектов последовательности</typeparam>
internal sealed class TakeObservable<T> : SimpleObservableEx<T>
{
    /// <summary>Исходный наблюдатель</summary>
    // ReSharper disable once NotAccessedField.Local
#pragma warning disable IDE0052 // Удалить непрочитанные закрытые члены
    private readonly IObserver<T> _Observer;
#pragma warning restore IDE0052 // Удалить непрочитанные закрытые члены

    /// <summary>Наблюдаемый объект с указанным числом генерации событий</summary>
    /// <param name="observable">Исходный наблюдаемый объект</param>
    /// <param name="Count">Количество извлекаемых событий</param>
    public TakeObservable(IObservable<T> observable, int Count) => _Observer = new TakeObserver<T>(observable, Count);
}