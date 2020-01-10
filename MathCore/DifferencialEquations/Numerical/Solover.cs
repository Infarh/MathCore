using System.Collections.Generic;
using DU = System.Func<double, double, double>;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace MathCore.DifferencialEquations.Numerical
{
    public static partial class Solver
    {
        public delegate double NextValueMethod(double x0, double dx, double y0, DU f);

        public static IEnumerable<double> RungeKutta4(this IEnumerable<double> X, double y0, DU f)
        {
            using (var xx = X.GetEnumerator())
            {
                if (!xx.MoveNext()) yield break;

                var x = xx.Current;
                var y = y0;
                yield return y;

                while (xx.MoveNext())
                {
                    var x1 = xx.Current;
                    var dx = x1 - x;

                    yield return y = RungeKutta.NextValue(x, dx, y, f);
                    x = x1;

                    //var dx_05 = dx * .5;

                    //var k1 = f(x, y);
                    //var k2 = f(x + dx_05, y + k1 / 2);
                    //var k3 = f(x + dx_05, y + k2 / 2);
                    //var k4 = f(x + dx, y + k3);

                    //x = x1;
                    //y = y + dx * (k1 + 2 * k2 + 2 * k3 + k4) / 6;
                    //yield return y;
                }
            }
        }

        public static IEnumerable<double> Eyler_Simple(this IEnumerable<double> X, double y0, DU f)
        {
            using (var xx = X.GetEnumerator())
            {
                if (!xx.MoveNext()) yield break;

                var x = xx.Current;
                var y = y0;
                yield return y;

                while (xx.MoveNext())
                {
                    var x1 = xx.Current;
                    var dx = x1 - x;
                    y += dx * f(x, y);
                    x = x1;
                    yield return y;
                }
            }
        }

        [DST]
        public static double[] FixedStep(double y0, double start, double stop, int Count, DU f, NextValueMethod NextValue) => FixedStep(y0, new Interval(start, stop), Count, f, NextValue);

        [DST]
        public static double[] FixedStep(double y0, Interval interval, int Count, DU f, NextValueMethod NextValue)
        {
            var lenght = interval.Length;
            var dx = lenght / (Count - 1);

            var x = interval.Min;
            var y = y0;
            var Y = new double[Count];
            Y[0] = y;

            for (var n = 1; n < Count; n++, x += dx)
                Y[n] = y = NextValue(x, dx, y, f);
            //y = result[n] = y + dx * f(x, y);

            return Y;
        }

        public static double[] ValuesByGreed(double y0, double[] X, DU f, NextValueMethod NextValue)
        {
            var Y = new double[X.Length];

            var x = X[0];
            var y = Y[0] = y0;
            for (var i = 1; i < Y.Length; i++)
            {
                var x1 = X[i];
                Y[i] = y = NextValue(x, x1 - x, y, f);
                x = x1;
            }

            return Y;
        }
    }
}