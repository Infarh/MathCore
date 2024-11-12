using System.Runtime.CompilerServices;

using static System.Math;

namespace MathCore;

public static partial class SpecialFunctions
{
    /// <summary>Эллиптические функции Якоби</summary>
    public static class EllipticJacobi
    {
        /// <summary>Вычисляет квадрат модифицированного параметра эллиптической функции Якоби</summary>
        /// <param name="k">Параметр эллиптической функции Якоби, где 0 <= k <= 1.</param>
        /// <returns>Квадрат модифицированного параметра k.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double GetKi(double k)
        {
            var ki = k / (1 + Sqrt(1 - k * k));
            return ki * ki;
        }

        private struct KValues
        {
            private double _k;
            private double _ki;

            public double Current { get; private set; }

            /// <summary>Вычисляет последовательность значений k, ki, ki' , ki'', ...</summary>
            /// <param name="k">Параметр эллиптической функции Якоби, где 0 <= k <= 1.</param>
            /// <remarks>
            ///     <para>Вычисляет последовательность значений k, ki, ki', ki'', ...</para>
            ///     <para>ki = k / (1 + Sqrt(1 - k * k))</para>
            ///     <para>ki' = ki / (1 + Sqrt(1 - ki * ki))</para>
            ///     <para>ki'' = ki' / (1 + Sqrt(1 - ki' * ki'))</para>
            ///     <para>...</para>
            ///     <para>Current = NaN до первого вызова MoveNext()</para>
            /// </remarks>
            public KValues(double k)
            {
                if (k < 0) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k меньше 0!");
                if (k > 1) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k больше 1!");
                _k      = k;
                _ki     = k;
                Current = double.NaN;
            }

            /// <summary>Вычисляет следующее значение ki</summary>
            /// <returns>true, если было вычислено новое значение, false, если последовательность закончилась</returns>
            /// <remarks>
            ///     <para>Вычисляет следующее значение ki</para>
            ///     <para>Если параметр k == 0, то последовательность закончилась</para>
            ///     <para>Если параметр k == 1, то последовательность закончилась</para>
            /// </remarks>
            public bool MoveNext()
            {
                if (Current is not double.NaN && Abs(_ki - _k) == 0) return false;

                _k      = _ki;
                _ki     = GetKi(_k);
                Current = _ki;
                var can_move_next = Abs(_ki - _k) > 0;
                return can_move_next;
            }

            public KValues GetEnumerator() => this;
        }

        /// <summary>Вычисляет последовательность значений k, ki, ki', ki'', ...</summary>
        /// <param name="k">Параметр k</param>
        /// <returns>Последовательность значений k, ki, ki', ki'', ...</returns>
        /// <remarks>
        ///     <para>ki = k / (1 + Sqrt(1 - k * k))</para>
        ///     <para>ki' = ki / (1 + Sqrt(1 - ki * ki))</para>
        ///     <para>ki'' = ki' / (1 + Sqrt(1 - ki' * ki'))</para>
        ///     <para>...</para>
        ///     <para>Если параметр k == 0, то последовательность закончилась</para>
        ///     <para>Если параметр k == 1, то последовательность закончилась</para>
        /// </remarks>
        private static List<double> GetKValues(double k)
        {
            if (k < 0) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k меньше 0!");
            if (k > 1) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k больше 1!");

            var kk = new List<double>(15);
            var ki = k;
            while (true)
            {
                k  = ki;
                ki = GetKi(k);
                if (Abs(ki - k) == 0 || ki == 0) break;
                kk.Add(ki);
            }

            return kk;
        }

        /// <summary>Вычисляет последовательность значений k, ki, ki', ki'', ...</summary>
        /// <param name="k">Параметр k</param>
        /// <returns>Перечисление значений k, ki, ki', ki'', ...</returns>
        /// <remarks>
        ///     <para>ki = k / (1 + Sqrt(1 - k * k))</para>
        ///     <para>ki' = ki / (1 + Sqrt(1 - ki * ki))</para>
        ///     <para>ki'' = ki' / (1 + Sqrt(1 - ki' * ki'))</para>
        ///     <para>...</para>
        ///     <para>Если параметр k == 0, то последовательность закончилась</para>
        ///     <para>Если параметр k == 1, то последовательность закончилась</para>
        /// </remarks>
        private static IEnumerable<double> EnumKValues(double k)
        {
            if (k < 0) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k меньше 0!");
            if (k > 1) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k больше 1!");

            var ki = k;
            do
            {
                k  = ki;
                ki = GetKi(k);
                yield return ki;
            } while (Abs(ki - k) > 0);
        }

        /// <summary>
        ///     Вычисляет эллиптический интеграл
        ///     <para>
        ///         I = (pi / 2) * (1 + k1) * (1 + k2) * (1 + k3) * ...
        ///     </para>
        /// </summary>
        /// <param name="kk">Перечисление значений k, ki, ki', ki'', ...</param>
        /// <returns>Значение эллиптического интеграла</returns>
        private static double CalculateEllipticIntegral(List<double> kk)
        {
            //return kk.Aggregate(PI / 2, (K, k) => K * (1 + k));
            var result = Consts.pi05;
            foreach (var k in kk)
                result *= 1 + k;
            return result;
        }

        /// <summary>
        ///     Вычисляет эллиптический интеграл
        ///     <para>
        ///         I = (pi / 2) * (1 + k1) * (1 + k2) * (1 + k3) * ...
        ///     </para>
        /// </summary>
        /// <param name="kk">Перечисление значений k, ki, ki', ki'', ...</param>
        /// <returns>Значение эллиптического интеграла</returns>
        private static double CalculateEllipticIntegral(KValues kk)
        {
            //return kk.Aggregate(PI / 2, (K, k) => K * (1 + k));
            var result = Consts.pi05;
            foreach (var k in kk)
                result *= 1 + k;
            return result;
        }

        /// <summary>
        ///     Вычисляет эллиптический интеграл
        ///     <para>
        ///         I = (pi / 2) * (1 + k1) * (1 + k2) * (1 + k3) * ...
        ///     </para>
        /// </summary>
        /// <param name="k">Параметр k</param>
        /// <returns>Значение эллиптического интеграла</returns>
        /// <remarks>
        ///     <para>Метод является итерационным, количество итераций составляет примерно 14</para>
        /// </remarks>
        private static double CalculateEllipticIntegral(double k)
        {
            var    result = Consts.pi05;
            var    ki     = k;
            double ki_last;
            do // примерно 14 итераций
            {
                ki_last = ki;

                ki =  ki_last / (1 + Sqrt(1 - ki_last * ki_last));
                ki *= ki;

                result *= 1 + ki;
            } while (Abs(ki - ki_last) > 0);

            return result;
        }

        /// <summary>Полный эллиптический интеграл</summary>
        /// <param name="k">Параметр интегрирования от 0 до 1</param>
        /// <returns>Значение полного эллиптического интеграла</returns>
        public static double FullEllipticIntegral(double k) => k switch
        {
            < 0 or > 1 => double.NaN,
            0          => Consts.pi05,
            1          => double.PositiveInfinity,
            _          => CalculateEllipticIntegral(k)
        };

        /// <summary>Полный комплиментарного эллиптический интеграл</summary>
        /// <param name="k">Параметр интегрирования от 0 до 1</param>
        /// <returns>Значение полного комплиментарного эллиптического интеграла</returns>
        public static double FullEllipticIntegralComplimentary(double k) => FullEllipticIntegral(Sqrt(1 - k * k));

        /// <summary>Полный эллиптический интеграл (рекурсивный алгоритм)</summary>
        /// <param name="k">Параметр интегрирования от 0 до 1</param>
        /// <returns>Значение полного эллиптического интеграла</returns>
        public static double FullEllipticIntegral_Recursive(double k)
        {
            if (k is < 0 or > 1) return double.NaN;

            var ki = GetKi(k);
            return k - ki > 0 ? (1 + ki) * FullEllipticIntegral_Recursive(ki) : PI / 2;
        }

        /// <summary>Полный комплиментарного эллиптический интеграл (рекурсивный алгоритм)</summary>
        /// <param name="k">Параметр интегрирования от 0 до 1</param>
        /// <returns>Значение полного комплиментарного эллиптического интеграла</returns>
        public static double FullEllipticIntegralComplimentary_Recursive(double k) => FullEllipticIntegral_Recursive(Sqrt(1 - k * k));

        /// <summary>
        ///     Вычисляет значение эллиптической функции sn(u,k) по формуле:
        ///     <para>
        ///         sn(u,k) = sin(u * pi / 2) * (1 + k1) / (1 / w + k1 * w)
        ///     </para>
        ///     <para>
        ///         где k1 = k / (1 + Sqrt(1 - k * k))
        ///     </para>
        ///     <para>
        ///         w = (1 + k2) / (1 / w + k2 * w)
        ///     </para>
        ///     <para>
        ///         k2 = k1 / (1 + Sqrt(1 - k1 * k1))
        ///     </para>
        ///     <para>
        ///         ...
        ///     </para>
        ///     <para>
        ///         до тех пор, пока ki != ki_last
        ///     </para>
        /// </summary>
        /// <param name="u">Значение параметра u</param>
        /// <param name="kk">Список значений k, ki, ki', ki'', ...</param>
        /// <returns>Значение эллиптической функции sn(u,k)</returns>
        private static double sn_uk(double u, List<double> kk)
        {
            var w = Sin(u * Consts.pi05);
            for (var i = kk.Count - 1; i >= 0; i--)
                w = (1 + kk[i]) / (1 / w + kk[i] * w);
            return w;
        }

        /// <summary>
        ///     Вычисляет значение эллиптической функции sn(u,k) по формуле:
        ///     <para>
        ///         sn(u,k) = sin(u * pi / 2) * (1 + k1) / (1 / w + k1 * w)
        ///     </para>
        ///     <para>
        ///         где k1 = k / (1 + Sqrt(1 - k * k))
        ///     </para>
        ///     <para>
        ///         w = (1 + k2) / (1 / w + k2 * w)
        ///     </para>
        ///     <para>
        ///         k2 = k1 / (1 + Sqrt(1 - k1 * k1))
        ///     </para>
        ///     <para>
        ///         ...
        ///     </para>
        ///     <para>
        ///         до тех пор, пока ki != ki_last
        ///     </para>
        /// </summary>
        /// <param name="u">Значение параметра u</param>
        /// <param name="kk">Список значений k, ki, ki', ki'', ...</param>
        /// <returns>Значение эллиптической функции sn(u,k)</returns>
        private static Complex sn_uk(Complex u, List<double> kk)
        {
            var w = Complex.Trigonometry.Sin(u * Consts.pi05);
            for (var i = kk.Count - 1; i >= 0; i--)
                w = (1 + kk[i]) / (1 / w + kk[i] * w);
            return w;
        }

        /// <summary>
        ///     Вычисляет значение эллиптической функции cd(u,k) по формуле:
        ///     <para>
        ///         cd(u,k) = cos(u * pi / 2) * (1 + k1) / (1 / w + k1 * w)
        ///     </para>
        ///     <para>
        ///         где k1 = k / (1 + Sqrt(1 - k * k))
        ///     </para>
        ///     <para>
        ///         w = (1 + k2) / (1 / w + k2 * w)
        ///     </para>
        ///     <para>
        ///         k2 = k1 / (1 + Sqrt(1 - k1 * k1))
        ///     </para>
        ///     <para>
        ///         ...
        ///     </para>
        ///     <para>
        ///         до тех пор, пока ki != ki_last
        ///     </para>
        /// </summary>
        /// <param name="u">Значение параметра u</param>
        /// <param name="kk">Список значений k, ki, ki', ki'', ...</param>
        /// <returns>Значение эллиптической функции cd(u,k)</returns>
        private static double cd_uk(double u, List<double> kk)
        {
            var w = Cos(u * Consts.pi05);
            for (var i = kk.Count - 1; i >= 0; i--)
                w = (1 + kk[i]) / (1 / w + kk[i] * w);
            return w;
        }

        /// <summary>
        ///     Вычисляет значение эллиптической функции cd(u,k) по формуле:
        ///     <para>
        ///         cd(u,k) = cos(u * pi / 2) * (1 + k1) / (1 / w + k1 * w)
        ///     </para>
        ///     <para>
        ///         где k1 = k / (1 + Sqrt(1 - k * k))
        ///     </para>
        ///     <para>
        ///         w = (1 + k2) / (1 / w + k2 * w)
        ///     </para>
        ///     <para>
        ///         k2 = k1 / (1 + Sqrt(1 - k1 * k1))
        ///     </para>
        ///     <para>
        ///         ...
        ///     </para>
        ///     <para>
        ///         до тех пор, пока ki != ki_last
        ///     </para>
        /// </summary>
        /// <param name="u">Значение параметра u</param>
        /// <param name="kk">Список значений k, ki, ki', ki'', ...</param>
        /// <returns>Значение эллиптической функции cd(u,k)</returns>
        private static Complex cd_uk(Complex u, List<double> kk)
        {
            var w = Complex.Trigonometry.Cos(u * Consts.pi05);
            for (var i = kk.Count - 1; i >= 0; i--)
                w = (1 + kk[i]) / (1 / w + kk[i] * w);
            return w;
        }

        /// <summary>Эллиптическая функция sn</summary>
        public static double sn_uk(double u, double k) => k is < 0 or > 1 ? double.NaN : sn_uk(u, GetKValues(k));

        /// <summary>Эллиптическая функция sn комплексного аргумента</summary>
        public static Complex sn_uk(Complex u, double k) => k is < 0 or > 1 ? double.NaN : sn_uk(u, GetKValues(k));

        /// <summary>Эллиптическая функция cd</summary>
        public static double cd_uk(double u, double k) => k is < 0 or > 1 ? double.NaN : cd_uk(u, GetKValues(k));

        /// <summary>Эллиптическая функция cd комплексного аргумента</summary>
        public static Complex cd_uk(Complex u, double k) => k is < 0 or > 1 ? double.NaN : cd_uk(u, GetKValues(k));

        /// <summary>Эллиптическая функция sn (итерационный алгоритм)</summary>
        public static double sn_iterative(double z, double k)
        {
            if (k is < 0 or > 1) return double.NaN;

            var kk = GetKValues(k);
            return sn_uk(z / CalculateEllipticIntegral(kk), kk);
        }

        /// <summary>Эллиптическая функция sn комплексного аргумента (итерационный алгоритм)</summary>
        public static Complex sn_iterative(Complex z, double k)
        {
            if (k is < 0 or > 1) return double.NaN;

            var kk = GetKValues(k);
            return sn_uk(z / CalculateEllipticIntegral(kk), kk);
        }

        /// <summary>Эллиптическая функция cd (итерационный алгоритм)</summary>
        public static double cd_iterative(double z, double k)
        {
            if (k is < 0 or > 1) return double.NaN;

            var kk = GetKValues(k);
            return cd_uk(z / CalculateEllipticIntegral(kk), kk);
        }

        /// <summary>Эллиптическая функция cd комплексного аргумента (итерационный алгоритм)</summary>
        public static Complex cd_iterative(Complex z, double k)
        {
            if (k is < 0 or > 1) return double.NaN;

            var kk = GetKValues(k);
            return cd_uk(z / CalculateEllipticIntegral(kk), kk);
        }

        /// <summary>
        ///     Эллиптическая функция sn (рекурсивный алгоритм)
        /// </summary>
        /// <param name="u">Входное значение, для которого вычисляется функция</param>
        /// <param name="k">Параметр эллиптической функции, где 0 <= k <= 1</param>
        /// <returns>Значение эллиптической функции sn для заданных u и k</returns>
        /// <remarks>
        ///     <para>Алгоритм вычисления функции sn основан на рекурсивном вычислении</para>
        /// </remarks>
        public static double sn_uk_recursive(double u, double k)
        {
            if (k is < 0 or > 1) return double.NaN;

            var ki = GetKi(k);

            if (k - ki <= 0)
                return Sin(u * Consts.pi05);

            var w = sn_uk_recursive(u, ki);
            return (1 + ki) / (1 / w + ki * w);
        }


        /// <summary>Эллиптическая функция sn комплексного аргумента (рекурсивный алгоритм)</summary>
        /// <param name="u">Входное значение, для которого вычисляется функция</param>
        /// <param name="k">Параметр эллиптической функции, где 0 <= k <= 1</param>
        /// <returns>Значение эллиптической функции sn для заданных u и k</returns>
        /// <remarks>
        ///     <para>Алгоритм вычисления функции sn основан на рекурсивном вычислении</para>
        /// </remarks>
        public static Complex sn_uk_recursive(Complex u, double k)
        {
            if (k is < 0 or > 1) return double.NaN;

            var ki = GetKi(k);

            if (k - ki <= 0)
                return Complex.Trigonometry.Sin(u * Consts.pi05);

            var w = sn_uk_recursive(u, ki);
            return (1 + ki) / (1 / w + ki * w);
        }


        /// <summary>Эллиптическая функция cd (рекурсивный алгоритм)</summary>
        /// <param name="u">Входное значение, для которого вычисляется функция</param>
        /// <param name="k">Параметр эллиптической функции, где 0 <= k <= 1</param>
        /// <returns>Значение эллиптической функции cd для заданных u и k</returns>
        /// <remarks>
        ///     <para>Алгоритм вычисления функции cd основан на рекурсивном вычислении</para>
        /// </remarks>
        public static double cd_uk_recursive(double u, double k)
        {
            if (k is < 0 or > 1) return double.NaN;

            var ki = GetKi(k);

            if (k - ki <= 0)
                return Cos(u * Consts.pi05);

            var w = cd_uk_recursive(u, ki);
            return (1 + ki) / (1 / w + ki * w);
        }

        /// <summary>Рекурсивно вычисляет значение эллиптической функции Якоби cd(u, k) для комплексного аргумента u и параметра k.</summary>
        /// <param name="u">Комплексный аргумент функции.</param>
        /// <param name="k">Параметр эллиптической функции Якоби, где 0 <= k <= 1.</param>
        /// <returns>Значение эллиптической функции Якоби cd(u, k).</returns>
        /// <remarks>
        /// Использует рекурсивный подход для вычисления значения функции, уменьшая параметр k на каждом шаге.
        /// Если параметр k выходит за пределы [0, 1], возвращает NaN.
        /// </remarks>
        public static Complex cd_uk_recursive(Complex u, double k)
        {
            if (k is < 0 or > 1) return double.NaN;

            var ki = GetKi(k);

            if (k - ki <= 0)
                return Complex.Trigonometry.Cos(u * Consts.pi05);

            var w = cd_uk_recursive(u, ki);
            return (1 + ki) / (1 / w + ki * w);
        }

        /// <summary>
        /// Обратная функция эллиптической функции Якоби sn(u, k).
        /// </summary>
        /// <param name="sn">Значение эллиптической функции Якоби sn(u, k).</param>
        /// <param name="k">Параметр эллиптической функции Якоби, где 0 <= k <= 1.</param>
        /// <returns>Значение u, такое, что sn(u, k) == sn.</returns>
        /// <remarks>
        /// Использует рекурсивный подход для вычисления значения функции, уменьшая параметр k на каждом шаге.
        /// </remarks>
        public static double sn_inverse(double sn, double k)
        {
            //var kk = GetKValues(k);
            //var u = sn * 2 / (1 + kk[0]) / (1 + Sqrt(1 - (k * sn).Pow2()));
            //for (int i = 1, count = kk.Count; i < count; i++)
            //    u *= 2 / (1 + kk[i]) / (1 + Sqrt(1 - (kk[i - 1] * u).Pow2()));

            var u = sn;
            for (var ki = GetKi(k); ki > 0 && Abs(ki - k) > 0; (k, ki) = (ki, GetKi(ki)))
                u *= 2 / ((1 + ki) * (1 + Sqrt(1 - (k * u).Pow2())));

            return Asin(u) / Consts.pi05;
        }

        /// <summary>Обратная функция эллиптической функции Якоби sn(u, k) для комплексного аргумента sn</summary>
        /// <param name="sn">Комплексное значение эллиптической функции Якоби sn(u, k).</param>
        /// <param name="k">Параметр эллиптической функции Якоби, где 0 <= k <= 1.</param>
        /// <returns>Значение u, такое, что sn(u, k) == sn.</returns>
        /// <remarks>
        ///     Использует рекурсивный подход для вычисления значения функции, уменьшая параметр k на каждом шаге.
        /// </remarks>
        public static Complex sn_inverse(Complex sn, double k)
        {
            //var kk = GetKValues(k);
            //var u = sn * 2 / (1 + kk[1]) / (1 + Complex.Sqrt(1 - (k * sn).Pow2()));
            //for (int i = 2, count = kk.Count; i < count; i++)
            //    u *= 2 / (1 + kk[i]) / (1 + Complex.Sqrt(1 - (kk[i - 1] * u).Pow2()));

            var u = sn;
            for (var ki = GetKi(k); ki > 0 && Abs(ki - k) > 0; (k, ki) = (ki, GetKi(ki)))
                u *= 2 / ((1 + ki) * (1 + Complex.Sqrt(1 - (k * u).Pow2())));

            return Complex.Trigonometry.Asin(u) / Consts.pi05;
        }

        /// <summary>Обратная функция эллиптической функции Якоби sn(u, k) для вещественного аргумента sn</summary>
        /// <param name="sn">Значение эллиптической функции Якоби sn(u, k).</param>
        /// <param name="k">Параметр эллиптической функции Якоби, где 0 <= k <= 1.</param>
        /// <returns>Значение u, такое, что sn(u, k) == sn.</returns>
        /// <remarks>
        ///     Использует рекурсивный подход для вычисления значения функции, уменьшая параметр k на каждом шаге.
        /// </remarks>
        public static double sn_inverse_recursive(double sn, double k)
        {
            var ki = GetKi(k);
            return k - ki > 0
                ? sn_inverse_recursive(2 * sn / ((1 + ki) * (1 + Sqrt(1 - (k * sn).Pow2()))), ki)
                : Asin(sn) / Consts.pi05;
        }

        /// <summary>Обратная функция эллиптической функции Якоби cd(u, k) для вещественного аргумента cd</summary>
        /// <param name="cd">Значение эллиптической функции Якоби cd(u, k).</param>
        /// <param name="k">Параметр эллиптической функции Якоби, где 0 <= k <= 1.</param>
        /// <returns>Значение u, такое, что cd(u, k) == cd.</returns>
        /// <remarks>
        ///     Использует рекурсивный подход для вычисления значения функции, уменьшая параметр k на каждом шаге.
        /// </remarks>
        public static double cd_inverse(double cd, double k)
        {
            //var kk = GetKValues(k);
            //var u = cd * 2 / ((1 + kk[0]) * (1 + Sqrt(1 - (k * cd).Pow2())));
            //for (int i = 1, count = kk.Count; i < count; i++) 
            //    u *= 2 / ((1 + kk[i]) * (1 + Sqrt(1 - (kk[i - 1] * u).Pow2())));

            //var u = cd;
            //var ki = k;
            //while (true)
            //{
            //    k = ki;
            //    ki = GetKi(k);
            //    if (Abs(ki - k) == 0 || ki == 0) break;

            //    u *= 2 / ((1 + ki) * (1 + Sqrt(1 - (k * u).Pow2())));
            //}

            var u = cd;
            for (var ki = GetKi(k); ki > 0 && Abs(ki - k) > 0; (k, ki) = (ki, GetKi(ki)))
                u *= 2 / ((1 + ki) * (1 + Sqrt(1 - (k * u).Pow2())));

            return Acos(u) / Consts.pi05;
        }

        /// <summary>Обратная функция эллиптической функции Якоби cd(u, k) для вещественного аргумента cd, рекурсивная версия</summary>
        /// <param name="cd">Значение эллиптической функции Якоби cd(u, k).</param>
        /// <param name="k">Параметр эллиптической функции Якоби, где 0 <= k <= 1.</param>
        /// <returns>Значение u, такое, что cd(u, k) == cd.</returns>
        /// <remarks>
        ///     Использует рекурсивный подход для вычисления значения функции, уменьшая параметр k на каждом шаге.
        /// </remarks>
        public static double cd_inverse_recursive(double cd, double k)
        {
            var ki = GetKi(k);
            return k - ki > 0
                ? cd_inverse_recursive(2 * cd / ((1 + ki) * (1 + Sqrt(1 - (k * cd).Pow2()))), ki)
                : Acos(cd) / Consts.pi05;
        }

        /// <summary>
        ///     Вычисляет эллиптическую функцию Якоби am(x, m) (amplitude) для вещественного аргумента x,
        ///     где m - параметр эллиптической функции (0 <= m <= 1).
        /// </summary>
        /// <param name="x">Значение, для которого вычисляется эллиптическая функция Якоби am(x, m).</param>
        /// <param name="m">Параметр эллиптической функции Якоби, где 0 <= m <= 1.</param>
        /// <returns>Значение am(x, m), которое является эллиптической функцией Якоби (amplitude).</returns>
        /// <remarks>
        ///     Использует рекурсивный подход для вычисления значения функции, уменьшая параметр m на каждом шаге.
        /// </remarks>
        public static double am(double x, double m)
        {
            var b = Sqrt(1 - m);
            var a = 1.0;

            var ca = new double[11];

            for (var i = 1; i <= 10; i++)
            {
                (a, b) = ((a + b) * 0.5, Sqrt(a * b));
                ca[i]  = (a - b) * 0.5 / a;
            }

            var t = 1024 * a * x;

            for (var i = 10; i > 0; i--)
            {
                var s = ca[i] * Sin(t);
                s = t + Asin(s);

                t = 0.5 * s;
            }

            return t;
        }

        /// <summary>Вычисляет полный эллиптический интеграл второго рода E(m) для вещественного параметра m, где 0 <= m <= 1</summary>
        /// <param name="m">Параметр эллиптического интеграла, где 0 <= m <= 1.</param>
        /// <returns>Значение E(m), которое является полным эллиптическим интегралом второго рода.</returns>
        /// <remarks>Использует рекурсивный подход для вычисления значения функции, уменьшая параметр m на каждом шаге</remarks>
        public static double E(double m)
        {
            if (m is < 0 or > 1) return double.NaN;

            var a = 1.0;
            var b = Sqrt(1.0 - m);

            var c = 1 - b * b;
            var s = 0.5 * c;
            var p = 1.0;

            for (var i = 1; i <= 8; i++)
            {
                (a, b) = ((a + b) * 0.5, Sqrt(a * b));

                c =  a * a - b * b;
                s += p * c;
                p += p;
            }

            return Consts.pi05 / a * (1 - s);
        }

        /// <summary>Вычисляет полный эллиптический интеграл второго рода E(m) для комплексного параметра m</summary>
        /// <param name="m">Комплексный параметр эллиптического интеграла</param>
        /// <returns>Значение E(m), которое является полным эллиптическим интегралом второго рода</returns>
        /// <remarks>Использует рекурсивный подход для вычисления значения функции, уменьшая параметр m на каждом шаге</remarks>
        public static Complex E(Complex m)
        {
            var a = Complex.Real;
            var b = Complex.Sqrt(1.0 - m);

            var c = 1 - b * b;
            var s = 0.5 * c;
            var p = 1.0;

            for (var i = 1; i <= 8; i++)
            {
                (a, b) = ((a + b) * 0.5, Complex.Sqrt(a * b));

                c =  a * a - b * b;
                s += p * c;
                p += p;
            }

            return Consts.pi05 / a * (1 - s);
        }
    }
}