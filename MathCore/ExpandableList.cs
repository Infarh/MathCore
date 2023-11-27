#nullable enable
using System.Collections;
using System.Collections.Generic;

namespace MathCore;

/// <summary>Расширяемый список</summary>
/// <typeparam name="T">Тип элементов списка</typeparam>
/// <remarks>Инициализация нового расширяемого списка</remarks>
/// <param name="BaseList">Базовый список</param>
public class ExpandableList<T>(List<T> BaseList) : IList<T>, IReadOnlyList<T>
{
    /// <summary>Инициализация нового расширяемого списка</summary>
    public ExpandableList() : this((List<T>)[]) { }

    /// <summary>Инициализация нового расширяемого списка</summary>
    /// <param name="Capacity">Ёмкость</param>
    public ExpandableList(int Capacity) : this((List<T>)new(Capacity)) { }

    /// <summary>Инициализация нового расширяемого списка</summary>
    /// <param name="items">Исходный набор элементов</param>
    public ExpandableList(IEnumerable<T> items) : this((List<T>)items.ToList()) { }

    /// <summary>Базовый список, обеспечивающий хранение данных</summary>
    public List<T> BaseList { get; } = BaseList;

    #region Implementation of IList<T>

    /// <inheritdoc />
    public int IndexOf(T? item) => BaseList.IndexOf(item);
    
    /// <inheritdoc />
    public void Insert(int index, T? item) => BaseList.Insert(index, item);
    
    /// <inheritdoc />
    public void RemoveAt(int index) => BaseList.RemoveAt(index);

    public T? this[int index]
    {
        get
        {
            var list = BaseList;
            return index < list.Count ? list[index] : default;
        }
        set
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), value, "Индекс должен быть больше, либо равно 0");

            var list  = BaseList;
            var count = list.Count;
            if (index < count)
            {
                list[index] = value;
                return;
            }

            if (list.Capacity < index + 1)
                list.Capacity = index + 1;

            for(var i = count; i < index; i++)
                list.Add(default);
            
            list.Add(value);
        }
    }

    /// <summary>Удаляет <c>default(T)</c> элементы в конце списка и уменьшает <see cref="BaseList"/>.<see cref="List{T}.Capacity"/> до числа используемых элементов</summary>
    /// <returns>Истина, если удаление пустого пространства в хвосте списка выполнено успешно</returns>
    public bool TrimExcess()
    {
        var list = BaseList;
        if (list.Count == 0) return false;

        var any = false;
        while (list.Count > 0 && Equals(list[^1], default(T)))
        {
            list.RemoveAt(list.Count - 1);
            any = true;
        }

        if (!any) return false;

        list.TrimExcess();
        return true;
    }

    #endregion

    #region Implementation of ICollection<T>

    /// <inheritdoc />
    public void Add(T? item) => BaseList.Add(item);
    
    /// <inheritdoc />
    public void Clear() => BaseList.Clear();
    
    /// <inheritdoc />
    public bool Contains(T? item) => BaseList.Contains(item);
    
    /// <inheritdoc />
    public void CopyTo(T[] array, int index) => BaseList.CopyTo(array, index);
    
    /// <inheritdoc />
    public bool Remove(T? item) => BaseList.Remove(item);

    /// <summary>Число элементов списка</summary>
    public int Count
    {
        get => BaseList.Count;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, "Размер списка не может быть отрицательным");

            if (value == 0)
            {
                Clear();
                return;
            }

            var list = BaseList;
            switch (value - BaseList.Count)
            {
                case 0: return; // устанавливаемая длина равна исходной - ничего не делаем

                case > 0 and var delta: // Требуется увеличить размер списка
                    if (list.Capacity - list.Count < delta)
                        list.Capacity = list.Count + delta;

                    for(var i = 0; i < delta; i++)
                        list.Add(default);

                    break;

                case < 0 and var delta: // требуется уменьшить размер списка

                    var last_index = list.Count - 1;
                    for (var i = 0; i < -delta; i++)
                        RemoveAt(last_index - i);

                    break;
            }
        }
    }

    public int Capacity
    {
        get => BaseList.Capacity;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, "Ёмкость списка не может быть отрицательным");

            if (value == 0)
            {
                Clear();
                BaseList.TrimExcess();
                return;
            }

            var list          = BaseList;
            var list_capacity = BaseList.Capacity;
            switch (value - list_capacity)
            {
                case 0: return; // устанавливаемая длина равна исходной - ничего не делаем

                case > 0: // Требуется увеличить размер списка
                    list.Capacity = value;
                    break;

                case < 0: // требуется уменьшить размер списка

                    if (value < list.Count)
                    {
                        Count = value;
                        list.TrimExcess();
                    }
                    else if (value > Count)
                        list.Capacity = value;
                    else
                        list.TrimExcess();

                    break;
            }
        }
    }

    /// <inheritdoc />
    public bool IsReadOnly => ((IList<T>)BaseList).IsReadOnly;

    #endregion

    #region Implementation of IEnumerable

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => BaseList.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)BaseList).GetEnumerator();

    #endregion
}
