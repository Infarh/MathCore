using System.Numerics;

namespace ConsoleTest;

public static class TailRecursionTest
{
    //private static int[] Test(int[] X)
    //{
    //    if (X is [var a, .. var c, var b])
    //    {
    //        var result = new int[c.Length + 2];
    //        c.CopyTo(result, 1);
    //        (c[0], c[^1]) = (b, a);
    //        return c;
    //    }
    //}

    public static BigInteger Factorial(int n, BigInteger? product = null)
    {
        if (n < 2)
            return product ?? new(1);
        return Factorial(n - 1, n * (product ?? new(1)));
    }

    public static int Sum(int n, int acc = 0)
    {
        if (n <= 0)
            return acc;

        return Sum(n - 1, n + acc);
    }
}
