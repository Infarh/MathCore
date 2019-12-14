using System;
using System.Linq;

namespace MathCore.Functions.PSO
{
    /// <summary>Рой двумерных частиц</summary>
    public class Swarm2D
    {
        /// <summary>Вес инерции</summary>
        [Hyperlink("http://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=00870279")]
        const double w = 0.729;

        /// <summary>Коэффициент локального веса</summary>
        const double c1 = 1.49445; // cognitive/local weight
        /// <summary>Коэффициент глобального веса</summary>
        const double c2 = 1.49445; // social/global weight

        /// <summary>Частица</summary>
        class Particle2D
        {
            public double BestValue;
            public double BestX;
            public double BestY;
            public double Value;
            public double X;
            public double Y;

            public Particle2D(Func<double, double, double> Function, Interval IntervalX, Interval IntervalY)
                : this(Function, IntervalX.RandomValue, IntervalY.RandomValue) { }
            private Particle2D(Func<double, double, double> Function, double X, double Y)
                : this(X, Y, Function(X, Y)) { }
            public Particle2D(double X, double Y, double Value) : this(X, Y, Value, X, Y, Value) { }
            public Particle2D(double X, double Y, double Value, double BestX, double BestY, double BestValue)
            {
                this.X = X;
                this.Y = Y;
                this.Value = Value;
                this.BestX = BestX;
                this.BestY = BestY;
                this.BestValue = BestValue;
            }
        }

        private static readonly Random __Random = new Random();

        /// <summary>Размер роя</summary>
        private readonly int _ParticleCount;

        public Swarm2D(int ParticleCount = 100) { _ParticleCount = ParticleCount; }

        public void Minimize(Func<double, double, double> func,
                    double minX, double maxX, double minY, double maxY,
                    int IterationCount,
                    out double X, out double Y, out double Value)
        {
            Minimize(func, new Interval(minX, maxX), new Interval(minY, maxY), IterationCount,
                        out X, out Y, out Value);
        }

        public void Minimize(Func<double, double, double> func, Interval IntervalX, Interval IntervalY,
                    int IterationCount,
                    out double X, out double Y, out double Value)
        {
            var IntervalVx = IntervalX;
            var IntervalVy = IntervalY;

            var swarm = new Particle2D[_ParticleCount].Initialize(IntervalX, IntervalY, (i, x, y) => new Particle2D(func, x, y));
            var start = swarm.GetMin(p => p.Value);
            X = start.X;
            Y = start.Y;
            Value = start.Value;

            for (var iteration = 0; iteration < IterationCount; iteration++)
                foreach (var p in swarm)
                {
                    var r1 = __Random.NextDouble();
                    var r2 = __Random.NextDouble();

                    var newVx = (w * p.X) + (c1 * r1 * (p.BestX - p.X)) + (c2 * r2 * (X - p.X));
                    IntervalVx.Normalize(ref newVx);

                    r1 = __Random.NextDouble();
                    r2 = __Random.NextDouble();

                    var newVy = (w * p.Y) + (c1 * r1 * (p.BestY - p.Y)) + (c2 * r2 * (Y - p.Y));
                    IntervalVy.Normalize(ref newVy);

                    var newX = IntervalX.Normalize(p.X + newVx);
                    var newY = IntervalX.Normalize(p.Y + newVy);
                    p.X = newX;
                    p.Y = newY;
                    p.Value = func(newX, newY);
                    if (p.Value < p.BestValue)
                    {
                        p.BestX = newX;
                        p.BestY = newY;
                        p.BestValue = p.Value;
                    }
                    if (!(p.Value < Value)) continue;
                    X = newX;
                    Y = newY;
                    Value = p.Value;
                }
        }

        public void Maximize(Func<double, double, double> func,
                    double minX, double maxX, double minY, double maxY,
                    int IterationCount,
                    out double X, out double Y, out double Value)
        {
            Maximize(func, new Interval(minX, maxX), new Interval(minY, maxY), IterationCount,
                        out X, out Y, out Value);
        }

        public void Maximize(Func<double, double, double> func, Interval IntervalX, Interval IntervalY,
                    int IterationCount,
                    out double X, out double Y, out double Value)
        {
            var IntervalVx = IntervalX;
            var IntervalVy = IntervalY;

            var swarm = new Particle2D[_ParticleCount].Initialize(IntervalX, IntervalY, (i, ix, iy) => new Particle2D(func, ix, iy));
            var start = swarm.GetMax(p => p.Value);
            X = start.X;
            Y = start.Y;
            Value = start.Value;

            for (var iteration = 0; iteration < IterationCount; iteration++)
                foreach (var p in swarm)
                {
                    var r1 = __Random.NextDouble();
                    var r2 = __Random.NextDouble();

                    var newVx = (w * p.X) + (c1 * r1 * (p.BestX - p.X)) + (c2 * r2 * (X - p.X));
                    IntervalVx.Normalize(ref newVx);

                    r1 = __Random.NextDouble();
                    r2 = __Random.NextDouble();

                    var newVy = (w * p.Y) + (c1 * r1 * (p.BestY - p.Y)) + (c2 * r2 * (Y - p.Y));
                    IntervalVy.Normalize(ref newVy);

                    var newX = IntervalX.Normalize(p.X + newVx);
                    var newY = IntervalX.Normalize(p.Y + newVy);
                    p.X = newX;
                    p.Y = newY;
                    p.Value = func(newX, newY);
                    if (p.Value > p.BestValue)
                    {
                        p.BestX = newX;
                        p.BestY = newY;
                        p.BestValue = p.Value;
                    }
                    if (!(p.Value > Value)) continue;
                    X = newX;
                    Y = newY;
                    Value = p.Value;
                }
        }
    }
}
