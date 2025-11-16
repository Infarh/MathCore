using System.Diagnostics.CodeAnalysis;

// ReSharper disable UnusedMember.Global

namespace MathCore.Tests;

[TestClass]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class PolynomTests : UnitTest
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

    /// <summary>Тест конструктора</summary>
    [TestMethod, Priority(1), Timeout(100, CooperativeCancellation = true), Description("Тест конструктора")]
    public void PolynomConstructor_Test()
    {
        var N       = GetRNDInt(5, 15);
        var A       = GetRNDDoubleArray(N);
        var polynom = new Polynom(A);
        Assert.AreEqual(A.Length - 1, polynom.Power,
            $"Степень полинома {polynom.Power} не соответствует числу коэффициентов при степенях {A.Length} - 1");

        for (var i = 0; i < N; i++)
            Assert.AreEqual(A[i], polynom[i], $"Коэффициент {polynom[i]} при степени {i} не соответствует исходному {A[i]}");

        polynom = new(new List<double>(A));
        Assert.AreEqual(A.Length - 1, polynom.Power,
            $"Степень полинома {polynom.Power} не соответствует числу коэффициентов при степенях {A.Length} - 1");
        for (var i = 0; i < N; i++)
            Assert.AreEqual(A[i], polynom[i], $"Коэффициент {polynom[i]} при степени {i} не соответствует исходному {A[i]}");
    }

    /// <summary>Тест значения полинома</summary>
    [TestMethod, Priority(1), Timeout(100, CooperativeCancellation = true), Description("Тест значения")]
    public void Value_Test()
    {
        double[] A = [3, 5, 7];
        var      p = new Polynom(A);
        Assert.AreEqual(3, p.Value(0));
        Assert.AreEqual(15, p.Value(1));
        Assert.AreEqual(41, p.Value(2));
        Assert.AreEqual(81, p.Value(3));

        var N = GetRNDInt(5, 15);
        A = GetRNDDoubleArray(N);
        p = new(A);
        var X = GetRNDDoubleArray(GetRNDInt(5, 15));

        double P(double x)
        {
            var result = 0.0;
            for (var i = 0; i < N; i++)
                result += A[i] * Math.Pow(x, i);
            return result;
        }

        foreach (var x in X)
            Assert.That.Value(p.Value(x)).IsEqual(P(x), 2.0e-15);
    }

    /// <summary>Тест клонирования</summary>
    [TestMethod, Description("Тест клонирования")]
    public void Clone_Test()
    {
        var expected = new Polynom(3, 5, 7);
        var actual   = expected.Clone();
        Assert.AreEqual(expected, actual);
        Assert.IsFalse(ReferenceEquals(actual, expected));
        Assert.IsFalse(ReferenceEquals(expected.Coefficients, actual.Coefficients));
        CollectionAssert.AreEqual(expected.Coefficients, actual.Coefficients);
    }

    /// <summary>Тестирование метода определения равенства полиномов</summary>
    [TestMethod, Priority(1), Description("Тестирование метода определения равенства полиномов")]
    public void Equals_Test()
    {
        var P = new Polynom(1, 3, 5);
        var Q = new Polynom(1, 3, 5);
        var Z = new Polynom(1, 3, 5, 7);

        Assert.IsTrue(P.Equals(P), $"Полином {P} не равен сам себе {P}");
        Assert.IsTrue(P.Equals(Q), $"Полином {P} не равен идентичному полиному {Q}");
        Assert.IsFalse(P.Equals(null), $"Полином {P} равен null");
        Assert.IsFalse(P.Equals(Z), $"Полином {P} равен неравному ему полиному {Z}");

        P = new(GetRNDDoubleArray(GetRNDInt(5, 15), -5, 5));
        Q = new(P.Coefficients);

        Assert.IsTrue(P.Equals(Q), $"Случайный полином {P} не равен полиному {Q}, составленному из его коэффициентов");
        Assert.IsFalse(P.Equals(Z), $"Случайный полином {P} равен неравному ему полиному {Z}");
    }

    /// <summary>Тестирование метода определения равенства полиномов</summary>
    [TestMethod, Priority(1), Description("Тестирование метода определения равенства полинома объекту")]
    public void EqualsTest1()
    {
        var P = new Polynom(1, 3, 5);
        var Q = new Polynom(1, 3, 5);
        var Z = new Polynom(1, 3, 5, 7);

        Assert.IsTrue(P.Equals((object)P), $"Полином {P} не равен сам себе {P}");
        Assert.IsTrue(P.Equals((object)Q), $"Полином {P} не равен идентичному полиному {Q}");
        Assert.IsFalse(P.Equals((object?)null), $"Полином {P} равен null");
        Assert.IsFalse(P.Equals(new object()), $"Полином {P} равен null");
        Assert.IsFalse(P.Equals(5), $"Полином {P} равен целому числу");
        // ReSharper disable once SuspiciousTypeConversion.Global
        Assert.IsFalse(P.Equals("Test"), $"Полином {P} равен строке");
        Assert.IsFalse(P.Equals((object)Z), $"Полином {P} равен неравному ему полиному {Z}");

        P = new(GetRNDDoubleArray(GetRNDInt(5, 15), -5, 5));
        Q = new(P.Coefficients);

        Assert.IsTrue(P.Equals((object)Q), $"Случайный полином {P} не равен полиному {Q}, составленному из его коэффициентов");
        Assert.IsFalse(P.Equals((object)Z), $"Случайный полином {P} равен неравному ему полиному {Z}");
    }

    private Polynom GetRandomPolynom(int Power = -1) => new(GetRNDDoubleArray(Power <= -1 ? GetRNDInt(5, 15) : Power + 1, -5, 5));

    /// <summary>Тест оператора сложения полиномов</summary>
    [TestMethod]
    public void op_Addition_Test()
    {
        var P = new Polynom(3, 5, 7);
        var Q = new Polynom(9, 8, 15, 23);
        var Z = new Polynom(12, 13, 22, 23);

        Assert.IsTrue((P + Q).Equals(Z), $"Сумма детерминированных тестовых полиномов рассчитана неверно: {P} + {Q} == {Z}");

        P = GetRandomPolynom();
        Q = GetRandomPolynom();

        var X = GetRNDDoubleArray(GetRNDInt(5, 15), -5, 5);

        var y_p = X.Select(P.Value);
        var y_q = X.Select(Q.Value);

        Z = P + Q;

        var y_actual   = X.Select(Z.Value);
        var y_expected = y_p.Zip(y_q, (a, b) => a + b);

        var Y = y_actual.Zip(y_expected, (actual, expected) => new { actual, expected });
        Y.Foreach(v => Assert.IsLessThanOrEqualTo(4.45e-14, Math.Abs(v.expected - v.actual) / v.expected, $"Относительная точность между ожидаемым {v.expected} и полученным {v.actual} значениями составила {Math.Abs(v.expected - v.actual) / v.expected}"));
    }

    ///// <summary>
    ///// A test for op_Implicit
    ///// </summary>
    //[TestMethod]
    //public void op_Implicit_Test()
    //{
    //    Polynom P = null; // TODO: Initialize to an appropriate value
    //    Converter<double, double> expected = null; // TODO: Initialize to an appropriate value
    //    Converter<double, double> actual;
    //    actual = P;
    //    Assert.AreEqual(expected, actual);
    //    Assert.Inconclusive("Verify the correctness of this test method.");
    //}

    ///// <summary>
    ///// A test for Coefficients
    ///// </summary>
    //[TestMethod]
    //public void Coefficients_Test()
    //{
    //    double[] a = null; // TODO: Initialize to an appropriate value
    //    Polynom target = new Polynom(a); // TODO: Initialize to an appropriate value
    //    double[] expected = null; // TODO: Initialize to an appropriate value
    //    double[] actual;
    //    target.Coefficients = expected;
    //    actual = target.Coefficients;
    //    Assert.AreEqual(expected, actual);
    //    Assert.Inconclusive("Verify the correctness of this test method.");
    //}

    ///// <summary>
    ///// A test for Item
    ///// </summary>
    //[TestMethod]
    //public void Item_Test()
    //{
    //    double[] a = null; // TODO: Initialize to an appropriate value
    //    Polynom target = new Polynom(a); // TODO: Initialize to an appropriate value
    //    int i = 0; // TODO: Initialize to an appropriate value
    //    double expected = 0F; // TODO: Initialize to an appropriate value
    //    double actual;
    //    target[i] = expected;
    //    actual = target[i];
    //    Assert.AreEqual(expected, actual);
    //    Assert.Inconclusive("Verify the correctness of this test method.");
    //}

    ///// <summary>
    ///// A test for GetPower
    ///// </summary>
    //[TestMethod]
    //public void Power_Test()
    //{
    //    double[] a = null; // TODO: Initialize to an appropriate value
    //    Polynom target = new Polynom(a); // TODO: Initialize to an appropriate value
    //    int actual;
    //    actual = target.GetPower;
    //    Assert.Inconclusive("Verify the correctness of this test method.");
    //}

    /// <summary>Тестирование унарного оператора отрицания -P</summary>
    [TestMethod, Priority(1), Description("Тестирование унарного оператора отрицания -P")]
    public void op_UnaryNegation_Test()
    {
        void Test(Polynom PP, Polynom QQ)
        {
            Assert.AreEqual(PP.Power, QQ.Power, $"Порядки полиномов P = {PP} и Q = {QQ} не совпадают!");
            PP.Zip(QQ, (p, q) => new { p, q })
               .Foreach((z, i) => Assert.AreEqual(z.p, -z.q, $"Для полинома P = {PP} значение {i} коэффициента P[{i}] = {z.p} не равно инвертированному значению полинома Q = {QQ} : -Q[{i}] = {-z.q}"));
            Assert.IsTrue(PP.Equals(-QQ), $"Не выполняется равенство P == Q, если Q = -P для P = {PP}, Q = {QQ}");

            GetRNDDoubleArray(GetRNDInt(5, 15), -5, 5).Foreach(x => Assert.AreEqual(0, PP.Value(x) + QQ.Value(x), 1e-16));
        }

        var P = new Polynom(3, 5, 7);
        Test(P, -P);

        P = GetRandomPolynom();
        Test(P, -P);
    }

    /// <summary>Тестирование оператора разности двух полиномов</summary>
    [TestMethod, Priority(1), Description("Тестирование оператора разности двух полиномов")]
    public void op_Subtraction_Test()
    {
        void Test(Polynom P, Polynom Q)
        {
            var Z = P - Q;
            Assert.AreEqual(Math.Max(P.Power, Q.Power), Z.Power, $"Степень полинома разности {Z} = {Z.Power} не равна максимуму из степеней уменьшаемого {P} = {P.Power} и вычитаемого {Q} = {Q.Power} полиномов");
            GetRNDDoubleArray(1000, -50, 50)
               .Select(x => new { yP       = P.Value(x), yQ      = Q.Value(x), yZ = Z.Value(x) })
               .Select(v => new { expected = v.yP - v.yQ, actual = v.yZ })
               .Foreach(v => Assert.AreEqual(0, (v.expected - v.actual) / v.expected, 1e-10));
        }

        var p = new Polynom(3, 5, 7);
        var q = new Polynom(2, 3, 5);
        Assert.AreEqual(new(1, 2, 2), p - q);
        Test(p, q);
        Test(GetRandomPolynom(4), GetRandomPolynom(5));
        Test(GetRandomPolynom(), GetRandomPolynom());
    }

    /// <summary>
    /// A test for op_Multiply
    /// </summary>
    [TestMethod]
    public void op_Multiply_Test()
    {
        var p = Polynom.Random(5);
        var q = Polynom.Random(7);
        var z = p * q;

        var x = GetRNDDouble();

        Assert.AreEqual(p.Value(x) * q.Value(x), z.Value(x), 0.0001);
    }

    [TestMethod]
    public void GetCoefficients_Test()
    {
        double[] x0         = [1, 3, 5, 7];
        double[] expected_a = [105, -176, 86, -16, 1];

        var a = Polynom.Array.GetCoefficients(x0);
        CollectionAssert.AreEqual(expected_a, a);
    }
}