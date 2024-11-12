#nullable enable
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using MathCore.Annotations;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

public class ObservableLinkedList<T> :
    ICollection,
    ISerializable,
    IDeserializationCallback,
    IList<T>,
    INotifyCollectionChanged,
    INotifyPropertyChanged
{
    private const string __IndexerName = "Item[]";

    #region INotifyCollectionChanged Members

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>Метод генерации события изменения коллекции</summary>
    /// <param name="Handler">Обработчик события</param>
    /// <param name="e">Параметры события</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventHandler? Handler, NotifyCollectionChangedEventArgs e)
    {
        e.NotNull();
        if (Handler is null) return;
        var invocation_list = Handler.GetInvocationList();
        var args = new object[] { this, e };
        foreach (var d in invocation_list)
        {
            var o = d.Target;
            switch (o)
            {
                case ISynchronizeInvoke { InvokeRequired: true } synchronize_invoke:
                    synchronize_invoke.Invoke(d, args);
                    break;
                default:
                    d.DynamicInvoke(args);
                    break;
            }
        }
    }

    protected virtual void OnCollectionItemAdded(T ChangedItem, int Index) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, ChangedItem, Index));
    protected virtual void OnCollectionItemRemoved(T ChangedItem, int Index) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ChangedItem, Index));
    protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Reset) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
    protected virtual void OnCollectionChanged(T NewValue, int index) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, NewValue, index));
    protected virtual void OnCollectionChanged(T OldValue, T NewValue, int index) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, NewValue, OldValue, index));

    /// <summary>
    /// Метод генерации события изменения коллекции
    /// </summary>
    /// <param name="e">Параметры события</param>
    /// <remarks>
    /// Метод генерирует событие <see cref="CollectionChanged"/> с указанными параметрами,
    /// а также события свойства <see cref="Count"/> и <see cref="this[int]"/>.
    /// </remarks>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        OnCollectionChanged(CollectionChanged, e);
        OnPropertyChanged(nameof(Count));
        OnPropertyChanged(__IndexerName);
    }

    #endregion

    #region INotiftPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null!) => PropertyChanged?.Invoke(this, new(PropertyName));

    #endregion

    #region Поля

    /// <summary>Связный список элементов</summary>
    private readonly LinkedList<T> _List;

    #endregion

    #region Свойства

    /// <summary>Число элементов</summary>
    public int Count => _List.Count;

    /// <summary>Первый элемент списка</summary>
    public LinkedListNode<T> First => _List.First;

    /// <summary>Последний элемент списка</summary>
    public LinkedListNode<T> Last => _List.Last;

    bool ICollection<T>.IsReadOnly => false;

    object ICollection.SyncRoot => ((ICollection)_List).SyncRoot;

    bool ICollection.IsSynchronized => ((ICollection)_List).IsSynchronized;

    public T this[int index]
    {
        get => NodeAt(index).Value;
        set
        {
            var node = NodeAt(index);
            var old_value = node.Value;
            node.Value = value;
            OnCollectionChanged(old_value, value, index);
        }
    }

    #endregion

    #region Конструкторы

    /// <summary>Инициализация нового экземпляра <see cref="ObservableLinkedList{T}"/></summary>
    public ObservableLinkedList() => _List = new();

    /// <summary>Инициализация нового экземпляра <see cref="ObservableLinkedList{T}"/></summary>
    /// <param name="collection">Исходная последовательность элементов, добавляемая в список при инициализации</param>
    public ObservableLinkedList(IEnumerable<T> collection) => _List = new(collection);

    #endregion

    #region Методы 

    public void Add(T? value) => AddLast(value);

    /// <summary>Добавить элемент после указанного элемента</summary>
    /// <param name="PrevNode">Элемент, после которого надо добавить значение в список</param>
    /// <param name="value">Добавляемое значение</param>
    /// <returns>Новый элемент списка</returns>
    public LinkedListNode<T> AddAfter(LinkedListNode<T> PrevNode, T value)
    {
        var result = _List.AddAfter(PrevNode, value);
        OnCollectionItemAdded(value, IndexOf(result));
        if (ReferenceEquals(result, _List.Last)) OnPropertyChanged(nameof(Last));
        return result;
    }

    public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> NewNode)
    {
        _List.AddAfter(node, NewNode);
        OnCollectionItemAdded(NewNode.Value, IndexOf(NewNode));
        if (ReferenceEquals(node, _List.Last)) OnPropertyChanged(nameof(Last));
    }

    /// <summary>Добавить новый элемент перед указанным элементом</summary>
    /// <param name="node">Элемент, перед которым надо добавить значение</param>
    /// <param name="value">Добавляемое значение</param>
    /// <returns>Новый элемент списка</returns>
    public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
    {
        var result = _List.AddBefore(node, value);
        OnCollectionItemAdded(value, IndexOf(result));
        if (ReferenceEquals(node, _List.First)) OnPropertyChanged(nameof(First));
        return result;
    }

    /// <summary>Добавить новый узел перед указанным узлом</summary>
    /// <param name="node">Узел, перед которым надо добавить новый узел</param>
    /// <param name="NewNode">Добавляемый узел</param>
    /// <remarks>
    /// Метод генерирует событие <see cref="CollectionChanged"/>,
    /// а также события свойства <see cref="First"/>, если новый узел добавлен перед первым узлом.
    /// </remarks>
    public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> NewNode)
    {
        _List.AddBefore(node, NewNode);
        OnCollectionItemAdded(NewNode.Value, IndexOf(NewNode));
        if (ReferenceEquals(node, _List.First)) OnPropertyChanged(nameof(First));
    }

    /// <summary>Добавить новый элемент в начало списка</summary>
    /// <param name="value">Добавляемое значение</param>
    /// <returns>Новый элемент списка</returns>
    /// <remarks>
    /// Метод генерирует событие <see cref="CollectionChanged"/>,
    /// а также событие свойства <see cref="First"/>.
    /// </remarks>
    public LinkedListNode<T> AddFirst(T value)
    {
        var result = _List.AddFirst(value);
        OnCollectionItemAdded(value, 0);
        OnPropertyChanged(nameof(First));
        return result;
    }

    /// <summary>Добавить новый узел в начало списка</summary>
    /// <param name="node">Добавляемый узел</param>
    /// <remarks>
    /// Метод генерирует событие <see cref="CollectionChanged"/>,
    /// а также событие свойства <see cref="First"/>.
    /// </remarks>
    public void AddFirst(LinkedListNode<T> node)
    {
        _List.AddFirst(node);
        OnCollectionItemAdded(node.Value, 0);
        OnPropertyChanged(nameof(First));
    }

    /// <summary>Добавить новый элемент в конец списка</summary>
    /// <param name="value">Добавляемое значение</param>
    /// <returns>Новый элемент списка</returns>
    /// <remarks>
    /// Метод генерирует событие <see cref="CollectionChanged"/>,
    /// а также событие свойства <see cref="Last"/>.
    /// </remarks>
    public LinkedListNode<T> AddLast(T value)
    {
        var result = _List.AddLast(value);
        OnCollectionItemAdded(value, _List.Count - 1);
        OnPropertyChanged(nameof(Last));
        return result;
    }

    /// <summary>Добавить новый узел в конец списка</summary>
    /// <param name="node">Добавляемый узел</param>
    /// <remarks>
    /// Метод генерирует событие <see cref="CollectionChanged"/>,
    /// а также событие свойства <see cref="Last"/>.
    /// </remarks>
    public void AddLast(LinkedListNode<T> node)
    {
        _List.AddLast(node);
        OnCollectionItemAdded(node.Value, _List.Count - 1);
        OnPropertyChanged(nameof(Last));
    }

    /// <summary>Очистка списка</summary>
    /// <remarks>
    /// Метод генерирует событие <see cref="CollectionChanged"/>,
    /// а также события свойств <see cref="First"/> и <see cref="Last"/>.
    /// </remarks>
    public void Clear()
    {
        _List.Clear();
        OnCollectionChanged();
        OnPropertyChanged(nameof(First));
        OnPropertyChanged(nameof(Last));
    }

    /// <summary>Определение индекса заданного узла</summary>
    /// <param name="node">Искомый узел</param>
    /// <returns>Индекс заданного узла, если он найден, либо -1</returns>
    public int IndexOf(LinkedListNode<T> node)
    {
        var root = _List.First;
        for (var n = 0; root != null; root = root.Next, n++)
            if (node == root) return n;
        return -1;
    }

    /// <summary>Определение индекса заданного значения</summary>
    /// <param name="value">Искомое значение</param>
    /// <returns>Индекс заданного значения, если он найден, либо -1</returns>
    public int IndexOf(T? value)
    {
        var node = _List.First;
        for (var n = 0; node != null; node = node.Next, n++)
            if (Equals(node.Value, value)) return n;
        return -1;
    }

    /// <summary>Определение индекса заданного значения и получение соответствующего узла</summary>
    /// <param name="value">Искомое значение</param>
    /// <param name="node">Узел, содержащий искомое значение, если найдено; иначе null</param>
    /// <returns>Индекс заданного значения, если он найден, либо -1</returns>
    public int IndexOf(T value, out LinkedListNode<T>? node)
    {
        node = _List.First;
        for (var n = 0; node != null; node = node.Next, n++)
            if (Equals(node.Value, value)) return n;
        node = null;
        return -1;
    }

    /// <summary>Получение узла по индексу</summary>
    /// <param name="index">Индекс узла</param>
    /// <returns>Узел по заданному индексу, если он существует, иначе null</returns>
    /// <remarks>
    /// Метод работает за O(n / 2), т.е. за половину от времени, которое потребовалось бы для прохода списка от начала до конца.
    /// </remarks>
    public LinkedListNode<T>? NodeAt(int index)
    {
        var count = Count;
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException(nameof(index), @"Индекс вышел за границы списка");
        LinkedListNode<T>? node;
        if (index > count / 2)
        {
            node = _List.Last;
            while (++index < count && node is not null) node = node.Previous;
        }
        else
        {
            node = _List.First;
            while (index-- > 0 && node is not null) node = node.Next;
        }
        return node;
    }

    public bool Contains(T? value) => _List.Contains(value);

    public void CopyTo(T[] array, int index) => _List.CopyTo(array, index);

    public void CopyTo(Array array, int index) => ((ICollection)_List).CopyTo(array, index);

    public bool LinkedListEquals(object obj) => _List.Equals(obj);

    public LinkedListNode<T>? Find(T value) => _List.Find(value);

    public LinkedListNode<T>? FindLast(T value) => _List.FindLast(value);

    /// <summary>Удалить элемент из списка</summary>
    /// <param name="value">Удаляемый элемент</param>
    /// <returns>Истина - элемент удален, ложь - элемент не найден</returns>
    /// <remarks>
    /// Генерирует событие <see cref="CollectionChanged"/> и события свойств <see cref="First"/> и <see cref="Last"/>,
    /// если соответствующие узлы изменяются.
    /// </remarks>
    public bool Remove(T? value)
    {
        var index = IndexOf(value, out var node);
        var is_first = ReferenceEquals(_List.First, node);
        var is_last = ReferenceEquals(_List.Last, node);

        if (!_List.Remove(value)) return false;

        OnCollectionItemRemoved(value, index);

        if (is_first) OnPropertyChanged(nameof(First));
        if (is_last) OnPropertyChanged(nameof(Last));

        return true;
    }

    /// <summary>Удаляет узел из списка</summary>
    /// <param name="node">Удаляемый узел</param>
    /// <remarks>
    /// Генерирует событие <see cref="CollectionChanged"/> и события свойств <see cref="First"/> и <see cref="Last"/>,
    /// если соответствующие узлы изменяются.
    /// </remarks>
    public void Remove(LinkedListNode<T> node)
    {
        var index = IndexOf(node.NotNull());
        var is_first = ReferenceEquals(_List.First, node);
        var is_last = ReferenceEquals(_List.Last, node);

        _List.Remove(node);

        OnCollectionItemRemoved(node.Value, index);

        if (is_first) OnPropertyChanged(nameof(First));
        if (is_last) OnPropertyChanged(nameof(Last));
    }

    /// <summary>Удаляет первый элемент из списка</summary>
    /// <remarks>
    /// Если список пуст, метод ничего не делает. Генерирует событие <see cref="CollectionChanged"/>
    /// и обновляет свойство <see cref="First"/>.
    /// </remarks>
    public void RemoveFirst()
    {
        if (_List.Count == 0) return;

        var item = _List.First.Value;
        _List.RemoveFirst();

        OnCollectionItemRemoved(item, 0);
        OnPropertyChanged(nameof(First));
    }

    /// <summary>Удаляет последний узел из списка</summary>
    /// <remarks>
    /// Если список пуст, метод ничего не делает. Генерирует событие <see cref="CollectionChanged"/>
    /// и обновляет свойство <see cref="Last"/>.
    /// </remarks>
    public void RemoveLast()
    {
        var count = _List.Count;
        if (count == 0) return;

        var item = _List.First.Value;
        _List.RemoveLast();
        
        OnCollectionItemRemoved(item, count);
        OnPropertyChanged(nameof(Last));
    }

    public Type GetLinkedListType() => _List.GetType();

    #endregion

    #region IEnumerable

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => (_List as IEnumerable<T>).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (_List as IEnumerable).GetEnumerator();

    #endregion

#pragma warning disable SYSLIB0051
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) => _List.GetObjectData(info, context);
#pragma warning restore SYSLIB0051

    void IDeserializationCallback.OnDeserialization(object? sender) => _List.OnDeserialization(sender);

    public void Insert(int index, T? item) => AddAfter(NodeAt(index), item);

    public void RemoveAt(int index) => Remove(NodeAt(index));
}