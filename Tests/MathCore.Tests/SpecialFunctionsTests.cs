namespace MathCore.Tests;

[TestClass]
public class SpecialFunctionsTests
{
    [TestMethod]
    public void Fibonacci()
    {
        var expected_value_last = 1;
        var expected_value      = 1;

        Assert.That.Value(MathCore.SpecialFunctions.Fibonacci(0)).IsEqual(0);
        Assert.That.Value(MathCore.SpecialFunctions.Fibonacci(1)).IsEqual(1);
        Assert.That.Value(MathCore.SpecialFunctions.Fibonacci(2)).IsEqual(1);

        for (var i = 0; i < 10; i++)
        {
            var tmp = expected_value_last;
            expected_value_last = expected_value;
            expected_value      = expected_value_last + tmp;

            var actual_value = MathCore.SpecialFunctions.Fibonacci(i + 3);
            Assert.That.Value(actual_value).IsEqual(expected_value);
        }
    }

    [TestMethod]
    public void Fibonacci2()
    {
        var expected_value_last = 1;
        var expected_value      = 1;

        Assert.That.Value(MathCore.SpecialFunctions.Fibonacci2(0)).IsEqual(0);
        Assert.That.Value(MathCore.SpecialFunctions.Fibonacci2(1)).IsEqual(1);
        Assert.That.Value(MathCore.SpecialFunctions.Fibonacci2(2)).IsEqual(1);

        for (var i = 0; i < 10; i++)
        {
            var tmp = expected_value_last;
            expected_value_last = expected_value;
            expected_value      = expected_value_last + tmp;

            var actual_value = MathCore.SpecialFunctions.Fibonacci2(i + 3);
            Assert.That.Value(actual_value).IsEqual(expected_value);
        }
    }

    [TestMethod]
    public void BinomialCoefficient_int()
    {
        const int n        = 4;
        long[]    expected = { 1, 4, 6, 4, 1 };

        for (var k = 0; k <= n; k++)
        {
            var actual = MathCore.SpecialFunctions.BinomialCoefficient(n, k);
            Assert.That.Value(actual).IsEqual(expected[k]);
        }
    }

    [TestMethod]
    public void BinomialCoefficient_int_k_Less_zero_equal_0()
    {
        const int n      = 4;
        var       actual = MathCore.SpecialFunctions.BinomialCoefficient(n, -1);
        Assert.That.Value(actual).IsEqual(0);
    }

    [TestMethod]
    public void BinomialCoefficient_int_k_Greater_n_equal_0()
    {
        const int n      = 4;
        var       actual = MathCore.SpecialFunctions.BinomialCoefficient(n, n + 1);
        Assert.That.Value(actual).IsEqual(0);
    }
}