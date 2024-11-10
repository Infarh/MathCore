using System.Collections.Concurrent;

namespace MathCore;

public static partial class SpecialFunctions
{
    /// <summary>Точность алгоритмов 5e-16</summary>
    public const double Eps = 5E-16;

    private const double __MaxRealNumber = 1E300;
    private const double __MinRealNumber = 1E-300;
    private const double __LogMinRealNumber = 690.77552789821368;
    private const double __LogMaxRealNumber = -690.77552789821368;

    /// <summary>Вычисляет <paramref name="n"/>-ое число Фибоначчи</summary>
    /// <param name="n">Номер числа Фибоначчи, которое нужно вычислить</param>
    /// <returns><paramref name="n"/>-ое число Фибоначчи</returns>
    /// <remarks>
    ///     Формула вычисления: <c>F(n) = (phi^n - cos(πn) / phi^n) / √5</c>
    ///     где <c>phi = (1 + √5) / 2</c> - золотое сечение.
    /// </remarks>
    [DST]
    public static int Fibonacci(int n)
    {
        var phi_n = Consts.GoldenRatio.Pow(n);
        return (int)Math.Round(Consts.sqrt_5_inv * (phi_n - Math.Cos(Consts.pi * n) / phi_n));
        // phn = [(√5 + 1)/2]^n
        // ( phn - cos(pi * n) / phn ) / √5
    }

    /// <summary>Вычисляет число Фибоначчи для комплексного аргумента.</summary>
    /// <param name="z">Комплексное число, для которого вычисляется число Фибоначчи.</param>
    /// <returns>Число Фибоначчи, округлённое до ближайшего целого.</returns>
    /// <remarks>
    ///     Формула вычисления: <c>F(z) = (phi^z - cos(πz) / phi^z) / √5</c>
    ///     где <c>phi = (1 + √5) / 2</c> - золотое сечение.
    /// </remarks>
    [DST]
    public static int Fibonacci(Complex z)
    {
        var phi_z = Consts.GoldenRatio.Pow(z);
        return (int)(Consts.sqrt_5_inv * (phi_z - Complex.Trigonometry.Cos(Consts.pi * z) / phi_z));
    }

    public static int Fibonacci2(int n) => (int)(Consts.sqrt_5_inv * Consts.GoldenRatio.Pow(n) + 0.5);

    private static readonly Lazy<ConcurrentDictionary<(int n, int k), ulong>> __BCR = new(() => new(), true);
    private static readonly Func<(int n, int k), ulong> __BCRCalculator = BCRCalculation;

    public static void BCRCacheClear() => __BCR.Value.Clear();
    public static IReadOnlyDictionary<(int n, int k), ulong> BCRCache => __BCR.Value;

    /// <summary>Вычисление биномиального коэффициента (n, k) по рекуррентной формуле</summary>
    /// <param name="v">Кортеж (n, k) - показатель степени бинома и индекс коэффициента соответственно.</param>
    /// <returns>Биномиальный коэффициент (n, k).</returns>
    /// <remarks>
    ///     Формула вычисления: <c>C(n, k) = C(n - 1, k) + C(n - 1, k - 1)</c>
    ///     где <c>C(n, k)</c> - биномиальный коэффициент (n, k).
    /// </remarks>
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
        if (n >= 0 && k > n || k < 0) return 0;
        if (k > n / 2) k = n - k;
        return k switch
        {
            0 => 1,
            1 => n,
            _ => n switch
            {
                < 0  => BinomialCoefficient(-n + k - 1, k) * (k.IsOdd() ? -1 : 1),
                > 20 => (long)(n.FactorialBigInt() / (k.FactorialBigInt() * (n - k).FactorialBigInt())),
                _    => n.Factorial() / (k.Factorial() * (n - k).Factorial())
            }
        };
    }

    /// <summary>Биномиальный коэффициент</summary>
    /// <param name="n">Показатель степени бинома (максимум 67)</param>
    /// <param name="k">Индекс коэффициента (максимум 76, 34)</param>
    /// <returns>Биномиальный коэффициент (n, k)</returns>
    public static ulong BinomialCoefficient(ulong n, ulong k)
    {
        if (k > n) return 0;
        if (k > n / 2) k = n - k;
        if (k == 0) return 1;
        if (k == 1) return n;

        var nn = 1ul;
        var kk = 1ul;
        var i  = 0ul;
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
        if (k > n) return 0;
        if (k > n / 2) k = n - k;
        if (k == 0) return 1;
        if (k == 1) return n;

        BigInt nn = 1ul;
        BigInt kk = 1ul;
        BigInt i  = 0ul;
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