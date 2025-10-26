using System.Collections;

namespace MathCore;

/// <summary>Ограниченная очередь с фиксированным размером буфера, использующая круговой буфер для перезаписи старых элементов при переполнении.</summary>
public class LimitedQueue<T>(int Length) : IEnumerable<T>
{
    private readonly T[] _Items = new T[Length];
    private int _Head;
    private int _Tail;
    private int _Count;

    /// <summary>Создает ограниченную очередь из коллекции элементов, ограничивая размер буфера.</summary>
    /// <param name="Items">Коллекция элементов для инициализации очереди.</param>
    /// <param name="Length">Максимальный размер буфера.</param>
    public LimitedQueue(IEnumerable<T> Items, int Length) : this(Length)
    {
        foreach (var item in Items)
            Enqueue(item);
    }

    /// <summary>Количество элементов в очереди.</summary>
    public int Count => _Count;

    /// <summary>Проверяет, пуста ли очередь.</summary>
    public bool IsEmpty => _Count == 0;

    /// <summary>Проверяет, полна ли очередь.</summary>
    public bool IsFull => _Count == _Items.Length;

    /// <summary>Добавляет элемент в очередь, перезаписывая старый при переполнении.</summary>
    /// <param name="Item">Элемент для добавления.</param>
    public void Enqueue(T Item)
    {
        _Items[_Tail] = Item;

        _Tail = (_Tail + 1) % _Items.Length;

        if (_Count < _Items.Length)
            _Count++;
        else
            _Head = (_Head + 1) % _Items.Length;
    }

    /// <summary>Удаляет и возвращает элемент из начала очереди.</summary>
    /// <returns>Элемент из начала очереди.</returns>
    /// <exception cref="InvalidOperationException">Если очередь пуста.</exception>
    public T Dequeue()
    {
        if (_Count == 0)
            throw new InvalidOperationException("Очередь пуста.");

        var item = _Items[_Head];

        _Head = (_Head + 1) % _Items.Length;
        _Count--;

        return item;
    }

    /// <summary>Возвращает элемент из начала очереди без удаления.</summary>
    /// <returns>Элемент из начала очереди.</returns>
    /// <exception cref="InvalidOperationException">Если очередь пуста.</exception>
    public T Peek()
    {
        if (_Count == 0)
            throw new InvalidOperationException("Очередь пуста.");

        return _Items[_Head];
    }

    /// <summary>Преобразует очередь в массив.</summary>
    /// <returns>Массив элементов очереди.</returns>
    public T[] ToArray()
    {
        var array = new T[_Count];
        for (var i = 0; i < _Count; i++)
            array[i] = _Items[(_Head + i) % _Items.Length];
        return array;
    }

    /// <summary>Очищает очередь.</summary>
    public void Clear()
    {
        _Head = 0;
        _Tail = 0;
        _Count = 0;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < _Count; i++)
            yield return _Items[(_Head + i) % _Items.Length];
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
