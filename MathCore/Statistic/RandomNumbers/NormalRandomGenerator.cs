using System;

namespace MathCore.Statistic.RandomNumbers;

public class NormalRandomGenerator : RandomGenerator
{
    /* ------------------------------------------------------------------------------------------ */

    private double? _Value;

    /* ------------------------------------------------------------------------------------------ */

    public NormalRandomGenerator(Random rnd = null) : base(rnd) { }
    public NormalRandomGenerator(double sigma, Random rnd = null) : base(sigma, rnd) { }
    public NormalRandomGenerator(double sigma, double mu, Random rnd = null) : base(sigma, mu, rnd) { }

    /* ------------------------------------------------------------------------------------------ */

    //protected override double GetValue() { return GetValue(_sigma, _m); }
    //public  override double GetValue(double sigma2, double m) { return GetNextValue() * sigma2 + m; }

    //private double[] GetNextValue1()
    //{
    //    var a = .0;
    //    var b = .0;
    //    while(Math.Abs(a) < double.Epsilon) a = _RND.NextDouble();
    //    while(Math.Abs(b) < double.Epsilon) b = _RND.NextDouble();
    //    a = Math.Sqrt(-2 * Math.Log(a));
    //    b *= Consts.pi2;
    //    return new[] { (Math.Cos(b) * a), (Math.Sin(b) * a) };
    //}
    //private double[] GetNextValue2()
    //{
    //    var a = .0;
    //    var b = .0;
    //    var d = .0;
    //    while((Math.Abs(d) < double.Epsilon) || (d > 1))
    //    {
    //        a = _RND.NextDouble() * 2 - 1;
    //        b = _RND.NextDouble() * 2 - 1;
    //        d = a * a + b * b;
    //    }
    //    var result = Math.Sqrt((-2 * Math.Log(d)) / d);
    //    return new[] { (a * result), (b * result) };
    //}

    public override double Distribution(double x) => Distributions.NormalGauss(x, Sigma, Mu);

    protected override double GetNextValue()
    {
        if(_Value is { } value)
        {
            _Value = null;
            return value;
        }

        double x, y, r;
        do
        {
            x = _Random.NextDouble() * 2 - 1;
            y = _Random.NextDouble() * 2 - 1;
            r = x * x + y * y;
        }
        while (Math.Abs(r) < double.Epsilon || r > 1);

        var sqrt_log_r = Math.Sqrt(-2 * Math.Log(r) / r);
        _Value = y * sqrt_log_r;
        return x * sqrt_log_r;

        //_Value = y * Math.Sqrt(-2 * Math.Log(r) / r);
        //return x * Math.Sqrt(-2 * Math.Log(r) / r);
    }

    /* ------------------------------------------------------------------------------------------ */
}