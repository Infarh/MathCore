using BenchmarkDotNet.Running;

namespace Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        _ = BenchmarkRunner.Run<Test>();
    }
}