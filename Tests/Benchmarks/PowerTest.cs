namespace Benchmarks;

[CategoriesColumn]
public class PowerTest
{
    // |  Method |       Mean |     Error |    StdDev | Ratio | RatioSD |
    // |-------- |-----------:|----------:|----------:|------:|--------:|
    // |     Pow | 101.712 ns | 1.3038 ns | 1.2195 ns | 18.55 |    0.23 |
    // | PowFast |   5.480 ns | 0.0346 ns | 0.0306 ns |  1.00 |    0.00 |


    //const double x = 2;
    //const int n = 2 * 2 * 3 * 3 * 5;

    //[Benchmark]
    //public double Pow() => x.Pow(n);

    //[Benchmark(Baseline = true)]
    //public double PowFast() => x.PowFast(n);

    [Params(0, 1, 2, 6, 2 * 3 * (1 + 2 * 3), 2 * 2 * 3 * 5 * 7, 2 * 2 * 3 * 5 * 7 * 11)]
    public int Power { get; set; } = 0;

    [Benchmark]
    public double PowerBase() => Math.Pow(2, Power);

    [Benchmark]
    public double PowerSimple() => PowSimple(2, Power);
    private static double PowSimple(double x, int p)
    {
        var result = x;

        while (--p > 0)
            result *= x;

        return result;
    }

    [Benchmark]
    public double PowerFast() => PowFast(2, Power);
    private static double PowFast(double x, int p)
    {
        switch (x)
        {
            case double.NaN: return double.NaN;
            case double.PositiveInfinity: return double.PositiveInfinity;
            case double.NegativeInfinity: return double.NegativeInfinity;
            case 0: return 0;
            case +1: return 1;
            case -1: return p % 2 == 0 ? 1 : -1;
        }

        switch (p)
        {
            case -4: return 1 / (x * x * x * x);
            case -3: return 1 / (x * x * x);
            case -2: return 1 / (x * x);
            case -1: return 1 / x;
            case < 0: return 1 / x.Pow(-p);
            case 0: return 1;
            case 1: return x;
            case 2: return x * x;
            case 3: return x * x * x;
            case 4: return x * x * x * x;
            default:
                var result = x;

                var power = p;
                while (power > 0 && power % 2 == 0)
                {
                    result *= result;
                    power >>= 1;
                }

                while (power > 0 && power % 3 == 0)
                {
                    result *= result * result;
                    power /= 3;
                }

                var x0 = result;
                while (--power > 0)
                    result *= x0;

                return result;
        }
    }

    [Benchmark(Baseline = true)]
    public double PowerFast2() => PowFast2(2, Power);
    private static double PowFast2(double x, int p)
    {
        switch (x)
        {
            case double.NaN: return double.NaN;
            case double.PositiveInfinity: return double.PositiveInfinity;
            case double.NegativeInfinity: return double.NegativeInfinity;
            case 0: return 0;
            case +1: return 1;
            case -1: return p % 2 == 0 ? 1 : -1;
        }

        switch (p)
        {
            case -4: return 1 / (x * x * x * x);
            case -3: return 1 / (x * x * x);
            case -2: return 1 / (x * x);
            case -1: return 1 / x;
            case < 0: return 1 / x.Pow(-p);
            case 0: return 1;
            case 1: return x;
            case 2: return x * x;
            case 3: return x * x * x;
            case 4: return x * x * x * x;
        }

        var result = x;
        var power = p;

        if (p < 11)
            while (--power > 0)
                result *= x;
        else
        {
            while (power > 0 && power % 2 == 0)
            {
                result *= result;
                power >>= 1;
            }

            while (power > 0 && power % 3 == 0)
            {
                result *= result * result;
                power /= 3;
            }

            if (power > 1)
                return result * result.Pow(power - 1);
        }

        return result;
    }
}
