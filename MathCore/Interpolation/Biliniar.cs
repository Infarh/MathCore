using System;
using System.Data;

namespace MathCore.Interpolation;

public class Biliniar : Interpolator
{
    private static double Surface(
        double x, double y,
        double x1, double y1, double z1,
        double x2, double y2, double z2, 
        double x3, double y3, double z3)
    {
        var dx21 = x2 - x1;
        var dx31 = x3 - x1;

        var dy21 = y2 - y1;
        var dy31 = y3 - y1;

        var dz21 = z2 - z1;
        var dz31 = z3 - z1;

        // (x - x1) * (dy21 * dz31 - dz21 * dy31) +
        // (y - y1) * (dz21 * dx31 - dx21 * dz31) +
        // (z - z1) * (dx21 * dy31 - dy21 * dx31) = 0

        // (z - z1) * (dx21 * dy31 - dy21 * dx31) =
        // (x - x1) * (dz21 * dy31 - dy21 * dz31) +
        // (y - y1) * (dx21 * dz31 - dz21 * dx31)

        // (z - z1) = (
        // (x - x1) * (dz21 * dy31 - dy21 * dz31) +
        // (y - y1) * (dx21 * dz31 - dz21 * dx31)
        // ) / (dx21 * dy31 - dy21 * dx31)

        // z = (
        // (x - x1) * (dz21 * dy31 - dy21 * dz31) +
        // (y - y1) * (dx21 * dz31 - dz21 * dx31)
        // ) / (dx21 * dy31 - dy21 * dx31) +
        // z1

        return (
            (x - x1) * (dz21 * dy31 - dy21 * dz31) + 
            (y - y1) * (dx21 * dz31 - dz21 * dx31)
            ) / (dx21 * dy31 - dy21 * dx31) 
            + z1;
    }

    public static double Interpolate(double x, double y, double[] X, double[] Y, double[,] Z)
    {
        var i = Array.BinarySearch(X, x);
        var j = Array.BinarySearch(Y, y);

        var (i1, i2) = i > 0 ? (i, i) : (~i - 1, ~i);
        var (j1, j2) = j > 0 ? (j, j) : (~j - 1, ~j);

        var (x1, x2) = i > 0 ? (X[i], X[i]) : (X[i1], X[i2]);
        var (y1, y2) = i > 0 ? (Y[i], Y[i]) : (Y[j1], Y[j2]);

        var z11 = Z[i1, j1];
        var z12 = Z[i1, j2];
        var z21 = Z[i2, j1];

        return Surface( 
            x, y,
            x1, y1, z11,
            x1, y2, z12,
            x2, y1, z21);
    }

    private readonly double[] _X;
    private readonly double[] _Y;
    private readonly double[,] _Z;

    private static double[,] CreateMesh(double[] X, double[] Y, Func<double, double, double> f)
    {
        var N = X.Length;
        var M = Y.Length;

        var Z = new double[N, M];
        for(var i = 0; i < N; i++)
            for (var j = 0; j < M; j++)
                Z[i, j] = f(X[i], Y[j]);
        return Z;
    }

    public Biliniar(double[] X, double[] Y, Func<double, double, double> f) : this(X, Y, CreateMesh(X, Y, f)) { }

    public Biliniar(double[] X, double[] Y, double[,] Z)
    {
        var (N, M) = Z;

        if (X.Length != N) 
            throw new ArgumentException("Число элементов X не равно числу строк Z")
            {
                Data =
                {
                    { $"{nameof(X)}.Length", X.Length },
                    { $"{nameof(Z)}.Length", N },
                }
            };
        if (X.Length != N) 
            throw new ArgumentException("Число элементов Y не равно числу столбцов Z")
            {
                Data =
                {
                    { $"{nameof(Y)}.Length", X.Length },
                    { $"{nameof(Z)}.Length", M },
                }
            };

        _X = X;
        _Y = Y;
        _Z = Z;
    }

    public double Interpolate(double x, double y) => Interpolate(x, y, _X, _Y, _Z);
}