using System.Numerics;

namespace MathCore
{
    public static class BigIntegerExtentions
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