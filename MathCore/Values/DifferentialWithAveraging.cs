#nullable enable
namespace MathCore.Values;

[Serializable]
public class DifferentialWithAveraging : IResettable, IValue<double>
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
            _LastValue  = value;
            _LastTime   = t;
            Initialized = true;
            return value;
        }

        var dv         = value - _LastValue;
        var delta_time = t - _LastTime;
        var dt         = delta_time.TotalSeconds;

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
            _LastV1    = 0;
            //_LastVf = 0.0;
            Value       = 0;
            _LastTime   = t;
            Initialized = true;
            return value;
        }

        var dt = (t - _LastTime).TotalSeconds;
        if(dt == 0) return Value;

        var dv      = (value - _LastV1) / Tau;
        var dv_dt   = _LastV1 + (dv * dt);
        var int_v = Value + (((dv - Value) / Tau) * dt);
        //var result = dv_dt + (Tau * int_v);

        _LastTime = t;
        _LastV1    = dv_dt;
        _LastValue = value;
        //_LastVf = result;
        Value = int_v;

        return int_v;
    }
}