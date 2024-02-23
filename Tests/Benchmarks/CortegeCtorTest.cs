namespace Benchmarks;

public class CortegeCtorTest
{
    public readonly ref struct TestComplexSimple(double Re, double Im)
    {
        public readonly double Re = Re;
        public readonly double Im = Im;
    }
    public readonly ref struct TestComplexCortege(double Re, double Im)
    {
        public readonly double Re = Re;
        public readonly double Im = Im;
    }

    private static double __Re = 5;
    private static double __Im = 7;
    private static int __Count = 321;

    [Benchmark]
    public TestComplexSimple Simple()
    {
        var re = __Re;
        var im = __Im;
        var z  = new TestComplexSimple(re, im);

        var count = __Count;
        for (var i = 0; i < count; i++)
            z = new(re, im);

        return z;
    }

    [Benchmark(Baseline = true)]
    public TestComplexCortege Cortege()
    {
        var re = __Re;
        var im = __Im;
        var z  = new TestComplexCortege(re, im);

        var count = __Count;
        for (var i = 0; i < count; i++)
            z = new(re, im);

        return z;
    }
}
