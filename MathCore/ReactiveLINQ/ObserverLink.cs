﻿#nullable enable
using System.Collections.Concurrent;

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive;

/// <summary>Класс объектов-связей между наблюдателем и списком наблюдателей, позволяющих удалять наблюдатель из писка наблюдателей в случае если объект удаляется из памяти</summary>
/// <typeparam name="T">Тип значений наблюдаемого объекта</typeparam>
internal sealed class ObserverLink<T> : IDisposable
{
    /// <summary>Получить хэш-код связи</summary>
    /// <param name="Observers">Коллекция наблюдателей</param>
    /// <param name="Observer">Добавляемый наблюдатель</param>
    /// <returns>Хэш-код связи</returns>
    private static int GetHash(ICollection<IObserver<T>> Observers, IObserver<T> Observer) { unchecked { return Observer.GetHashCode() * 397 ^ Observers.GetHashCode(); } }

    /// <summary>Словарь связей</summary>
    private static readonly ConcurrentDictionary<int, ObserverLink<T>> __Links = new();

    /// <summary>Получить связь между наблюдателем и списком наблюдателей</summary>
    /// <param name="Observers">Коллекция наблюдателей</param>
    /// <param name="Observer">Добавляемый наблюдатель</param>
    /// <returns>Связь между наблюдателем и списком наблюдателей</returns>
    public static ObserverLink<T> GetLink(ICollection<IObserver<T>> Observers, IObserver<T> Observer) => __Links.GetOrAdd(GetHash(Observers, Observer), _ => new(Observers, Observer));

    /// <summary>Удаляемый наблюдатель</summary>
    private IObserver<T> _Observer;
    /// <summary>Коллекция наблюдателей, из которой требуется удалить отслеживаемый наблюдатель</summary>
    private ICollection<IObserver<T>> _Observers;
    /// <summary>Объект межпотоковой синхронизации</summary>
    private readonly object _SyncRoot = new();

    /// <summary>Инициализация новой связи между списком наблюдателей и отслеживаемым наблюдателем</summary>
    /// <param name="Observers">Список наблюдателей</param>
    /// <param name="Observer">Отслеживаемый наблюдатель</param>
    private ObserverLink(ICollection<IObserver<T>> Observers, IObserver<T> Observer)
    {
        _Observers = Observers;
        _Observer  = Observer;
        if (!_Observers.Contains(_Observer))
            _Observers.Add(_Observer);
    }

    private bool _IsDisposed;
    void IDisposable.Dispose()
    {
        if (_IsDisposed) return;
        lock (_SyncRoot)
        {
            if (_IsDisposed) return;
            _IsDisposed = true;
            __Links.TryRemove(GetHash(_Observers, _Observer), out _);
            _Observers.Remove(_Observer);
            _Observers = null;
            _Observer  = null;
        }
    }
}