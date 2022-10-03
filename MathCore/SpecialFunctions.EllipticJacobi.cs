using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using static System.Math;

namespace MathCore;

public static partial class SpecialFunctions
{
    /// <summary>Эллиптические функции Якоби</summary>
    public static class EllipticJacobi
    {
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

            public KValues(double k)
            {
                if (k < 0) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k меньше 0!");
                if (k > 1) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k больше 1!");
                _k      = k;
                _ki     = k;
                Current = double.NaN;
            }

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

        private static double CalculateEllipticIntegral(List<double> kk)
        {
            //return kk.Aggregate(PI / 2, (K, k) => K * (1 + k));
            var result = Consts.pi05;
            foreach (var k in kk)
                result *= 1 + k;
            return result;
        }

        private static double CalculateEllipticIntegral(KValues kk)
        {
            //return kk.Aggregate(PI / 2, (K, k) => K * (1 + k));
            var result = Consts.pi05;
            foreach (var k in kk)
                result *= 1 + k;
            return result;
        }

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

        private static double sn_uk(double u, List<double> kk)
        {
            var w = Sin(u * Consts.pi05);
            for (var i = kk.Count - 1; i >= 0; i--)
                w = (1 + kk[i]) / (1 / w + kk[i] * w);
            return w;
        }

        private static Complex sn_uk(Complex u, List<double> kk)
        {
            var w = Complex.Trigonometry.Sin(u * Consts.pi05);
            for (var i = kk.Count - 1; i >= 0; i--)
                w = (1 + kk[i]) / (1 / w + kk[i] * w);
            return w;
        }

        private static double cd_uk(double u, List<double> kk)
        {
            var w = Cos(u * Consts.pi05);
            for (var i = kk.Count - 1; i >= 0; i--)
                w = (1 + kk[i]) / (1 / w + kk[i] * w);
            return w;
        }

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

        public static double sn_uk_recursive(double u, double k)
        {
            if (k is < 0 or > 1) return double.NaN;

            var ki = GetKi(k);

            if (k - ki <= 0)
                return Sin(u * Consts.pi05);

            var w = sn_uk_recursive(u, ki);
            return (1 + ki) / (1 / w + ki * w);
        }

        public static Complex sn_uk_recursive(Complex u, double k)
        {
            if (k is < 0 or > 1) return double.NaN;

            var ki = GetKi(k);

            if (k - ki <= 0)
                return Complex.Trigonometry.Sin(u * Consts.pi05);

            var w = sn_uk_recursive(u, ki);
            return (1 + ki) / (1 / w + ki * w);
        }

        public static double cd_uk_recursive(double u, double k)
        {
            if (k is < 0 or > 1) return double.NaN;

            var ki = GetKi(k);

            if (k - ki <= 0)
                return Cos(u * Consts.pi05);

            var w = cd_uk_recursive(u, ki);
            return (1 + ki) / (1 / w + ki * w);
        }

        public static Complex cd_uk_recursive(Complex u, double k)
        {
            if (k is < 0 or > 1) return double.NaN;

            var ki = GetKi(k);

            if (k - ki <= 0)
                return Complex.Trigonometry.Cos(u * Consts.pi05);

            var w = cd_uk_recursive(u, ki);
            return (1 + ki) / (1 / w + ki * w);
        }

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

        public static double sn_inverse_recursive(double sn, double k)
        {
            var ki = GetKi(k);
            return k - ki > 0
                ? sn_inverse_recursive(2 * sn / ((1 + ki) * (1 + Sqrt(1 - (k * sn).Pow2()))), ki)
                : Asin(sn) / Consts.pi05;
        }

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

        public static double cd_inverse_recursive(double cd, double k)
        {
            var ki = GetKi(k);
            return k - ki > 0
                ? cd_inverse_recursive(2 * cd / ((1 + ki) * (1 + Sqrt(1 - (k * cd).Pow2()))), ki)
                : Acos(cd) / Consts.pi05;
        }

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