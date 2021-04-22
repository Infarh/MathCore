using System;

namespace MathCore.DifferentialEquations.Numerical
{
    public static class AdamsBashforth
    {
        public static double Step1(
            Func<double, double, double> f,
            double t, double dt,
            double y) =>
            Euler.Step(f, t, dt, y);

        public static double Step2(
            Func<double, double, double> f,
            double t, double dt,
            double y, double YPrev1) =>
            y + dt * (3 * f(t, y) / 2
                - f(t - dt, YPrev1) / 2
                );

        public static double Step3(
            Func<double, double, double> f,
            double t, double dt,
            double y, double YPrev1, double YPrev2) =>
            y + dt * (23 * f(t, y) / 12
                - 4 * f(t - dt, YPrev1) / 3
                + 5 * f(t - dt, YPrev2) / 12
                );

        public static double Step4(
            Func<double, double, double> f,
            double t, double dt,
            double y, double YPrev1, double YPrev2, double YPrev3) =>
            y + dt * (55 * f(t, y)
                - 59 * f(t - dt, YPrev1)
                + 37 * f(t - 2 * dt, YPrev2)
                - 9 * f(t - 3 * dt, YPrev3)
                ) / 24;

        public static double Step5(
            Func<double, double, double> f,
            double t, double dt,
            double y, double YPrev1, double YPrev2, double YPrev3, double YPrev4) =>
            y + dt * (1901 * f(t, y) / 720
                - 1387 * f(t - dt, YPrev1) / 360
                + 109 * f(t - 2 * dt, YPrev2) / 30
                - 637 * f(t - 3 * dt, YPrev3) / 360
                + 251 * f(t - 4 * dt, YPrev4) / 720
                );

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

            if (N > 1) Y[1] = Step1(f, t0, dt, y0);
            if (N > 2) Y[2] = Step2(f, t0 + dt, dt, Y[1], y0);
            if (N > 3) Y[3] = Step3(f, t0 + 2 * dt, dt, Y[2], Y[1], y0);
            if (N > 4) Y[4] = Step4(f, t0 + 3 * dt, dt, Y[3], Y[2], Y[1], y0);
            if (N > 5) Y[5] = Step5(f, t0 + 4 * dt, dt, Y[5], Y[3], Y[2], Y[1], y0);
            for (var (i, t) = (6, t0 + 5 * dt); i < N; i++, t += dt)
                Y[i] = Step5(f, t, dt, Y[i - 1], Y[i - 2], Y[i - 3], Y[i - 4], Y[i - 5]);

            return (T, Y);
        }
    }
}
