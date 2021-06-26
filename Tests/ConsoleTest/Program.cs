using System;
using System.Linq;
using System.Numerics;

using MathCore;

// ReSharper disable ConvertToUsingDeclaration
// ReSharper disable DoubleEquals

namespace ConsoleTest
{
    internal static class Program
    {
        private static RecursionResult<BigInteger> Factorial(int n, BigInteger product) =>
            n < 2
                ? TailRecursion.Return(product)
                : TailRecursion.Next(() => Factorial(n - 1, n * product));

        private static void Main()
        {
            ulong n = 68;
            ulong k = 34;

            var r = SpecialFunctions.BinomialCoefficientBigInt(n, k);

            if (k > n / 2) k = n - k;

            var b = SpecialFunctions.BinomialCoefficient(n, k);
            //var b = SpecialFunctions.BCR((int)n, (int)k);
            //var vv = SpecialFunctions.BCRCache.OrderBy(v => v.Key).ThenBy(v => v.Value).ToArray();
            // n! / (k!*(n-k)!)


            var nn = 1ul;
            var kk = 1ul;
            var i = 0ul;
            while (i < k)
            {
                var n0 = n - i;
                i++;
                var k0 = i;

                if (!Fraction.Simplify(ref nn, ref k0) || k0 > 1) kk *= k0;
                Fraction.Simplify(ref n0, ref kk);

                nn *= n0;
            }

            Console.WriteLine(nn);
            Console.WriteLine();

            //Console.WriteLine();
        }
    }

    public static class TailRecursion
    {
        public static T Execute<T>(Func<RecursionResult<T>> func)
        {
            while (true)
            {
                var recursion_result = func();
                if (recursion_result.IsFinalResult)
                    return recursion_result.Result;
                func = recursion_result.NextStep;
            }
        }

        public static RecursionResult<T> Return<T>(T result) => new(true, result, null);

        public static RecursionResult<T> Next<T>(Func<RecursionResult<T>> NextStep) => new(false, default, NextStep);
    }

    public class RecursionResult<T>
    {
        private readonly bool _IsFinalResult;
        private readonly T _Result;
        private readonly Func<RecursionResult<T>> _NextStep;
        internal RecursionResult(bool IsFinalResult, T result, Func<RecursionResult<T>> NextStep)
        {
            _IsFinalResult = IsFinalResult;
            _Result = result;
            _NextStep = NextStep;
        }

        public bool IsFinalResult => _IsFinalResult;
        public T Result => _Result;
        public Func<RecursionResult<T>> NextStep => _NextStep;
    }
}