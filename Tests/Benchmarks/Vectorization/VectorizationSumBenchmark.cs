namespace Benchmarks.Vectorization;

public class VectorizationSumBenchmark
{
    private double[] _Data;

    [Params(3, 8, 16, 20, 64, 80, 256, 300, 1024, 1050, 1024 * 1024)]
    public int ArraySize { get; set; }

    [GlobalSetup]
    public void Initialize()
    {
        var data = new double[ArraySize];

        for (var i = 0; i < data.Length; i++)
            data[i] = i;

        _Data = data;
    }

    [Benchmark(Baseline = true)]
    public double Native()
    {
        var sum = 0d;

        for (var i = 0; i < _Data.Length; i++)
            sum += _Data[i];

        return sum;
    }

    [Benchmark]
    public double Vectorized()
    {
        var vector = DoubleVector.Create(_Data);
        var sum = vector.Sum();

        return sum;
    }
}
