using System;
using System.IO;
using System.IO.Compression;

using MathCore.Interpolation;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.Interpolation;

[TestClass]
public class BiliniarTests
{
    private static double Sinc(double x) => x == 0 ? 1 : Math.Sin(x) / x;

    [TestMethod]
    public void InterpolationTest()
    {
        const int N = 10;
        const int M = 20;
        var X = GC.AllocateUninitializedArray<double>(N).Initialize(i => ((double)i / (N - 1)).Pow2() * N);
        var Y = GC.AllocateUninitializedArray<double>(M).Initialize(i => ((double)i / (M - 1)).Pow2() * M);


        static double F(double x, double y) => Sinc(x + y);

        var Z = new double[N, M].Initialize((i, j) => F(X[i], Y[j]));

        var (x0, y0) = (2d, 4d);
        var z0 = F(x0, y0);

        var interpolator = new Biliniar(X, Y, Z);

        var result = interpolator.Interpolate(x0, y0);

        Assert.That.Value(result).IsEqual(z0, 9.10e-004);
    }
}