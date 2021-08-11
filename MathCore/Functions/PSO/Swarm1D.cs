using System;
using System.Linq;

namespace MathCore.Functions.PSO
{
    public class Swarm1D
    {
        private const double w = 0.729; // inertia weight. see http://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=00870279
        private const double c1 = 1.49445; // cognitive/local weight
        private const double c2 = 1.49445; // social/global weight

        private sealed class Particle1D
        {
            public double BestValue;
            public double BestX;
            public double Value;
            public double X;

            public Particle1D(Func<double, double> Function, Interval IntervalX)
                : this(Function, IntervalX.RandomValue) { }
            private Particle1D(Func<double, double> Function, double X)
                : this(X, Function(X)) { }

            private Particle1D(double X, double Value) : this(X, Value, X, Value) { }

            private Particle1D(double X, double Value, double BestX, double BestValue)
            {
                this.X = X;
                this.Value = Value;
                this.BestX = BestX;
                this.BestValue = BestValue;
            }
        }

        private static readonly Random __Random = new();

        private readonly int _ParticleCount;

        public Swarm1D(int ParticleCount = 100) => _ParticleCount = ParticleCount;

        public void Minimize(
            Func<double, double> func, 
            double minX, 
            double maxX,
            int IterationCount,
            out double X, 
            out double Value) => 
            Minimize(func, new Interval(minX, maxX), IterationCount, out X, out Value);

        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public void Minimize(
            Func<double, double> func, 
            Interval IntervalX, 
            int IterationCount,
            out double X, 
            out double Value)
        {
            var IntervalVx = IntervalX;

            var swarm = new Particle1D[_ParticleCount].Initialize(func, IntervalVx, (i, f, vx) => new Particle1D(f, vx));
            var start = swarm.GetMin(p => p.Value);
            X = start.X;
            Value = start.Value;

            for (var iteration = 0; iteration < IterationCount; iteration++)
                foreach (var p in swarm)
                {
                    var r1 = __Random.NextDouble();
                    var r2 = __Random.NextDouble();

                    var newVx = w * p.X + c1 * r1 * (p.BestX - p.X) + c2 * r2 * (X - p.X);
                    IntervalVx.Normalize(ref newVx);

                    var newX = IntervalX.Normalize(p.X + newVx);
                    p.X = newX;
                    p.Value = func(newX);
                    if (p.Value < p.BestValue)
                    {
                        p.BestX = newX;
                        p.BestValue = p.Value;
                    }

                    if (!(p.Value < Value)) continue;
                    X = newX;
                    Value = p.Value;
                }
        }

        public void Maximize(Func<double, double> func, double minX, double maxX, int IterationCount,
                    out double X, out double Value) => Maximize(func, new Interval(minX, maxX), IterationCount, out X, out Value);

        public void Maximize(Func<double, double> func, Interval IntervalX, int IterationCount,
                    out double X, out double Value)
        {
            var IntervalVx = IntervalX;

            var swarm = new Particle1D[_ParticleCount].Initialize(func, IntervalVx, (i, f, vx) => new Particle1D(f, vx));
            var start = swarm.GetMax(p => p.Value);
            X = start.X;
            Value = start.Value;

            for (var iteration = 0; iteration < IterationCount; iteration++)
                foreach (var p in swarm)
                {
                    var r1 = __Random.NextDouble();
                    var r2 = __Random.NextDouble();

                    var newVx = w * p.X + c1 * r1 * (p.BestX - p.X) + c2 * r2 * (X - p.X);
                    IntervalVx.Normalize(ref newVx);

                    var newX = IntervalX.Normalize(p.X + newVx);
                    p.X = newX;
                    p.Value = func(newX);
                    if (p.Value > p.BestValue)
                    {
                        p.BestX = newX;
                        p.BestValue = p.Value;
                    }
                    if (!(p.Value > Value)) continue;
                    X = newX;
                    Value = p.Value;
                }
        }
    }
}
