using System;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
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
            [DST]
            get
            {
                Contract.Ensures(Contract.Result<T>() is { });
                return _Obj;
            }
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Упаковка объекта в оболочку с указанием метода освобождения ресурсов, занимаемых указанным объектом</summary>
        /// <param name="obj">Уничтожаемый объект</param>
        /// <param name="Disposer">Метод освобождения ресурсов</param>
        [DST]
        public UsingObject(T obj, Action<T> Disposer)
        {
            Contract.Requires(obj is { });
            Contract.Requires(Disposer is { });
            Contract.Ensures(_Obj is { });
            Contract.Ensures(_Disposer is { });
            if(obj is null) throw new ArgumentNullException(nameof(obj));
            if(Disposer is null) throw new ArgumentNullException(nameof(Disposer));

            _Obj = obj;
            _Disposer = Disposer;
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
        public static implicit operator T(UsingObject<T> obj) => obj._Obj;

        /* ------------------------------------------------------------------------------------------ */
    }
}