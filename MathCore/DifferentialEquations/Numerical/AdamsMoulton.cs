using System;
using static System.Math;

namespace MathCore.DifferentialEquations.Numerical
{
    public static class AdamsMoulton
    {
        public static (double Y, double Error) Step0(
            Func<double, double, double> f,
            double t, double dt,
            double y,
            double Eps, int IterationsCount)
        {
            var y1 = y;
            double y2 = double.NaN, delta = Eps + 1;
            var t1 = t + dt;
            for (var i = 0; i < IterationsCount && Abs(delta) > Eps; i++)
            {
                y2 = y + dt * f(t1, y1);
                delta = y2 - y1;
                y1 = y2;
            }
            return (y2, delta);
        }

        public static (double Y, double Error) Step1(
            Func<double, double, double> f,
            double t, double dt,
            double y,
            double Eps, int IterationsCount)
        {
            var y1 = AdamsBashforth.Step1(f, t, dt, y);
            double y2 = double.NaN, delta = Eps + 1;
            var t1 = t + dt;
            for (var i = 0; i < IterationsCount && Abs(delta) > Eps; i++)
            {
                y2 = y + dt * (f(t1, y1) + f(t, y)) / 2;
                delta = y2 - y1;
                y1 = y2;
            }
            return (y2, delta);
        }

        public static (double Y, double Error) Step2(
            Func<double, double, double> f,
            double t, double dt,
            double y, double YPrev1,
            double Eps, int IterationsCount)
        {
            var y1 = AdamsBashforth.Step2(f, t, dt, y, YPrev1);
            double y2 = double.NaN, delta = Eps + 1;
            var t1 = t + dt;
            var t_1 = t - dt;
            for (var i = 0; i < IterationsCount && Abs(delta) > Eps; i++)
            {
                y2 = y + dt * (5 * f(t1, y1)  + 8 * f(t, y) - f(t_1, YPrev1)) / 12;
                delta = y2 - y1;
                y1 = y2;
            }
            return (y2, delta);
        }

        public static (double Y, double Error) Step3(
            Func<double, double, double> f,
            double t, double dt,
            double y, double YPrev1, double YPrev2,
            double Eps, int IterationsCount)
        {
            var y1 = AdamsBashforth.Step3(f, t, dt, y, YPrev1, YPrev2);
            double y2 = double.NaN, delta = Eps + 1;
            var t1 = t + dt;
            var t_1 = t - dt;
            var t_2 = t - 2 * dt;
            for (var i = 0; i < IterationsCount && Abs(delta) > Eps; i++)
            {
                y2 = y + dt * (9 * f(t1, y1) + 19 * f(t, y) - 5 * f(t_1, YPrev1) + 24 * f(t_2, YPrev2)) / 24;
                delta = y2 - y1;
                y1 = y2;
            }
            return (y2, delta);
        }

        public static (double Y, double Error) Step4(
            Func<double, double, double> f,
            double t, double dt,
            double y, double YPrev1, double YPrev2, double YPrev3,
            double Eps, int IterationsCount)
        {
            var y1 = AdamsBashforth.Step4(f, t, dt, y, YPrev1, YPrev2, YPrev3);
            double y2 = double.NaN, delta = Eps + 1;
            var t1 = t + dt;
            var t_1 = t - dt;
            var t_2 = t - 2 * dt;
            var t_3 = t - 3 * dt;
            for (var i = 0; i < IterationsCount && Abs(delta.Abs()) > Eps; i++)
            {
                y2 = y + dt * (251 * f(t1, y1) + 646 * f(t, y) - 264 * f(t_1, YPrev1) + 106 * f(t_2, YPrev2) - 19 * f(t_3, YPrev3)) / 720;
                delta = y2 - y1;
                y1 = y2;
            }

            return (y2, delta);
        }

        public static (double[] T, (double Y, double Eps)[] Y) Solve(
            Func<double, double, double> f,
            double dt,
            double Tmax,
            double y0 = 0,
            double t0 = 0,
            double Eps = 0.001, int IterationsCount = 100)
        {
            var N = (int)((Tmax - t0) / dt) + 1;
            var T = new double[N];
            var Y = new (double Y, double Eps)[N];

            T[0] = t0;
            Y[0] = (y0, 0);

            if (N > 1) Y[1] = Step1(f, t0, dt, y0, Eps, IterationsCount);
            if (N > 2) Y[2] = Step2(f, t0 + dt, dt, Y[1].Y, y0, Eps, IterationsCount);
            if (N > 3) Y[3] = Step3(f, t0 + 2 * dt, dt, Y[2].Y, Y[1].Y, y0, Eps, IterationsCount);
            if (N > 4) Y[4] = Step4(f, t0 + 3 * dt, dt, Y[3].Y, Y[2].Y, Y[1].Y, y0, Eps, IterationsCount);
            for (var (i, t) = (6, t0 + 5 * dt); i < N; i++, t += dt)
                Y[i] = Step4(f, t, dt, Y[i - 1].Y, Y[i - 2].Y, Y[i - 3].Y, Y[i - 4].Y, Eps, IterationsCount);

            return (T, Y);
        }
    }
}