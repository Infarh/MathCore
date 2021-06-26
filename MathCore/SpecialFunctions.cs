using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace MathCore
{
    public static partial class SpecialFunctions
    {
        /// <summary>Точность алгоритмов 5e-16</summary>
        public const double Eps = 5E-16;

        private const double __MaxRealNumber = 1E300;
        private const double __MinRealNumber = 1E-300;
        private const double __LogMinRealNumber = 690.77552789821368;
        private const double __LogMaxRealNumber = -690.77552789821368;

        [DST]
        public static int Fibonacci(int n)
        {
            var phi_n = Consts.GoldenRatio.Power(n);
            return (int)(Consts.sqrt_5_inv * (phi_n - Math.Cos(Consts.pi * n) / phi_n));
        }

        [DST]
        public static int Fibonacci(Complex z)
        {
            var phi_z = Consts.GoldenRatio.Power(z);
            return (int)(Consts.sqrt_5_inv * (phi_z - Complex.Trigonometry.Cos(Consts.pi * z) / phi_z));
        }

        public static int Fibonacci2(int n) => (int)(Consts.sqrt_5_inv * Consts.GoldenRatio.Power(n) + 0.5);

        private static readonly Lazy<ConcurrentDictionary<(int n, int k), ulong>> __BCR = new(() => new(), true);
        private static readonly Func<(int n, int k), ulong> __BCRCalculator = BCRCalculation;

        public static void BCRCacheClear() => __BCR.Value.Clear();
        public static IReadOnlyDictionary<(int n, int k), ulong> BCRCache => __BCR.Value;

        private static ulong BCRCalculation((int n, int k) v)
        {
            var (n, k) = v;
            if (k > n / 2) k = n - k;
            if (k == 0) return 1;
            if (k == 1) return (ulong)n;
            return BCR(n - 1, k) + BCR(n - 1, k - 1);
        }

        /// <summary>Биномиальный коэффициент</summary>
        public static ulong BCR(int n, int k) => __BCR.Value.GetOrAdd((n, k), __BCRCalculator);

        /// <summary>Биномиальный коэффициент</summary>
        /// <param name="n">Показатель степени бинома</param>
        /// <param name="k">Индекс коэффициента</param>
        /// <returns>Биномиальный коэффициент (n, k)</returns>
        public static long BinomialCoefficient(int n, int k)
        {
            if (n >= 0 && k > n || k <= 0) return 0;
            if (k > n / 2) k = n - k;
            if (k == 1) return n;

            if (n < 0) return BinomialCoefficient(-n + k - 1, k) * (k.IsOdd() ? -1 : 1);

            if (n > 20)
                return (long)(n.FactorialBigInt() / (k.FactorialBigInt() * (n - k).FactorialBigInt()));
            return n.Factorial() / (k.Factorial() * (n - k).Factorial());
        }

        /// <summary>Биномиальный коэффициент</summary>
        /// <param name="n">Показатель степени бинома (максимум 67)</param>
        /// <param name="k">Индекс коэффициента (максимум 76, 34)</param>
        /// <returns>Биномиальный коэффициент (n, k)</returns>
        public static ulong BinomialCoefficient(ulong n, ulong k)
        {
            if (k > n || k == 0) return 0;
            if (k > n / 2) k = n - k;
            if (k == 1) return n;

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

            return nn;
        }

        /// <summary>Биномиальный коэффициент</summary>
        /// <param name="n">Показатель степени бинома (максимум 67)</param>
        /// <param name="k">Индекс коэффициента (максимум 76, 34)</param>
        /// <returns>Биномиальный коэффициент (n, k)</returns>
        public static BigInt BinomialCoefficientBigInt(ulong n, ulong k)
        {
            if (k > n || k == 0) return 0;
            if (k > n / 2) k = n - k;
            if (k == 1) return n;

            BigInt nn = 1ul;
            BigInt kk = 1ul;
            BigInt i = 0ul;
            while (i < k)
            {
                var n0 = n - i;
                i++;
                var k0 = i;

                if (!Fraction.Simplify(ref nn, ref k0) || k0 > 1) kk *= k0;
                Fraction.Simplify(ref n0, ref kk);

                nn *= n0;
            }

            return nn;
        }

        /// <summary>Символ Кронекера δ(i,j) = 1 - если i = j, и = 0 - если i ≠ j</summary>
        /// <param name="i">Индекс</param><param name="j">Индекс</param>
        /// <returns>1 - если i = j, и = 0 - если i ≠ j</returns>
        [DST]
        public static int KroneckerDelta(int i, int j) => i == j ? 1 : 0;
    }
}