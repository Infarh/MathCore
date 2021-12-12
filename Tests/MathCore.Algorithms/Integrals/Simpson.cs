using System;

namespace MathCore.Algorithms.Integrals;

public static class Simpson
{
    public static double IntegrateSimpson(this Func<double, double> f, double a, double b, double dx = 0.01)
    {
        if (a == b) return 0;

        var dx05 = 0.5 * dx;
        var x = a;
        var s = .0;
        var s_dx05 = f(x + dx05);
        while ((x += dx) < b)
        {
            s += f(x);
            s_dx05 += f(x + dx05);
        }
        return (f(a) + 2 * s + 4 * s_dx05 + f(b)) * dx / 6;
    }
}

public static class Adaptive
{
    public static double IntegrateAdaptive(this Func<double, double> f, double a, double b, double dx = 0.01, double Eps = 1e-6)
    {
        var I1 = f.IntegrateSimpson(a, b, dx);
        var I2 = f.IntegrateSimpson(a, b, dx / 2);
        return Math.Abs(I1 - I2) < Eps
            ? I2
            : f.IntegrateAdaptive(a, (a + b) / 2, dx / 2, Eps) 
            + f.IntegrateAdaptive((a + b) / 2, b, dx / 2, Eps);
    }
}