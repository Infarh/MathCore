// ReSharper disable once CheckNamespace
namespace System.Numerics
{
    public static class BigIntegerExtensions
    {
        public static BigInteger Factorial(BigInteger n)
        {
            BigInteger result = 1;
            for(BigInteger i = 1; i <= n; i++)
                result *= i;
            return result;
        }
    }
}