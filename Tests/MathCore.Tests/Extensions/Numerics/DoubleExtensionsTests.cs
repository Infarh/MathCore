using System.Diagnostics;

namespace MathCore.Tests.Extensions.Numerics;

[TestClass]
public class DoubleExtensionsTests
{
    [TestMethod]
    public void PowIntTest()
    {
        const double x = 2;
        const int n = 2 * 2 * 3 * 3 * 5;

        var expected = Math.Pow(x, n);

        var actual = x.Pow(n);
        //var actual2 = x.PowFast(n);

        actual.AssertEquals(expected);
        //actual2.AssertEquals(expected);
        actual.ToDebug();
    }

    [TestMethod]
    public void PowReturnsNaNWhenArgIsNaN()
    {
        const double x = double.NaN;
        const double expected = double.NaN;

        foreach (var n in Enumerable.Range(0, 10))
        {
            var actual = x.Pow(n);
            actual.AssertEquals(expected);
        }
    }

    [TestMethod]
    public void PowReturnsPositiveInfinityWhenArgIsPositiveInfinity()
    {
        const double x = double.PositiveInfinity;
        const double expected = double.PositiveInfinity;

        foreach (var n in Enumerable.Range(0, 10))
        {
            var actual = x.Pow(n);
            actual.AssertEquals(expected);
        }
    }

    [TestMethod]
    public void PowReturnsNegativeInfinityWhenArgIsNegativeInfinity()
    {
        const double x = double.NegativeInfinity;
        const double expected = double.NegativeInfinity;

        foreach (var n in Enumerable.Range(0, 10))
        {
            var actual = x.Pow(n);
            actual.AssertEquals(expected);
        }
    }

    [TestMethod]
    public void PowReturnsZeroWhenArgIsZero()
    {
        const double x = 0;
        const double expected = 0;

        foreach (var n in Enumerable.Range(0, 10))
        {
            var actual = x.Pow(n);
            actual.AssertEquals(expected);
        }
    }

    [TestMethod]
    public void PowReturnsOneWhenArgIsOne()
    {
        const double x = 1;
        const double expected = 1;

        foreach (var n in Enumerable.Range(0, 10))
        {
            var actual = x.Pow(n);
            actual.AssertEquals(expected);
        }
    }

    [TestMethod]
    public void PowReturnsOneWhenArgIsMinusOneAndPowerOdd()
    {
        const double x = -1;
        const double expected = -1;

        foreach (var n in Enumerable.Range(0, 10))
        {
            var actual = x.Pow(2 * n + 1);
            actual.AssertEquals(expected);
        }
    }

    [TestMethod]
    public void PowReturnsOneWhenArgIsMinusOneAndPowerEven()
    {
        const double x = -1;
        const double expected = 1;

        foreach (var n in Enumerable.Range(0, 10))
        {
            var actual = x.Pow(2 * n);
            actual.AssertEquals(expected);
        }
    }

    [TestMethod]
    [DataRow(2, -5, DisplayName = "2^-5 = 0.03125")]
    [DataRow(2, -4, DisplayName = "2^-4 = 0.0625")]
    [DataRow(2, -3, DisplayName = "2^-3 = 0.125")]
    [DataRow(2, -2, DisplayName = "2^-2 = 0.25")]
    [DataRow(2, -1, DisplayName = "2^-1 = 0.5")]
    [DataRow(2, +0, DisplayName = "2^ 0 = 1")]
    [DataRow(2, +1, DisplayName = "2^ 1 = 2")]
    [DataRow(2, +2, DisplayName = "2^ 2 = 4")]
    [DataRow(2, +3, DisplayName = "2^ 3 = 8")]
    [DataRow(2, +4, DisplayName = "2^ 4 = 16")]
    [DataRow(2, +6, DisplayName = "2^ 6 = 64")]
    [DataRow(2, 42, DisplayName = "2^ (2 * 3 * [1 + 6]) = 2^30 = 4 398 046 511 104")]
    public void PowReturnExpectedValue(double x, int p)
    {
        var expected = Math.Pow(x, p);
        var actual = x.Pow(p);
        actual.AssertEquals(expected);

    }
    
    [TestMethod]
    public void PowOfGoldenRatio()
    {
        const double x = Consts.GoldenRatio;
        const int p = 12;

        var expected = Math.Pow(x, p);
        var actual = x.Pow(p);

        //var y = 1d;
        //for (var i = 0; i < p; i++)
        //    y *= x;

        actual.AssertEquals(expected, 1.72e-13);
    }
}
