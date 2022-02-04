namespace MathCore.Algorithms.Polynoms;

public static class BinomialRatio
{
    public static ulong Value(int k, int n)
    {
        var result = 1ul;
        
        for (var i = n - k + 1; i <= n; i++)
            result *= (ulong)i;

        for (var i = 2; i <= k; i++)
            result /= (ulong)i;

        return result;
    }
}