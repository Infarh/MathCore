using System;

namespace MathCore.Values
{
    [Serializable]
    public class DifferentialWithAveraging : IResetable, IValue<double>
    {
        protected double _LastValue;
        protected TimeSpan _LastTime;

        public double Tau { get; set; }
        public double Value { get; set; }
        public bool Initialized { get; protected set; }

        public DifferentialWithAveraging() { }
        public DifferentialWithAveraging(double Tau) => this.Tau = Tau;

        //public DifferentialWithAveraging(double Tau, double Value) : this(Tau) { Add(Value); }

        public void Reset() => Initialized = false;

        public virtual double Add(double value)
        {
            var t = DateTime.Now.TimeOfDay;

            if(!Initialized)
            {
                _LastValue = value;
                _LastTime = t;
                Initialized = true;
                return value;
            }

            var dv = value - _LastValue;
            var lv_DeltaTime = t - _LastTime;
            var dt = lv_DeltaTime.TotalSeconds;

            if(dt == 0) return Value;
            _LastValue = Tau == 0 ? value : _LastValue + (dv *= dt / Tau);

            _LastTime = t;
            return Value = dv / dt;
        }

        public static implicit operator double(DifferentialWithAveraging D) => D.Value;
    }

    [Serializable]
    public class DifferentialWithAveraging2 : DifferentialWithAveraging
    {
        protected double _LastV1;
        //protected double _LastVf;

        public override double Add(double value)
        {
            var t = DateTime.Now.TimeOfDay;
            if(!Initialized)
            {
                _LastValue = value;
                _LastV1 = 0;
                //_LastVf = 0.0;
                Value = 0;
                _LastTime = t;
                Initialized = true;
                return value;
            }

            var dt = (t - _LastTime).TotalSeconds;
            if(dt == 0) return Value;

            var dv = (value - _LastV1) / Tau;
            var dvdt = _LastV1 + (dv * dt);
            var lv_IntV = Value + (((dv - Value) / Tau) * dt);
            //var result = dvdt + (Tau * lv_IntV);

            _LastTime = t;
            _LastV1 = dvdt;
            _LastValue = value;
            //_LastVf = result;
            Value = lv_IntV;

            return lv_IntV;
        }
    }


}