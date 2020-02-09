using System;
using MathCore.Annotations;

namespace MathCore.Values
{
    ///<summary>"Ленивое" значение</summary>
    ///<typeparam name="T">Тип значения</typeparam>
    public class LazyValue<T> : IInitializable, IInitializable<Func<T>>, IValueRead<T>, IResettable
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
        ///<param name="Initializer">Инициализатор значения</param>
        public LazyValue(Func<T> Initializer) => Initialize(Initializer);

        ///<summary>Инициализация "ленивого" значения</summary>
        ///<param name="Initializer">Инициализатор</param>
        public void Initialize(Func<T> Initializer)
        {
            lock (_LockObject)
            {
                _Initializator = Initializer;
                Reset();
            }
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Вызов метода инициализации для интерфейса <see cref="IInitializable"/></summary>
        void IInitializable.Initialize() => Reset();

        ///<summary>Сброс состояния</summary>
        public void Reset() => _Initialized = false;

        /* ------------------------------------------------------------------------------------------ */

        ///<summary>Оператор неявного преобразования "ленивого" значения в обычное</summary>
        ///<param name="value">"Ленивое" значение</param>
        ///<returns>Обычное значение, получаемое при вычислении "ленивого" значения</returns>
        public static implicit operator T([NotNull] LazyValue<T> value) => value.Value;

        ///<summary>Оператор неявного преобразования метода инициализации в "ленивое значение"</summary>
        ///<param name="Initializer">Метод инициализации "ленивого" значения</param>
        ///<returns>"Ленивое" значение с указанным методом инициализации</returns>
        [NotNull]
        public static implicit operator LazyValue<T>(Func<T> Initializer) => new LazyValue<T>(Initializer);

        /* ------------------------------------------------------------------------------------------ */
    }
}