#nullable enable
namespace MathCore.Optimization.ParticleSwarm;

public class Swarm
{
    /// <summary>Вес инерции</summary>
    private double _Inertia;

    public double Inertia
    {
        get => _Inertia;
        set => _Inertia = value switch
        {
            < 0  => throw new ArgumentOutOfRangeException(nameof(value), value, nameof(Inertia) + " величина должна быть > 0"),
            >= 1 => throw new ArgumentOutOfRangeException(nameof(value), value, nameof(Inertia) + " величина должна быть < 1"),
            _    => value
        };
    }

    /// <summary>Коэффициент локального веса</summary>
    private double _LocalWeight = 1.49445; // cognitive/local weight

    /// <summary>Коэффициент стремления точки к своему собственному лучшему значению</summary>
    public double LocalWeight { get => _LocalWeight; set => _LocalWeight = value; }

    /// <summary>Коэффициент стремления всех точек к лучшему значению</summary>
    private double _GlobalWeight = 1.49445; // social/global weight

    /// <summary>Коэффициент стремления всех точек к лучшему значению</summary>
    public double GlobalWeight { get => _GlobalWeight; set => _GlobalWeight = value; }

    private class Particle(double[] X, double Value)
    {
        private double _BestValue = Value;
        public readonly double[] BestX = (double[])X.Clone();
        public double Value = Value;
        public readonly double[] X = X;

        private void SetBest()
        {
            _BestValue = Value;
            X.CopyTo(BestX, 0);
        }

        public void SetMin(Func<double[], double> F)
        {
            Value = F(X);
            if (Value < _BestValue) 
                SetBest();
        }

        public void SetMax(Func<double[], double> F)
        {
            Value = F(X);
            if (Value > _BestValue) 
                SetBest();
        }
    }

    private static readonly Random __Random = new();

    private readonly int _ParticleCount;

    public Swarm(int ParticleCount = 100) => _ParticleCount = ParticleCount;

    public void Minimize(
        in Func<double[], double> F,
        in double[] MinX,
        in double[] MaxX,
        int IterationCount,
        out double[] X,
        out double Value) =>
        Minimize(F, MinX.Zip(MaxX, (min, max) => new Interval(min, max)).ToArray(), IterationCount, out X, out Value);

    public void Minimize(
        in Func<double[], double> F,
        in Interval[] IntervalX,
        in int IterationCount,
        out double[] X,
        out double Value)
    {
        var dimensions = IntervalX.Length;

        var swarm = new Particle[_ParticleCount];
        for (var i = 0; i < _ParticleCount; i++)
        {
            var xx = __Random.NextUniform(IntervalX);
            swarm[i] = new Particle(xx, F(xx));
        }
        X = new double[dimensions];

        var start = swarm.GetMin(p => p.Value);
        start!.X.CopyTo(X, 0);
        Value = start.Value;

        for (var i = 0; i < IterationCount; i++)
            foreach (var p in swarm)
            {
                for (var j = 0; j < dimensions; j++)
                {
                    var x  = p.X[j];
                    var r1 = __Random.NextDouble();
                    var r2 = __Random.NextDouble();

                    x      += _Inertia * x + _LocalWeight * r1 * (p.BestX[j] - x) + _GlobalWeight * r2 * (X[j] - x);
                    p.X[j] =  IntervalX[j].Normalize(x);
                }
                p.SetMin(F);

                if (p.Value >= Value) continue;

                p.X.CopyTo(X, 0);
                Value = p.Value;
            }
    }

    public void Maximize(
        in Func<double[], double> F,
        in double[] MinX,
        in double[] MaxX,
        in int IterationCount,
        out double[] X,
        out double Value) =>
        Maximize(F, MinX.Zip(MaxX, (min, max) => new Interval(min, max)).ToArray(), IterationCount, out X, out Value);

    public void Maximize(
        in Func<double[], double> F,
        in Interval[] IntervalX,
        int IterationCount,
        out double[] X,
        out double Value)
    {
        var dimensions = IntervalX.Length;

        var swarm = new Particle[_ParticleCount];
        for (var i = 0; i < _ParticleCount; i++)
        {
            var xx = __Random.NextUniform(IntervalX);
            swarm[i] = new Particle(xx, F(xx));
        }
        X = new double[dimensions];

        var start = swarm.GetMin(p => p.Value);
        start!.X.CopyTo(X, 0);
        Value = start.Value;

        for (var i = 0; i < IterationCount; i++)
            foreach (var p in swarm)
            {
                var xx = p.X;
                for (var j = 0; j < dimensions; j++)
                {
                    var x  = xx[j];
                    var r1 = __Random.NextDouble();
                    var r2 = __Random.NextDouble();

                    x      += _Inertia * x + _LocalWeight * r1 * (p.BestX[j] - x) + _GlobalWeight * r2 * (X[j] - x);
                    p.X[j] =  IntervalX[j].Normalize(x);
                }
                p.SetMax(F);

                if (p.Value > Value)
                {
                    p.X.CopyTo(X, 0);
                    Value = p.Value;
                }
            }
    }
}