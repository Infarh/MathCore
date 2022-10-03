namespace Benchmarks.Interpolation;

[MemoryDiagnoser]
public class Polynoms
{
    private static readonly double[] _X =
    {
        -3/2d,
        -3/4d,
        0,
        3/4d,
        3/2d
    };

    private static double[] _Y =
    {
        -14.1014,
        -0.931596,
        0,
        0.931596,
        14.1014
    };

    [Benchmark]
    public double[] Lagrange() => MathCore.Interpolation.Lagrange.GetPolynomCoefficients(_X, _Y);

    [Benchmark(Baseline = true)]
    public double[] Newton() => MathCore.Interpolation.Newton.GetPolynomCoefficients(_X, _Y);
}