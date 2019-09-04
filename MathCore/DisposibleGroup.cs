using System;
using System.Collections;
using System.Collections.Generic;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using System.Linq;

namespace MathCore
{
    /// <summary>Группа объектов, поддерживающих интерфейс <see cref="T:System.IDisposable">освобождения ресурсов</see></summary>
    /// <typeparam name="T">Тип объектов, подерживающих интерфейс <see cref="T:System.IDisposable"/></typeparam>
    public class DisposableGroup<T> : IDisposable, IEnumerable<T>, IIndexableRead<int, T> where T : IDisposable
    {
        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Массив содержащихся объектов интерфейса <see cref="T:System.IDisposable"/></summary>
        private readonly T[] _Items;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Число элементов группы</summary>
        public int Count => _Items.Length;

        /// <summary>Массив элементов группы</summary>
        public T[] Items => _Items;

        /// <summary>Элемент группы</summary>
        /// <param name="i">Номер элемента группы</param>
        /// <returns>Элемент группы с номером <paramref name="i"/></returns>
        public T this[int i] { [DST] get => _Items[i]; }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Группа <typeparam name="T">объектов</typeparam> интерфейса <see cref="T:System.IDisposable"/></summary>
        /// <param name="item"><typeparam name="T">Объект</typeparam> интерфейса <see cref="T:System.IDisposable"/></param>
        [DST]
        public DisposableGroup(params T[] item) => _Items = item;

        /// <summary>Группа <typeparam name="T">объектов</typeparam> интерфейса <see cref="T:System.IDisposable"/></summary>
        /// <param name="items">Перечисление <typeparam name="T">объектов</typeparam> интерфейса <see cref="T:System.IDisposable"/></param>
        [DST]
        public DisposableGroup(IEnumerable<T> items) : this(items.ToArray()) { }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Освободить ресурсы группы</summary>
        [DST]
        public void Dispose() => _Items.Foreach(i => i.Dispose());

        /// <summary>Получить перечислитель элементов группы</summary>
        /// <returns>Перечислитель элементов группы</returns>
        [DST]
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_Items).GetEnumerator();


        /// <summary>Возвращает перечислитель, который осуществляет перебор элементов коллекции.</summary>
        /// <returns>
        /// Объект <see cref="T:System.Collections.IEnumerator"/>, который может использоваться для перебора элементов коллекции.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        [DST]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /* ------------------------------------------------------------------------------------------ */
    }
}