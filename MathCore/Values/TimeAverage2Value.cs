#nullable enable
using System;

namespace MathCore.Values;

[Serializable]
public class TimeAverage2Value : TimeAverageValue
{
    /* ------------------------------------------------------------------------------------------ */

    // ReSharper disable once IdentifierTypo
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
        var tau          = Tau;
        var current_time = DateTime.Now;
        if(!Initialized || Math.Abs(tau) < double.Epsilon)
        {
            Value       = value;
            _LastV1     = 0;
            _LastdVf    = 0;
            _LastTime   = current_time;
            Initialized = true;
            return value;
        }

        var dt = (current_time - _LastTime).TotalSeconds;
        if(Math.Abs(dt) < double.Epsilon) return Value;

        var last_V1  = _LastV1;
        var last_dVf = _LastdVf;

        var dV     = (value - last_V1) / tau;
        var dVdt   = last_V1 + dV * dt;
        var d2V    = (dV - last_dVf) / tau;
        var d2Vdt  = last_dVf + d2V * dt;
        var result = dVdt + (d2Vdt * tau);

        _LastTime = current_time;
        _LastV1   = dVdt;
        Value     = result;
        _LastdVf  = d2Vdt;

        return result;
        //}
    }

    /* ------------------------------------------------------------------------------------------ */
}

public class TimeAverage3Value : TimeAverage2Value
{
    private Func<double, double> _AverageFunc;

    public TimeAverage3Value(double tau) : base(tau) => _AverageFunc = Average;

    public TimeAverage3Value(double tau, double Value) : base(tau, Value) => _AverageFunc = Average;

    public override double Add(double value) => _AverageFunc(value);

    private double Average(double value)
    {
        var v2 = base.Add(value);
        var k  = OverrideTime;
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