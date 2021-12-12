using System;

namespace MathCore.Algorithms.Integrals
{
    public static class Rectangles
    {
        public static double IntegrateRect(this Func<double, double> f, double a, double b, double dx = 0.01)
        {
            var x = a;
            var I = 0d;
            while (x < b)
            {
                I += f(x);
                x += dx;
            }

            return I * dx;
        }
    }
}
