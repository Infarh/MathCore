using System;
using System.Collections.Generic;

namespace MathCore.DifferentialEquations.Numerical
{
    public static class Euler
    {
        public static double Step(
            Func<double, double, double> f,
            double t, double dt, double y) =>
            y + dt * f(t, y);

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
            var (dy1, dy2) = f(t, y);
            return (y.y1 + dt * dy1, y.y2 * dt * dy2);
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
            var (dy1, dy2, dy3) = f(t, y);
            return (y.y1 + dt * dy1, y.y2 * dt * dy2, y.y3 * dt * dy3);
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

        public static double[] Step(
            Func<double, IReadOnlyList<double>, double[]> f,
            int M,
            double t, double dt, IReadOnlyList<double> y)
        {
            var result = f(t, y);
            for (var i = 0; i < M; i++)
                result[i] = y[i] + dt * result[i];
            return result;
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
}