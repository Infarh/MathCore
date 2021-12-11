using System;

using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class Test
    {
        [Benchmark(Baseline = true)]
        public int[] NewArray() => new int[1024];

        [Benchmark]
        public int[] AllocateUninitializedArray() => GC.AllocateUninitializedArray<int>(1024);
    }
}
