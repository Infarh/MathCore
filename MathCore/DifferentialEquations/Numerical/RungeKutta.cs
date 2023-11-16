#nullable enable
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global

namespace MathCore.DifferentialEquations.Numerical;

/// <summary>Метод Рунге-Кутты</summary>
public static class RungeKutta
{
    private static double[] GetKY(double[] YY, IReadOnlyList<double> Y, double[] k, double dt, int Count)
    {
        for (var i = 0; i < Count; i++)
            YY[i] = Y[i] + k[i] * dt;
        return YY;
    }

    public static double Step4(
        Func<double, double, double> f,
        double t, double dt, double y)
    {
        var dt2 = dt / 2;
        var k1  = f(t, y);
        var k2  = f(t + dt2, y + k1 * dt2);
        var k3  = f(t + dt2, y + k2 * dt2);
        var k4  = f(t + dt, y + k3 * dt);
        return y + dt * (k1 + 2 * k2 + 2 * k3 + k4) / 6;
    }

    public static (double[] T, double[] Y) Solve4(
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
            Y[i] = Step4(f, t, dt, Y[i - 1]);
            T[i] = t;
        }

        return (T, Y);
    }

    public static (double y1, double y2) Step4(
        Func<double, (double y1, double y2), (double y1, double y2)> f,
        double t,
        double dt,
        (double y1, double y2) y)
    {
        var dt2 = dt / 2;
        var (y1, y2)       = y;
        var (k1_y1, k1_y2) = f(t, y);
        var (k2_y1, k2_y2) = f(t + dt2, (y1 + k1_y1 * dt2, y2 + k1_y2 * dt2));
        var (k3_y1, k3_y2) = f(t + dt2, (y1 + k2_y1 * dt2, y2 + k2_y2 * dt2));
        var (k4_y1, k4_y2) = f(t + dt, (y1 + k3_y1 * dt, y2 + k3_y2 * dt));

        return (
            y1 + dt * (k1_y1 + 2 * k2_y1 + 2 * k3_y1 + k4_y1) / 6,
            y2 + dt * (k1_y2 + 2 * k2_y2 + 2 * k3_y2 + k4_y2) / 6
        );
    }

    public static (double[] T, (double y1, double y2)[] Y) Solve4(
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
            Y[i] = Step4(f, t, dt, Y[i - 1]);
            T[i] = t;
        }

        return (T, Y);
    }

    public static (double y1, double y2, double y3) Step4(
        Func<double, (double y1, double y2, double y3), (double y1, double y2, double y3)> f,
        double t,
        double dt,
        (double y1, double y2, double y3) y)
    {
        var dt2 = dt / 2;
        var (y1, y2, y3)          = y;
        var (k1_y1, k1_y2, k1_y3) = f(t, y);
        var (k2_y1, k2_y2, k2_y3) = f(t + dt2, (y1 + k1_y1 * dt2, y2 + k1_y2 * dt2, y3 + k1_y3 * dt2));
        var (k3_y1, k3_y2, k3_y3) = f(t + dt2, (y1 + k2_y1 * dt2, y2 + k2_y2 * dt2, y3 + k2_y3 * dt2));
        var (k4_y1, k4_y2, k4_y3) = f(t + dt, (y1 + k3_y1 * dt, y2 + k3_y2 * dt, y3 + k3_y3 * dt));

        return (
            y1 + dt * (k1_y1 + 2 * k2_y1 + 2 * k3_y1 + k4_y1) / 6,
            y2 + dt * (k1_y2 + 2 * k2_y2 + 2 * k3_y2 + k4_y2) / 6,
            y3 + dt * (k1_y3 + 2 * k2_y3 + 2 * k3_y3 + k4_y3) / 6
        );
    }

    public static (double[] T, (double y1, double y2, double y3)[] Y) Solve4(
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
            Y[i] = Step4(f, t, dt, Y[i - 1]);
            T[i] = t;
        }

        return (T, Y);
    }

    public static double[] Step4(
        Func<double, IReadOnlyList<double>, double[]> f,
        double t, double dt, IReadOnlyList<double> y)
    {
        var m   = y.Count;
        var dt2 = dt / 2;
        var yt  = new double[m];

        var k1 = f(t, y);
        var k2 = f(t + dt2, GetKY(yt, y, k1, dt2, m));
        var k3 = f(t + dt2, GetKY(yt, y, k2, dt2, m));
        var k4 = f(t + dt,  GetKY(yt, y, k3, dt,  m));

        for (var i = 0; i < m; i++)
            yt[i] = y[i] + dt * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]) / 6;

        return yt;
    }

    public static (double[] T, double[][] Y) Solve4(
        Func<double, IReadOnlyList<double>, double[]> f,
        double dt,
        double Tmax,
        double[] y0,
        double t0 = 0)
    {
        var N = (int)((Tmax - t0) / dt) + 1;
        var T = new double[N];
        var Y = new double[N][];

        T[0] = t0;
        Y[0] = y0;

        for (var (i, t) = (1, t0 + dt); i < N; i++, t += dt)
        {
            Y[i] = Step4(f, t, dt, Y[i - 1]);
            T[i] = t;
        }

        return (T, Y);
    }

    //https://ru.qaz.wiki/wiki/Dormand–Prince_method
    /// <summary>Метод Дорманда – Принса Метод Дорманда – Принса - https://ru.abcdef.wiki/wiki/Dormand%E2%80%93Prince_method</summary>
    public static (double y, double err) Step45(
        Func<double, double, double> f,
        double t, double dt, double y)
    {
        var k1 = f(t, y);
        var k2 = f(t + dt * 1 / 5,  y + dt * (k1 * 1 / 5));
        var k3 = f(t + dt * 3 / 10, y + dt * (k1 * 3 / 40       + k2 * 9 / 40));
        var k4 = f(t + dt * 4 / 5,  y + dt * (k1 * 44 / 45      - k2 * 56 / 15      + k3 * 32 / 9));
        var k5 = f(t + dt * 8 / 9,  y + dt * (k1 * 19372 / 6561 - k2 * 25360 / 2187 + k3 * 64448 / 6561 - k4 * 212 / 729));
        var k6 = f(t + dt,          y + dt * (k1 * 9017 / 3168  - k2 * 355 / 33     + k3 * 46732 / 5247 + k4 * 49 / 176 - k5 * 5103 / 18656));

        var v5 = k1 * 35 / 384 + k3 * 500 / 1113 + k4 * 125 / 192 - k5 * 2187 / 6784 + k6 * 11 / 84;

        var k7 = f(t + dt, y + dt * v5);
        var v4 = k1 * 5179 / 57600 + k3 * 7571 / 16695 + k4 * 393 / 640 - k5 * 92097 / 339200 + k6 * 187 / 2100 + k7 * 1 / 40;

        return (y + dt * v5, v5 - v4);
    }

    public static (double[] T, double[] Y, double[] eps) Solve45(
        Func<double, double, double> f,
        double dt,
        double Tmax,
        double y0 = 0,
        double t0 = 0)
    {
        var N   = (int)((Tmax - t0) / dt) + 1;
        var T   = new double[N];
        var Y   = new double[N];
        var err = new double[N];

        T[0] = t0;
        Y[0] = y0;

        for (var (i, t) = (1, t0 + dt); i < N; i++, t += dt)
        {
            (Y[i], err[i]) = Step45(f, t, dt, Y[i - 1]);
            T[i]           = t;
        }

        return (T, Y, err);
    }

    public static ((double y1, double y2) y, (double e1, double e2) err) Step45(
        Func<double, (double y1, double y2), (double y1, double y2)> f,
        double t, double dt, (double y1, double y2) y)
    {
        var (y1, y2) = y;

        var (k1_y1, k1_y2) = f(t, y);
        var (k2_y1, k2_y2) = f(t + dt * 1 / 5, (
            y1 + dt * (k1_y1 * 1 / 5),
            y2 + dt * (k1_y2 * 1 / 5)));

        var (k3_y1, k3_y2) = f(t + dt * 3 / 10, (
            y1 + dt * (k1_y1 * 3 / 40 + k2_y1 * 9 / 40),
            y2 + dt * (k1_y2 * 3 / 40 + k2_y2 * 9 / 40)
        ));

        var (k4_y1, k4_y2) = f(t + dt * 4 / 5, (
            y1 + dt * (k1_y1 * 44 / 45 - k2_y1 * 56 / 15 + k3_y1 * 32 / 9),
            y2 + dt * (k1_y2 * 44 / 45 - k2_y2 * 56 / 15 + k3_y2 * 32 / 9)
        ));

        var (k5_y1, k5_y2) = f(t + dt * 8 / 9, (
            y1 + dt * (k1_y1 * 19372 / 6561 - k2_y1 * 25360 / 2187 + k3_y1 * 64448 / 6561 - k4_y1 * 212 / 729),
            y2 + dt * (k1_y2 * 19372 / 6561 - k2_y2 * 25360 / 2187 + k3_y2 * 64448 / 6561 - k4_y2 * 212 / 729)
        ));

        var (k6_y1, k6_y2) = f(t + dt, (
            y1 + dt * (k1_y1 * 9017 / 3168 - k2_y1 * 355 / 33 + k3_y1 * 46732 / 5247 + k4_y1 * 49 / 176 - k5_y1 * 5103 / 18656),
            y2 + dt * (k1_y2 * 9017 / 3168 - k2_y2 * 355 / 33 + k3_y2 * 46732 / 5247 + k4_y2 * 49 / 176 - k5_y2 * 5103 / 18656)
        ));

        var v51 = k1_y1 * 35 / 384 + k2_y1 * 500 / 1113 + k4_y1 * 125 / 192 - k5_y1 * 2187 / 6784 + k6_y1 * 11 / 84;
        var v52 = k1_y2 * 35 / 384 + k2_y2 * 500 / 1113 + k4_y2 * 125 / 192 - k5_y2 * 2187 / 6784 + k6_y2 * 11 / 84;

        var (k7_y1, k7_y2) = f(t + dt, (y1 + dt * v51, y2 + dt * v52));
        var v41 = k1_y1 * 5179 / 57600 + k3_y1 * 7571 / 16695 + k4_y1 * 393 / 640 - k5_y1 * 92097 / 339200 + k6_y1 * 187 / 2100 + k7_y1 * 1 / 40;
        var v42 = k1_y2 * 5179 / 57600 + k3_y2 * 7571 / 16695 + k4_y2 * 393 / 640 - k5_y2 * 92097 / 339200 + k6_y2 * 187 / 2100 + k7_y2 * 1 / 40;

        return ((y1 + dt * v51, y2 + dt * v52), (v51 - v41, v52 - v42));
    }

    public static (double[] T, (double y1, double y2)[] Y, (double e1, double e2)[] err) Solve45(
        Func<double, (double y1, double y2), (double y1, double y2)> f,
        double dt,
        double Tmax,
        (double y1, double y2) y0 = default,
        double t0 = 0)
    {
        var N   = (int)((Tmax - t0) / dt) + 1;
        var T   = new double[N];
        var Y   = new (double y1, double y2)[N];
        var err = new (double e1, double e2)[N];

        T[0] = t0;
        Y[0] = y0;

        for (var (i, t) = (1, t0 + dt); i < N; i++, t += dt)
        {
            (Y[i], err[i]) = Step45(f, t, dt, Y[i - 1]);
            T[i]           = t;
        }

        return (T, Y, err);
    }

    public static ((double y1, double y2, double y3) y, (double e1, double e2, double e3) err) Step45(
        Func<double, (double y1, double y2, double y3), (double y1, double y2, double y3)> f,
        double t, double dt, (double y1, double y2, double y3) y)
    {
        var (y1, y2, y3) = y;

        var (k1_y1, k1_y2, k1_y3) = f(t, y);
        var (k2_y1, k2_y2, k2_y3) = f(t + dt * 1 / 5, (
                y1 + dt * (k1_y1 * 1 / 5),
                y2 + dt * (k1_y2 * 1 / 5),
                y3 + dt * (k1_y3 * 1 / 5))
        );

        var (k3_y1, k3_y2, k3_y3) = f(t + dt * 3 / 10, (
            y1 + dt * (k1_y1 * 3 / 40 + k2_y1 * 9 / 40),
            y2 + dt * (k1_y2 * 3 / 40 + k2_y2 * 9 / 40),
            y3 + dt * (k1_y3 * 3 / 40 + k2_y3 * 9 / 40)
        ));

        var (k4_y1, k4_y2, k4_y3) = f(t + dt * 4 / 5, (
            y1 + dt * (k1_y1 * 44 / 45 - k2_y1 * 56 / 15 + k3_y1 * 32 / 9),
            y2 + dt * (k1_y2 * 44 / 45 - k2_y2 * 56 / 15 + k3_y2 * 32 / 9),
            y3 + dt * (k1_y3 * 44 / 45 - k2_y3 * 56 / 15 + k3_y3 * 32 / 9)
        ));

        var (k5_y1, k5_y2, k5_y3) = f(t + dt * 8 / 9, (
            y1 + dt * (k1_y1 * 19372 / 6561 - k2_y1 * 25360 / 2187 + k3_y1 * 64448 / 6561 - k4_y1 * 212 / 729),
            y2 + dt * (k1_y2 * 19372 / 6561 - k2_y2 * 25360 / 2187 + k3_y2 * 64448 / 6561 - k4_y2 * 212 / 729),
            y3 + dt * (k1_y3 * 19372 / 6561 - k2_y3 * 25360 / 2187 + k3_y3 * 64448 / 6561 - k4_y3 * 212 / 729)
        ));

        var (k6_y1, k6_y2, k6_y3) = f(t + dt, (
            y1 + dt * (k1_y1 * 9017 / 3168 - k2_y1 * 355 / 33 + k3_y1 * 46732 / 5247 + k4_y1 * 49 / 176 - k5_y1 * 5103 / 18656),
            y2 + dt * (k1_y2 * 9017 / 3168 - k2_y2 * 355 / 33 + k3_y2 * 46732 / 5247 + k4_y2 * 49 / 176 - k5_y2 * 5103 / 18656),
            y3 + dt * (k1_y3 * 9017 / 3168 - k2_y3 * 355 / 33 + k3_y3 * 46732 / 5247 + k4_y3 * 49 / 176 - k5_y3 * 5103 / 18656)
        ));

        var v51 = k1_y1 * 35 / 384 + k2_y1 * 500 / 1113 + k4_y1 * 125 / 192 - k5_y1 * 2187 / 6784 + k6_y1 * 11 / 84;
        var v52 = k1_y2 * 35 / 384 + k2_y2 * 500 / 1113 + k4_y2 * 125 / 192 - k5_y2 * 2187 / 6784 + k6_y2 * 11 / 84;
        var v53 = k1_y3 * 35 / 384 + k2_y3 * 500 / 1113 + k4_y3 * 125 / 192 - k5_y3 * 2187 / 6784 + k6_y3 * 11 / 84;

        var (k7_y1, k7_y2, k7_y3) = f(t + dt, (y1 + dt * v51, y2 + dt * v52, y3 + dt * v53));
        var v41 = k1_y1 * 5179 / 57600 + k3_y1 * 7571 / 16695 + k4_y1 * 393 / 640 - k5_y1 * 92097 / 339200 + k6_y1 * 187 / 2100 + k7_y1 * 1 / 40;
        var v42 = k1_y2 * 5179 / 57600 + k3_y2 * 7571 / 16695 + k4_y2 * 393 / 640 - k5_y2 * 92097 / 339200 + k6_y2 * 187 / 2100 + k7_y2 * 1 / 40;
        var v43 = k1_y3 * 5179 / 57600 + k3_y3 * 7571 / 16695 + k4_y3 * 393 / 640 - k5_y3 * 92097 / 339200 + k6_y3 * 187 / 2100 + k7_y3 * 1 / 40;

        return ((y1 + dt * v51, y2 + dt * v52, y3 + dt * v53), (v51 - v41, v52 - v42, v53 - v43));
    }

    public static (double[] T, (double y1, double y2, double y3)[] Y, (double e1, double e2, double e3)[] eps) Solve45(
        Func<double, (double y1, double y2, double y3), (double y1, double y2, double y3)> f,
        double dt,
        double Tmax,
        (double y1, double y2, double y3) y0 = default,
        double t0 = 0)
    {
        var N   = (int)((Tmax - t0) / dt) + 1;
        var T   = new double[N];
        var Y   = new (double y1, double y2, double y3)[N];
        var err = new (double e1, double e2, double e3)[N];

        T[0] = t0;
        Y[0] = y0;

        for (var (i, t) = (1, t0 + dt); i < N; i++, t += dt)
        {
            (Y[i], err[i]) = Step45(f, t, dt, Y[i - 1]);
            T[i]           = t;
        }

        return (T, Y, err);
    }

    public static (double[] Y, double[] Error) Step45(
        Func<double, IReadOnlyList<double>, double[]> f,
        double t, double dt, IReadOnlyList<double> y)
    {
        var m  = y.Count;
        var yt = new double[m];

        var k1 = f(t, y);

        static double[] GetY(double[] Y, IReadOnlyList<double> YY, double dt, int M, params (double[] K, double k)[] kk)
        {
            for (int i = 0, mm = kk.Length; i < M; i++)
            {
                var yy = 0d;
                for (var j = 0; j < mm; j++)
                    yy += kk[j].K[i] * kk[j].k;
                Y[i] = YY[i] + dt * yy;
            }
            return Y;
        }

        var k2 = f(t + dt * 1 / 5,
            GetY(yt, y, dt, m,
                (k1, 1 / 5d)));

        var k3 = f(t + dt * 3 / 10,
            GetY(yt, y, dt, m,
                (k1, 3 / 40d),
                (k2, 9 / 40d)));

        var k4 = f(t + dt * 4 / 5,
            GetY(yt, y, dt, m,
                (k1, 44 / 45d),
                (k2, -56 / 15d),
                (k3, 32 / 9d)));

        var k5 = f(t + dt * 8 / 9,
            GetY(yt, y, dt, m,
                (k1, 19372 / 6561d),
                (k2, -25360 / 2187d),
                (k3, 64448 / 6561d),
                (k4, -212 / 729d)));

        var k6 = f(t + dt,
            GetY(yt, y, dt, m,
                (k1, 9017 / 3168d),
                (k2, -355 / 33d),
                (k3, 46732 / 5247d),
                (k4, 49 / 176d),
                (k5, -5103 / 18656d)));

        static double[] GetV(double[] Y, int M, params (double[] K, double k)[] kk)
        {
            for (int i = 0, mm = kk.Length; i < M; i++)
            {
                var y = 0d;
                for (var j = 0; j < mm; j++)
                    y += kk[j].K[i] * kk[j].k;
                Y[i] = y;
            }
            return Y;
        }

        var v5 = GetV(yt, m,
            (k1, 35 / 384d),
            (k3, 500 / 1113d),
            (k4, 125 / 192d),
            (k5, -2187 / 6784d),
            (k6, 11 / 84d));

        var k7 = f(t + dt, GetKY(yt = new double[yt.Length], y, v5, dt, m));
        var v4 = GetV(yt, m,
            (k1, 5179 / 57600d),
            (k3, 7571 / 16695d),
            (k4, 393 / 640d),
            (k5, -92097 / 339200d),
            (k6, 187 / 2100d),
            (k7, 1 / 40d));

        for (var i = 0; i < m; i++)
        {
            yt[i] = v5[i] - v4[i];
            v5[i] = y[i] + dt * v5[i];
        }
        return (v5, yt);
    }
        
    public static (double[] T, double[][] Y, double[][] Error) Solve45(
        Func<double, IReadOnlyList<double>, double[]> f,
        double dt,
        double Tmax,
        double[] y0,
        double t0 = 0)
    {
        var N   = (int)((Tmax - t0) / dt) + 1;
        var T   = new double[N];
        var Y   = new double[N][];
        var err = new double[N][];

        T[0]   = t0;
        Y[0]   = y0;
        err[0] = new double[y0.Length];

        for (var (i, t) = (1, t0 + dt); i < N; i++, t += dt)
        {
            (Y[i], err[i]) = Step45(f, t, dt, Y[i - 1]);
            T[i]           = t;
        }

        return (T, Y, err);
    }
}