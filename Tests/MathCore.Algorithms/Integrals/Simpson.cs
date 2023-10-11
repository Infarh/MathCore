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
