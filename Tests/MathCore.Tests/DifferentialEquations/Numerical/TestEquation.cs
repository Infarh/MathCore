using System;
using System.Linq;

namespace MathCore.Tests.DifferentialEquations.Numerical
{
    internal static class TestEquation
    {
        public static double dY(double x, double y) => x.Pow(2) * Math.Sin(x) - 3 * y;

        public static Func<double, double, double> Eq => dY;

        public static double Y(double x) =>
            (-x.Pow2() / 10 + 3 / 25d * x - 13 / 250d) * Math.Cos(x)
            + (3 / 10d * x.Pow2() - 4 / 25d * x + 9 / 250d) * Math.Sin(x)
            + 10013 / 250d * Math.Exp(-3 * x);

        public static double[] Y(double[] X) => X.ToArray(Y);

        public static double GetError(double[] Y0, double[] Y)
        {
            var len = Y0.Length;
            if (Y.Length != len)
                throw new ArgumentException("Длина вектора значений не равна длине вектора опорных значений", nameof(Y));
            var err = 0d;
            for (var i = 0; i < len; i++)
                err += (Y0[i] - Y[i]).Pow2();

            return err.Sqrt() / 2;
        }
    }
}