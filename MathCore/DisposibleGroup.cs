using System;
using System.Collections;
using System.Collections.Generic;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using System.Linq;
using System.Runtime.Serialization.Json;
using MathCore.Annotations;

// ReSharper disable UnusedMember.Global

// ReSharper disable UnusedType.Global

namespace MathCore
{
    /// <summary>Группа объектов, поддерживающих интерфейс <see cref="T:System.IDisposable">освобождения ресурсов</see></summary>
    /// <typeparam name="T">Тип объектов, поддерживающих интерфейс <see cref="T:System.IDisposable"/></typeparam>
    public class DisposableGroup<T> : IDisposable, IEnumerable<T>, IIndexableRead<int, T> where T : IDisposable
    {
        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Массив содержащихся объектов интерфейса <see cref="T:System.IDisposable"/></summary>
        private readonly T[] _Items;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Число элементов группы</summary>
        public int Count => _Items.Length;

        /// <summary>Массив элементов группы</summary>
        public IReadOnlyList<T> Items => _Items;

        /// <inheritdoc />
        T IIndexableRead<int, T>.this[int i] { [DST] get => _Items[i]; }

         /// <summary>Элемент группы</summary>
        /// <param name="i">Номер элемента группы</param>
        /// <returns>Элемент группы с номером <paramref name="i"/></returns>
        public ref readonly T this[int i] => ref _Items[i];

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Группа <typeparamref name="T"/> интерфейса <see cref="T:System.IDisposable"/></summary>
        /// <param name="item"><typeparamref name="T"/> интерфейса <see cref="T:System.IDisposable"/></param>
        [DST]
        public DisposableGroup(params T[] item) => _Items = item;

        /// <summary>Группа <typeparamref name="T"/> интерфейса <see cref="T:System.IDisposable"/></summary>
        /// <param name="items">Перечисление <typeparamref name="T"/> интерфейса <see cref="T:System.IDisposable"/></param>
        [DST]
        public DisposableGroup([NotNull] IEnumerable<T> items) : this(items.ToArray()) { }

        /* ------------------------------------------------------------------------------------------ */

        /// <inheritdoc />
        [DST]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Объект уничтожен</summary>
        private bool _Disposed;

        /// <summary>Освободить ресурсы группы</summary>
        /// <param name="disposing">Признак того, что требуется освобождение управляемых ресурсов</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _Disposed) return;
            _Disposed = true;
            _Items.Foreach(i => i.Dispose());
        }

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