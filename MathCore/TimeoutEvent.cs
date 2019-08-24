using System;
using System.Threading;

namespace MathCore
{
    /// <summary>Класс задержки реакции на событие</summary>
    /// <typeparam name="TEventArgs">Тип аргумента события</typeparam>
    public class TimeoutEvent<TEventArgs> where TEventArgs : EventArgs
    {
        /* ------------------------------------------------------------------------------------------ */


        /// <summary>Метод подписки на событие</summary>
        /// <param name="Timeout">Таймаут</param>
        /// <param name="OnTimeout">Метод обработки первичного вызова события</param>
        /// <param name="OnInvoke">Метод обраобтки вторичного события</param>
        /// <returns>Обработчик исходного события</returns>
        public static EventHandler<TEventArgs> Subscribe(int Timeout, EventHandler<TEventArgs> OnTimeout, EventHandler<TEventArgs> OnInvoke = null)
        {
            var Event = new TimeoutEvent<TEventArgs>(Timeout);
            EventHandler<TEventArgs> handler = Event.Invoke;
            if(OnInvoke != null) Event.Invoked += (s, e) => OnInvoke(e?.EventSender, e?.E);
            Event.Timeouted += (s, e) => OnTimeout(e?.EventSender, e?.E);
            return handler;
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Константа бесконечного периода ожидания</summary>
        private const int c_Infinite = System.Threading.Timeout.Infinite;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Аргумент события</summary>
        public class Info : EventArgs
        {
            /// <summary>Источник сходного события</summary>
            public object EventSender { get; }
            /// <summary>Акргумент исходного события</summary>
            public TEventArgs E { get; }
            /// <summary>Аргумент события</summary>
            /// <param name="EventSender">Источник исходного события</param>
            /// <param name="e">Аргумент исходного события</param>
            [System.Diagnostics.DebuggerStepThrough]
            public Info(object EventSender, TEventArgs e)
            {
                this.EventSender = EventSender;
                E = e;
            }
        }

        /// <summary>Первычная генерация события</summary>
        public event EventHandler<Info> Invoked;

        /// <summary>Метод первичной генерации события <see cref="Invoked"/></summary>
        /// <param name="e">Аргумент первичного вызова события</param>
        //[System.Diagnostics.DebuggerStepThrough]
        protected virtual void OnInvoked(Info e)
        {
            _LastEventArgs = e;
            var handlers = Invoked;
            if(handlers != null)
                handlers.Invoke(this, e);
        }

        /// <summary>Событие, возникающее после последнего вызова метода <see cref="Invoke"/> через период времени <see cref="Timeout"/></summary>
        public event EventHandler<Info> Timeouted;

        /// <summary>Мeтод генерации события <see cref="Timeouted"/></summary>
        /// <param name="e">Аргумент события</param>
        //[System.Diagnostics.DebuggerStepThrough]
        protected virtual void OnTimeouted(Info e)
        {
            var handlers = Timeouted;
            if(handlers != null)
                handlers.Invoke(this, e);
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Таймер</summary>
        private readonly Timer _Timer;

        /// <summary>Период времени таймаута</summary>
        private int _Timeout;

        /// <summary>Время последнего вызова</summary>
        private DateTime _LastCallTime;

        /// <summary>Признак ожидания таймаута события</summary>
        private bool _InProcess;

        /// <summary>Аргумент последнего выхова метода <see cref="Invoke"/></summary>
        private Info _LastEventArgs;

        /// <summary>Признак ожидания отмены генерации события <see cref="Timeouted"/></summary>
        private bool _NeedAbort;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Задейржка во времени генерации события <see cref="Timeouted"/> в миллисекундах</summary>
        public int Timeout { get { return _Timeout; } set { _Timeout = value; } }

        /// <summary>Время последнего выхова метода <see cref="Invoke"/></summary>
        public DateTime LastCallTime => _LastCallTime;

        /// <summary>Признак ожидания генерации события <see cref="Invoked"/></summary>
        public bool InProcess => _InProcess;

        /// <summary>Признак отмены генерации собития <see cref="Timeouted"/></summary>
        public bool NeedAbort { get { return _NeedAbort; } set { _NeedAbort = value; } }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Инициализация нового объекта задержки генерации соытия</summary>
        /// <param name="Timeout">Временная задержка в миллисекундах</param>
        public TimeoutEvent(int Timeout)
        {
            _Timeout = Timeout;
            _Timer = new Timer(OnTimer, null, c_Infinite, c_Infinite);
        }

        /// <summary>Инициализация нового объекта задержки генерации соытия</summary>
        /// <param name="Timeout">Временная задержка в миллисекундах</param>
        /// <param name="OnTimeout">Метод вторичной обработки события</param>
        public TimeoutEvent(int Timeout, EventHandler<TEventArgs> OnTimeout) : this(Timeout) { Timeouted += (s, e) => OnTimeout(e.EventSender, e.E); }

        /// <summary>Инициализация нового объекта задержки генерации соытия</summary>
        /// <param name="Timeout">Временная задержка в миллисекундах</param>
        /// <param name="OnTimeout">Метод вторичной обработки события</param>
        /// <param name="OnInvoke">Метод первичной обработки события</param>
        public TimeoutEvent(int Timeout, EventHandler<TEventArgs> OnTimeout, EventHandler<TEventArgs> OnInvoke) : this(Timeout, OnTimeout) { Invoked += (s, e) => OnInvoke(e.EventSender, e.E); }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Метод оброботки события таймера</summary>
        /// <param name="State">Состояние таймера</param>
        private void OnTimer(object State)
        {
            bool timeouted;
            lock(_Timer)
            {
                if(!_InProcess) return;
                if(_NeedAbort)
                {
                    _InProcess = _NeedAbort = false;
                    return;
                }
                var dt = _Timeout - (DateTime.Now - _LastCallTime).TotalMilliseconds;
                // ReSharper disable once AssignmentInConditionalExpression
                if(timeouted = dt <= 0)
                    _InProcess = false;
                else
                    _Timer.Change((int)dt, c_Infinite);
            }
            if(timeouted) OnTimeouted(_LastEventArgs);
        }

        /// <summary>Метод генерации события</summary>
        /// <param name="EventSender">Источник события</param>
        /// <param name="args">Аргументы события</param>
        public void Invoke(object EventSender = null, TEventArgs args = null)
        {
            lock(_Timer)
            {
                _LastCallTime = DateTime.Now;
                _NeedAbort = false;
                if(!_InProcess)
                {
                    _InProcess = true;
                    _Timer.Change(_Timeout, c_Infinite);
                }
            }
            OnInvoked(new Info(EventSender, args));
        }

        /// <summary>Отмена реакции на событие</summary>
        public void Abort()
        {
            lock(_Timer)
            {
                if(!_InProcess) return;
                _NeedAbort = true;
            }
        }

        /* ------------------------------------------------------------------------------------------ */
    }
}
