using System;

namespace MathCore.Statistic.RandomNumbers
{
    public class NormalRandomGenerator : RandomGenerator
    {
        /* ------------------------------------------------------------------------------------------ */

        private readonly Random _RND = new Random(DateTime.Now.Millisecond);
        private double? _Value;

        /* ------------------------------------------------------------------------------------------ */

        public NormalRandomGenerator() { }
        public NormalRandomGenerator(double sigma) : base(sigma) { }
        public NormalRandomGenerator(double sigma, double mu) : base(sigma, mu) { }

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

        public override double Distribution(double x) => Distributions.NormalGaus(x, Sigma, Mu);

        protected override double GetNextValue()
        {
            if(_Value.HasValue)
            {
                var num = _Value.Value;
                _Value = null;
                return num;
            }
            var a = 0d;
            var b = 0d;
            var d = 0d;
            while(Math.Abs(d) < double.Epsilon || d > 1)
            {
                a = _RND.NextDouble() * 2 - 1;
                b = _RND.NextDouble() * 2 - 1;
                d = (a * a) + (b * b);
            }
            _Value = b * Math.Sqrt(-2 * Math.Log(d) / d);
            return a * Math.Sqrt(-2 * Math.Log(d) / d);
        }

        /* ------------------------------------------------------------------------------------------ */
    }
}