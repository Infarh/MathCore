namespace MathCore.Tests;

[TestClass]
public class SpecialFunctionsTests
{
    [TestMethod/*, Ignore*/]
    public void Fibonacci()
    {
        var expected_value_last = 1;
        var expected_value      = 1;

        MathCore.SpecialFunctions.Fibonacci(0).AssertEquals(0);
        MathCore.SpecialFunctions.Fibonacci(1).AssertEquals(1);
        MathCore.SpecialFunctions.Fibonacci(2).AssertEquals(1);

        var fibonacci_12 = MathCore.SpecialFunctions.Fibonacci(12);
        fibonacci_12.AssertEquals(144);

        for (var i = 0; i < 47 - 3; i++)
        {
            var tmp = expected_value_last;
            expected_value_last = expected_value;
            expected_value      = expected_value_last + tmp;

            var actual_value = MathCore.SpecialFunctions.Fibonacci(i + 3);

            actual_value.AssertEquals(expected_value, $"Число Фибоначчи при n = {i+3}");
        }
    }

    [TestMethod]
    public void Fibonacci2()
    {
        var expected_value_last = 1;
        var expected_value      = 1;

        MathCore.SpecialFunctions.Fibonacci2(0).AssertEquals(0);
        MathCore.SpecialFunctions.Fibonacci2(1).AssertEquals(1);
        MathCore.SpecialFunctions.Fibonacci2(2).AssertEquals(1);

        for (var i = 0; i < 47 - 3; i++)
        {
            var tmp = expected_value_last;
            expected_value_last = expected_value;
            expected_value      = expected_value_last + tmp;

            var actual_value = MathCore.SpecialFunctions.Fibonacci2(i + 3);
            actual_value.AssertEquals(expected_value, $"Число Фибоначчи при n = {i + 3}");
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