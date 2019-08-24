using System;
// ReSharper disable UnusedMember.Global

namespace MathCore.Statistic.RandomNumbers
{
    public class TriangularRandomGenerator : RandomGenerator
    {
        private double _a;
        private double _b;

        public double a { get { return _a; } set { if(Math.Abs(value - _a) > double.Epsilon) SetAB(value, b); } }
        public double b { get { return _b; } set { if(Math.Abs(value - _b) > double.Epsilon) SetAB(a, value); } }

        public TriangularRandomGenerator(double a, double b)
            : base(mu: (a + b) / 2, sigma: (b - a) / Math.Sqrt(24))
        {
            _a = a;
            _b = b;
        }

        public void SetAB(double a, double b)
        {
            _a = a;
            _b = b;
            sigma = (b - a) / Math.Sqrt(24);
            mu = (a + b) / 2;
        }

        public override double Distribution(double x) => Distributions.Triangular(x, _a, _b);

        protected override double GetNextValue()
        {
            //var rnd = SystemRandomGenerator.Value;
            throw new NotImplementedException();
            //return rnd.NextDouble() + rnd.NextDouble();
        }
    }
}