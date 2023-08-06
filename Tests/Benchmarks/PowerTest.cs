namespace Benchmarks;

[CategoriesColumn]
public class PowerTest
{
    // |  Method |       Mean |     Error |    StdDev | Ratio | RatioSD |
    // |-------- |-----------:|----------:|----------:|------:|--------:|
    // |     Pow | 101.712 ns | 1.3038 ns | 1.2195 ns | 18.55 |    0.23 |
    // | PowFast |   5.480 ns | 0.0346 ns | 0.0306 ns |  1.00 |    0.00 |


    const double x = 2;
    const int n = 2 * 2 * 3 * 3 * 5;

    //[Benchmark]
    //public double Pow() => x.Pow(n);

    //[Benchmark(Baseline = true)]
    //public double PowFast() => x.PowFast(n);
}
