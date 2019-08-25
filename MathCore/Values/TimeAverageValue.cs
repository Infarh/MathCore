using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MathCore.Values
{
    /// <summary>Усредняемая по времени величина</summary>
    [Serializable]
    public class TimeAverageValue : IValue<double>, IResetable
    {
        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Последнее время доступа</summary>
        protected DateTime _LastTime;

        /// <summary>Постоянная времени усреднения</summary>
        private double _Tau;

        protected DateTime _StartTime;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Постоянная времени</summary>
        public double Tau
        {
            get => _Tau;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                Contract.Requires(value >= 0);
                Contract.Ensures(_Tau >= 0);
                _Tau = value;
            }
        }

        /// <summary>Значение</summary>
        public double Value { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        /// <summary>Признак инициализации</summary>
        public bool Initialized { get; protected set; }

        public DateTime StartTime => _StartTime;

        public TimeSpan ElapsedTime => DateTime.Now - _StartTime;

        public double OverrideTime => ElapsedTime.TotalSeconds / Tau;

        /* ------------------------------------------------------------------------------------------ */

        ///// <summary>Усредняемая по времени величина</summary>
        //public TimeAverageValue() { Initialized = false; }

        /// <summary>Усредняемая по времени величина</summary>
        /// <param name="tau">Постоянная времени усреднения >= 0</param>
        public TimeAverageValue(double tau)
        {
            Contract.Requires(tau > 0, "Tau >= 0");
            Contract.Ensures(Tau >= 0);
            Initialized = false;
            _StartTime = _LastTime = DateTime.Now;
            Tau = tau;
        }

        /// <summary>Усредняемая по времени величина</summary>
        /// <param name="Value">Начальное значение</param><param name="tau">постоянная времени >= 0</param>
        public TimeAverageValue(double Value, double tau)
            : this(tau)
        {
            this.Value = Value;
            _StartTime = _LastTime = DateTime.Now;
            Initialized = true;
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Добавить значение</summary>
        /// <param name="value">Значение</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual double Add(double value)
        {
            var lv_CurrentTime = DateTime.Now;
            if(!Initialized || Math.Abs(_Tau) < double.Epsilon)
            {
                Initialized = true;
                Value = value;
                _StartTime = _LastTime = lv_CurrentTime;
                return value;
            }

            var lv_Delta = value - Value;
            var dt = (lv_CurrentTime - _LastTime).TotalSeconds;
            _LastTime = lv_CurrentTime;
            if(Math.Abs(lv_Delta) < double.Epsilon) return Value;
            Value += lv_Delta * dt / _Tau;
            return Value;
        }

        /// <summary>Сброс состояния</summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Reset() => Initialized = false;

        /* ------------------------------------------------------------------------------------------ */
    }
}
