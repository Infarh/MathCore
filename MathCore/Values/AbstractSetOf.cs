using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MathCore.Values
{
    /// <summary>Абстрактное множество элементов</summary>
    /// <typeparam name="T">Тип элементов множества</typeparam>
    public abstract class AbstractSetOf<T> : IEnumerable<T>
    {
        /// <summary>Мощность множества</summary>
        public abstract int Power { get; }

        /// <summary>Признак вхождения элемента в множество</summary>
        /// <param name="value">Проверяемый элемент</param>
        /// <returns>Истина, если элемент принедлежит множеству</returns>
        public virtual bool Contains(T value) => ((IEnumerable<T>)this).Contains(value);

        /// <summary>Признак того, что элемент не входит в множество</summary>
        /// <param name="value">Проверяемый элемент</param>
        /// <returns>Истина, если элемент не принадлежит множеству</returns>
        public virtual bool NotContains(T value) => !Contains(value);

        /// <summary>Получить перечислитель множества</summary>
        /// <returns>Перечислитель множества</returns>
        public abstract IEnumerator<T> GetEnumerator();

        /// <summary>Получить перечислитель множества</summary>
        /// <returns>Перечислитель множества</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}