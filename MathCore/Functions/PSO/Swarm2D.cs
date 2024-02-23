using MathCore.Annotations;

// ReSharper disable ArgumentsStyleOther
// ReSharper disable ArgumentsStyleNamedExpression

namespace MathCore.Functions.PSO;

/// <summary>Рой двумерных частиц</summary>
[Hyperlink("http://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=00870279")]
public class Swarm2D(int ParticleCount = 100)
{
    /// <summary>Вес инерции</summary>
    const double w = 0.729;

    /// <summary>Коэффициент локального веса</summary>
    const double c1 = 1.49445; // cognitive/local weight

    /// <summary>Коэффициент глобального веса</summary>
    const double c2 = 1.49445; // social/global weight

    /// <summary>Частица</summary>
    private class Particle2D
    {
        public double BestValue;
        public double BestX;
        public double BestY;
        public double Value;
        public double X;
        public double Y;

        public Particle2D(Func<double, double, double> Function, Interval IntervalX, Interval IntervalY)
            : this(Function, IntervalX.RandomValue, IntervalY.RandomValue) { }
        private Particle2D([NotNull] Func<double, double, double> Function, double X, double Y)
            : this(X, Y, Function(X, Y)) { }

        private Particle2D(double X, double Y, double Value) : this(X, Y, Value, X, Y, Value) { }

        private Particle2D(double X, double Y, double Value, double BestX, double BestY, double BestValue)
        {
            this.X         = X;
            this.Y         = Y;
            this.Value     = Value;
            this.BestX     = BestX;
            this.BestY     = BestY;
            this.BestValue = BestValue;
        }
    }

    private static readonly Random __Random = new();

    /// <summary>Размер роя</summary>
    private readonly int _ParticleCount = ParticleCount;

    public void Minimize(
        Func<double, double, double> func,
        double MinX, 
        double MaxX, 
        double MinY, 
        double MaxY,
        int IterationCount,
        out double X, 
        out double Y, 
        out double Value) =>
        Minimize(
            func: func, 
            IntervalX: new(Min: MinX, Max: MaxX),
            IntervalY: new(Min: MinY, Max: MaxY),
            IterationCount: IterationCount,
            X: out X, 
            Y: out Y, 
            Value: out Value);

    public void Minimize(
        Func<double, double, double> func,
        Interval IntervalX, 
        Interval IntervalY,
        int IterationCount,
        out double X,
        out double Y,
        out double Value)
    {
        var IntervalVx = IntervalX;
        var IntervalVy = IntervalY;

        var swarm = new Particle2D[_ParticleCount].Initialize(IntervalX, IntervalY, (_, x, y) => new(func, x, y));
        var start = swarm.GetMin(p => p.Value) ?? throw new InvalidOperationException("Минимум не найден");
        X     = start.X;
        Y     = start.Y;
        Value = start.Value;

        for (var iteration = 0; iteration < IterationCount; iteration++)
            foreach (var p in swarm)
            {
                var r1 = __Random.NextDouble();
                var r2 = __Random.NextDouble();

                var new_vx = w * p.X + c1 * r1 * (p.BestX - p.X) + c2 * r2 * (X - p.X);
                IntervalVx.Normalize(ref new_vx);

                r1 = __Random.NextDouble();
                r2 = __Random.NextDouble();

                var new_vy = w * p.Y + c1 * r1 * (p.BestY - p.Y) + c2 * r2 * (Y - p.Y);
                IntervalVy.Normalize(ref new_vy);

                var new_x = IntervalX.Normalize(p.X + new_vx);
                var new_y = IntervalX.Normalize(p.Y + new_vy);
                p.X     = new_x;
                p.Y     = new_y;
                p.Value = func(new_x, new_y);
                if (p.Value < p.BestValue)
                {
                    p.BestX     = new_x;
                    p.BestY     = new_y;
                    p.BestValue = p.Value;
                }
                if (!(p.Value < Value)) continue;
                X     = new_x;
                Y     = new_y;
                Value = p.Value;
            }
    }

    public void Maximize(
        Func<double, double, double> func,
        double MinX, 
        double MaxX,
        double MinY, 
        double MaxY,
        int IterationCount,
        out double X,
        out double Y, 
        out double Value) =>
        Maximize(
            func: func, 
            IntervalX: new(Min: MinX, Max: MaxX), 
            IntervalY: new(Min: MinY, Max: MaxY),
            IterationCount: IterationCount,
            X: out X, 
            Y: out Y,
            Value: out Value);

    public void Maximize(
        Func<double, double, double> func,
        Interval IntervalX, 
        Interval IntervalY,
        int IterationCount,
        out double X, 
        out double Y,
        out double Value)
    {
        var IntervalVx = IntervalX;
        var IntervalVy = IntervalY;

        var swarm = new Particle2D[_ParticleCount].Initialize(IntervalX, IntervalY, (_, ix, iy) => new(func, ix, iy));
        var start = swarm.GetMax(p => p.Value) ?? throw new InvalidOperationException("Максимум не найден");
        X     = start.X;
        Y     = start.Y;
        Value = start.Value;

        for (var iteration = 0; iteration < IterationCount; iteration++)
            foreach (var p in swarm)
            {
                var r1 = __Random.NextDouble();
                var r2 = __Random.NextDouble();

                var new_vx = w * p.X + c1 * r1 * (p.BestX - p.X) + c2 * r2 * (X - p.X);
                IntervalVx.Normalize(ref new_vx);

                r1 = __Random.NextDouble();
                r2 = __Random.NextDouble();

                var new_vy = w * p.Y + c1 * r1 * (p.BestY - p.Y) + c2 * r2 * (Y - p.Y);
                IntervalVy.Normalize(ref new_vy);

                var new_x = IntervalX.Normalize(p.X + new_vx);
                var new_y = IntervalX.Normalize(p.Y + new_vy);
                p.X     = new_x;
                p.Y     = new_y;
                p.Value = func(new_x, new_y);
                if (p.Value > p.BestValue)
                {
                    p.BestX     = new_x;
                    p.BestY     = new_y;
                    p.BestValue = p.Value;
                }
                if (!(p.Value > Value)) continue;
                X     = new_x;
                Y     = new_y;
                Value = p.Value;
            }
    }
}