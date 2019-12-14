using System;

namespace MathCore.Functions.PSO
{
    public static class PSOFuncExtensions
    {
        public static void Minimize(this Func<double, double> f, double Xmin, double Xmax, int Intervals,
            out double X, out double Y)
        {
            var swarm = new Swarm1D();
            swarm.Minimize(f, Xmin, Xmax, Intervals, out X, out Y);
        }

        public static void Maximize(this Func<double, double> f, double Xmin, double Xmax, int Intervals,
            out double X, out double Y)
        {
            var swarm = new Swarm1D();
            swarm.Minimize(x => -f(x), Xmin, Xmax, Intervals, out X, out Y);
            Y = -Y;
        }

        public static void Minimize(this Func<double, double, double> f,
            double Xmin, double Xmax, double Ymin, double Ymax,
            int Intervals,
            out double X, out double Y, out double Z)
        {
            var swarm = new Swarm2D();
            swarm.Minimize(f, Xmin, Xmax, Ymin, Ymax, Intervals, out X, out Y, out Z);
        }

        public static void Maximize(this Func<double, double, double> f,
            double Xmin, double Xmax, double Ymin, double Ymax,
            int Intervals,
            out double X, out double Y, out double Z)
        {
            var swarm = new Swarm2D();
            swarm.Minimize((x, y) => -f(x, y), Xmin, Xmax, Ymin, Ymax, Intervals, out X, out Y, out Z);
        }
    }

    //static class ParticleSwarmTest
    //{
    //    private static double f0(double x0, double x1) { return 3 + x0 * x0 + x1 * x1; }

    //    public static void Test()
    //    {
    //        Func<double, double> f = x => x * x;
    //        Func<double, double> q = x => f(x - 3) + 2;
    //        var swarm = new Swarm1D();
    //        double X;
    //        double Y;
    //        swarm.Minimize(q, -100, 100, 1000, out X, out Y);
    //        Console.WriteLine("X = {0}; Y = {1}", X, Y);
    //        Console.ReadLine();
    //    }
    //}


    //public class Swarm
    //{
    //    const double w = 0.729; // inertia weight. see http://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=00870279
    //    const double c1 = 1.49445; // cognitive/local weight
    //    const double c2 = 1.49445; // social/global weight

    //    class Particle
    //    {
    //        public double BestValue;
    //        public double[] BestX;
    //        public double Value;
    //        public double[] X;

    //        public Particle(Func<double[], double> Function, Interval[] IntervalX)
    //            : this(Function, IntervalX.Select(i => i.RandomValue).ToArray()) { }
    //        private Particle(Func<double[], double> Function, double[] X)
    //            : this(X, Function(X)) { }
    //        public Particle(double[] X, double Value) : this(X, Value, X, Value) { }
    //        public Particle(double[] X, double Value, double[] BestX, double BestValue)
    //        {
    //            this.X = X;
    //            this.Value = Value;
    //            this.BestX = BestX;
    //            this.BestValue = BestValue;
    //        }
    //    }

    //    private static readonly Random __Random = new Random();

    //    private readonly int _ParticleCount;

    //    public Swarm(int ParticleCount = 100) { _ParticleCount = ParticleCount; }

    //    public void Minimize(Func<double[], double> func, double[] minX, double[] maxX, int IterationCount,
    //        out double[] X, out double Value)
    //    {
    //        Minimize(func, minX.Zip(maxX, (min, max) => new Interval(min, max)).ToArray(),
    //            IterationCount, out X, out Value);
    //    }

    //    public void Minimize(Func<double[], double> func, Interval[] IntervalX, int IterationCount,
    //        out double[] X, out double Value)
    //    {
    //        var IntervalVx = new Interval[IntervalX.Length].Initialize(i => IntervalX[i].Clone());

    //        var swarm = new Particle[_ParticleCount].Initialize(i => new Particle(func, IntervalX));
    //        var start = swarm.GetMin(p => p.Value);
    //        X = start.X;
    //        Value = start.Value;

    //        for(var iteration = 0; iteration < IterationCount; iteration++)
    //            foreach(var p in swarm)
    //            {
    //                var r1 = __Random.NextDouble();
    //                var r2 = __Random.NextDouble();

    //                var newVx = p.X.Zip(p.BestX, (x, BestX) => new { x, BestX })
    //                    .Zip(X, (v, GlobalBestX) => new { v.x, v.BestX, GlobalBestX })
    //                    .Zip(IntervalVx, (v, I) => new { v.x, v.BestX, v.GlobalBestX, I })
    //                    .Select(v => new { value = (w * v.x) + (c1 * r1 * (v.BestX - v.x)) + (c2 * r2 * (v.GlobalBestX - v.x)), v.I })
    //                    .Select(v => v.I.Normalize(v.value))
    //                    .ToArray();

    //                var newX = p.X.Zip(IntervalX, (x, I) => new { x, I })
    //                            .Zip(newVx, (v, vx) => v.I.Normalize(v.x + vx)).ToArray();
    //                p.X = newX;
    //                p.Value = func(newX);
    //                if(p.Value < p.BestValue)
    //                {
    //                    p.BestX = newX;
    //                    p.BestValue = p.Value;
    //                }

    //                if(!(p.Value < Value)) continue;
    //                X = newX;
    //                Value = p.Value;
    //            }
    //    }

    //    public void Maximize(Func<double[], double> func, double[] minX, double[] maxX, int IterationCount,
    //        out double[] X, out double Value)
    //    {
    //        Maximize(func, minX.Zip(maxX, (min, max) => new Interval(min, max)).ToArray(),
    //            IterationCount, out X, out Value);
    //    }

    //    public void Maximize(Func<double[], double> func, Interval[] IntervalX, int IterationCount,
    //        out double[] X, out double Value)
    //    {
    //        var IntervalVx = new Interval[IntervalX.Length].Initialize(i => IntervalX[i].Clone());

    //        var swarm = new Particle[_ParticleCount].Initialize(i => new Particle(func, IntervalX));
    //        var start = swarm.GetMax(p => p.Value);
    //        X = start.X;
    //        Value = start.Value;

    //        for(var iteration = 0; iteration < IterationCount; iteration++)
    //            foreach(var p in swarm)
    //            {
    //                var r1 = __Random.NextDouble();
    //                var r2 = __Random.NextDouble();

    //                var newVx = p.X.Zip(p.BestX, (x, BestX) => new { x, BestX })
    //                    .Zip(X, (v, GlobalBestX) => new { v.x, v.BestX, GlobalBestX })
    //                    .Zip(IntervalVx, (v, I) => new { v.x, v.BestX, v.GlobalBestX, I })
    //                    .Select(v => new { value = (w * v.x) + (c1 * r1 * (v.BestX - v.x)) + (c2 * r2 * (v.GlobalBestX - v.x)), v.I })
    //                    .Select(v => v.I.Normalize(v.value))
    //                    .ToArray();

    //                var newX = p.X.Zip(IntervalX, (x, I) => new { x, I })
    //                            .Zip(newVx, (v, vx) => v.I.Normalize(v.x + vx)).ToArray();
    //                p.X = newX;
    //                p.Value = func(newX);
    //                if(p.Value > p.BestValue)
    //                {
    //                    p.BestX = newX;
    //                    p.BestValue = p.Value;
    //                }
    //                if(!(p.Value > Value)) continue;
    //                X = newX;
    //                Value = p.Value;
    //            }
    //    }
    //}
}
