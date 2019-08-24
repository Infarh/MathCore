using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

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
        public T Object
        {
            [DebuggerStepThrough]
            get
            {
                Contract.Ensures(!ReferenceEquals(Contract.Result<T>(), null));
                return _Obj;
            }
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Упаковка объекта в оболочку с указанием метода освобождения ресурсов, занимаемых указанным объектом</summary>
        /// <param name="obj">Уничтожаемый объект</param>
        /// <param name="Disposer">Метод освобождения ресурсов</param>
        [DebuggerStepThrough]
        public UsingObject(T obj, Action<T> Disposer)
        {
            Contract.Requires(!ReferenceEquals(obj, null));
            Contract.Requires(!ReferenceEquals(Disposer, null));
            Contract.Ensures(!ReferenceEquals(_Obj, null));
            Contract.Ensures(!ReferenceEquals(_Disposer, null));
            if(ReferenceEquals(obj, null)) throw new ArgumentNullException(nameof(obj));
            if(ReferenceEquals(Disposer, null)) throw new ArgumentNullException(nameof(Disposer));

            _Obj = obj;
            _Disposer = Disposer;
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Разрушение обёртки, влекущее разрушение исопльзуемого объекта</summary>
        [DebuggerStepThrough]
        public void Dispose() { _Disposer(_Obj); }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Оператор неявного приведения типов</summary>
        /// <param name="obj">ОБъект-оболочка</param>
        /// <returns>Внутренний объект</returns>
        [DebuggerStepThrough]
        public static implicit operator T(UsingObject<T> obj) { return obj._Obj; }

        /* ------------------------------------------------------------------------------------------ */
    }
}
