namespace MathCore.Algorithms.Integrals;

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