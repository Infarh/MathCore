using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

namespace MathCore.Values
{
    /// <summary>Множество объектов типа <typeparamref name="T"/></summary>
    /// <typeparam name="T">Тип элементов множества</typeparam>
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
            return (T[])(Mixed ? IListExtentions.Mix(result) : result);
        }

        /// <summary>Клонирование множества элементов</summary>
        /// <returns></returns>
        [NotNull]
        public SetOf<T> Clone() => new SetOf<T>(this);

        public bool Equals(SetOf<T> other) => other is { }
                                              && (other.Power == Power
                                                  && (!this.Any(other.NotContains)
                                                      && !other.Any(NotContains)));

        public override string ToString() => $"Set of {typeof(T).Name}[{Power}]: {{{this.ToSeparatedStr(",").TrimByLength(40, " ... ")}}}";

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
        public bool Add(T item)
        {
            if (_List.Contains(item)) return false;
            _List.Add(item);
            return true;
        }

        void ICollection<T>.Add(T item) => Add(item);

        [NotNull]
        public KeyValuePair<T, bool>[] AddRange([NotNull] IEnumerable<T> collection) => collection.Select(item => new KeyValuePair<T, bool>(item, Add(item))).ToArray();

        /// <summary>Удаляет все элементы из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>.</summary>
        /// <exception cref="T:System.NotSupportedException">Интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения. </exception>
        public void Clear() => _List.Clear();

        /// <summary>
        /// Определяет, содержит ли интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> указанное значение.
        /// </summary>
        /// <returns>
        /// Значение true, если объект <paramref name="item"/> найден в <see cref="T:System.Collections.Generic.ICollection`1"/>; в противном случае — значение false.
        /// </returns>
        /// <param name="item">Объект, который требуется найти в <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public override bool Contains(T item) => _List.Contains(item);

        /// <summary>
        /// Копирует элементы <see cref="T:System.Collections.Generic.ICollection`1"/> в массив <see cref="T:System.Array"/>, начиная с указанного индекса <see cref="T:System.Array"/>.
        /// </summary>
        /// <param name="array">
        /// Одномерный массив <see cref="T:System.Array"/>, в который копируются элементы из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>. 
        /// Индексация в массиве <see cref="T:System.Array"/> должна начинаться с нуля.
        /// </param>
        /// <param name="arrayIndex">
        /// Значение индекса (с нуля) в массиве <paramref name="array"/>, с которого начинается копирование.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">Параметр <paramref name="array"/> имеет значение null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="arrayIndex"/> меньше 0.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// Массив <paramref name="array"/> является многомерным.
        ///   -или- Значение индекса массива <paramref name="arrayIndex"/> больше или равно длине массива <paramref name="array"/>.
        ///   -или-Количество элементов в исходном интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/> превышает размер доступного места, начиная с индекса <paramref name="arrayIndex"/> и до конца массива назначения <paramref name="array"/>.
        ///   -или-Тип не может быть автоматически приведен к типу массива назначения <paramref name="array"/>.
        /// </exception>
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => _List.CopyTo(array, arrayIndex);

        /// <summary>
        /// Удаляет первое вхождение указанного объекта из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// Значение true, если объект <paramref name="item"/> успешно удален из <see cref="T:System.Collections.Generic.ICollection`1"/>, в противном случае — значение false. Этот метод также возвращает значение false, если параметр <paramref name="item"/> не найден в исходном интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">Объект, который необходимо удалить из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">Интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.</exception>
        public bool Remove(T item) => _List.Remove(item);

        /// <summary>
        /// Получает число элементов, содержащихся в интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// Число элементов, содержащихся в интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        int ICollection<T>.Count => Power;

        /// <summary>
        /// Получает значение, указывающее, доступен ли интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> только для чтения.
        /// </summary>
        /// <returns>
        /// Значение true, если интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения, в противном случае — значение false.
        /// </returns>
        bool ICollection<T>.IsReadOnly => false;

        #endregion
    }
}