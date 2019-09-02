using DU = System.Func<double, double, double>;

namespace MathCore.DifferencialEquations.Numerical
{
    public static partial class Solover
    {

        public static class RungeKutta
        {
            public static double[] FixedStep(double y0, double start, double stop, int Count, DU f) => FixedStep(y0, new Interval(start, stop), Count, f);

            public static double[] FixedStep(double y0, Interval interval, int Count, DU f)
            {
                var lenght = interval.Length;
                var dx = lenght / (Count - 1);

                var dx05 = dx / 2;

                var x = interval.Min;
                var y = y0;
                var Y = new double[Count];
                Y[0] = y;

                for(var n = 1; n < Count; n++, x += dx)
                    Y[n] = y = NextValue(x, dx, y, f);

                return Y;
            }

            public static double[] ValuesByGreed(double y0, double[] X, DU f)
            {
                var Y = new double[X.Length];

                var x = X[0];
                var y = Y[0] = y0;
                for(var i = 1; i < Y.Length; i++)
                {
                    var x1 = X[i];
                    Y[i] = y = NextValue(x, x1 - x, y, f);
                    x = x1;
                }

                return Y;
            }

            public static double NextValue(double x0, double dx, double y0, DU f)
            {
                var dx_05 = dx * .5;

                var k1 = dx * f(x0, y0);
                var k2 = dx * f(x0 + dx_05, y0 + k1 * .5);
                var k3 = dx * f(x0 + dx_05, y0 + k2 * .5);
                var k4 = dx * f(x0 + dx, y0 + k3);

                return y0 + (k1 + 2 * k2 + 2 * k3 + k4) / 6;
            }
        }
    }
}
