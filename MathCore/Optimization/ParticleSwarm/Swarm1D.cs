#nullable enable
namespace MathCore.Optimization.ParticleSwarm;

public class Swarm1D
{
    /// <summary>Вес инерции</summary>
    private double _Inertia;

    public double Inertia
    {
        get => _Inertia;
        set => _Inertia = value switch
        {
            < 0  => throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(Inertia)} величина должна быть > 0"),
            >= 1 => throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(Inertia)} величина должна быть < 1"),
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

    private class Particle1D
    {
        private double _BestValue;
        public double BestX;
        public double Value;
        public double X;

        public Particle1D(double X, double Value) 
        {
            this.X     = X;
            this.Value = Value;
            SetBest();
        }

        private void SetBest()
        {
            BestX      = X;
            _BestValue = Value;
        }

        public void SetMin(Func<double, double> F)
        {
            Value = F(X);
            if(Value < _BestValue)
                SetBest();
        }

        public void SetMax(Func<double, double> F)
        {
            Value = F(X);
            if(Value < _BestValue)
                SetBest();
        }
    }

    private static readonly Random __Random = new();

    private readonly int _ParticleCount;

    public Swarm1D(int ParticleCount = 100) => _ParticleCount = ParticleCount;

    private double Get(in Interval interval, double px, double best, double x) =>
        interval.Normalize(px + (_Inertia * px + _LocalWeight * __Random.NextDouble() * (best - px) + _GlobalWeight * __Random.NextDouble() * (x - px)));

    public void Minimize(
        in Func<double, double> F, 
        in double MinX, 
        in double MaxX,
        in int IterationCount, 
        out double X,
        out double Value) =>
        Minimize(F, new Interval(MinX, MaxX), IterationCount, out X, out Value);

    public void Minimize(
        in Func<double, double> F, 
        in Interval IntervalX, 
        int IterationCount,
        out double X,
        out double Value)
    {
        var delta_x = IntervalX.Length;
        var min_x   = IntervalX.Min;

        var swarm = new Particle1D[_ParticleCount];
        for (var i = 0; i < _ParticleCount; i++)
        {
            var x = __Random.NextDouble() * delta_x + min_x;
            swarm[i] = new Particle1D(x, F(x));
        }

        var start = swarm.GetMin(p => p.Value);
        X     = start.X;
        Value = start.Value;

        for (var i = 0; i < IterationCount; i++)
            foreach (var p in swarm)
            {
                p.X = Get(IntervalX, p.X, p.BestX, X);
                p.SetMin(F);

                if (p.Value < Value)
                {
                    X     = p.X;
                    Value = p.Value;
                }
            }
    }

    public void Maximize(
        in Func<double, double> F,
        in double MinX, 
        in double MaxX,
        in int IterationCount,
        out double X,
        out double Value) =>
        Maximize(F, new Interval(MinX, MaxX), IterationCount, out X, out Value);

    public void Maximize(
        in Func<double, double> F, 
        in Interval IntervalX,
        in int IterationCount,
        out double X,
        out double Value)
    {
        var delta_x = IntervalX.Length;
        var min_x   = IntervalX.Min;

        var swarm = new Particle1D[_ParticleCount];
        for (var i = 0; i < _ParticleCount; i++)
        {
            var x = __Random.NextDouble() * delta_x + min_x;
            swarm[i] = new Particle1D(x, F(x));
        }

        var start = swarm.GetMin(p => p.Value);
        X     = start.X;
        Value = start.Value;

        for (var i = 0; i < IterationCount; i++)
            foreach (var p in swarm)
            {
                    

                p.X = Get(IntervalX, p.X, p.BestX, X);
                p.SetMax(F);

                if (p.Value > Value)
                {
                    X     = p.X;
                    Value = p.Value;
                }
            }
    }
}