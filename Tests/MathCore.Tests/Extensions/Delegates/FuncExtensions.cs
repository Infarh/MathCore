using System.Diagnostics;

namespace MathCore.Tests.Extensions.Delegates;

[TestClass]
public class FuncExtensionsIntegrals
{
    private class TestIntegralFunc(Func<double, double> f)
    {
        private int _CallCount;

        public int CallCount => _CallCount;

        public double GetValue(double x)
        {
            _CallCount++;
            return f(x);
        }

        public Func<double, double> GetFunc() => GetValue;
    }

    private static double TestFunc(double x) => 3 * x * x;
    private const double __ExpectedIntegralOfTestFunc_0_1 = 1d;

    private static TestIntegralFunc GetTestFunc(Func<double, double> f) => new(f);

    private static TestIntegralFunc GetTestFunc() => GetTestFunc(TestFunc);

    private static void PrintTestResult(double Result, double Expected, int CallCount)
    {
        Debug.WriteLine("Результат: {0}", Result);
        Debug.WriteLine("Ожидалось: {0}", Expected);
        Debug.WriteLine("Ошибка: {0}", Expected - Result);
        Debug.WriteLine("Квадрат ошибки: {0}", (Expected - Result).Pow2());
        Debug.WriteLine("Относительная ошибка: {0} ({0:p2})", (Expected - Result) / Expected);
        Debug.WriteLine("Число вызовов: {0}", CallCount);
    }

    private static void PrintTestResult(double Result, double Expected, int CallCount, double Eps)
    {
        Debug.WriteLine("Результат: {0}", Result);
        Debug.WriteLine("Ожидалось: {0}", Expected);

        var error_delta = Expected - Result;
        Debug.WriteLine("Ошибка: {0}", error_delta);
        Debug.WriteLine("Квадрат ошибки: {0}", error_delta.Pow2());
        Debug.WriteLine("Относительная ошибка: {0} ({0:p2})", error_delta / Expected);
        Debug.WriteLine("Ожидаемая ошибка: {0}", Eps);

        var expected_error_delta = Eps - Math.Abs(error_delta);
        Debug.WriteLine("Разница между ожидаемой и полученной ошибкой: {0} (запас {1:p2})", 
            expected_error_delta, expected_error_delta / Eps);

        Debug.WriteLine(
            expected_error_delta < 0
                ? "Полученная ошибка превысила ожидаемую. Тест провален!"
                : "Полученная ошибка меньше ожидаемой. Тест пройден успешно!");

        Debug.WriteLine("Число вызовов: {0}", CallCount);
    }

    [TestMethod]
    public void GetIntegralValue_Adaptive()
    {
        var test = GetTestFunc();

        var func = test.GetFunc();

        var integral_value = func.GetIntegralValue_Adaptive(0, 1);
         
        PrintTestResult(integral_value, 1, test.CallCount);

        Assert.That.Value(integral_value).IsEqual(__ExpectedIntegralOfTestFunc_0_1);
    }

    [TestMethod]
    public void GetIntegralValue_AdaptiveTrap()
    {
        var test = GetTestFunc();

        var func = test.GetFunc();

        const double eps            = 1e-7;
        var          integral_value = func.GetIntegralValue_AdaptiveTrap(0, 1, eps);

        PrintTestResult(integral_value, 1, test.CallCount, eps);

        Assert.That.Value(integral_value).IsEqual(__ExpectedIntegralOfTestFunc_0_1, eps);

    }
}