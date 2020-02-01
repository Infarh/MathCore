using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable StaticMemberInGenericType
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.Values
{
    /// <summary>Множество объектов типа <typeparamref name="T"/></summary>
    /// <typeparam name="T">Тип элементов множества</typeparam>
    [Obsolete("Используйте Set<T>")]
    public partial class SetOf<T> : AbstractSetOf<T>, ICollection<T>, ICloneable<SetOf<T>>, IEquatable<SetOf<T>>
    {
        /// <summary>Перемешивать содержимое при выдаче массива элементов</summary>
        public static bool Mixed { get; set; }

        /// <summary>Список элементов</summary>
        private readonly List<T> _List;

        /// <summary>Мощность множества</summary>
        public override int Power => _List.Count;

        /// <summary>Новое множество элементов</summary>
        public SetOf() => _List = new List<T>();

        /// <summary>Новое множество элементов</summary>
        /// <param name="Capacity">Ёмкость множества</param>
        public SetOf(int Capacity) => _List = new List<T>(Capacity);

        /// <summary>Новое множество элементов</summary>
        /// <param name="collection">Коллекция элементов</param>
        public SetOf([NotNull] IEnumerable<T> collection) => _List = new List<T>(collection);

        /// <summary>Новое множество элементов</summary>
        /// <param name="element">Элементы множества</param>
        public SetOf([NotNull] params T[] element) : this((IEnumerable<T>)element) { }

        /// <summary>Преобразование в список</summary>
        /// <returns>Список элементов</returns>
        public List<T> ToList() => (List<T>)(Mixed ? new List<T>(_List).Mix() : new List<T>(_List));

        /// <summary>Преобразование множества элементов в массив</summary>
        /// <returns>Массив элементов</returns>
        public T[] ToArray()
        {
            var result = _List.ToArray();
            return (T[])(Mixed ? IListExtensions.Mix(result) : result);
        }

        /// <summary>Клонирование множества элементов</summary>
        /// <returns></returns>
        [NotNull]
        public SetOf<T> Clone() => new SetOf<T>(this);

        /// <inheritdoc />
        public bool Equals(SetOf<T> other) => other is { }
                                              && (other.Power == Power
                                                  && (!this.Any(other.NotContains)
                                                      && !other.Any(NotContains)));

        /// <inheritdoc />
        [NotNull] public override string ToString() => $"Set of {typeof(T).Name}[{Power}]: {{{this.ToSeparatedStr(",").TrimByLength(40, " ... ")}}}";

        object ICloneable.Clone() => Clone();

        #region Implementation of IEnumerable

        /// <summary>Возвращает перечислитель, выполняющий перебор элементов в коллекции.</summary>
        /// <returns>
        /// Интерфейс <see cref="T:System.Collections.Generic.IEnumerator`1"/>, который может использоваться для перебора элементов коллекции.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override IEnumerator<T> GetEnumerator() => ToList().GetEnumerator();

        /// <summary>Возвращает перечислитель, который осуществляет перебор элементов коллекции.</summary>
        /// <returns>
        /// Объект <see cref="T:System.Collections.IEnumerator"/>, который может использоваться для перебора элементов коллекции.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Implementation of ICollection<T>

        /// <summary>Добавляет элемент в интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/>.</summary>
        /// <param name="item">Объект, добавляемый в интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">Интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.</exception>
        public override bool Add(T item)
        {
            if (_List.Contains(item)) return false;
            _List.Add(item);
            return true;
        }

        /// <inheritdoc />
        void ICollection<T>.Add(T item) => Add(item);

        [NotNull]
        public KeyValuePair<T, bool>[] AddRange([NotNull] IEnumerable<T> collection) => collection.Select(item => new KeyValuePair<T, bool>(item, Add(item))).ToArray();

        /// <inheritdoc />
        public void Clear() => _List.Clear();

        /// <inheritdoc cref="T:System.Collections.Generic.ICollection`1" />
        public override bool Contains(T item) => _List.Contains(item);

        /// <inheritdoc />
        void ICollection<T>.CopyTo(T[] array, int ArrayIndex) => _List.CopyTo(array, ArrayIndex);

        /// <inheritdoc />
        public bool Remove(T item) => _List.Remove(item);

        /// <inheritdoc />
        int ICollection<T>.Count => Power;

        /// <inheritdoc />
        bool ICollection<T>.IsReadOnly => false;

        #endregion
    }
}