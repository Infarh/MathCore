using System;
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

        var delta = Y.Zip(yy, (y0, y1) => y0 - y1).ToArray();
        var error = delta.Sum(v => v.Pow2() / 2).Sqrt();

        Assert.That.Value(error).LessThan(1.06e-014);
    }
}