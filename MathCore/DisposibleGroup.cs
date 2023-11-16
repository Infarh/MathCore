#nullable enable
using System.Collections;

// ReSharper disable UnusedMember.Global

// ReSharper disable UnusedType.Global

namespace MathCore;

/// <summary>Группа объектов, поддерживающих интерфейс <see cref="T:System.IDisposable">освобождения ресурсов</see></summary>
/// <typeparam name="T">Тип объектов, поддерживающих интерфейс <see cref="T:System.IDisposable"/></typeparam>
/// <remarks>Группа <typeparamref name="T"/> интерфейса <see cref="T:System.IDisposable"/></remarks>
/// <param name="item"><typeparamref name="T"/> интерфейса <see cref="T:System.IDisposable"/></param>
[method: DST]
/// <summary>Группа объектов, поддерживающих интерфейс <see cref="T:System.IDisposable">освобождения ресурсов</see></summary>
/// <typeparam name="T">Тип объектов, поддерживающих интерфейс <see cref="T:System.IDisposable"/></typeparam>
public class DisposableGroup<T>(params T[] item) : IDisposable, IEnumerable<T>, IIndexableRead<int, T> where T : IDisposable
{
    /// <summary>Группа <typeparamref name="T"/> интерфейса <see cref="T:System.IDisposable"/></summary>
    /// <param name="items">Перечисление <typeparamref name="T"/> интерфейса <see cref="T:System.IDisposable"/></param>
    [DST]
    public DisposableGroup(IEnumerable<T> items) : this(items.Where(v => v is not null).ToArray()) { }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Число элементов группы</summary>
    public int Count => item.Length;

    /// <summary>Массив элементов группы</summary>
    public IReadOnlyList<T> Items => item;

    /// <inheritdoc />
    T IIndexableRead<int, T>.this[int i] { [DST] get => item[i]; }

    /// <summary>Элемент группы</summary>
    /// <param name="i">Номер элемента группы</param>
    /// <returns>Элемент группы с номером <paramref name="i"/></returns>
    public ref readonly T this[int i] => ref item[i];

    /* ------------------------------------------------------------------------------------------ */

    /// <inheritdoc />
    [DST]
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Объект уничтожен</summary>
    private bool _Disposed;

    /// <summary>Освободить ресурсы группы</summary>
    /// <param name="disposing">Признак того, что требуется освобождение управляемых ресурсов</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || _Disposed) return;
        _Disposed = true;
        item.Foreach(i => i?.Dispose());
    }

    /// <summary>Получить перечислитель элементов группы</summary>
    /// <returns>Перечислитель элементов группы</returns>
    [DST]
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)item).GetEnumerator();


    /// <summary>Возвращает перечислитель, который осуществляет перебор элементов коллекции.</summary>
    /// <returns>
    /// Объект <see cref="T:System.Collections.IEnumerator"/>, который может использоваться для перебора элементов коллекции.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    [DST]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /* ------------------------------------------------------------------------------------------ */
}