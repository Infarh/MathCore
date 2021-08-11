#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;

namespace MathCore.Collections
{
    /// <summary>Функциональный односвязный список</summary>
    public static class FList
    {
        /// <summary>Пустой список</summary>
        /// <typeparam name="T">Тип элементов списка</typeparam>
        /// <returns>Возвращает единственный экземпляр пустого списка</returns>
        public static FList<T> Empty<T>() => FList<T>.Empty;

        /// <summary>Создать новый список, содержащий указанный элемент</summary>
        /// <typeparam name="T">Тип элементов списка</typeparam>
        /// <param name="item">Элемент, добавляемый в создаваемый список</param>
        /// <returns>Возвращает новый экземпляр списка, содержащий указанный элемент</returns>
        public static FList<T> New<T>(T item) => FList<T>.New(item, FList<T>.Empty);

        /// <summary>Создать новый список, содержащий указанные элементы</summary>
        /// <typeparam name="T">Тип элементов списка</typeparam>
        /// <param name="items">Элементы, добавляемые в список</param>
        /// <returns>Возвращает новый список, содержащий указанные элементы</returns>
        public static FList<T> New<T>(params T[] items) => New((IEnumerable<T>)items);

        /// <summary>Создать новый список, содержащий указанное перечисление элементов</summary>
        /// <typeparam name="T">Тип элементов списка</typeparam>
        /// <param name="items">Перечисление элементов, на основе которых необходимо создать новый список</param>
        /// <returns>Возвращает новый список, содержащий элементы из указанного перечисления</returns>
        public static FList<T> New<T>(IEnumerable<T> items) => FList<T>.New(items);
    }

    /// <summary>Функциональный список</summary>
    /// <typeparam name="T">Тип элементов списка</typeparam>
    public class FList<T> : IEnumerable<T>
    {
        /// <summary>Пустой список</summary>
        /// <remarks>Свойство содержит единственный для всего приложения экземпляр пустого списка</remarks>
        public static FList<T> Empty { get; } = new();

        /// <summary>Признак того, что текущий список является пустым</summary>
        public bool IsEmpty => ReferenceEquals(this, Empty);

        /// <summary>Головной элемент списка</summary>
        public T Head { get; }

        /// <summary>Хвост списка</summary>
        public FList<T> Tail { get; }

        /// <summary>Инициализация нового экземпляра функционального списка</summary>
        /// <remarks>
        /// Данный конструктор порождает пустой список.
        /// В процессе работы приложения данный конструктор может быть вызван единожды для
        /// инициализации значения свойства <see cref="Empty"/>. В противном случае конструктор
        /// генерирует исключение <see cref="InvalidOperationException"/>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Возникает в случае если конструктор вызывается в месте, отличном от инициализатора свойства <see cref="Empty"/>
        /// </exception>
        private FList()
        {
            if (Empty is not null)
                throw new InvalidOperationException("Нельзя создать ещё один пустой список. Используйте статическое свойство Empty");
            Head = default!;
            Tail = null!;
        }

        /// <summary>Инициализация нового списка с указанием его текущего головного элемента и хвоста</summary>
        /// <param name="Head"></param>
        /// <param name="Tail"></param>
        private FList(T Head, FList<T>? Tail = null)
        {
            this.Head = Head;
            this.Tail = Tail ?? Empty;
        }

        /// <summary>Создание нового списка на основе его головного элемента и хвоста</summary>
        /// <param name="Head">Головной элемент списка</param>
        /// <param name="Tail">Хвост списка. Если не указан, то используется пустой список <see cref="Empty"/></param>
        /// <returns></returns>
        public static FList<T> New(T Head, FList<T>? Tail = null) => new(Head, Tail ?? Empty);

        /// <summary>Создание нового списка на основе указанного перечисления элементов</summary>
        /// <param name="Items">Перечисление элементов создаваемого списка</param>
        /// <returns>Новый список, содержащий указанные элементы</returns>
        public static FList<T> New(IEnumerable<T> Items)
        {
            var stack = new Stack<T>(Items);
            if (stack.Count == 0) return Empty;
            var list = Empty;
            while (stack.Count > 0)
                list = list.AddFirst(stack.Pop());
            return list;
        }

        /// <summary>Добавить элемент в голову списка</summary>
        /// <param name="Item">Добавляемый элемент</param>
        /// <returns>Новый список, головным элементом которого является указанный, а хвостом - текущий список</returns>
        public FList<T> AddFirst(T Item) => New(Item, this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            var node = this;
            while (!node.IsEmpty)
            {
                yield return node.Head;
                node = node.Tail;
            }
        }

        /// <inheritdoc />
        public override string ToString() => $"[{string.Join("; ", this)}]";
    }
}
