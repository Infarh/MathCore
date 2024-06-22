using System.Diagnostics;

namespace MathCore.Tests.Extensions.Numerics;

[TestClass]
public class DoubleExtensionsTests
{
    [TestMethod]
    public void PowFast05_Float()
    {
        const double p = -0.3;

        var error2 = 0d;
        var n = 0;
        for (var x = 0.0f; x <= 100; x += 0.1f)
        {
            var y0 = Math.Pow(x, p);
            var y1 = x.PowFast(p);

            var delta = y0 - y1;
            error2 += delta.Pow2();
            n++;
        }

        var error = (error2 / n).Sqrt();

        error.ToDebug();
        //error.AssertLessThan(0.0013);
    }

    [TestMethod]
    public void SqrtFast_Float()
    {
        var error2 = 0d;
        var n = 0;
        for (var x = 0.0f; x <= 4; x += 0.1f)
        {
            var y0 = Math.Sqrt(x);
            var y1 = x.SqrtFast();

            var delta = y0 - y1;
            error2 += delta.Pow2();
            n++;
        }

        var error = (error2 / n).Sqrt();

        error.ToDebug();
        //error.AssertLessThan(0.0013);
    }

    [TestMethod]
    public void SqrtInvFast_Double()
    {
        var error2 = 0d;
        var n = 0;
        for (var x = 0.1; x <= 4; x += 0.1)
        {
            var y0 = 1 / Math.Sqrt(x);
            var y1 = x.SqrtInvFast();

            var delta = y0 - y1;
            error2 += delta.Pow2();
            n++;
        }

        var error = (error2/ n).Sqrt();

        //error.ToDebug();
        error.AssertLessThan(0.0013);
    }

    [TestMethod]
    public void SqrtInvFast2_Double()
    {
        var error2 = 0d;
        var n = 0;
        for (var x = 0.1; x <= 4; x += 0.1)
        {
            var y0 = 1 / Math.Sqrt(x);
            var y1 = x.SqrtInvFast2();

            var delta = y0 - y1;
            error2 += delta.Pow2();
            n++;
        }

        var error = (error2/ n).Sqrt();

        //error.ToDebug();
        error.AssertLessThan(2.84E-6);
    }

    [TestMethod]
    public void SqrtInvFast3_Double()
    {
        var error2 = 0d;
        var n = 0;
        for (var x = 0.1; x <= 4; x += 0.1)
        {
            var y0 = 1 / Math.Sqrt(x);
            var y1 = x.SqrtInvFast3();

            var delta = y0 - y1;
            error2 += delta.Pow2();
            n++;
        }

        var error = (error2/ n).Sqrt();

        error.ToDebug();
        error.AssertLessThan(1.62E-11);
    }

    [TestMethod]
    [DataRow(1, 0.0013, DisplayName = "SqrtInvFastK1_Double err2 < 0.0013")]
    [DataRow(2, 2.9E-6, DisplayName = "SqrtInvFastK2_Double err2 < 2.9E-6")]
    [DataRow(3, 3.7E-8, DisplayName = "SqrtInvFastK3_Double err2 < 1.7E-11")]
    public void SqrtInvFastK_Double(int K, double ExpectedError)
    {
        var error2 = 0d;
        var n = 0;
        for (var x = 0.1; x <= 4; x += 0.1)
        {
            var y0 = 1 / Math.Sqrt(x);
            var y1 = x.SqrtInvFast(K);

            var delta = y0 - y1;
            error2 += delta.Pow2();
            n++;
        }

        var error = (error2 / n).Sqrt();

        error.ToDebug();
        error.AssertLessThan(ExpectedError);
    }

    [TestMethod]
    public void SqrtInvFast_Float()
    {
        var error2 = 0d;
        var n = 0;
        for (var x = 0.1f; x <= 4; x += 0.1f)
        {
            var y0 = 1 / Math.Sqrt(x);
            var y1 = x.SqrtInvFast();

            var delta = y0 - y1;
            error2 += delta.Pow2();
            n++;
        }

        var error = (error2/ n).Sqrt();

        //error.ToDebug();
        error.AssertLessThan(0.0013);
    }

    [TestMethod]
    public void SqrtInvFast2_Float()
    {
        var error2 = 0d;
        var n = 0;
        for (var x = 0.1f; x <= 4; x += 0.1f)
        {
            var y0 = 1 / Math.Sqrt(x);
            var y1 = x.SqrtInvFast2();

            var delta = y0 - y1;
            error2 += delta.Pow2();
            n++;
        }

        var error = (error2/ n).Sqrt();

        //error.ToDebug();
        error.AssertLessThan(2.9E-6);
    }

    [TestMethod]
    public void SqrtInvFast3_Float()
    {
        var error2 = 0d;
        var n = 0;
        for (var x = 0.1f; x <= 4; x += 0.1f)
        {
            var y0 = 1 / Math.Sqrt(x);
            var y1 = x.SqrtInvFast3();

            var delta = y0 - y1;
            error2 += delta.Pow2();
            n++;
        }

        var error = (error2/ n).Sqrt();

        error.ToDebug();
        error.AssertLessThan(3.7E-8);
    }

    [TestMethod]
    [DataRow(1, 0.0013, DisplayName = "SqrtInvFastK1_Float err2 < 0.0013")]
    [DataRow(2, 2.9E-6, DisplayName = "SqrtInvFastK2_Float err2 < 2.9E-6")]
    [DataRow(3, 3.7E-8, DisplayName = "SqrtInvFastK3_Float err2 < 3.7E-8")]
    public void SqrtInvFastK_Float(int K, double ExpectedError)
    {
        var error2 = 0d;
        var n = 0;
        for (var x = 0.1f; x <= 4; x += 0.1f)
        {
            var y0 = 1 / Math.Sqrt(x);
            var y1 = x.SqrtInvFast(K);

            var delta = y0 - y1;
            error2 += delta.Pow2();
            n++;
        }

        var error = (error2 / n).Sqrt();

        //error.ToDebug();
        error.AssertLessThan(ExpectedError);
    }

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

    [TestMethod]
    public void RoundAdaptive_Value0_Num3()
    {
        const double value = 0;
        const int digits = 3;

        var actual_value = value.RoundAdaptive(digits);
    }
}
