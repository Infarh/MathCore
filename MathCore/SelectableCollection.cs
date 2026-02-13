using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using MathCore.Annotations;

namespace MathCore;

/// <summary>Коллекция, поддерживающая указание выбранного элемента</summary>
/// <typeparam name="T">Тип элементов коллекции</typeparam>
public class SelectableCollection<T> :
    INotifyPropertyChanged, INotifyCollectionChanged,
    ICollection<T?>, ICollection,
    IEnumerable<T?>, IEnumerable,
    IList<T?>, IList,
    IReadOnlyCollection<T?>,
    IReadOnlyList<T?>
{
    #region INotifyPropertyChanged

    /// <summary>Событие происходит при изменении значения свойства</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Метод генерации события изменения значения свойства</summary>
    /// <param name="PropertyName">Имя изменившегося свойства</param>
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null!) => PropertyChanged?.Invoke(this, new(PropertyName));

    /// <summary>Метод установки значения свойства с генерацией события изменения значения свойства</summary>
    /// <param name="field">Ссылка на поле</param>
    /// <param name="value">Устанавливаемое значение</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства было установлено</returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool Set<TValue>(ref TValue field, TValue value, [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(PropertyName);
        return true;
    }

    #endregion

    #region INotifyCollectionChanged

    /// <summary>Событие происходит при изменении коллекции</summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>Метод генерации события изменения коллекции</summary>
    /// <param name="e">Параметры события</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

    #endregion

    #region SelectedItem : T? - Выбранный элемент

    /// <summary>Выбранный элемент</summary>
    private T? _SelectedItem;

    /// <summary>Выбранный элемент</summary>
    public T? SelectedItem
    {
        get => _SelectedItem;
        set
        {
            if (ReferenceEquals(_SelectedItem, value)) return;
            if (value is null)
            {
                _SelectedItem = value;
                OnPropertyChanged();
                return;
            }
            if (!Contains(value)) return;
            _SelectedItem = value;
            OnPropertyChanged();
        }
    }

    #endregion

    /// <summary>Внутренняя коллекция</summary>
    private readonly ICollection<T?> _Collection;

    /// <summary>Коллекция поддерживает уведомления об изменениях</summary>
    private readonly bool _IsNotifyCollection;

    public T? this[int index]
    {
        get => _Collection switch
        {
            T?[] array => array[index],
            List<T?> list => list[index],
            IList<T?> list => list[index],
            _ => _Collection.ElementAt(index),
        };
        set
        {
            switch (_Collection)
            {
                case T?[] array: array[index] = value; break;
                case List<T?> list: list[index] = value; break;
                case IList<T?> list: list[index] = value; break;
                default:
                    var old_item = _Collection.ElementAt(index);
                    _Collection.Replace(old_item, value);
                    break;
            }
        }
    }

    /// <summary>Инициализация новой коллекции с возможностью выбора элемента</summary>
    public SelectableCollection() : this([]) { }

    /// <summary>Инициализация новой коллекции с возможностью выбора элемента</summary>
    /// <param name="Capacity">Ёмкость коллекции</param>
    public SelectableCollection(int Capacity) : this(new List<T?>(Capacity)) { }

    /// <summary>Инициализация новой коллекции с возможностью выбора элемента</summary>
    /// <param name="Collection">Внутренняя коллекция</param>
    public SelectableCollection(ICollection<T?> Collection)
    {
        if (Collection.NotNull() is not { IsReadOnly: false })
            throw new ArgumentException($"Коллекция {Collection.GetType()} доступна только для чтения", nameof(Collection));

        if (Collection is T[])
            throw new ArgumentException("Коллекция не должна быть массивом", nameof(Collection));

        _Collection = Collection;

        if (Collection is not INotifyCollectionChanged notify_collection) return;
        _IsNotifyCollection = true;
        notify_collection.CollectionChanged += OnSourceCollectionChanged;
    }

    /// <summary>Обработчик события изменения внутренней коллекции</summary>
    protected virtual void OnSourceCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
    {
        OnPropertyChanged(nameof(Count));

        switch (E.Action)
        {
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Replace:
                if (!Equals(_SelectedItem, default(T)) && E.OldItems?.Contains(_SelectedItem) == true)
                    SelectedItem = default;
                break;
            case NotifyCollectionChangedAction.Reset:
                if (!Equals(_SelectedItem, default(T)) && !_Collection.Contains(_SelectedItem))
                    SelectedItem = default;
                break;
        }

        OnCollectionChanged(E);
    }

    /// <summary>Автоматически выбирать последний добавленный элемент</summary>
    public bool SelectAddedItem { get; set => Set(ref field, value); }

    /// <inheritdoc />
    public int Count => _Collection.Count;

    /// <inheritdoc />
    public virtual void Add(T? item)
    {
        if (_IsNotifyCollection)
        {
            _Collection.Add(item);
            if (SelectAddedItem) SelectedItem = item;
            return;
        }

        var old_count = _Collection.Count;
        _Collection.Add(item);
        if (SelectAddedItem) SelectedItem = item;

        if (old_count == _Collection.Count) return;
        OnPropertyChanged(nameof(Count));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item, old_count));
    }

    /// <inheritdoc />
    public virtual void Clear()
    {
        if (_Collection.Count == 0) return;
        _Collection.Clear();
        OnPropertyChanged(nameof(Count));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        SelectedItem = default;
    }

    /// <inheritdoc />
    public bool Contains(T? item) => _Collection.Contains(item);

    /// <inheritdoc />
    public void CopyTo(T?[] array, int Index) => _Collection.CopyTo(array, Index);

    /// <inheritdoc />
    public virtual bool Remove(T? item)
    {
        var index = -1;
        switch (_Collection)
        {
            case List<T?> list:
                index = list.IndexOf(item);
                if (index < 0) return false;

                list.RemoveAt(index);
                break;

            case IList<T?> list:
                index = list.IndexOf(item);
                if (index < 0) return false;

                list.RemoveAt(index);
                break;

            default:
                if (!_Collection.Remove(item)) return false;
                break;
        }

        OnPropertyChanged(nameof(Count));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item, index));

        if (Equals(_SelectedItem, item))
            SelectedItem = default;

        return true;
    }

    /// <summary>Выбрать первый элемент коллекции</summary>
    /// <returns>Текущая коллекция</returns>
    public SelectableCollection<T> SelectFirst()
    {
        SelectedItem = _Collection.FirstOrDefault();
        return this;
    }

    /// <summary>Выбрать последний элемент коллекции</summary>
    /// <returns>Текущая коллекция</returns>
    public SelectableCollection<T> SelectLast()
    {
        SelectedItem = _Collection.LastOrDefault();
        return this;
    }

    /// <summary>Выбрать последний элемент коллекции</summary>
    /// <returns>Текущая коллекция</returns>
    public SelectableCollection<T?> SelectItem(T? item)
    {
        if (_Collection.Contains(item))
            SelectedItem = item;
        return this;
    }

    #region IList<T>

    int IList<T?>.IndexOf(T? item) => _Collection.FirstIndexOf(item);

    void IList<T?>.Insert(int index, T? item)
    {
        switch (_Collection)
        {
            default: throw new NotSupportedException($"Коллекция {_Collection.GetType()} не поддерживает операцию {typeof(IList<T>).Name}.Insert(index, item)");
            case T?[]: throw new NotSupportedException($"Невозможно свтавить элемент в массив по индексу {index}");
            case List<T?> list:
                list.Insert(index, item);
                OnPropertyChanged(nameof(Count));
                OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item, index));
                break;
            case IList<T?> list:
                list.Insert(index, item);
                OnPropertyChanged(nameof(Count));
                OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item, index));
                break;
        }
    }

    void IList<T?>.RemoveAt(int index)
    {
        switch (_Collection)
        {
            default: throw new NotSupportedException($"Коллекция {_Collection.GetType()} не поддерживает операцию {typeof(IList<T>).Name}.RemoveAt(index)");
            case T?[]: throw new NotSupportedException($"Невозможно удалить элемент в массив по индексу {index}");
            case List<T?> list:
                var item = this[index];
                list.RemoveAt(index);
                OnPropertyChanged(nameof(Count));
                OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item, index));
                break;
            case IList<T?> list:
                item = this[index];
                list.RemoveAt(index);
                OnPropertyChanged(nameof(Count));
                OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item, index));
                break;
        }
    }

    #endregion

    #region IList

    bool IList.IsFixedSize => (_Collection as IList)?.IsFixedSize ?? true;

    bool IList.IsReadOnly => (_Collection as IList)?.IsReadOnly ?? true;

    object? IList.this[int index] { get => this[index]; set => this[index] = (T?)value; }

    int IList.Add(object? value)
    {
        if (value is { } && !value.GetType().IsAssignableFrom(typeof(T)))
            throw new InvalidCastException($"Значение типа {value.GetType()} не может быть присвоено переменной типа {typeof(T)}");

        switch (_Collection)
        {
            case T?[]:
            default: throw new NotSupportedException($"Коллекция {_Collection.GetType()} не поддерживает операцию ILIst.Add(object)");
            case List<T?>:
            case IList<T?>:
            case IList:
                Add((T?)value);
                var index = Count - 1;
                OnPropertyChanged(nameof(Count));
                OnCollectionChanged(new(NotifyCollectionChangedAction.Add, (T?)value, index));
                return index;
        }
    }

    bool IList.Contains(object? value)
    {
        if (value is { } && !value.GetType().IsAssignableFrom(typeof(T)))
            throw new InvalidCastException($"Значение типа {value.GetType()} не может быть присвоено переменной типа {typeof(T)}");

        return _Collection switch
        {
            T?[] array => array.Contains((T?)value),
            List<T?> list => list.Contains((T?)value),
            IList<T?> list => list.Contains((T?)value),
            IList list => list.Contains((T?)value),
            _ => throw new NotSupportedException($"Коллекция {_Collection.GetType()} не поддерживает операцию ILIst.Contains(object)"),
        };
    }

    int IList.IndexOf(object? value)
    {
        if (value is { } && !value.GetType().IsAssignableFrom(typeof(T)))
            throw new InvalidCastException($"Значение типа {value.GetType()} не может быть присвоено переменной типа {typeof(T)}");

        return _Collection switch
        {
            T?[] array => Array.IndexOf(array, value),
            List<T?> list => list.IndexOf((T?)value),
            IList<T?> list => list.IndexOf((T?)value),
            IList list => list.IndexOf((T?)value),
            _ => throw new NotSupportedException($"Коллекция {_Collection.GetType()} не поддерживает операцию ILIst.Contains(object)"),
        };
    }

    void IList.Insert(int index, object? value)
    {
        if (value is { } && !value.GetType().IsAssignableFrom(typeof(T)))
            throw new InvalidCastException($"Значение типа {value.GetType()} не может быть присвоено переменной типа {typeof(T)}");

        switch (_Collection)
        {
            default: throw new NotSupportedException($"Коллекция {_Collection.GetType()} не поддерживает операцию IList.Insert(index, object)");
            case T?[]: throw new NotSupportedException($"Невозможно вставить элемент в массив по индексу {index}");
            case List<T?> list:
                list.Insert(index, (T?)value);
                OnPropertyChanged(nameof(Count));
                OnCollectionChanged(new(NotifyCollectionChangedAction.Add, (T?)value, index));
                break;
            case IList<T?> list:
                list.Insert(index, (T?)value);
                OnPropertyChanged(nameof(Count));
                OnCollectionChanged(new(NotifyCollectionChangedAction.Add, (T?)value, index));
                break;
        }
    }

    void IList.Remove(object? value)
    {
        if (value is { } && !value.GetType().IsAssignableFrom(typeof(T)))
            throw new InvalidCastException($"Значение типа {value.GetType()} не может быть присвоено переменной типа {typeof(T)}");

        switch (_Collection)
        {
            default: throw new NotSupportedException($"Коллекция {_Collection.GetType()} не поддерживает операцию IList.Remove(object)");
            case T?[]: throw new NotSupportedException($"Невозможно удалить элемент из массива");
            case List<T?>:
            case IList<T?>:
                if (Remove((T?)value))
                {
                    OnPropertyChanged(nameof(Count));
                    OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, new T?[] { (T?)value }));
                }
                break;
        }
    }

    void IList.RemoveAt(int index)
    {
        switch (_Collection)
        {
            default: throw new NotSupportedException($"Коллекция {_Collection.GetType()} не поддерживает операцию IList.RemoveAt(index)");
            case T?[]: throw new NotSupportedException($"Невозможно удалить элемент из массива по индексу {index}");
            case List<T?> list:
                var item = list[index];
                list.RemoveAt(index);
                OnPropertyChanged(nameof(Count));
                OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item, index));
                break;
            case IList<T?> list:
                item = list[index];
                list.RemoveAt(index);
                OnPropertyChanged(nameof(Count));
                OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, item, index));
                break;
        }
    }

    #endregion

    #region ICollection

    bool ICollection<T?>.IsReadOnly => _Collection.IsReadOnly;

    int ICollection.Count => _Collection.Count;

    bool ICollection.IsSynchronized => (_Collection as ICollection)?.IsSynchronized ?? false;

    object ICollection.SyncRoot => (_Collection as ICollection)?.SyncRoot ?? (field ??= new());

    void ICollection.CopyTo(Array array, int index)
    {
        var i = index;
        foreach (var item in _Collection)
        {
            if (index >= array.Length) return;
            array.SetValue(item, i++);
        }
    }

    #endregion

    #region IEnumerable<T>

    /// <inheritdoc />
    public IEnumerator<T?> GetEnumerator() => _Collection.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Collection).GetEnumerator();

    #endregion
}