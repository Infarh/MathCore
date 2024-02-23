#nullable enable
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using MathCore.Annotations;

namespace MathCore;

/// <summary>Коллекция, поддерживающая указание выбранного элемента</summary>
/// <typeparam name="T">Тип элементов коллекции</typeparam>
public class SelectableCollection<T> : ICollection<T>, INotifyPropertyChanged, INotifyCollectionChanged
{
    #region INotifyPropertyChanged

    /// <summary>Событие происходит при изменении значения свойства</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Метод генерации события изменения значения свойства</summary>
    /// <param name="PropertyName">Имя изменившегося свойства</param>
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null!) => PropertyChanged?.Invoke(this, new(PropertyName));

    #endregion

    #region INotifyCollectionChanged

    /// <summary>Событие происходит при изменении коллекции</summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>Метод генерации события изменения коллекции</summary>
    /// <param name="e">Параметры события</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

    #endregion

    #region SelectedItem : T - Выбранный элемент

    /// <summary>Выбранный элемент</summary>
    private T _SelectedItem;

    /// <summary>Выбранный элемент</summary>
    public T SelectedItem
    {
        get => _SelectedItem;
        set
        {
            if (ReferenceEquals(_SelectedItem, value)) return;
            if (ReferenceEquals(value, default))
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
    private readonly ICollection<T> _Collection;

    /// <summary>Коллекция поддерживает уведомления об изменениях</summary>
    private readonly bool _IsNotifyCollection;

    /// <summary>Инициализация новой коллекции с возможностью выбора элемента</summary>
    public SelectableCollection() : this(new List<T>()) { }

    /// <summary>Инициализация новой коллекции с возможностью выбора элемента</summary>
    /// <param name="Capacity">Ёмкость коллекции</param>
    public SelectableCollection(int Capacity) : this(new List<T>(Capacity)) { }

    /// <summary>Инициализация новой коллекции с возможностью выбора элемента</summary>
    /// <param name="Collection">Внутренняя коллекция</param>
    public SelectableCollection(ICollection<T> Collection)
    {
        if(Collection.NotNull() is not { IsReadOnly: false })
            throw new ArgumentException($"Коллекция {Collection.GetType()} доступна только для чтения", nameof(Collection));

        if (Collection is T[])
            throw new ArgumentException("Коллекция не должна быть массивом", nameof(Collection));

        _Collection = Collection.NotNull();

        if (Collection is not INotifyCollectionChanged notify_collection) return;
        _IsNotifyCollection                 =  true;
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
                if (!Equals(_SelectedItem, default(T)) && E.OldItems.Contains(_SelectedItem))
                    SelectedItem = default;
                break;
            case NotifyCollectionChangedAction.Reset:
                if (!Equals(_SelectedItem, default(T)) && !_Collection.Contains(_SelectedItem))
                    SelectedItem = default;
                break;
        }

        OnCollectionChanged(E);
    }

    /// <inheritdoc />
    public int Count => _Collection.Count;

    /// <inheritdoc />
    bool ICollection<T>.IsReadOnly => _Collection.IsReadOnly;

    /// <inheritdoc />
    public void Add(T? item)
    {
        if (_IsNotifyCollection)
        {
            _Collection.Add(item);
            return;
        }

        var old_count = _Collection.Count;
        _Collection.Add(item);
        if (old_count == _Collection.Count) return;
        OnPropertyChanged(nameof(Count));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Add, item, old_count));
    }

    /// <inheritdoc />
    public void Clear()
    {
        if(_Collection.Count == 0) return;
        _Collection.Clear();
        OnPropertyChanged(nameof(Count));
        OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        SelectedItem = default;
    }

    /// <inheritdoc />
    public bool Contains(T? item) => _Collection.Contains(item);

    /// <inheritdoc />
    public void CopyTo(T[] array, int Index) => _Collection.CopyTo(array, Index);

    /// <inheritdoc />
    public bool Remove(T? item)
    {
        var index = -1;
        switch (_Collection)
        {
            case List<T> list:
                index = list.IndexOf(item);
                if (index < 0) return false;

                list.RemoveAt(index);
                break;

            case IList<T> list:
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

    #region IEnumerable<T>

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => _Collection.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Collection).GetEnumerator();

    #endregion
}