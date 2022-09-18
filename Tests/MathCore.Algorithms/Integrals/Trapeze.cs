namespace MathCore.Algorithms.Integrals;

public static class Trapeze
{
    public static double IntegrateTrapeze(this Func<double, double> f, double a, double b, double dx = 0.01)
    {
        var x = a + dx;
        var I = 0d;
        var last_y = f(a);
        while (x < b)
        {
            var y = f(x);
            I += (y + last_y) / 2;
            last_y = y;
            x += dx;
        }

        return I * dx;
    }
}