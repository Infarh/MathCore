using System;
using System.Linq;

using static System.Math;
using Runge_Kutta = MathCore.DifferentialEquations.Numerical.RungeKutta;

using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable RedundantArgumentDefaultValue

namespace MathCore.Tests.DifferentialEquations.Numerical
{
    internal static class TestEquation
    {
        public static double Equation(double x, double y) => x.Pow(2) * Sin(x) - 3 * y;

        public static double ExpectedResult(double x) =>
            - x.Pow2() / 10 * Cos(x)
            + 3 / 25d * x * Cos(x)
            - 13 / 250d * Cos(x)
            + 3 / 10d * x.Pow2() * Sin(x)
            - 4 / 25d * x * Sin(x)
            + 9 / 250d * Sin(x)
            + 10013 / 250d * Exp(-3 * x);
    }

    [TestClass]
    public class RungeKutta
    {
        [TestMethod]
        public void Solve()
        {
            const double x0 = 0;
            const double x1 = 20;
            const double y0 = 40;
            const int n = 100;
            const double dx = (x1 - x0) / n;
            Func<double, double, double> f = TestEquation.Equation;

            var (T, runge4Y) = Runge_Kutta.Solve4(f, dx, x1, y0, x0);
            var (_, runge45Y, err) = Runge_Kutta.Solve45(f, dx, x1, y0, x0);

            var expected_y = TestEquation.ExpectedResult(T[^1]);
            var err1 = expected_y - runge4Y[^1];
            var err1rel = err1 / expected_y;

            var err2 = expected_y - runge45Y[^1];
            var err2rel = err2 / expected_y;

            var q1 = err.Average(x => x * x).Sqrt();
        }
    }
}
