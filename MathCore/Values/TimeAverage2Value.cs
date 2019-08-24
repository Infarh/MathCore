using System;

namespace MathCore.Values
{
    [Serializable]
    public class TimeAverage2Value : TimeAverageValue
    {
        /* ------------------------------------------------------------------------------------------ */

        protected double _LastdVf;
        protected double _LastV1;
        //private readonly object _Locker = new object();

        /* ------------------------------------------------------------------------------------------ */

        //public TimeAverage2Value() : base(1) { }

        public TimeAverage2Value(double tau) : base(tau) { }

        public TimeAverage2Value(double tau, double Value) : base(tau, Value) { }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Добавить значение</summary><param name="value">Значение</param>
        public override double Add(double value)
        {
            //lock(_Locker)
            //{
            var tau = Tau;
            var lv_CurrentTime = DateTime.Now;
            if(!Initialized || Math.Abs(tau) < double.Epsilon)
            {
                Value = value;
                _LastV1 = 0;
                _LastdVf = 0;
                _LastTime = lv_CurrentTime;
                Initialized = true;
                return value;
            }

            var dt = (lv_CurrentTime - _LastTime).TotalSeconds;
            if(Math.Abs(dt) < double.Epsilon) return Value;

            var lv_LastV1 = _LastV1;
            var lv_LastdVf = _LastdVf;

            var dV = (value - lv_LastV1) / tau;
            var dVdt = lv_LastV1 + dV * dt;
            var d2V = (dV - lv_LastdVf) / tau;
            var d2Vdt = lv_LastdVf + d2V * dt;
            var result = dVdt + (d2Vdt * tau);

            _LastTime = lv_CurrentTime;
            _LastV1 = dVdt;
            Value = result;
            _LastdVf = d2Vdt;

            return result;
            //}
        }

        /* ------------------------------------------------------------------------------------------ */
    }

    public class TimeAverage3Value : TimeAverage2Value
    {
        private Func<double, double> _AverageFunc;

        public TimeAverage3Value(double tau) : base(tau) { _AverageFunc = Average; }

        public TimeAverage3Value(double tau, double Value) : base(tau, Value) { _AverageFunc = Average; }

        public override double Add(double value) => _AverageFunc(value);

        private double Average(double value)
        {
            var v2 = base.Add(value);
            var k = OverrideTime;
            if(!(k >= 1)) return v2*k + value*(1 - k);
            _AverageFunc = base.Add;
            return v2;
        }

        public override void Reset()
        {
            base.Reset();
            _AverageFunc = Average;
        }
    }
}