using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable UnusedMember.Global

namespace MathCore;

/// <summary>Предоставляет базовую реализацию для коллекций типа производитель-потребитель, которые обертывают другие коллекции производитель-потребитель</summary>
/// <typeparam name="T">Указывает тип элементов в коллекции</typeparam>
[Serializable]
public abstract class ProducerConsumerCollectionBase<T> : IProducerConsumerCollection<T>
{
    private readonly IProducerConsumerCollection<T> _Contained;

    /// <summary>Инициализирует экземпляр класса ProducerConsumerCollectionBase</summary>
    /// <param name="contained">Коллекция, которая будет обёрнута этим экземпляром</param>
    protected ProducerConsumerCollectionBase(IProducerConsumerCollection<T> contained) => _Contained = contained.NotNull();

    /// <summary>Получает содержащуюся коллекцию</summary>
    protected IProducerConsumerCollection<T> ContainedCollection => _Contained;

    /// <summary>Пытается добавить указанное значение в конец коллекции</summary>
    /// <param name="item">Элемент для добавления</param>
    /// <returns>true, если элемент был добавлен; в противном случае false</returns>
    protected virtual bool TryAdd(T item) => _Contained.TryAdd(item);

    /// <summary>Пытается удалить и вернуть элемент из коллекции</summary>
    /// <param name="item">
    /// Когда метод возвращает значение, если операция была успешной, содержит удалённый элемент. Если
    /// элемент не был доступен для удаления, значение не определено
    /// </param>
    /// <returns>
    /// true, если элемент был удален и возвращен из коллекции; в противном случае false
    /// </returns>
    protected virtual bool TryTake([MaybeNullWhen(false)] out T item) => _Contained.TryTake(out item);

    /// <summary>Пытается добавить указанное значение в конец коллекции</summary>
    /// <param name="item">Элемент для добавления</param>
    /// <returns>true, если элемент был добавлен; в противном случае false</returns>
    bool IProducerConsumerCollection<T>.TryAdd(T item) => TryAdd(item);

    /// <summary>Пытается удалить и вернуть элемент из коллекции</summary>
    /// <param name="item">
    /// Когда метод возвращает значение, если операция была успешной, содержит удалённый элемент. Если
    /// элемент не был доступен для удаления, значение не определено
    /// </param>
    /// <returns>
    /// true, если элемент был удален и возвращен из коллекции; в противном случае false
    /// </returns>
    bool IProducerConsumerCollection<T>.TryTake(out T item) => TryTake(out item!);

    /// <summary>Получает количество элементов в коллекции</summary>
    public int Count => _Contained.Count;

    /// <summary>Создает массив, содержащий элементы коллекции</summary>
    /// <returns>Массив элементов</returns>
    public T[] ToArray() => [.. _Contained];

    /// <summary>Копирует элементы коллекции в массив</summary>
    /// <param name="array">Массив, в который должны быть скопированы данные</param>
    /// <param name="index">Начальный индекс, с которого должны быть скопированы данные</param>
    public void CopyTo(T[] array, int index) => _Contained.CopyTo(array, index);

    /// <summary>Копирует элементы коллекции в массив</summary>
    /// <param name="array">Массив, в который должны быть скопированы данные</param>
    /// <param name="index">Начальный индекс, с которого должны быть скопированы данные</param>
    void ICollection.CopyTo(Array array, int index) => _Contained.CopyTo(array, index);

    /// <summary>Получает перечислитель для коллекции</summary>
    /// <returns>Перечислитель</returns>
    public IEnumerator<T> GetEnumerator() => _Contained.GetEnumerator();

    /// <summary>Получает перечислитель для коллекции</summary>
    /// <returns>Перечислитель</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Получает значение, указывающее, является ли коллекция синхронизированной</summary>
    bool ICollection.IsSynchronized => _Contained.IsSynchronized;

    /// <summary>Получает объект корневой синхронизации для коллекции</summary>
    object ICollection.SyncRoot => _Contained.SyncRoot;
}

/// <summary>Представление отладки для интерфейса IProducerConsumerCollection</summary>
/// <typeparam name="T">Указывает тип агрегируемых данных</typeparam>
internal sealed class IProducerConsumerCollection_DebugView<T>(IProducerConsumerCollection<T> collection)
{
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Values => collection.ToArray();
}

/// <summary>Предоставляет потокобезопасный пул объектов</summary>
/// <typeparam name="T">Указывает тип элементов, хранящихся в пуле</typeparam>
/// <param name="Factory">Функция, используемая для создания элементов, когда в пуле нет элементов</param>
/// <param name="collection">Коллекция, используемая для хранения элементов пула</param>
/// <remarks>
/// ObjectPool предоставляет эффективный способ переиспользования объектов, избегая их постоянного создания и удаления.
/// Когда объект получается из пула, если пул пуст, используется предоставленная фабрика для создания нового объекта.
/// </remarks>
/// <example>
/// <code>
/// var pool = new ObjectPool&lt;StringBuilder&gt;(() => new StringBuilder());
/// var sb = pool.GetObject();
/// sb.Append("Hello");
/// pool.PutObject(sb); // вернуть в пул для переиспользования
/// </code>
/// </example>
[DebuggerDisplay("Count={" + nameof(Count) + "}")]
[DebuggerTypeProxy(typeof(IProducerConsumerCollection_DebugView<>))]
public sealed class ObjectPool<T>(Func<T> Factory, IProducerConsumerCollection<T> collection) : ProducerConsumerCollectionBase<T>(collection)
{
    private readonly Func<T> _Factory = Factory.NotNull();

    /// <summary>Инициализирует экземпляр класса ObjectPool с использованием очереди по умолчанию</summary>
    /// <param name="Factory">Функция, используемая для создания элементов, когда в пуле нет элементов</param>
    /// <example>
    /// <code>
    /// var pool = new ObjectPool&lt;byte[]&gt;(() => new byte[4096]);
    /// var buffer = pool.GetObject();
    /// // использовать буфер...
    /// pool.PutObject(buffer);
    /// </code>
    /// </example>
    public ObjectPool(Func<T> Factory) : this(Factory, new ConcurrentQueue<T>()) { }

    /// <summary>Добавляет предоставленный элемент в пул</summary>
    /// <param name="item">Элемент для добавления в пул</param>
    public void PutObject(T item) => base.TryAdd(item);

    /// <summary>Получает элемент из пула</summary>
    /// <returns>Удалённый или созданный элемент</returns>
    /// <remarks>Если пул пуст, будет создан и возвращен новый элемент с помощью функции Factory</remarks>
    public T GetObject() => base.TryTake(out var value) ? value : _Factory();

    /// <summary>Очищает пул объектов, возвращая все находящиеся в нём данные</summary>
    /// <returns>Массив, содержащий все элементы в пуле</returns>
    /// <remarks>После вызова этого метода пул будет пуст, и все элементы будут возвращены в массиве</remarks>
    public T[] ToArrayAndClear()
    {
        var items = new List<T>();
        while (base.TryTake(out var value))
            items.Add(value);
        return [.. items];
    }

    protected override bool TryAdd(T item)
    {
        PutObject(item);
        return true;
    }

    protected override bool TryTake(out T item)
    {
        item = GetObject();
        return true;
    }
}