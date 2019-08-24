using System;
using System.Diagnostics.Contracts;

namespace MathCore.Values
{
    ///<summary>"Ленивое" значение</summary>
    ///<typeparam name="T">Тип значения</typeparam>
    public class LazyValue<T> : IInitializable, IInitializable<Func<T>>, IValueRead<T>, IResetable
    {
        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Инициализатор значения</summary>
        private Func<T> _Initializator;

        /// <summary>Значение</summary>
        private object _Value;

        /// <summary>Объект кросспоточной блокировки</summary>
        private readonly object _LockObject = new object();

        /// <summary>Флаг инициализации</summary>
        private bool _Initialized;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Признак инициализации</summary>
        public bool Initialized
        {
            get
            {
                Contract.Ensures(Contract.Result<bool>() && _Value != null);
                bool result;
                lock (_LockObject) result = _Initialized;
                return result;
            }
        }

        ///<summary>Значение</summary>
        public T Value
        {
            get
            {
                T result;
                lock (_LockObject)
                    if(_Initialized) result = (T)_Value;
                    else
                    {
                        result = (T)(_Value = _Initializator());
                        _Initialized = true;
                    }
                return result;
            }
        }

        /* ------------------------------------------------------------------------------------------ */

        ///<summary>Создание нового "ленивого" значения</summary>
        ///<param name="Initializator">Инициализатор значения</param>
        public LazyValue(Func<T> Initializator)
        {
            Contract.Requires(Initializator != null);
            Initialize(Initializator);
        }

        ///<summary>Инициализация "ленивого" значения</summary>
        ///<param name="Initializator">Инициализатор</param>
        public void Initialize(Func<T> Initializator)
        {
            Contract.Requires(Initializator != null);
            Contract.Ensures(_Initializator != null);
            lock (_LockObject)
            {
                _Initializator = Initializator;
                Reset();
            }
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Невный вызов метода инициализации для интерфейса IInitializable</summary>
        void IInitializable.Initialize() => Reset();

        ///<summary>Сброс состояния</summary>
        public void Reset() => _Initialized = false;

        /* ------------------------------------------------------------------------------------------ */

        ///<summary>Оператор неявного преобразования "ленивого" значения в обычное</summary>
        ///<param name="value">"Ленивое" значение</param>
        ///<returns>Обычное значение, получаемое при вычислении "ленивого" значения</returns>
        public static implicit operator T(LazyValue<T> value) => value.Value;

        ///<summary>Оператор неявного преобразования метода инициализации в "ленивое значение"</summary>
        ///<param name="Initializator">Метод инициализации "ленивого" значения</param>
        ///<returns>"Ленивое" значение с указанным методом инициализации</returns>
        public static implicit operator LazyValue<T>(Func<T> Initializator) => new LazyValue<T>(Initializator);

        /* ------------------------------------------------------------------------------------------ */

        [ContractInvariantMethod]
        private void InvariantMethod()
        {
            Contract.Invariant(_Initializator != null);
            Contract.Invariant(!_Initialized || _Value != null);
        }

        /* ------------------------------------------------------------------------------------------ */
    }

    public class TimeBufferedValue<TValue> : IFactory<TValue>
    {
        private readonly LazyValue<TValue> _Value;
        private DateTime _LastAccessTime = DateTime.MinValue;

        public TimeBufferedValue(Func<TValue> Generator, TimeSpan Timeout)
        {
            _Value = new LazyValue<TValue>(() =>
            {
                _LastAccessTime = DateTime.Now;
                return Generator();
            });
        }

        public TValue Create()
        {
            throw new NotImplementedException();
        }
    }
}
