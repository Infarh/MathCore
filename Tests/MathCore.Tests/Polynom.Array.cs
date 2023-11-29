using System.Collections;
using System.Diagnostics;

using MathCore.Tests.Infrastructure;

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

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_doubleX_doubleA_Throw_ArgumentNullException()
    {
        const double x = 0;
        double[] a = null;
        Polynom.Array.GetValue(x, a!);
    }

    [TestMethod]
    [DataRow(2d, new double[] { }, double.NaN, DisplayName = "NaN")]
    [DataRow(2d, new double[] { 1 }, 1, DisplayName = "p(2)=1")]
    [DataRow(2d, new double[] { 1, 2 }, 1 + 2d * 2, DisplayName = "p(2)=1+2x=5")]
    [DataRow(2d, new double[] { 1, 2, 3 }, 1 + 2d * 2 + 3d * 2 * 2, DisplayName = "p(2)=1+2x+3x2=1+4+12=17")]
    [DataRow(2d, new double[] { 1, 2, 3, 4 }, 1 + 2d * 2 + 3d * 2 * 2 + 4d * 2 * 2 * 2, DisplayName = "p(2)=1+2x+3x2+4x3=1+4+12+32=49")]
    public void GetValue_doubleX_doubleA(double X, double[] A, double ExpectedY)
    {
        var y = Polynom.Array.GetValue(X, A);
        Assert.That.Value(y).IsEqual(ExpectedY);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_ComplexX_doubleA_Throw_ArgumentNullException()
    {
        Complex x = 0;
        double[] a = null;
        Polynom.Array.GetValue(x, a!);
    }

    [TestMethod]
    [DataRow(2d, new double[] { }, double.NaN, DisplayName = "p(2+0j):NaN")]
    [DataRow(2d, new double[] { 1 }, 1, DisplayName = "p(2+0j)=1")]
    [DataRow(2d, new double[] { 1, 2 }, 1 + 2d * 2, DisplayName = "p(2+0j)=1+2x=5")]
    [DataRow(2d, new double[] { 1, 2, 3 }, 1 + 2d * 2 + 3d * 2 * 2, DisplayName = "p(2+0j)=1+2x+3x2=1+4+12=17")]
    [DataRow(2d, new double[] { 1, 2, 3, 4 }, 1 + 2d * 2 + 3d * 2 * 2 + 4d * 2 * 2 * 2, DisplayName = "p(2+0j)=1+2x+3x2+4x3=1+4+12+32=49")]
    public void GetValue_ComplexX_doubleA(double X, double[] A, double ExpectedY)
    {
        Complex Z = X;
        var y = Polynom.Array.GetValue(Z, A);
        Assert.That.Value(y).IsEqual(ExpectedY);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_duobleX_ComplexA_Throw_ArgumentNullException()
    {
        const double x = 0;
        Complex[] a = null;
        Polynom.Array.GetValue(x, a!);
    }

    [TestMethod]
    [DataRow(2d, new double[] { }, double.NaN, DisplayName = "p(2+0j):NaN")]
    [DataRow(2d, new double[] { 1 }, 1, DisplayName = "p(2+0j)=1")]
    [DataRow(2d, new double[] { 1, 2 }, 1 + 2d * 2, DisplayName = "p(2+0j)=1+2x=5")]
    [DataRow(2d, new double[] { 1, 2, 3 }, 1 + 2d * 2 + 3d * 2 * 2, DisplayName = "p(2+0j)=1+2x+3x2=1+4+12=17")]
    [DataRow(2d, new double[] { 1, 2, 3, 4 }, 1 + 2d * 2 + 3d * 2 * 2 + 4d * 2 * 2 * 2, DisplayName = "p(2+0j)=1+2x+3x2+4x3=1+4+12+32=49")]
    public void GetValue_doubleX_ComplexA(double X, double[] A, double ExpectedY)
    {
        var a = A.ToArray(v => (Complex)v);
        var y = Polynom.Array.GetValue(X, a);
        Assert.That.Value(y).IsEqual(ExpectedY);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_ComplexX_ComplexA_Throw_ArgumentNullException()
    {
        Complex x = 0;
        Complex[] a = null;
        Polynom.Array.GetValue(x, a!);
    }

    [TestMethod]
    [DataRow(2d, new double[] { }, double.NaN, DisplayName = "Complex p(2+0j):NaN")]
    [DataRow(2d, new double[] { 1 }, 1, DisplayName = "Complex p(2+0j)=1")]
    [DataRow(2d, new double[] { 1, 2 }, 1 + 2d * 2, DisplayName = "Complex p(2+0j)=1+2x=5")]
    [DataRow(2d, new double[] { 1, 2, 3 }, 1 + 2d * 2 + 3d * 2 * 2, DisplayName = "Complex p(2+0j)=1+2x+3x2=1+4+12=17")]
    [DataRow(2d, new double[] { 1, 2, 3, 4 }, 1 + 2d * 2 + 3d * 2 * 2 + 4d * 2 * 2 * 2, DisplayName = "Complex p(2+0j)=1+2x+3x2+4x3=1+4+12+32=49")]
    public void GetValue_ComplexX_ComplexA(double X, double[] A, double ExpectedY)
    {
        Complex Z = X;
        var a = A.ToArray(v => (Complex)v);
        var y = Polynom.Array.GetValue(Z, a);
        Assert.That.Value(y).IsEqual(ExpectedY);
    }

    /* ---------------------------------------------------------------------------- */

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_doubleX_doubleListA_Throw_ArgumentNullException()
    {
        const double x = 0;
        IList<double> a = null;
        Polynom.Array.GetValue(x, a!);
    }

    [TestMethod]
    [DataRow(2d, new double[] { }, double.NaN, DisplayName = "ListPolynom NaN")]
    [DataRow(2d, new double[] { 1 }, 1, DisplayName = "ListPolynom p(2)=1")]
    [DataRow(2d, new double[] { 1, 2 }, 1 + 2d * 2, DisplayName = "ListPolynom p(2)=1+2x=5")]
    [DataRow(2d, new double[] { 1, 2, 3 }, 1 + 2d * 2 + 3d * 2 * 2, DisplayName = "ListPolynom p(2)=1+2x+3x2=1+4+12=17")]
    [DataRow(2d, new double[] { 1, 2, 3, 4 }, 1 + 2d * 2 + 3d * 2 * 2 + 4d * 2 * 2 * 2, DisplayName = "ListPolynom p(2)=1+2x+3x2+4x3=1+4+12+32=49")]
    public void GetValue_doubleX_doubleListA(double X, double[] A, double ExpectedY)
    {
        var y = Polynom.Array.GetValue(X, (IList<double>)A);
        Assert.That.Value(y).IsEqual(ExpectedY);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_ComplexX_doubleListA_Throw_ArgumentNullException()
    {
        Complex x = 0;
        IList<double> a = null;
        Polynom.Array.GetValue(x, a!);
    }

    [TestMethod]
    [DataRow(2d, new double[] { }, double.NaN, DisplayName = "ListPolynom p(2+0j)=NaN")]
    [DataRow(2d, new double[] { 1 }, 1, DisplayName = "ListPolynom p(2+0j)=1")]
    [DataRow(2d, new double[] { 1, 2 }, 1 + 2d * 2, DisplayName = "p(2+0j)=1+2x=5")]
    [DataRow(2d, new double[] { 1, 2, 3 }, 1 + 2d * 2 + 3d * 2 * 2, DisplayName = "ListPolynom p(2+0j)=1+2x+3x2=1+4+12=17")]
    [DataRow(2d, new double[] { 1, 2, 3, 4 }, 1 + 2d * 2 + 3d * 2 * 2 + 4d * 2 * 2 * 2, DisplayName = "ListPolynom p(2+0j)=1+2x+3x2+4x3=1+4+12+32=49")]
    public void GetValue_ComplexX_doubleListA(double X, double[] A, double ExpectedY)
    {
        var y = Polynom.Array.GetValue((Complex)X, (IList<double>)A);
        Assert.That.Value(y).IsEqual(ExpectedY);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_doubleX_ComplexListA_Throw_ArgumentNullException()
    {
        const double x = 0;
        IList<Complex> a = null;
        Polynom.Array.GetValue(x, a!);
    }

    [TestMethod]
    [DataRow(2d, new double[] { }, double.NaN, DisplayName = "ListComplexPolynom NaN")]
    [DataRow(2d, new double[] { 1 }, 1, DisplayName = "ListComplexPolynom p(2)=1")]
    [DataRow(2d, new double[] { 1, 2 }, 1 + 2d * 2, DisplayName = "ListComplexPolynom p(2)=1+2x=5")]
    [DataRow(2d, new double[] { 1, 2, 3 }, 1 + 2d * 2 + 3d * 2 * 2, DisplayName = "ListComplexPolynom p(2)=1+2x+3x2=1+4+12=17")]
    [DataRow(2d, new double[] { 1, 2, 3, 4 }, 1 + 2d * 2 + 3d * 2 * 2 + 4d * 2 * 2 * 2, DisplayName = "ListComplexPolynom p(2)=1+2x+3x2+4x3=1+4+12+32=49")]
    public void GetValue_doubleX_ComplexListA(double X, double[] A, double ExpectedY)
    {
        var y = Polynom.Array.GetValue(X, A.ToList(v => (Complex)v));
        Assert.That.Value(y).IsEqual(ExpectedY);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_ComplexX_ComplexListA_Throw_ArgumentNullException()
    {
        Complex x = 0;
        IList<Complex> a = null;
        Polynom.Array.GetValue(x, a!);
    }

    [TestMethod]
    [DataRow(2d, new double[] { }, double.NaN, DisplayName = "ListComplexPolynom p(2+0j)=NaN")]
    [DataRow(2d, new double[] { 1 }, 1, DisplayName = "ListComplexPolynom p(2+0j)=1")]
    [DataRow(2d, new double[] { 1, 2 }, 1 + 2d * 2, DisplayName = "ListComplexPolynom p(2+0j)=1+2x=5")]
    [DataRow(2d, new double[] { 1, 2, 3 }, 1 + 2d * 2 + 3d * 2 * 2, DisplayName = "ListComplexPolynom p(2+0j)=1+2x+3x2=1+4+12=17")]
    [DataRow(2d, new double[] { 1, 2, 3, 4 }, 1 + 2d * 2 + 3d * 2 * 2 + 4d * 2 * 2 * 2, DisplayName = "ListComplexPolynom p(2+0j)=1+2x+3x2+4x3=1+4+12+32=49")]
    public void GetValue_ComplexX_ComplexListA(double X, double[] A, double ExpectedY)
    {
        var y = Polynom.Array.GetValue((Complex)X, A.ToList(v => (Complex)v));
        Assert.That.Value(y).IsEqual(ExpectedY);
    }

    /* ---------------------------------------------------------------------------- */

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_doubleX_doubleEnumerable_Throw_ArgumentNullException()
    {
        double x = 0;
        IEnumerable<double> A = null;
        Polynom.Array.GetValue(x, A!);
    }

    [TestMethod]
    [DataRow(2, new double[] { }, double.NaN)]
    [DataRow(2, new double[] { 1 }, 1)]
    [DataRow(2, new double[] { 1, 2 }, 5)]
    public void GetValue_doubleX_doubleEnumerable(double x, IEnumerable<double> A, double Y)
    {
        var y = Polynom.Array.GetValue(x, A);
        Assert.That.Value(y).IsEqual(Y);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_ComplexX_doubleEnumerable_Throw_ArgumentNullException()
    {
        Complex x = 0;
        IEnumerable<double> A = null;
        Polynom.Array.GetValue(x, A!);
    }

    [TestMethod]
    [DataRow(2, new double[] { }, double.NaN)]
    [DataRow(2, new double[] { 1 }, 1)]
    [DataRow(2, new double[] { 1, 2 }, 5)]
    public void GetValue_ComplexX_doubleEnumerable(double x, IEnumerable<double> A, double Y)
    {
        var y = Polynom.Array.GetValue((Complex)x, A);
        Assert.That.Value(y).IsEqual(Y);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_doubleX_ComplexEnumerable_Throw_ArgumentNullException()
    {
        double x = 0;
        IEnumerable<Complex> A = null;
        Polynom.Array.GetValue(x, A!);
    }

    [TestMethod]
    [DataRow(2, new double[] { }, double.NaN)]
    [DataRow(2, new double[] { 1 }, 1)]
    [DataRow(2, new double[] { 1, 2 }, 5)]
    public void GetValue_doubleX_ComplexEnumerable(double x, IEnumerable<double> A, double Y)
    {
        var y = Polynom.Array.GetValue(x, A.Select(v => (Complex)v));
        Assert.That.Value(y).IsEqual(Y);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_ComplexX_ComplexEnumerable_Throw_ArgumentNullException()
    {
        Complex x = 0;
        IEnumerable<Complex> A = null;
        Polynom.Array.GetValue(x, A!);
    }

    [TestMethod]
    [DataRow(2, new double[] { }, double.NaN)]
    [DataRow(2, new double[] { 1 }, 1)]
    [DataRow(2, new double[] { 1, 2 }, 5)]
    public void GetValue_ComplexX_ComplexEnumerable(double x, IEnumerable<double> A, double Y)
    {
        var y = Polynom.Array.GetValue((Complex)x, A.Select(v => (Complex)v));
        Assert.That.Value(y).IsEqual(Y);
    }

    /* ---------------------------------------------------------------------------- */

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_doubleX_doubleA_With_dy_throw_ArgumentNullException()
    {
        double x = 1;
        double[] a = null;

        Polynom.Array.GetValue(x, out _, a!);
    }

    [TestMethod]
    [DataRow(double.NaN, new double[] { 1 }, double.NaN, double.NaN)]
    [DataRow(1, new double[] { }, double.NaN, double.NaN)]
    [DataRow(0, new double[] { 1 }, 1, 0)]
    [DataRow(1, new double[] { 1, 3 }, 4, 3)]
    [DataRow(1, new double[] { 1, 3, 5 }, 9, 13)]
    [DataRow(1, new double[] { 1, 3, 5, 7, 9 }, 25, 70)]
    public void GetValue_doubleX_doubleA_With_dy(double x, double[] A, double Y, double dY)
    {
        var y = Polynom.Array.GetValue(x, out var dy, A);
        Assert.That.Value(y).IsEqual(Y);
        Assert.That.Value(dy).IsEqual(dY);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_ComplexX_doubleA_With_dy_throw_ArgumentNullException()
    {
        Complex x = 1;
        double[] a = null;

        Polynom.Array.GetValue(x, out _, a!);
    }

    [TestMethod]
    [DataRow(double.NaN, new double[] { 1 }, double.NaN, double.NaN)]
    [DataRow(1, new double[] { }, double.NaN, double.NaN)]
    [DataRow(0, new double[] { 1 }, 1, 0)]
    [DataRow(1, new double[] { 1, 3 }, 4, 3)]
    [DataRow(1, new double[] { 1, 3, 5 }, 9, 13)]
    [DataRow(1, new double[] { 1, 3, 5, 7, 9 }, 25, 70)]
    public void GetValue_ComplexX_doubleA_With_dy(double x, double[] A, double Y, double dY)
    {
        var y = Polynom.Array.GetValue((Complex)x, out var dy, A);
        Assert.That.Value(y).IsEqual(Y);
        Assert.That.Value(dy).IsEqual(dY);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_doubleX_ComplexA_With_dy_throw_ArgumentNullException()
    {
        double x = 1;
        Complex[] a = null;

        Polynom.Array.GetValue(x, out _, a!);
    }

    [TestMethod]
    [DataRow(double.NaN, new double[] { 1 }, double.NaN, double.NaN)]
    [DataRow(1, new double[] { }, double.NaN, double.NaN)]
    [DataRow(0, new double[] { 1 }, 1, 0)]
    [DataRow(1, new double[] { 1, 3 }, 4, 3)]
    [DataRow(1, new double[] { 1, 3, 5 }, 9, 13)]
    [DataRow(1, new double[] { 1, 3, 5, 7, 9 }, 25, 70)]
    public void GetValue_doubleX_ComplexA_With_dy(double x, double[] A, double Y, double dY)
    {
        var y = Polynom.Array.GetValue(x, out var dy, A.ToArray(v => (Complex)v));
        Assert.That.Value(y).IsEqual(Y);
        Assert.That.Value(dy).IsEqual(dY);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_ComplexX_ComplexA_With_dy_throw_ArgumentNullException()
    {
        Complex x = 1;
        Complex[] a = null;

        Polynom.Array.GetValue(x, out _, a!);
    }

    [TestMethod]
    [DataRow(double.NaN, new double[] { 1 }, double.NaN, double.NaN)]
    [DataRow(1, new double[] { }, double.NaN, double.NaN)]
    [DataRow(0, new double[] { 1 }, 1, 0)]
    [DataRow(1, new double[] { 1, 3 }, 4, 3)]
    [DataRow(1, new double[] { 1, 3, 5 }, 9, 13)]
    [DataRow(1, new double[] { 1, 3, 5, 7, 9 }, 25, 70)]
    public void GetValue_ComplexX_ComplexA_With_dy(double x, double[] A, double Y, double dY)
    {
        var y = Polynom.Array.GetValue((Complex)x, out var dy, A.ToArray(v => (Complex)v));
        Assert.That.Value(y).IsEqual(Y);
        Assert.That.Value(dy).IsEqual(dY);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_doubleX_doubleA_With_d2y_throw_ArgumentNullException()
    {
        double x = 1;
        double[] a = null;

        Polynom.Array.GetValue(x, out _, out _, a!);
    }

    [TestMethod]
    [DataRow(double.NaN, new double[] { 1 }, double.NaN, double.NaN, double.NaN)]
    [DataRow(1, new double[] { }, double.NaN, double.NaN, double.NaN)]
    [DataRow(0, new double[] { 1 }, 1, 0, 0)]
    [DataRow(1, new double[] { 1, 3 }, 4, 3, 0)]
    [DataRow(1, new double[] { 1, 3, 5 }, 9, 13, 10)]
    [DataRow(1, new double[] { 1, 3, 5, 7, 9 }, 25, 70, 160)]
    public void GetValue_doubleX_doubleA_With_d2y(double x, double[] A, double Y, double dY, double d2Y)
    {
        var y = Polynom.Array.GetValue(x, out var dy, out var d2y, A);
        Assert.That.Value(y).IsEqual(Y);
        Assert.That.Value(dy).IsEqual(dY);
        Assert.That.Value(d2y).IsEqual(d2Y);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_doubleX_ComplexA_With_d2y_throw_ArgumentNullException()
    {
        double x = 1;
        Complex[] a = null;

        Polynom.Array.GetValue(x, out _, out _, a!);
    }

    [TestMethod]
    [DataRow(double.NaN, new double[] { 1 }, double.NaN, double.NaN, double.NaN)]
    [DataRow(1, new double[] { }, double.NaN, double.NaN, double.NaN)]
    [DataRow(0, new double[] { 1 }, 1, 0, 0)]
    [DataRow(1, new double[] { 1, 3 }, 4, 3, 0)]
    [DataRow(1, new double[] { 1, 3, 5 }, 9, 13, 10)]
    [DataRow(1, new double[] { 1, 3, 5, 7, 9 }, 25, 70, 160)]
    public void GetValue_doubleX_ComplexA_With_d2y(double x, double[] A, double Y, double dY, double d2Y)
    {
        var y = Polynom.Array.GetValue(x, out var dy, out var d2y, A.ToArray(v => (Complex)v));
        Assert.That.Value(y).IsEqual(Y);
        Assert.That.Value(dy).IsEqual(dY);
        Assert.That.Value(d2y).IsEqual(d2Y);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_ComplexX_doubleA_With_d2y_throw_ArgumentNullException()
    {
        Complex x = 1;
        double[] a = null;

        Polynom.Array.GetValue(x, out _, out _, a!);
    }

    [TestMethod]
    [DataRow(double.NaN, new double[] { 1 }, double.NaN, double.NaN, double.NaN)]
    [DataRow(1, new double[] { }, double.NaN, double.NaN, double.NaN)]
    [DataRow(0, new double[] { 1 }, 1, 0, 0)]
    [DataRow(1, new double[] { 1, 3 }, 4, 3, 0)]
    [DataRow(1, new double[] { 1, 3, 5 }, 9, 13, 10)]
    [DataRow(1, new double[] { 1, 3, 5, 7, 9 }, 25, 70, 160)]
    public void GetValue_ComplexX_doubleA_With_d2y(double x, double[] A, double Y, double dY, double d2Y)
    {
        var y = Polynom.Array.GetValue((Complex)x, out var dy, out var d2y, A);
        Assert.That.Value(y).IsEqual(Y);
        Assert.That.Value(dy).IsEqual(dY);
        Assert.That.Value(d2y).IsEqual(d2Y);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetValue_ComplexX_ComplexA_With_d2y_throw_ArgumentNullException()
    {
        Complex x = 1;
        Complex[] a = null;

        Polynom.Array.GetValue(x, out _, out _, a!);
    }

    [TestMethod]
    [DataRow(double.NaN, new double[] { 1 }, double.NaN, double.NaN, double.NaN)]
    [DataRow(1, new double[] { }, double.NaN, double.NaN, double.NaN)]
    [DataRow(0, new double[] { 1 }, 1, 0, 0)]
    [DataRow(1, new double[] { 1, 3 }, 4, 3, 0)]
    [DataRow(1, new double[] { 1, 3, 5 }, 9, 13, 10)]
    [DataRow(1, new double[] { 1, 3, 5, 7, 9 }, 25, 70, 160)]
    public void GetValue_ComplexX_ComplexA_With_d2y(double x, double[] A, double Y, double dY, double d2Y)
    {
        var y = Polynom.Array.GetValue((Complex)x, out var dy, out var d2y, A.ToArray(v => (Complex)v));
        Assert.That.Value(y).IsEqual(Y);
        Assert.That.Value(dy).IsEqual(dY);
        Assert.That.Value(d2y).IsEqual(d2Y);
    }

    /* ---------------------------------------------------------------------------- */

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

    private static double GetCoefficientsValue(IEnumerable<double> A, double x)
    {
        var y = 0d;
        var xx = 1d;

        foreach (var a in A)
        {
            y += a * xx;
            xx *= x;
        }

        return y;
    }

    private static Complex GetCoefficientsValue(IEnumerable<double> A, Complex x)
    {
        var y = Complex.Zero;
        var xx = Complex.Real;

        foreach (var a in A)
        {
            y += a * xx;
            xx *= x;
        }

        return y;
    }

    private static Complex GetCoefficientsValue(IEnumerable<Complex> A, Complex x)
    {
        var y = Complex.Zero;
        var xx = Complex.Real;

        foreach (var a in A)
        {
            y += a * xx;
            xx *= x;
        }

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

    /* ---------------------------------------------------------------------------- */

    [TestMethod]
    public void GetCoefficients()
    {
        double[] roots = [3, 5, 7];

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
        double[] roots = [3, 5, 7];
        var coefficients = Polynom.Array.GetCoefficients((IEnumerable<double>)roots);

        Debug.WriteLine("Polynom({0}):\r\n{1}",
            coefficients.Length,
            string.Join("\r\n", coefficients.Select((a, i) => $"a[{i}] = {a}")));

        CheckRoots(roots, coefficients, 23, 17, 0, -17, -23);
    }

    [TestMethod]
    public void GetCoefficients_List()
    {
        double[] roots = [3, 5, 7];
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
        double[] roots = [3, 5, 7];

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

        Complex[] roots = [3, 5, 7];

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
        Complex[] roots = [3, 5, 7];

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

    /* ---------------------------------------------------------------------------- */

    [TestMethod]
    public void GetDifferential_doubleArrays_Throw_ArgumentNullException_p()
    {
        double[] p = null;
        double[] result = [];
        const int order = 1;
        var exception = Assert.ThrowsException<ArgumentNullException>(() => Polynom.Array.GetDifferential(p, result, order));
        Assert.That.Value(exception.ParamName).IsEqual("p");
    }

    [TestMethod]
    public void GetDifferential_doubleArrays_Throw_ArgumentNullException_Result()
    {
        double[] p = [];
        double[] result = null;
        const int order = 1;
        var exception = Assert.ThrowsException<ArgumentNullException>(() => Polynom.Array.GetDifferential(p, result, order));
        Assert.That.Value(exception.ParamName).IsEqual("Result");
    }

    [TestMethod]
    public void GetDifferential_doubleArrays_Order_0_Copy_Array()
    {
        double[] p = [1, 3, 5, 7, 9];
        var result = new double[p.Length];
        const int order = 0;

        var result_2 = Polynom.Array.GetDifferential(p, result, order);

        Assert.That.Collection(p).IsEqualTo(result);
        Assert.That.Value(result).IsReferenceEquals(result_2);
    }

    [TestMethod]
    public void GetDifferential_doubleArrays_Throw_ArgumentException_when_ResultLength_less_pLength()
    {
        double[] p = [1, 3, 5, 7, 9];
        var result = new double[p.Length - 2];
        const int order = 1;

        var exception = Assert.ThrowsException<ArgumentException>(() => Polynom.Array.GetDifferential(p, result, order));
        Assert.That.Value(exception.Data)
           .Where(e => e["p.Length"]).CheckEquals(p.Length)
           .Where(e => e["Result.Length"]).CheckEquals(result.Length)
           .Where(e => e["Order"]).CheckEquals(order);
    }

    [TestMethod]
    public void GetDifferential_doubleArrays_ClearResult_when_ResultLength_greater_pLength()
    {
        double[] p = [1, 3, 5, 7, 9];
        var result = Enumerable.Range(1, p.Length + 1).ToArray(v => (double)v);
        const int order = 1;

        Polynom.Array.GetDifferential(p, result, order);
        var empty_array = result[4..];
        Assert.That.Collection(empty_array).ElementsAreEqualTo(0);
    }

    [TestMethod]
    [DataRow(0, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 1, 3, 5, 7, 9, 12, 14 })]
    [DataRow(1, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 3, 10, 21, 36, 60, 84 })]
    [DataRow(2, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 10, 42, 108, 240, 420 })]
    [DataRow(3, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 42, 216, 720, 1680 })]
    [DataRow(4, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 216, 1440, 5040 })]
    [DataRow(5, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 1440, 10080 })]
    [DataRow(6, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 10080 })]
    [DataRow(7, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 0 })]
    [DataRow(8, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 0 })]
    public void GetDifferential_doubleArrays(int Order, double[] p, double[] dp)
    {
        var diff = Polynom.Array.GetDifferential(p, new double[Math.Max(1, p.Length - Order)], Order);
        Assert.That.Collection(diff).IsEqualTo(dp);
    }

    [TestMethod]
    public void GetDifferential_ComplexArrays_Throw_ArgumentNullException_p()
    {
        Complex[] p = null;
        Complex[] result = [];
        const int order = 1;
        var exception = Assert.ThrowsException<ArgumentNullException>(() => Polynom.Array.GetDifferential(p, result, order));
        Assert.That.Value(exception.ParamName).IsEqual("p");
    }

    [TestMethod]
    public void GetDifferential_ComplexArrays_Throw_ArgumentNullException_Result()
    {
        Complex[] p = [];
        Complex[] result = null;
        const int order = 1;
        var exception = Assert.ThrowsException<ArgumentNullException>(() => Polynom.Array.GetDifferential(p, result, order));
        Assert.That.Value(exception.ParamName).IsEqual("Result");
    }

    [TestMethod]
    public void GetDifferential_ComplexArrays_Order_0_Copy_Array()
    {
        Complex[] p = [1, 3, 5, 7, 9];
        var result = new Complex[p.Length];
        const int order = 0;

        var result_2 = Polynom.Array.GetDifferential(p, result, order);

        Assert.That.Collection(p).IsEqualTo(result);
        Assert.That.Value(result).IsReferenceEquals(result_2);
    }

    [TestMethod]
    public void GetDifferential_ComplexArrays_Throw_ArgumentException_when_ResultLength_less_pLength()
    {
        Complex[] p = [1, 3, 5, 7, 9];
        var result = new Complex[p.Length - 2];
        const int order = 1;

        var exception = Assert.ThrowsException<ArgumentException>(() => Polynom.Array.GetDifferential(p, result, order));
        Assert.That.Value(exception.Data)
           .Where(e => e["p.Length"]).CheckEquals(p.Length)
           .Where(e => e["Result.Length"]).CheckEquals(result.Length)
           .Where(e => e["Order"]).CheckEquals(order);
    }

    [TestMethod]
    public void GetDifferential_ComplexArrays_ClearResult_when_ResultLength_greater_pLength()
    {
        Complex[] p = [1, 3, 5, 7, 9];
        var result = Enumerable.Range(1, p.Length + 1).ToArray(v => (Complex)v);
        const int order = 1;

        Polynom.Array.GetDifferential(p, result, order);
        var empty_array = result[4..];
        Assert.That.Collection(empty_array).IsEqualTo(Enumerable.Repeat(Complex.Zero, empty_array.Length));
    }

    [TestMethod]
    [DataRow(0, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 1, 3, 5, 7, 9, 12, 14 })]
    [DataRow(1, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 3, 10, 21, 36, 60, 84 })]
    [DataRow(2, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 10, 42, 108, 240, 420 })]
    [DataRow(3, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 42, 216, 720, 1680 })]
    [DataRow(4, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 216, 1440, 5040 })]
    [DataRow(5, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 1440, 10080 })]
    [DataRow(6, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 10080 })]
    [DataRow(7, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 0 })]
    [DataRow(8, new double[] { 1, 3, 5, 7, 9, 12, 14 }, new double[] { 0 })]
    public void GetDifferential_ComplexArrays(int Order, double[] p, double[] dp)
    {
        var diff = Polynom.Array.GetDifferential(p.ToArray(v => (Complex)v), new Complex[Math.Max(1, p.Length - Order)], Order);
        Assert.That.Collection(diff).IsEqualTo(dp.Select(v => (Complex)v));
    }

    /* ---------------------------------------------------------------------------- */

    [TestMethod]
    public void GetDifferential_doubleArray_throw_ArgumentNullException_p()
    {
        double[] p = null;
        const int order = 1;

        var exception = Assert.ThrowsException<ArgumentNullException>(() => Polynom.Array.GetDifferential(p, order));
        Assert.That.Value(exception.ParamName).IsEqual("p");
    }

    [TestMethod]
    public void GetDifferential_doubleArray_throw_ArgumentOutOfRangeException_Order_greater_20()
    {
        double[] p = [1, 3, 5, 7, 9, 12, 14];
        const int order = 21;

        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() => Polynom.Array.GetDifferential(p, order));
        Assert.That.Value(exception)
           .Where(e => e.ParamName).CheckEquals("Order")
           .Where(e => e.ActualValue).CheckEquals(order);
    }

    [TestMethod]
    public void GetDifferential_doubleArray_Return_source_array_when_Order_0()
    {
        double[] p = [1, 3, 5, 7, 9, 12, 14];
        const int order = 0;

        var result = Polynom.Array.GetDifferential(p, order);

        Assert.That.Value(result).IsReferenceEquals(p);
    }

    [TestMethod]
    public void GetDifferential_doubleArray_Return_last_value_mult_OrderFactorial_when_Order_eq_SourceLength_without_1()
    {
        double[] p = [1, 3, 5, 7, 9, 12, 14];
        var order = p.Length - 1;

        var result = Polynom.Array.GetDifferential(p, order);
        Assert.That.Collection(result).ValuesAreEqualTo(p[^1] * order.Factorial());
    }

    [TestMethod]
    public void GetDifferential_doubleArray_Return_result_with_1_element_eq_0_When_Order_greater_orEqual_SourceLength()
    {
        double[] p = [1, 3, 5, 7, 9, 12, 14];
        var order = p.Length;

        var result = Polynom.Array.GetDifferential(p, order);
        Assert.That.Collection(result).ValuesAreEqualTo(0);

        var result2 = Polynom.Array.GetDifferential(p, order + 1);
        Assert.That.Collection(result2).ValuesAreEqualTo(0);
    }

    [TestMethod]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 0, new double[] { 1, 3, 5, 7, 9, 12, 14 }, DisplayName = "GetDifferential d0{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx0 = 1+3x+5x2+7x3+9x4+12x5+14x6")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 1, new double[] { 3, 10, 21, 36, 60, 84 }, DisplayName = "GetDifferential d1{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx1 = 3+10x+21x2+36x3+60x4+84x5")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 2, new double[] { 10, 42, 108, 240, 420 }, DisplayName = "GetDifferential d2{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx2 = 10+42x+108x2+240x3+420x4")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 3, new double[] { 42, 216, 720, 1680 }, DisplayName = "GetDifferential d3{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx3 = 42+216x+720x2+1680x3")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 4, new double[] { 216, 1440, 5040 }, DisplayName = "GetDifferential d4{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx4 = 216+1440x+5040x2")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 5, new double[] { 1440, 10080 }, DisplayName = "GetDifferential d5{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx5 = 1440+10080x")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 6, new double[] { 10080 }, DisplayName = "GetDifferential d6{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx6 = 10080")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 7, new double[] { 0 }, DisplayName = "GetDifferential d7{ 1+3x+5x2+7x3+9x4+12x5+14x6 }dx7 = 0")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 8, new double[] { 0 }, DisplayName = "GetDifferential d8{ 1+3x+5x2+7x3+9x4+12x5+14x6 }dx8 = 0")]
    public void GetDifferential_doubleArray(double[] a, int Order, double[] Expected)
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
    public void GetDifferential_ComplexArray_throw_ArgumentNullException_p()
    {
        Complex[] p = null;
        const int order = 1;

        var exception = Assert.ThrowsException<ArgumentNullException>(() => Polynom.Array.GetDifferential(p, order));
        Assert.That.Value(exception.ParamName).IsEqual("p");
    }

    [TestMethod]
    public void GetDifferential_ComplexArray_throw_ArgumentOutOfRangeException_Order_greater_20()
    {
        Complex[] p = [1, 3, 5, 7, 9, 12, 14];
        const int order = 21;

        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() => Polynom.Array.GetDifferential(p, order));
        Assert.That.Value(exception)
           .Where(e => e.ParamName).CheckEquals("Order")
           .Where(e => e.ActualValue).CheckEquals(order);
    }

    [TestMethod]
    public void GetDifferential_ComplexArray_Return_source_array_when_Order_0()
    {
        Complex[] p = [1, 3, 5, 7, 9, 12, 14];
        const int order = 0;

        var result = Polynom.Array.GetDifferential(p, order);

        Assert.That.Value(result).IsReferenceEquals(p);
    }

    [TestMethod]
    public void GetDifferential_ComplexArray_Return_last_value_mult_OrderFactorial_when_Order_eq_SourceLength_without_1()
    {
        Complex[] p = [1, 3, 5, 7, 9, 12, 14];
        var order = p.Length - 1;

        var result = Polynom.Array.GetDifferential(p, order);
        Assert.That.Collection(result).IsEqualTo(p[^1] * order.Factorial());
    }

    [TestMethod]
    public void GetDifferential_ComplexArray_Return_result_with_1_element_eq_0_When_Order_greater_orEqual_SourceLength()
    {
        Complex[] p = [1, 3, 5, 7, 9, 12, 14];
        var order = p.Length;

        var result = Polynom.Array.GetDifferential(p, order);
        Assert.That.Collection(result).IsEqualTo(0);

        var result2 = Polynom.Array.GetDifferential(p, order + 1);
        Assert.That.Collection(result2).IsEqualTo(0);
    }

    [TestMethod]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 0, new double[] { 1, 3, 5, 7, 9, 12, 14 }, DisplayName = "GetDifferential d0{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx0 = 1+3x+5x2+7x3+9x4+12x5+14x6")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 1, new double[] { 3, 10, 21, 36, 60, 84 }, DisplayName = "GetDifferential d1{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx1 = 3+10x+21x2+36x3+60x4+84x5")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 2, new double[] { 10, 42, 108, 240, 420 }, DisplayName = "GetDifferential d2{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx2 = 10+42x+108x2+240x3+420x4")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 3, new double[] { 42, 216, 720, 1680 }, DisplayName = "GetDifferential d3{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx3 = 42+216x+720x2+1680x3")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 4, new double[] { 216, 1440, 5040 }, DisplayName = "GetDifferential d4{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx4 = 216+1440x+5040x2")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 5, new double[] { 1440, 10080 }, DisplayName = "GetDifferential d5{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx5 = 1440+10080x")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 6, new double[] { 10080 }, DisplayName = "GetDifferential d6{ 1+3x+5x2+7x3+9x4+12x5+14x6 }/dx6 = 10080")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 7, new double[] { 0 }, DisplayName = "GetDifferential d7{ 1+3x+5x2+7x3+9x4+12x5+14x6 }dx7 = 0")]
    [DataRow(new double[] { 1, 3, 5, 7, 9, 12, 14 }, 8, new double[] { 0 }, DisplayName = "GetDifferential d8{ 1+3x+5x2+7x3+9x4+12x5+14x6 }dx8 = 0")]
    public void GetDifferential_ComplexArray(double[] a, int Order, double[] Expected)
    {
        var actual = Polynom.Array.GetDifferential(a.ToArray(v => (Complex)v), Order);

        try
        {
            Assert.That.Collection(actual).IsEqualTo(Expected.ToArray(v => (Complex)v));
        }
        catch (AssertFailedException)
        {
            Debug.WriteLine(actual.ToSeparatedStr(", "));
            throw;
        }
    }

    /* ---------------------------------------------------------------------------- */

    [TestMethod]
    [DataRow(0, 225961d, DisplayName = "DifferentialValue Order 0")]
    [DataRow(1, 157510d, DisplayName = "DifferentialValue Order 1")]
    [DataRow(2, 87916d, DisplayName = "DifferentialValue Order 2")]
    [DataRow(3, 36834d, DisplayName = "DifferentialValue Order 3")]
    [DataRow(3, 36834d, DisplayName = "DifferentialValue Order 4")]
    [DataRow(4, 10296, DisplayName = "DifferentialValue Order 5")]
    public void GetDifferentialValue(int Order, double ExpectedValue)
    {
        double[] a = [1, 3, 5, 7, 9, 12];
        const double x = 7;

        var value = Polynom.Array.GetDifferentialValue(x, a, Order);

        Assert.That.Value(value).IsEqual(ExpectedValue);
    }

    [TestMethod]
    public void Differential()
    {
        double[] a = [3, 5, 7, 9, 12];

        var actual_differential_1 = (double[])a.Clone();
        Polynom.Array.Differential(actual_differential_1, 1);

        double[] expected_differential_1 = [5, 14, 27, 48, 0];

        Assert.That.Collection(actual_differential_1).IsEqualTo(expected_differential_1);

        var actual_differential_3 = (double[])a.Clone();
        Polynom.Array.Differential(actual_differential_3, 3);

        double[] expected_differential_3 = [54, 288, 0, 0, 0];
        Assert.That.Collection(actual_differential_3).IsEqualTo(expected_differential_3);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetDifferential_ArgumentNullException() => Polynom.Array.GetDifferential(((double[])null!)!);

    [TestMethod]
    public void GetDifferential_Complex()
    {
        Complex[] a = [3, 5, 7];

        var actual_differential = Polynom.Array.GetDifferential(a);

        Complex[] expected_differential = [5, 14];
        CollectionAssert.AreEqual(expected_differential, actual_differential);

        a = [3, 5, 7, 9, 12];
        actual_differential = Polynom.Array.GetDifferential(a, 3);
        expected_differential = [54, 288];
        CollectionAssert.AreEqual(expected_differential, actual_differential);

        actual_differential = Polynom.Array.GetDifferential(a, 0);
        CollectionAssert.AreEqual(a, actual_differential);
        Assert.That.Value(actual_differential).IsReferenceEquals(a);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetDifferential_Complex_ArgumentNullException() => Polynom.Array.GetDifferential(((Complex[])null)!);

    [TestMethod]
    public void GetIntegral()
    {
        double[] a = [18, 30, 42];
        const int c = 17;

        var integral = Polynom.Array.GetIntegral(a, c);
        double[] expected_integral = [c, 18, 15, 14];

        CollectionAssert.AreEqual(expected_integral, integral);
    }

    [TestMethod]
    public void Integral()
    {
        double[] a = [18, 30, 42, 0];
        const int c = 17;

        Polynom.Array.Integral(a, c);
        double[] expected_integral = [c, 18, 15, 14];

        CollectionAssert.AreEqual(expected_integral, a);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetIntegral_Exceptions() => Polynom.Array.GetIntegral(((double[])null)!);

    [TestMethod]
    public void GetIntegral_Complex()
    {
        Complex[] a = [18, 30, 42];
        Complex c = 17;

        var integral = Polynom.Array.GetIntegral(a, c);
        Complex[] expected_integral = [c, 18, 15, 14];

        CollectionAssert.AreEqual(expected_integral, integral);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetIntegral_Complex_Exceptions() => Polynom.Array.GetIntegral(((Complex[])null)!);


    [TestMethod]
    public void Sum()
    {
        double[] p = [3, 5, 7];
        double[] q = [1, 2, 3, 4, 5];
        double[] expected_sum = [4, 7, 10, 4, 5];

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
        double[] p = [3, 5, 7];
        double[] q = [1, 2, 3, 4, 5];
        double[] expected_subtract = [2, 3, 4, -4, -5];

        var actual_subtract = Polynom.Array.Subtract(p, q);
        CollectionAssert.That.Collection(actual_subtract).IsEqualTo(expected_subtract);

        expected_subtract = [-2, -3, -4, 4, 5];
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
        double[] A = [3, 5, 7];
        double[] X = [0, 1, -1, 2, -2, 5, -5, 10, -10];

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
        double[] A = [];
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
        double[] A = [3, 5, 7];
        Complex[] X =
        [
            0, 1, -1, 2, -2, 5, -5,
            Complex.i, -Complex.i,
            new(5, 7), new(-5, 7), new(5, -7), new(-5, -7)
        ];

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
        double[] A = [];
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

    private static IEnumerable GetValue_ComplexX_ComplexArray_DataSource()
    {
        Complex[][] AA =
        [
            [1],
            [1, 3],
            [1, 3, 5],
            [1, 3, 5, 7, 9],
        ];

        Complex[] X =
        [
            0, 1, -1, 2, -2, 5, -5,
            Complex.i, -Complex.i,
            new(5, 7), new(-5, 7), new(5, -7), new(-5, -7)
        ];

        foreach (var A in AA)
            foreach (var x in X)
                yield return (A, x, Y: GetCoefficientsValue(A, x));
    }

    //[DataTestMethod, DynamicData()] //https://www.meziantou.net/mstest-v2-data-tests.htm
    [TestMethod]
    [TestData(MethodSourceName = nameof(GetValue_ComplexX_ComplexArray_DataSource))]
    //[DataRowSource(MethodSourceName = nameof(GetValue_ComplexX_ComplexArray_DataSource))]
    public void GetValue_ComplexX_ComplexArray(Complex[] A, Complex x, Complex Y)
    {
        var y = Polynom.Array.GetValue(x, A);
        Assert.That.Value(y).IsEqual(Y);
    }

    [TestMethod]
    public void GetValue_ComplexX_ComplexArray_with_ZeroLength_ResultNaN()
    {
        Complex[] A = [];
        Complex x = 0;

        var actual = Polynom.Array.GetValue(x, A);

        Assert.That.Value(actual).IsEqual(double.NaN);
    }

    [TestMethod]
    public void GetValue_ComplexX_ComplexArray_with_null_thrown_ArgumentNullException_Z()
    {
        var exception = Assert.ThrowsException<ArgumentNullException>(() => Polynom.Array.GetValue((Complex)0, ((Complex[])null)!));
        Assert.That.Value(exception.ParamName).IsEqual("A");
    }

    [TestMethod]
    public void Multiply()
    {
        double[] a = [3, 5, 7]; // p1(x) = 3 + 5x + 7x^2
        double[] b = [5, 4, 8]; // p2(x) = 5 + 4x + 8x^2

        // p3(x) = (5 + 4x + 8x^2)           * 3
        //       +     (5  + 4x + 8x^2)      * 5x
        //       +          (5  + 4x + 8x^2) * 7x^2

        // p3(x) = 15 + 12x + 24x^2
        //       +      25x + 20x^2 + 40x^3
        //       +            35x^2 + 28x^3 + 56x^4

        double[] c_expect = [15, 37, 79, 68, 56];

        var c = Polynom.Array.Multiply(a, b);

        const double x0 = 2;

        var y_a = Polynom.Array.GetValue(x0, a);
        var y_b = Polynom.Array.GetValue(x0, b);
        var y_c = Polynom.Array.GetValue(x0, c);

        var y_c1 = y_a * y_b;

        CollectionAssert.AreEqual(c_expect, c);
    }
}