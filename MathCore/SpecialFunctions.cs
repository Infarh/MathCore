using System;
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
        public static int Fibonachi(int n)
        {
            var phi_n = Consts.GoldenRatio.Power(n);
            return (int)(Consts.sqrt_5_inv * (phi_n - Math.Cos(Consts.pi * n) / phi_n));
        }

        [DST]
        public static int Fibonachi(Complex z)
        {
            var phi_z = Consts.GoldenRatio.Power(z);
            return (int)(Consts.sqrt_5_inv * (phi_z - Complex.Trigonometry.Cos(Consts.pi * z) / phi_z));
        }

        public static int Fibonachi2(int n) => (int)(Consts.sqrt_5_inv * Consts.GoldenRatio.Power(n) + 0.5);

        /// <summary>Биномиальный коэффициент</summary>
        /// <param name="n">Показатель степени бинома</param>
        /// <param name="k">Индекс коэффициента</param>
        /// <returns>Биномиальный коэффициент (n, k)</returns>
        public static long BinomialCoefficient(int n, int k) =>
            n >= 0 && k > n || k < 0
                ? 0
                : n < 0
                    ? BinomialCoefficient(-n + k - 1, k) * (k.IsOdd() ? -1 : 1)
                    : n > 20
                        ? (n.FactorialBigInt() / (k.FactorialBigInt() * (n - k).FactorialBigInt())).LongValue()
                        : n.Factorial() / (k.Factorial() * (n - k).Factorial());

        /// <summary>Символ Кронекера δ(i,j) = 1 - если i = j, и = 0 - если i ≠ j</summary>
        /// <param name="i">Индекс</param><param name="j">Индекс</param>
        /// <returns>1 - если i = j, и = 0 - если i ≠ j</returns>
        [DST]
        public static int KroneckerDelta(int i, int j) => i == j ? 1 : 0;
    }
}