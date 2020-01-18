using System;
using System.Collections.Generic;
using System.Linq;

namespace MathCore
{
    public static partial class SpecialFunctions
    {
        public static class EllipticJacobi
        {
            private static double GetKi(double k)
            {
                var ki = k / (1 + Math.Sqrt(1 - k * k));
                return ki * ki;
            }

            private static double[] GetKValues(double k)
            {
                if (k < 0) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k меньше 0!");
                if (k > 1) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k больше 1!");

                var kk = new List<double>(20);
                var ki = k;
                do
                {
                    k = ki;
                    ki = GetKi(k);
                    kk.Add(ki);
                } while (Math.Abs(ki - k) > 0);

                return kk.ToArray();
            }

            private static double CalculateEllipticIntegral(double[] kk) => kk.Aggregate(Math.PI / 2, (K, k) => K * (1 + k));

            /// <summary>Полный эллиптический интеграл</summary>
            /// <param name="k">Параметр интегрирования от 0 до 1</param>
            /// <returns></returns>
            public static double FullEllipticIntegral(double k) => CalculateEllipticIntegral(GetKValues(k));

            public static double FullEllipticIntegralComplimentary(double k) => FullEllipticIntegral(Math.Sqrt(1 - k * k));

            public static double FullEllipticIntegral_Recursive(double k)
            {
                if (k < 0) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k меньше 0!");
                if (k > 1) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k больше 1!");

                var ki = GetKi(k);
                return k - ki > 0 ? (1 + ki) * FullEllipticIntegral_Recursive(ki) : Math.PI / 2;
            }

            public static double FullEllipticIntegralComplimentary_Recursive(double k) => FullEllipticIntegral_Recursive(Math.Sqrt(1 - k * k));


            private static double sn_uk(double u, double[] kk)
            {
                var w = Math.Sin(u * Math.PI / 2);
                for (var i = kk.Length - 1; i >= 0; i--) w = (1 + kk[i]) / (1 / w + kk[i] * w);
                return w;
            }

            private static Complex sn_uk(Complex u, double[] kk)
            {
                var w = Complex.Trigonometry.Sin(u * Math.PI / 2);
                for (var i = kk.Length - 1; i >= 0; i--) w = (1 + kk[i]) / (1 / w + kk[i] * w);
                return w;
            }

            private static double cd_uk(double u, double[] kk)
            {
                var w = Math.Cos(u * Math.PI / 2);
                for (var i = kk.Length - 1; i >= 0; i--) w = (1 + kk[i]) / (1 / w + kk[i] * w);
                return w;
            }  

            private static Complex cd_uk(in Complex u, double[] kk)
            {
                var w = Complex.Trigonometry.Cos(u * Math.PI / 2);
                for (var i = kk.Length - 1; i >= 0; i--) w = (1 + kk[i]) / (1 / w + kk[i] * w);
                return w;
            }

            public static double sn_uk(double u, double k) => sn_uk(u, GetKValues(k));
            public static Complex sn_uk(in Complex u, double k) => sn_uk(u, GetKValues(k));

            public static double cd_uk(double u, double k) => cd_uk(u, GetKValues(k));
            public static Complex cd_uk(in Complex u, double k) => cd_uk(u, GetKValues(k));

            public static double sn_iterative(double z, double k)
            {
                var kk = GetKValues(k);
                return sn_uk(z / CalculateEllipticIntegral(kk), kk);
            }

            public static Complex sn_iterative(in Complex z, double k)
            {
                var kk = GetKValues(k);
                return sn_uk(z / CalculateEllipticIntegral(kk), kk);
            }

            public static double cd_iterative(double z, double k)
            {
                var kk = GetKValues(k);
                return cd_uk(z / CalculateEllipticIntegral(kk), kk);
            }  

            public static Complex cd_iterative(in Complex z, double k)
            {
                var kk = GetKValues(k);
                return cd_uk(z / CalculateEllipticIntegral(kk), kk);
            }

            public static double sn_uk_recursive(double u, double k)
            {
                if (k < 0) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k меньше 0!");
                if (k > 1) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k больше 1!");

                var ki = GetKi(k);

                if (!(k - ki > 0)) return Math.Sin(u * Math.PI / 2);
                var w = sn_uk_recursive(u, ki);
                return (1 + ki) / (1 / w + ki * w);

            }

            public static Complex sn_uk_recursive(in Complex u, double k)
            {
                if (k < 0) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k меньше 0!");
                if (k > 1) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k больше 1!");

                var ki = GetKi(k);

                if (!(k - ki > 0)) return Complex.Trigonometry.Sin(u * Math.PI / 2);
                var w = sn_uk_recursive(u, ki);
                return (1 + ki) / (1 / w + ki * w);

            }

            public static double cd_uk_recursive(double u, double k)
            {
                if (k < 0) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k меньше 0!");
                if (k > 1) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k больше 1!");

                var ki = GetKi(k);

                if (!(k - ki > 0)) return Math.Cos(u * Math.PI / 2);
                var w = cd_uk_recursive(u, ki);
                return (1 + ki) / (1 / w + ki * w);

            }    

            public static Complex cd_uk_recursive(in Complex u, double k)
            {
                if (k < 0) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k меньше 0!");
                if (k > 1) throw new ArgumentOutOfRangeException(nameof(k), k, "Значение параметра k больше 1!");

                var ki = GetKi(k);

                if (!(k - ki > 0)) return Complex.Trigonometry.Cos(u * Math.PI / 2);
                var w = cd_uk_recursive(u, ki);
                return (1 + ki) / (1 / w + ki * w);

            }

            public static double sn_inverse(double sn, double k)
            {
                var kk = GetKValues(k).AppendFirst(k).ToArray();

                var u = sn;
                for (var i = 1; i < kk.Length; i++)
                    u *= 2 / (1 + kk[i]) / (1 + Math.Sqrt(1 - (kk[i - 1] * u).Pow2()));

                return 2 * Math.Asin(u) / Math.PI;
            }

            public static Complex sn_inverse(in Complex sn, double k)
            {
                var kk = GetKValues(k).AppendFirst(k).ToArray();

                var u = sn;
                for (var i = 1; i < kk.Length; i++)
                    u *= 2 / (1 + kk[i]) / (1 + Complex.Sqrt(1 - (kk[i - 1] * u).Pow2()));

                return 2 * Complex.Trigonometry.Asin(u) / Math.PI;
            }

            public static double sn_inverse_recursive(double sn, double k)
            {
                var ki = GetKi(k);
                return k - ki > 0
                    ? sn_inverse_recursive(2 * sn / ((1 + ki) * (1 + Math.Sqrt(1 - k * k * sn * sn))), ki)
                    : 2 * Math.Asin(sn) / Math.PI;
            }

            public static double cd_inverse(double cd, double k)
            {
                var kk = GetKValues(k).AppendFirst(k).ToArray();

                var u = cd;
                for (var i = 1; i < kk.Length; i++)
                    u *= 2 / ((1 + kk[i]) * (1 + Math.Sqrt(1 - (kk[i - 1] * kk[i - 1] * u * u))));

                return 2 * Math.Acos(u) / Math.PI;
            }

            public static double cd_inverse_recursive(double cd, double k)
            {
                var ki = GetKi(k);
                return k - ki > 0
                    ? cd_inverse_recursive(2 * cd / ((1 + ki) * (1 + Math.Sqrt(1 - k * k * cd * cd))), ki)
                    : 2 * Math.Acos(cd) / Math.PI;
            }
        }
    }
}