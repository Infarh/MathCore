using static System.Math;

namespace Benchmarks;

[CategoriesColumn]
public class PowerIterationTest
{
    [Params(1e-306, 1e-100, 1e-25, 1e-10, 1e-5, 1e5, 1e10, 1e25, 1e100, 1e306)]
    public double X { get; set; }

    [GlobalSetup]
    public void TestIteration() => Console.Title = $"X = {X:e0}";

    [Benchmark(Baseline = true)]
    public (double, short) MathPow()
    {
        
        var x = X;
        var exp2 = Floor(Log2(x));
        var mantissa = x * Pow(2, -exp2);

        return (mantissa - 1, (short)exp2);
    }

    [Benchmark]
    public (double, short) Iteration()
    {
        var x = X;
        var n = 0;

        if (x > 2)
            while (x > 2) (x, n) = (x / 2, n + 1);
        else
            while (x < 1) (x, n) = (x * 2, n - 1);

        return (x - 1, (short)n);
    }

    [Benchmark]
    public (double, short) Complex()
    {
        var x = X;
        var n = 0;
        switch (x)
        {
            case > 2 and < 5e15:
            {
                while (x > 2) (x, n) = (x / 2, n + 1);
                return (x - 1, (short)n);
            }
            case > 1e-15:
            {
                while (x < 1) (x, n) = (x * 2, n - 1);
                return (x - 1, (short)n);
            }

            default:
                var exp2 = Floor(Log2(x));
                return (x * Pow(2, -exp2) - 1, (short)exp2);
        }
    }
}
