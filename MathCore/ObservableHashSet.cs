using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using MathCore.Annotations;

namespace MathCore
{
    /// <summary>Хеш-таблица с уведомлением об изменениях</summary>
    /// <typeparam name="T">Тип элементов коллекции</typeparam>
    public class ObservableHashSet<T> : ICollection<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        #region INotifyPropertyChanged

        /// <summary>Событие происходит при изменении значения свойства</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Метод генерации события изменения значения свойства</summary>
        /// <param name="PropertyName">Имя изменившегося свойства</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        #endregion

        #region INotifyCollectionChanged

        /// <summary>Событие происходит при изменении коллекции</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>Метод генерации события изменения коллекции</summary>
        /// <param name="e">Параметры события</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

        #endregion

        /// <summary>Внутренняя хеш-таблица</summary>
        private readonly HashSet<T> _HashSet;

        /// <summary>Инициализация новой хеш-таблицы с уведомлениями об изменениях в содержимом</summary>
        public ObservableHashSet() : this(new()) { }

        /// <summary>Инициализация новой хеш-таблицы с уведомлениями об изменениях в содержимом</summary>
        /// <param name="Set">Внутренняя таблица</param>
        public ObservableHashSet(HashSet<T> Set) => _HashSet = Set ?? throw new ArgumentNullException(nameof(Set));

        /// <summary>Инициализация новой хеш-таблицы с уведомлениями об изменениях в содержимом</summary>
        /// <param name="Items">Исходный набор элементов</param>
        public ObservableHashSet(IEnumerable<T> Items) : this(new(Items)) { }

        /// <inheritdoc />
        public int Count => _HashSet.Count;

        /// <inheritdoc />
        bool ICollection<T>.IsReadOnly => false;

        /// <inheritdoc />
        public void Add(T item)
        {
            if (!_HashSet.Add(item)) return;
            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        /// <inheritdoc />
        public void Clear()
        {
            if (_HashSet.Count == 0) return;
            _HashSet.Clear();
            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <inheritdoc />
        public bool Contains(T item) => _HashSet.Contains(item);

        /// <inheritdoc />
        public void CopyTo(T[] array, int Index) => _HashSet.CopyTo(array, Index);

        /// <inheritdoc />
        public bool Remove(T item)
        {
            if (!_HashSet.Remove(item)) return false;
            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return true;
        }

        #region IEnumerable<T>

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => _HashSet.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_HashSet).GetEnumerator();

        #endregion
    }
}