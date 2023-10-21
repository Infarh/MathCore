using Function = System.Func<int, int>;

namespace MathCore.Algorithms.Functions;

public static class YCombinator
{
    public static void Test()
    {
        var factorial = Y<int>(f => n => n == 0 ? 1 : n * f(n - 1));

        var y = factorial(5);
        Console.WriteLine(y);
    }

    private static Func<T, T> Y<T>(Func<Func<T, T>, Func<T, T>> F) => t => F(Y(F))(t);

    private static Function Y(Func<Function, Function> F) => t => F(Y(F))(t);
}
