﻿using System;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace MathCore
{
    /// <summary>Оболочка, обеспечивающая освобождение ресурсов указаннцм методом для указанного объекта</summary>
    /// <typeparam name="T">Тип объекта, с которым работает оболочка</typeparam>
    public class UsingObject<T> : IDisposable
    {
        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Используемый объект</summary>
        private readonly T _Obj;

        /// <summary>МЕтод освобождения ресурсов</summary>
        private readonly Action<T> _Disposer;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Используемый объект</summary>
        public T Object => _Obj;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Упаковка объекта в оболочку с указанием метода освобождения ресурсов, занимаемых указанным объектом</summary>
        /// <param name="obj">Уничтожаемый объект</param>
        /// <param name="Disposer">Метод освобождения ресурсов</param>
        [DST]
        public UsingObject([NotNull] T obj, [NotNull] Action<T> Disposer)
        {
            if(obj is null) throw new ArgumentNullException(nameof(obj));
            _Obj = obj;
            _Disposer = Disposer ?? throw new ArgumentNullException(nameof(Disposer));
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Разрушение обёртки, влекущее разрушение исопльзуемого объекта</summary>
        [DST]
        public void Dispose() => _Disposer(_Obj);

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Оператор неявного приведения типов</summary>
        /// <param name="obj">ОБъект-оболочка</param>
        /// <returns>Внутренний объект</returns>
        [DST]
        public static implicit operator T([NotNull] UsingObject<T> obj) => obj._Obj;

        /* ------------------------------------------------------------------------------------------ */
    }
}