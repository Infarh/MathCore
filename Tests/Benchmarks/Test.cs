using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class Test
    {
        private static void Swap<T>(ref T a, ref T b)
        {
            var tmp = a;
            a = b;
            b = tmp;
        }

        private static void Swap2<T>(ref T a, ref T b) => (a, b) = (b, a);

        private static (int, int) TestSwapOld(int a, int b)
        {
            Swap(ref a, ref b);
            return (a, b);
        }

        private static (int, int) TestSwapNew(int a, int b)
        {
            Swap2(ref a, ref b);
            return (a, b);
        }

        private static (int, int) TestSwapInline(int a, int b)
        {
            var tmp = a;
            a = b;
            b = tmp;
            return (a, b);
        }

        [Benchmark]
        public (int a, int b) SwapOld()
        {
            var a = 5;
            var b = 7;
            return TestSwapOld(a, b);
        }

        [Benchmark]
        public (int a, int b) SwapNew()
        {
            var a = 5;
            var b = 7;
            return TestSwapNew(a, b);
        }

        [Benchmark]
        public (int a, int b) SwapInline()
        {
            var a = 5;
            var b = 7;
            return TestSwapInline(a, b);
        }
    }
}
