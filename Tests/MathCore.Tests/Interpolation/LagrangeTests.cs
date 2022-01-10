using System;
using System.Diagnostics;
using System.Linq;

using MathCore.Interpolation;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static System.Math;
// ReSharper disable InconsistentNaming

namespace MathCore.Tests.Interpolation;

[TestClass]
public class LagrangeTests
{
    private static double Sinc(double x) => x == 0 ? 1 : Sin(x) / x;

    [TestMethod]
    public void GetPolynomCoefficients()
    {
        var X = Interval.Range(0, 2, 0.1).ToArray();
        //var X = Interval.RangeN(0, 2, 5).ToArray();
        var Y = X.ToArray(Sinc);

        var polynom = Lagrange.GetPolynom(X, Y);

        var yy = X.ToArray(polynom.Value);

        var delta = Y.Zip(yy, (y0, y1) => y0 - y1);
        var error = delta.Sum(v => v.Pow2()) / (Y.Length + 1);

        Assert.That.Value(error).LessThan(2.00e-4, 6.36e-007);
    }

    [TestMethod]
    public void Integral()
    {
        static double f(double x) => Cos(x) * Sqrt(x);

        var x = Interval.Range(0, 10, 1).ToArray();
        var y = x.ToArray(f);

        var p = Lagrange.Integral(x, y, 2, 6);

        Debug.WriteLine(p);
    }
}