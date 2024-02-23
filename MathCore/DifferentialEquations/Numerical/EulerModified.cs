using System.Collections;

namespace MathCore.DifferentialEquations.Numerical;

public static class EulerModified
{
    public static double Step(
        Func<double, double, double> f,
        double t, double dt, double y)
    {
        var dy = f(t, y);
        var yy = y + dt * dy;
        return y + .5 * dt * (dy + f(t + dt, yy));
    }

    public static (double[] T, double[] Y) Solve(
        Func<double, double, double> f,
        double dt,
        double Tmax,
        double y0 = 0,
        double t0 = 0)
    {
        var N = (int)((Tmax - t0) / dt) + 1;
        var T = new double[N];
        var Y = new double[N];

        T[0] = t0;
        Y[0] = y0;

        for (var (i, t) = (1, t0 + dt); i < N; i++, t += dt)
        {
            Y[i] = Step(f, t, dt, Y[i - 1]);
            T[i] = t;
        }

        return (T, Y);
    }

    public static (double y1, double y2) Step(
        Func<double, (double y1, double y2), (double y1, double y2)> f,
        double t, double dt, (double y1, double y2) y)
    {
        var (y1, y2)   = y;
        var (dy1, dy2) = f(t, y);
        var yy = (y1 + dt * dy1, y2 + dt * dy2);
        var (dyy1, dyy2) = f(t + dt, yy);
        return (
            y1 + .5 * dt * (dy1 + dyy1),
            y2 + .5 * dt * (dy1 + dyy2));
    }

    public static (double[] T, (double y1, double y2)[] Y) Solve(
        Func<double, (double y1, double y2), (double y1, double y2)> f,
        double dt,
        double Tmax,
        (double y1, double y2) y0 = default,
        double t0 = 0)
    {
        var N = (int)((Tmax - t0) / dt) + 1;
        var T = new double[N];
        var Y = new (double y1, double y2)[N];

        T[0] = t0;
        Y[0] = y0;

        for (var (i, t) = (1, t0 + dt); i < N; i++, t += dt)
        {
            Y[i] = Step(f, t, dt, Y[i - 1]);
            T[i] = t;
        }

        return (T, Y);
    }

    public static (double y1, double y2, double y3) Step(
        Func<double, (double y1, double y2, double y3), (double y1, double y2, double y3)> f,
        double t, double dt, (double y1, double y2, double y3) y)
    {
        var (y1, y2, y3)    = y;
        var (dy1, dy2, dy3) = f(t, y);
        var yy = (y1 + dt * dy1, y2 + dt * dy2, y3 + dt * dy3);
        var (dyy1, dyy2, dyy3) = f(t + dt, yy);
        return (
            y1 + .5 * dt * (dy1 + dyy1),
            y2 + .5 * dt * (dy1 + dyy2),
            y3 + .5 * dt * (dy1 + dyy3));
    }

    public static (double[] T, (double y1, double y2, double y3)[] Y) Solve(
        Func<double, (double y1, double y2, double y3), (double y1, double y2, double y3)> f,
        double dt,
        double Tmax,
        (double y1, double y2, double y3) y0 = default,
        double t0 = 0)
    {
        var N = (int)((Tmax - t0) / dt) + 1;
        var T = new double[N];
        var Y = new (double y1, double y2, double y3)[N];

        T[0] = t0;
        Y[0] = y0;

        for (var (i, t) = (1, t0 + dt); i < N; i++, t += dt)
        {
            Y[i] = Step(f, t, dt, Y[i - 1]);
            T[i] = t;
        }

        return (T, Y);
    }

    private readonly struct AddMulList(IReadOnlyList<double> Y, double K, double[] X) : IReadOnlyList<double>
    {
        public int Count => X.Length;

        public double this[int i] => Y[i] + K * X[i];

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<double> GetEnumerator()
        {
            for (var (i, count) = (0, Count); i < count; i++)
                yield return this[i];
        }
    }

    public static double[] Step(
        Func<double, IReadOnlyList<double>, double[]> f,
        int M,
        double t, double dt, IReadOnlyList<double> y)
    {
        var dy  = f(t, y);
        var dy2 = f(t + dt, new AddMulList(y, dt, dy));

        for (var (i, count) = (0, dy.Length); i < count; i++)
            dy[i] = y[i] + 0.5 * dt * (dy[i] + dy2[i]);

        return dy;
    }

    public static (double[] T, double[][] Y) Solve(
        Func<double, IReadOnlyList<double>, double[]> f,
        double dt,
        double Tmax,
        double[] y0,
        double t0 = 0)
    {
        var m = y0.Length;
        var N = (int)((Tmax - t0) / dt) + 1;
        var T = new double[N];
        var Y = new double[N][];

        T[0] = t0;
        Y[0] = y0;

        for (var (i, t) = (1, t0 + dt); i < N; i++, t += dt)
        {
            Y[i] = Step(f, m, t, dt, Y[i - 1]);
            T[i] = t;
        }

        return (T, Y);
    }
}