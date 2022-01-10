using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable InconsistentNaming
// ReSharper disable ExpressionIsAlwaysNull
// ReSharper disable ArrangeMethodOrOperatorBody
// ReSharper disable EmptyArray
// ReSharper disable UseArrayEmptyMethod

namespace MathCore.Tests;

[TestClass]
public class PolynomArray
{

    public TestContext TestContext { get; set; }

    #region Additional test attributes

    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize]
    //public static void MyClassInitialize(TestContext testContext) { }

    //[ClassCleanup]
    //public static void MyClassCleanup() { }

    //[TestInitialize]
    //public void MyTestInitialize() { }

    //[TestCleanup]
    //public void MyTestCleanup() { }

    #endregion


    [TestMethod]
    public void GetValue()
    {
        double[] a = { 3, 5, 7 };

        Assert.AreEqual(3, Polynom.Array.GetValue(0, a));
        Assert.AreEqual(a.Sum(), Polynom.Array.GetValue(1, a));
        Assert.AreEqual(41, Polynom.Array.GetValue(2, a));
        Assert.AreEqual(81, Polynom.Array.GetValue(3, a));

        Assert.AreEqual(double.NaN, Polynom.Array.GetValue(10, Array.Empty<double>()));
    }

    [TestMethod]
    public void GetValue_Enumerable()
    {
        var a = Enumerable.Range(0, 3).Select(x => x * 2 + 3d).ToArray();

        Assert.That.Value(Polynom.Array.GetValue(0, a)).IsEqual(3);
        Assert.AreEqual(a.Sum(), Polynom.Array.GetValue(1, a));
        Assert.AreEqual(41, Polynom.Array.GetValue(2, a));
        Assert.AreEqual(81, Polynom.Array.GetValue(3, a));
    }

    [TestMethod]
    public void GetValue_Enumerable_Exceptions()
    {
        var exception = Assert.ThrowsException<ArgumentNullException>(() => Polynom.Array.GetValue(0, ((IEnumerable<double>)null!)!));
        Assert.That.Value(exception.ParamName).IsEqual("A");
    }

    [TestMethod]
    public void GetComplexValue()
    {
        double[] a = { 3, 5, 7 };

        Assert.AreEqual(3, Polynom.Array.GetValue(new Complex(), a));
        Assert.AreEqual(a.Sum(), Polynom.Array.GetValue(new Complex(1), a));
        Assert.AreEqual(41, Polynom.Array.GetValue(new Complex(2), a));
        Assert.AreEqual(81, Polynom.Array.GetValue(new Complex(3), a));

        Assert.AreEqual(Complex.NaN, Polynom.Array.GetValue(new Complex(), Array.Empty<double>()));
    }

    [TestMethod]
    public void GetValue_Complex_Enumerable()
    {
        var a = Enumerable.Range(0, 3).Select(x => x * 2 + 3d);

        Assert.AreEqual(3, Polynom.Array.GetValue(new Complex(), a));
        Assert.AreEqual(a.Sum(), Polynom.Array.GetValue(new Complex(1), a));
        Assert.AreEqual(41, Polynom.Array.GetValue(new Complex(2), a));
        Assert.AreEqual(81, Polynom.Array.GetValue(new Complex(3), a));
    }

    [TestMethod]
    public void GetValue_Complex_Enumerable_Exceptions()
    {
        var exception = Assert.ThrowsException<ArgumentNullException>(() => Polynom.Array.GetValue(new Complex(), ((IEnumerable<double>)null)!));
        Assert.That.Value(exception.ParamName).IsEqual("A");
    }

    private static double GetRootsValue(IEnumerable<double> X0, double x)
    {
        var y = 1d;

        foreach (var x0 in X0)
            y *= x - x0;

        return y;
    }

    private static Complex GetRootsValue(IEnumerable<Complex> X0, Complex x)
    {
        var y = Complex.Real;

        foreach (var x0 in X0)
            y *= x - x0;

        return y;
    }

    private static Complex GetCoefficientsValue(IEnumerable<double> A, Complex x)
    {
        var y = Complex.Zero;

        foreach (var a in A.Reverse())
            y = y * x + a;

        return y;
    }

    private static double GetCoefficientsValue(IEnumerable<double> A, double x)
    {
        var y = 0d;

        foreach (var a in A.Reverse())
            y = y * x + a;

        return y;
    }

    private static Complex GetCoefficientsValue(IEnumerable<Complex> A, Complex x)
    {
        var y = Complex.Zero;

        foreach (var a in A.Reverse())
            y = y * x + a;

        return y;
    }

    private static void CheckRoots(double[] Roots, IList<double> a, params double[] X)
    {
        foreach (var x in X)
        {
            var y = Polynom.Array.GetValue(x, a);
            var y0 = GetRootsValue(Roots, x);
            Assert.That.Value(y).IsEqual(y0);
        }
    }

    private static void CheckRoots(Complex[] Roots, IList<Complex> a, params Complex[] X)
    {
        foreach (var x in X)
        {
            var y = Polynom.Array.GetValue(x, a);
            var y0 = GetRootsValue(Roots, x);
            Assert.That.Value(y).IsEqual(y0);
        }
    }

    private static void CheckCoefficients(double[] a, params double[] X)
    {
        foreach (var x in X)
        {
            var actual = Polynom.Array.GetValue(x, a);
            var expected = GetCoefficientsValue(a, x);

            Assert.That.Value(actual).IsEqual(expected);
        }
    }

    private static void CheckCoefficients(Complex[] a, params Complex[] X)
    {
        foreach (var x in X)
        {
            var actual = Polynom.Array.GetValue(x, a);
            var expected = GetCoefficientsValue(a, x);

            Assert.That.Value(actual).IsEqual(expected);
        }
    }

    [TestMethod]
    public void GetCoefficients()
    {
        double[] roots = { 3, 5, 7 };

        var coefficients = Polynom.Array.GetCoefficients(roots);

        Debug.WriteLine("Polynom({0}):\r\n{1}",
            coefficients.Length,
            string.Join("\r\n", coefficients.Select((a, i) => $"a[{i}] = {a}")));

        CheckRoots(roots, coefficients, 23, 17, 0, -17, -23);

        coefficients = Polynom.Array.GetCoefficients(5);
        Assert.That.Collection(coefficients).IsEqualTo(new[] { -5d, 1 });
    }

    [TestMethod]
    public void GetCoefficients_Enum()
    {
        double[] roots = { 3, 5, 7 };
        var coefficients = Polynom.Array.GetCoefficients((IEnumerable<double>)roots);

        Debug.WriteLine("Polynom({0}):\r\n{1}",
            coefficients.Length,
            string.Join("\r\n", coefficients.Select((a, i) => $"a[{i}] = {a}")));

        CheckRoots(roots, coefficients, 23, 17, 0, -17, -23);
    }

    [TestMethod]
    public void GetCoefficients_List()
    {
        double[] roots = { 3, 5, 7 };
        List<double> coefficients = new();
        Polynom.Array.GetCoefficients(roots, coefficients);

        Debug.WriteLine("Polynom({0}):\r\n{1}",
            coefficients.Count,
            string.Join("\r\n", coefficients.Select((a, i) => $"a[{i}] = {a}")));

        CheckRoots(roots, coefficients, 23, 17, 0, -17, -23);
    }

    [TestMethod]
    public void GetCoefficients_Exceptions()
    {
        var null_exception = Assert.ThrowsException<ArgumentNullException>(() => Polynom.Array.GetCoefficients(((double[])null)!));
        Assert.That.Value(null_exception).Where(e => e.ParamName).CheckEquals("Root");

        var empty_exception = Assert.ThrowsException<ArgumentException>(() => Polynom.Array.GetCoefficients(Array.Empty<double>()));
        Assert.That.Value(empty_exception).Where(e => e.ParamName).CheckEquals("Root");
    }

    [TestMethod]
    public void GetCoefficientsInverted()
    {
        double[] roots = { 3, 5, 7 };

        var coefficients = Polynom.Array.GetCoefficients(roots);                   // -105 71 -15 1
        var coefficients_inverted = Polynom.Array.GetCoefficientsInverted(roots);
        Array.Reverse(coefficients);
        Assert.That.Collection(coefficients_inverted).IsEqualTo(coefficients);

        coefficients = Polynom.Array.GetCoefficientsInverted(5);
        Assert.That.Collection(coefficients).IsEqualTo(new[] { 1, -5d });
    }

    [TestMethod]
    public void GetCoefficientsInverted_Exceptions()
    {
        Exception exception = null;
        try
        {
            double[] roots = null;
            Polynom.Array.GetCoefficientsInverted(roots!);
        }
        catch (Exception e)
        {
            exception = e;
        }
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(ArgumentNullException));
        Assert.AreEqual("Root", ((ArgumentNullException)exception).ParamName);

        exception = null;
        try
        {
            var roots = Array.Empty<double>();
            Polynom.Array.GetCoefficientsInverted(roots);
        }
        catch (Exception e)
        {
            exception = e;
        }
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(ArgumentException));
        Assert.AreEqual("Root", ((ArgumentException)exception).ParamName);
    }

    [TestMethod]
    public void GetCoefficients_Complex()
    {
        //Complex[] roots = { 3, 5, 7 };

        //var coefficients = Polynom.Array.GetCoefficients(roots);

        //void Check(Complex x) => Assert.AreEqual(GetTestPolynomValue(roots, x), Polynom.Array.GetValue(x, coefficients));
        //Check(23);
        //Check(17);
        //Check(0);
        //Check(-17);
        //Check(-23);

        //coefficients = Polynom.Array.GetCoefficients(new Complex(5));
        //Assert.That.Collection(coefficients).IsEqualTo(-5, 1);
        ////CollectionAssert.AreEqual(new[] { new Complex(-5), 1 }, coefficients);

        Complex[] roots = { 3, 5, 7 };

        var coefficients = Polynom.Array.GetCoefficients(roots);

        Debug.WriteLine("Polynom({0}):\r\n{1}",
            coefficients.Length,
            string.Join("\r\n", coefficients.Select((a, i) => $"a[{i}] = {a}")));

        CheckRoots(roots, coefficients, 23, 17, 0, -17, -23);

        coefficients = Polynom.Array.GetCoefficients(new Complex(5));
        Assert.That.Collection(coefficients).IsEqualTo(-5, 1);
    }

    [TestMethod]
    public void GetCoefficients_Complex_Exceptions()
    {
        Assert.That.Method((Complex[])null, Polynom.Array.GetCoefficients)
           .Throw<ArgumentNullException>()
           .Where(e => e.ParamName).IsEqual("Root");

        Assert.That.Method(Array.Empty<Complex>(), Polynom.Array.GetCoefficients)
           .Throw<ArgumentException>()
           .Where(e => e.ParamName).IsEqual("Root");
    }

    [TestMethod]
    public void GetCoefficientsInverted_Complex()
    {
        Complex[] roots = { 3, 5, 7 };

        var coefficients = Polynom.Array.GetCoefficients(roots);                   // -105 71 -15 1
        var coefficients_inverted = Polynom.Array.GetCoefficientsInverted(roots);
        Array.Reverse(coefficients);
        CollectionAssert.AreEqual(coefficients, coefficients_inverted);

        coefficients = Polynom.Array.GetCoefficientsInverted(new Complex(5));
        CollectionAssert.That.Collection(coefficients).IsEqualTo(new[] { 1, new Complex(-5) });
    }

    [TestMethod]
    public void GetCoefficientsInverted_Complex_Exceptions()
    {
        Assert.ThrowsException<ArgumentNullException>(() => Polynom.Array.GetCoefficientsInverted(((Complex[])null)!));
        Assert.ThrowsException<ArgumentException>(() => Polynom.Array.GetCoefficientsInverted(Array.Empty<Complex>()));
    }

    [TestMethod]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 0, new double[] { 1, 3, 5, 7, 9, 12, 14 }, DisplayName = "d0{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx0 = 1+3x+5x2+7x3+9x4+12x5+14x6")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 1, new double[] { 3, 10, 21, 36, 60, 84 }, DisplayName = "d1{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx1 = 3+10x+21x2+36x3+60x4+84x5")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 2, new double[] { 10, 42, 108, 240, 420 }, DisplayName = "d2{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx2 = 10+42x+108x2+240x3+420x4")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 3, new double[] { 42, 216, 720, 1680 }, DisplayName = "d3{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx3 = 42+216x+720x2+1680x3")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 4, new double[] { 216, 1440, 5040 }, DisplayName = "d4{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx4 = 216+1440x+5040x2")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 5, new double[] { 1440, 10080 }, DisplayName = "d5{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx5 = 1440+10080x")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 6, new double[] { 10080 }, DisplayName = "d6{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx6 = 10080")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 7, new double[] { 0 }, DisplayName = "d7{ 1+3x+5x2+7x3+9x4+12x5+14x6 }dx7 = 0")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 8, new double[] { 0 }, DisplayName = "d8{ 1+3x+5x2+7x3+9x4+12x5+14x6 }dx8 = 0")]
    public void GetDifferential(double[] a, int Order, double[] Expected)
    {
        var actual = Polynom.Array.GetDifferential(a, Order);

        try
        {
            Assert.That.Collection(actual).IsEqualTo(Expected);
        }
        catch (AssertFailedException)
        {
            Debug.WriteLine(actual.ToSeparatedStr(", "));
            throw;
        }
    }

    [TestMethod]
    public void GetDifferentialValue()
    {
        double[] a = { 1, 3, 5, 7, 9, 12 };
        const int order = 3;
        const double x = 7;

        var value = Polynom.Array.GetDifferentialValue(x, a, order);

        Assert.That.Value(value).IsEqual(36834);
    }

    [TestMethod]
    public void Differential()
    {
        double[] a = { 3, 5, 7, 9, 12 };

        var actual_differential_1 = (double[])a.Clone();
        Polynom.Array.Differential(actual_differential_1, 1);

        double[] expected_differential_1 = { 5, 14, 27, 48, 0 };

        Assert.That.Collection(actual_differential_1).IsEqualTo(expected_differential_1);

        var actual_differential_3 = (double[])a.Clone();
        Polynom.Array.Differential(actual_differential_3, 3);

        double[] expected_differential_3 = { 54, 288, 0, 0, 0 };
        Assert.That.Collection(actual_differential_3).IsEqualTo(expected_differential_3);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetDifferential_ArgumentNullException() => Polynom.Array.GetDifferential(((double[])null!)!);

    [TestMethod]
    public void GetDifferential_Complex()
    {
        Complex[] a = { 3, 5, 7 };

        var actual_differential = Polynom.Array.GetDifferential(a);

        Complex[] expected_differential = { 5, 14 };
        CollectionAssert.AreEqual(expected_differential, actual_differential);

        a = new Complex[] { 3, 5, 7, 9, 12 };
        actual_differential = Polynom.Array.GetDifferential(a, 3);
        expected_differential = new Complex[] { 54, 288 };
        CollectionAssert.AreEqual(expected_differential, actual_differential);

        actual_differential = Polynom.Array.GetDifferential(a, 0);
        CollectionAssert.AreEqual(a, actual_differential);
        Assert.IsFalse(ReferenceEquals(a, actual_differential));
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetDifferential_Complex_ArgumentNullException() => Polynom.Array.GetDifferential(((Complex[])null)!);

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetDifferential_Complex_ArgumentOutOfRangeException() => Polynom.Array.GetDifferential(new Complex[5], -1);

    [TestMethod]
    public void GetIntegral()
    {
        double[] a = { 18, 30, 42 };
        const int c = 17;

        var integral = Polynom.Array.GetIntegral(a, c);
        double[] expected_integral = { c, 18, 15, 14 };

        CollectionAssert.AreEqual(expected_integral, integral);
    }

    [TestMethod]
    public void Integral()
    {
        double[] a = { 18, 30, 42, 0 };
        const int c = 17;

        Polynom.Array.Integral(a, c);
        double[] expected_integral = { c, 18, 15, 14 };

        CollectionAssert.AreEqual(expected_integral, a);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetIntegral_Exceptions() => Polynom.Array.GetIntegral(((double[])null)!);

    [TestMethod]
    public void GetIntegral_Complex()
    {
        Complex[] a = { 18, 30, 42 };
        Complex c = 17;

        var integral = Polynom.Array.GetIntegral(a, c);
        Complex[] expected_integral = { c, 18, 15, 14 };

        CollectionAssert.AreEqual(expected_integral, integral);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetIntegral_Complex_Exceptions() => Polynom.Array.GetIntegral(((Complex[])null)!);


    [TestMethod]
    public void Sum()
    {
        double[] p = { 3, 5, 7 };
        double[] q = { 1, 2, 3, 4, 5 };
        double[] expected_sum = { 4, 7, 10, 4, 5 };

        var actual_sum = Polynom.Array.Sum(p, q);
        CollectionAssert.AreEqual(expected_sum, actual_sum);

        actual_sum = Polynom.Array.Sum(q, p);
        CollectionAssert.AreEqual(expected_sum, actual_sum);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Sum_ArgumentNullException_p() => Polynom.Array.Sum(null!, new double[5]);

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Sum_ArgumentNullException_q() => Polynom.Array.Sum(new double[5], null!);

    [TestMethod]
    public void subtract()
    {
        double[] p = { 3, 5, 7 };
        double[] q = { 1, 2, 3, 4, 5 };
        double[] expected_subtract = { 2, 3, 4, -4, -5 };

        var actual_subtract = Polynom.Array.Subtract(p, q);
        CollectionAssert.That.Collection(actual_subtract).IsEqualTo(expected_subtract);

        expected_subtract = new double[] { -2, -3, -4, 4, 5 };
        actual_subtract = Polynom.Array.Subtract(q, p);
        CollectionAssert.That.Collection(actual_subtract).IsEqualTo(expected_subtract);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void subtract_ArgumentNullException_p() => Polynom.Array.Subtract(null!, new double[5]);

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void subtract_ArgumentNullException_q() => Polynom.Array.Subtract(new double[5], null!);

    /* --------------------------------------------------------------------------------------------- */

    [TestMethod]
    public void GetValue_DoubleX_DoubleArray()
    {
        double[] A = { 3, 5, 7 };
        double[] X = { 0, 1, -1, 2, -2, 5, -5, 10, -10 };

        foreach (var x in X)
        {
            var actual = Polynom.Array.GetValue(x, A);

            var expected = GetCoefficientsValue(A, x);
            Assert.That.Value(actual).IsEqual(expected);
        }
    }

    [TestMethod]
    public void GetValue_DoubleX_DoubleArray_with_ZeroLength_ResultNaN()
    {
        double[] A = { };
        double x = 0;

        var actual = Polynom.Array.GetValue(x, A);

        Assert.That.Value(actual).IsEqual(double.NaN);
    }

    [TestMethod]
    public void GetValue_DoubleX_DoubleArray_with_null_thrown_ArgumentNullException_A()
    {
        var exception = Assert.ThrowsException<ArgumentNullException>(() => Polynom.Array.GetValue(0d, ((double[])null)!));
        Assert.That.Value(exception.ParamName).IsEqual("A");
    }

    [TestMethod]
    public void GetValue_ComplexX_DoubleArray()
    {
        double[] A = { 3, 5, 7 };
        Complex[] X =
        {
            0, 1, -1, 2, -2, 5, -5,
            Complex.i, -Complex.i,
            new(5, 7), new(-5, 7), new(5, -7), new(-5, -7)
        };

        foreach (var x in X)
        {
            var actual = Polynom.Array.GetValue(x, A);

            var expected = GetCoefficientsValue(A, x);
            Assert.That.Value(actual).IsEqual(expected);
        }
    }

    [TestMethod]
    public void GetValue_ComplexX_DoubleArray_with_ZeroLength_ResultNaN()
    {
        double[] A = { };
        Complex x = 0;

        var actual = Polynom.Array.GetValue(x, A);

        Assert.That.Value(actual).IsEqual(double.NaN);
    }

    [TestMethod]
    public void GetValue_ComplexX_DoubleArray_with_null_thrown_ArgumentNullException_A()
    {
        var exception = Assert.ThrowsException<ArgumentNullException>(() => Polynom.Array.GetValue((Complex)0, ((double[])null)!));
        Assert.That.Value(exception.ParamName).IsEqual("A");
    }

    [TestMethod]
    public void GetValue_ComplexX_ComplexArray()
    {
        Complex[] A = { 3, 5, 7 };
        Complex[] X =
        {
            0, 1, -1, 2, -2, 5, -5,
            Complex.i, -Complex.i,
            new(5, 7), new(-5, 7), new(5, -7), new(-5, -7)
        };

        foreach (var x in X)
        {
            var actual = Polynom.Array.GetValue(x, A);

            var expected = GetCoefficientsValue(A, x);
            Assert.That.Value(actual).IsEqual(expected);
        }
    }

    [TestMethod]
    public void GetValue_ComplexX_ComplexArray_with_ZeroLength_ResultNaN()
    {
        Complex[] A = { };
        Complex x = 0;

        var actual = Polynom.Array.GetValue(x, A);

        Assert.That.Value(actual).IsEqual(double.NaN);
    }

    [TestMethod]
    public void GetValue_ComplexX_ComplexArray_with_null_thrown_ArgumentNullException_Z()
    {
        var exception = Assert.ThrowsException<ArgumentNullException>(() => Polynom.Array.GetValue((Complex)0, ((Complex[])null)!));
        Assert.That.Value(exception.ParamName).IsEqual("Z");
    }
}