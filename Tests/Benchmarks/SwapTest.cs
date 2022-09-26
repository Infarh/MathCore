namespace Benchmarks;

//[CategoriesColumn]
public class SwapTest
{
    private static class Swap
    {
        public static void Simple(ref int x, ref int y)
        {
            var t = x;
            x = y;
            y = t;
        }

        public static void XOR(ref int x, ref int y)
        {
            x ^= y;
            y ^= x;
            x ^= y;
        }

        public static void Sum(ref int x, ref int y)
        {
            x += y;
            y = x - y;
            x -= y;
        }

        public static void Cortege(ref int x, ref int y) => (x, y) = (y, x);
    }

    private static readonly Random __RND = new(10);
    private readonly int[] _X = Enumerable.Range(1, __RND.Next(10, 1000)).ToArray();
    private int _Count = 1000;

    [Benchmark]
    public int[] Inline()
    {
        var xx = _X;
        for (var n = 0; n < _Count; n++)
        {
            var i = __RND.Next(xx.Length);
            var j = __RND.Next(xx.Length);

            var t = xx[i];
            xx[i] = xx[j];
            xx[i] = t;
        }

        return xx;
    }

    [Benchmark(Baseline = true)]
    public int[] Simple()
    {
        var xx = _X;
        for (var n = 0; n < _Count; n++)
        {
            var i = __RND.Next(xx.Length);
            var j = __RND.Next(xx.Length);

            Swap.Simple(ref xx[i], ref xx[j]);
        }

        return xx;
    }

    [Benchmark]
    public int[] XOR()
    {
        var xx = _X;
        for (var n = 0; n < _Count; n++)
        {
            var i = __RND.Next(xx.Length);
            var j = __RND.Next(xx.Length);

            Swap.XOR(ref xx[i], ref xx[j]);
        }

        return xx;
    }

    [Benchmark]
    public int[] Sum()
    {
        var xx = _X;
        for (var n = 0; n < _Count; n++)
        {
            var i = __RND.Next(xx.Length);
            var j = __RND.Next(xx.Length);

            Swap.Sum(ref xx[i], ref xx[j]);
        }

        return xx;
    }

    [Benchmark]
    public int[] Cortege()
    {
        var xx = _X;
        for (var n = 0; n < _Count; n++)
        {
            var i = __RND.Next(xx.Length);
            var j = __RND.Next(xx.Length);

            Swap.Cortege(ref xx[i], ref xx[j]);
        }

        return xx;
    }

    [Benchmark]
    public int[] CortegeInline()
    {
        var xx = _X;
        for (var n = 0; n < _Count; n++)
        {
            var i = __RND.Next(xx.Length);
            var j = __RND.Next(xx.Length);

            (xx[i], xx[j]) = (xx[j], xx[i]);
        }

        return xx;
    }
}
