using OxyPlot;

namespace MathCore.Algorithms.Functions;

public static class AdaptiveSampling
{
    private static double SampleFunc(double x) => Math.Sin(2 * Math.PI * Math.Pow(10 - x, 3) / 1000);

    public static (double x, double y)[] SampleAdaptive(Func<double, double> f, double a, double b, double eps = 0.5)
    {
        var points = new LinkedList<(double x, double y)>();

        var current = points.AddFirst((a, f(a)));
        var next = points.AddLast((b, f(b)));

        while (current != points.Last)
        {
            var (x1, y1) = current.Value;
            var (x2, y2) = next.Value;

            var x0 = (x1 + x2) / 2;
            var y0 = f(x0);

            var dx = x2 - x1;
            var abs_dy = (Math.Abs(y2 - y0) + Math.Abs(y0 - y1)) / 2;

            if (dx <= 2.5 && abs_dy < 0.05)
                (current, next) = (next, current.Next);
            else
                next = points.AddAfter(current, (x0, y0));
        }

        return points.ToArray();
    }

    public static void Test()
    {
        var points = SampleAdaptive(SampleFunc, 0, 20);

        Console.WriteLine("       X |      dx | Y");
        Console.WriteLine("---------|---------|------------");
        var last_x = 0.0;
        foreach (var (x, y) in points)
        {
            var dx = x - last_x;
            last_x = x;
            Console.WriteLine("{0,8:f4} | {1:f5} | {2,10:f7}", x, dx, y);
        }

        last_x = 0.0;
        var plot = Plot.Cartesian
            .Line(points, Color: OxyColors.Red)
            .Line(points.Select(p => (p.x, y:-(last_x - (last_x = p.x)))), Color: OxyColors.Blue)
            .ToPNG()
            .Execute();
    }
}
