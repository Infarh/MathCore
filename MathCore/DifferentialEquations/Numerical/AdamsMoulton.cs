using System;

using static System.Math;

namespace MathCore.DifferentialEquations.Numerical
{
    public static class AdamsMoulton
    {
        public static (double Y, double Error, int Iterations) StepX(
            Func<double, double, double> f,
            double t, double dt,
            double y,
            double Eps, int IterationsCount)
        {
            double y1 = y, y2, delta;
            var t1 = t + dt;
            var i = 0;
            do
            {
                y2 = y + dt * f(t1, y1);
                delta = y2 - y1;
                y1 = y2;
            }
            while (Abs(delta) > Eps && i++ < IterationsCount);

            return (y2, delta, i);
        }

        public static (double Y, double Error, int Iterations) Step0(
            Func<double, double, double> f,
            double t, double dt,
            double y,
            double Eps, int IterationsCount)
        {
            double y1 = AdamsBashforth.Step0(f, t, dt, y), y2, delta;
            var t1 = t + dt;

            var i = 0;
            do
            {
                y2 = y + dt * (
                    0.5 * f(t1, y1) + 
                    0.5 * f(t, y));
                delta = y2 - y1;
                y1 = y2;
            }
            while (Abs(delta) > Eps && i++ < IterationsCount);

            return (y2, delta, i);
        }

        public static (double Y, double Error, int Iterations) Step1(
            Func<double, double, double> f,
            double t, double dt,
            double y, double YPrev1,
            double Eps, int IterationsCount)
        {
            double y1 = AdamsBashforth.Step1(f, t, dt, y, YPrev1), y2, delta;
            var t1 = t + dt;
            var t_1 = t - dt;

            var i = 0;
            do
            {
                y2 = y + dt * (
                    5 / 12d * f(t1, y1) +
                    8 / 12d * f(t, y) - 
                    1 / 12d * f(t_1, YPrev1));
                delta = y2 - y1;
                y1 = y2;
            }
            while (Abs(delta) > Eps && i++ < IterationsCount);

            return (y2, delta, i);
        }

        public static (double Y, double Error, int Iterations) Step2(
            Func<double, double, double> f,
            double t, double dt,
            double y, double YPrev1, double YPrev2,
            double Eps, int IterationsCount)
        {
            double y1 = AdamsBashforth.Step2(f, t, dt, y, YPrev1, YPrev2), y2, delta;
            var t1 = t + dt;
            var t_1 = t - dt;
            var t_2 = t - 2 * dt;

            var i = 0;
            do
            {
                y2 = y + dt * (
                    9 / 24d * f(t1, y1) +
                    19 / 24d * f(t, y) -
                    5 / 24d * f(t_1, YPrev1) +
                    24 / 24d * f(t_2, YPrev2));
                delta = y2 - y1;
                y1 = y2;
            }
            while (Abs(delta) > Eps && i++ < IterationsCount);

            return (y2, delta, i);
        }

        public static (double Y, double Error, int Iterations) Step3(
            Func<double, double, double> f,
            double t, double dt,
            double y, double YPrev1, double YPrev2, double YPrev3,
            double Eps, int IterationsCount)
        {
            double y1 = AdamsBashforth.Step3(f, t, dt, y, YPrev1, YPrev2, YPrev3), y2, delta;
            var t1 = t + dt;
            var t_1 = t - dt;
            var t_2 = t - 2 * dt;
            var t_3 = t - 3 * dt;

            var i = 0;
            do
            {
                y2 = y + dt * (
                    251 / 720d * f(t1, y1) +
                    646 / 720d * f(t, y) -
                    264 / 720d * f(t_1, YPrev1) +
                    106 / 720d * f(t_2, YPrev2) -
                    19 / 720d * f(t_3, YPrev3));
                delta = y2 - y1;
                y1 = y2;
            }
            while (Abs(delta) > Eps && i++ < IterationsCount);

            return (y2, delta, i);
        }

        public static (double[] T, (double Y, double Eps, int Iterations)[] Y) Solve(
            Func<double, double, double> f,
            double dt,
            double Tmax,
            double y0 = 0,
            double t0 = 0,
            double Eps = 0.001, int IterationsCount = 100)
        {
            var N = (int)((Tmax - t0) / dt) + 1;
            var T = new double[N];
            var Y = new (double Y, double Eps, int Iterations)[N];

            T[0] = t0;
            Y[0] = (y0, 0, 0);

            if (N > 1) Y[1] = Step0(f, t0, dt, y0, Eps, IterationsCount);
            if (N > 2) Y[2] = Step1(f, t0 + dt, dt, Y[1].Y, y0, Eps, IterationsCount);
            if (N > 3) Y[3] = Step2(f, t0 + 2 * dt, dt, Y[2].Y, Y[1].Y, y0, Eps, IterationsCount);
            if (N > 4) Y[4] = Step3(f, t0 + 3 * dt, dt, Y[3].Y, Y[2].Y, Y[1].Y, y0, Eps, IterationsCount);
            for (var (i, t) = (6, t0 + 5 * dt); i < N; i++, t += dt)
                Y[i] = Step3(f, t, dt, Y[i - 1].Y, Y[i - 2].Y, Y[i - 3].Y, Y[i - 4].Y, Eps, IterationsCount);

            return (T, Y);
        }
    }
}