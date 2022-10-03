using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks;

public class CortegeCtorTest
{
    public readonly ref struct TestComplexSimple
    {
        public readonly double Re;
        public readonly double Im;

        public TestComplexSimple(double Re, double Im)
        {
            this.Re = Re;
            this.Im = Im;
        }
    }
    public readonly ref struct TestComplexCortege
    {
        public readonly double Re;
        public readonly double Im;

        public TestComplexCortege(double Re, double Im) => (this.Re, this.Im) = (Re, Im);
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
