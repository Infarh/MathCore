using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class Test
    {
        private readonly string[] _Data = Enumerable.Range(1, 1).ToArray(i => i.ToString());

        [Benchmark]
        public List<int> Lambda()
        {
            var result = new List<int>(50000);

            for (var i = 0; i < 50000; i++)
                result.AddRange(_Data.Select(s => int.Parse(s)));
            return result;
        }

        [Benchmark]
        public List<int> MethodGroup()
        {
            var result = new List<int>(50000);

            for (var i = 0; i < 50000; i++)
                result.AddRange(_Data.Select(int.Parse));
            return result;
        }
    }
}
