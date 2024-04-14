#nullable enable
namespace MathCore.Optimization.ParticleSwarm;

/// <summary>Рой двумерных частиц</summary>
public class Swarm2D(int ParticleCount = 100)
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

    /// <summary>Частица</summary>
    private class Particle2D
    {
        private double _BestValue;
        public double BestX;
        public double BestY;
        public double Value;
        public double X;
        public double Y;

        public Particle2D(double X, double Y, double Value)
        {
            this.X     = X;
            this.Y     = Y;
            this.Value = Value;
            SetBest();
        }

        private void SetBest()
        {
            _BestValue = Value;
            BestX      = X;
            BestY      = Y;
        }

        public void SetMin(Func<double, double, double> F)
        {
            Value = F(X, Y);
            if(Value < _BestValue)
                SetBest();
        }

        public void SetMax(Func<double, double, double> F)
        {
            Value = F(X, Y);
            if(Value > _BestValue)
                SetBest();
        }

        public void Deconstruct(out double X, out double Y, out double Value) => (X, Y, Value) = (this.X, this.Y, this.Value);
    }

    private static readonly Random __Random = new();

    /// <summary>Размер роя</summary>
    private readonly int _ParticleCount = ParticleCount;

    private double Get(in Interval Interval, double X, double CurrentBestX, double GlobalBestX)
    {
        var dx_local = _LocalWeight * __Random.NextDouble() * (CurrentBestX - X);
        var dx_global = _GlobalWeight * __Random.NextDouble() * (GlobalBestX - X);

        var dx = dx_local + dx_global;

        var new_position = X * (1 + _Inertia) + dx;
        return Interval.Normalize(new_position);
    }

    public void Minimize(
        in Func<double, double, double> F,
        in double MinX,
        in double MaxX,
        in double MinY,
        in double MaxY,
        in int IterationCount,
        out double X,
        out double Y,
        out double Value) =>
        Minimize(F, new(MinX, MaxX), new(MinY, MaxY), IterationCount, out X, out Y, out Value);

    public void Minimize(
        in Func<double, double, double> F,
        in Interval IntervalX,
        in Interval IntervalY,
        in int IterationCount,
        out double X,
        out double Y,
        out double Value)
    {
        var delta_x = IntervalX.Length;
        var delta_y = IntervalY.Length;
        var min_x   = IntervalX.Min;
        var min_y   = IntervalY.Min;

        var swarm = new Particle2D[_ParticleCount];
        for (var i = 0; i < _ParticleCount; i++)
        {
            var x = __Random.NextDouble() * delta_x + min_x;
            var y = __Random.NextDouble() * delta_y + min_y;
            swarm[i] = new(x, y, F(x, y));
        }

        (X, Y, Value) = swarm.GetMin(p => p.Value);

        for (var i = 0; i < IterationCount; i++)
            foreach (var p in swarm)
            {
                p.X = Get(IntervalX, p.X, p.BestX, X);
                p.Y = Get(IntervalY, p.Y, p.BestY, Y);
                p.SetMin(F);

                if (p.Value >= Value) continue;

                (X, Y, Value) = p;
            }
    }

    public void Maximize(
        in Func<double, double, double> F,
        in double MinX,
        in double MaxX,
        in double MinY,
        in double MaxY,
        in int IterationCount,
        out double X,
        out double Y,
        out double Value) =>
        Maximize(
            F: F, 
            IntervalX: new(Min: MinX, Max: MaxX),
            IntervalY: new(Min: MinY, Max: MaxY), 
            IterationCount: IterationCount,
            X: out X,
            Y: out Y, 
            Value: out Value);

    public void Maximize(
        in Func<double, double, double> F,
        in Interval IntervalX,
        in Interval IntervalY,
        in int IterationCount,
        out double X,
        out double Y,
        out double Value)
    {
        var delta_x = IntervalX.Length;
        var delta_y = IntervalY.Length;
        var min_x   = IntervalX.Min;
        var min_y   = IntervalY.Min;

        var swarm = new Particle2D[_ParticleCount];
        for (var i = 0; i < _ParticleCount; i++)
        {
            var x = __Random.NextDouble() * delta_x + min_x;
            var y = __Random.NextDouble() * delta_y + min_y;
            swarm[i] = new(x, y, F(x, y));
        }

        (X, Y, Value) = swarm.GetMin(p => p.Value);

        for (var i = 0; i < IterationCount; i++)
            foreach (var p in swarm)
            {
                p.X = Get(IntervalX, p.X, p.BestX, X);
                p.Y = Get(IntervalY, p.Y, p.BestY, Y);
                p.SetMax(F);

                if (p.Value <= Value) continue;

                (X, Y, Value) = p;
            }
    }
}