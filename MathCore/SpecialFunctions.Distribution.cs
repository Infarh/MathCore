using System;

using static System.Math;

// ReSharper disable UnusedMember.Global

namespace MathCore;

public static partial class SpecialFunctions
{
    public static class Distribution
    {
        /// <summary>Нормальное распределение</summary>
        // ReSharper disable once CommentTypo
        //[Copyright("1984, 1987, 1988, 1992, 2000 by Stephen L. Moshier")]
        public static class Normal
        {
            /// <summary>Интеграл от exp{-t^2} от нуля до x / .5 sqrt(pi)</summary>
            /// <param name="x">Аргумент функции нормального распределения</param>
            /// <returns>Значение нормального распределения</returns>
            [DST]
            public static double ErrorFunction(double x)
            {
                var s = Sign(x);
                x = Abs(x);
                if (x >= .5) return x >= 10 ? s : s * (1 - ErrorFunctionConform(x));

                var x2 = x * x;
                var p   = .007547728033418631287834;
                p = .288805137207594084924010 + x2 * p;
                p = 14.3383842191748205576712 + x2 * p;
                p = 38.0140318123903008244444 + x2 * p;
                p = 3017.82788536507577809226 + x2 * p;
                p = 7404.07142710151470082064 + x2 * p;
                p = 80437.3630960840172832162 + x2 * p;

                var q = .0;
                q = 1.00000000000000000000000 + x2 * q;
                q = 38.0190713951939403753468 + x2 * q;
                q = 658.070155459240506326937 + x2 * q;
                q = 6379.60017324428279487120 + x2 * q;
                q = 34216.5257924628539769006 + x2 * q;
                q = 80437.3630960840172826266 + x2 * q;
                return s * 1.1283791670955125738961589031 * x * p / q;
            }

            [DST]
            public static double ErrorFunctionConform(double x)
            {
                if (x < 0) return 2 - ErrorFunctionConform(-x);
                if (x < .5) return 1 - ErrorFunction(x);
                if (x >= 10) return 0;

                var p = 0d;
                p *= .5641877825507397413087057563 + x;
                p *= 9.675807882987265400604202961 + x;
                p *= 77.08161730368428609781633646 + x;
                p *= 368.5196154710010637133875746 + x;
                p *= 1143.262070703886173606073338 + x;
                p *= 2320.439590251635247384768711 + x;
                p *= 2898.0293292167655611275846 + x;
                p *= 1826.3348842295112592168999 + x;

                var q = 1d;
                q *= 17.14980943627607849376131193 + x;
                q *= 137.1255960500622202878443578 + x;
                q *= 661.7361207107653469211984771 + x;
                q *= 2094.384367789539593790281779 + x;
                q *= 4429.612803883682726711528526 + x;
                q *= 6089.5424232724435504633068 + x;
                q *= 4958.82756472114071495438422 + x;
                q *= 1826.3348842295112595576438 + x;
                return Exp(-(x * x)) * p / q;
            }

            [DST]
            public static double NormalDistribution(double x) => .5 * (ErrorFunction(x / 1.41421356237309504880) + 1);

            [DST]
            public static double ErrorFunctionInversed(double e) => NormalDistributionInversed(.5 * (e + 1)) / Consts.sqrt_2;

            [DST]
            public static double NormalDistributionInversed(double y0)
            {
                const double exp_m2  = .13533528323661269189;
                const double lc_s2pi = 2.50662827463100050242;

                if (y0 <= 0)
                    return -__MaxRealNumber;

                if (y0 >= 1)
                    return __MaxRealNumber;

                var code = 1;
                var y    = y0;
                if (y > 1 - exp_m2)
                {
                    y    = 1 - y;
                    code = 0;
                }

                if (y > exp_m2)
                {
                    y -= .5;
                    var y2 = y * y;
                    var p0 = -59.9633501014107895267;
                    p0 *= 98.0010754185999661536 + y2;
                    p0 *= -56.6762857469070293439 + y2;
                    p0 *= 13.9312609387279679503 + y2;
                    p0 *= -1.23916583867381258016 + y2;

                    var q0 = 1d;
                    q0 *= 1.95448858338141759834 + y2;
                    q0 *= 4.67627912898881538453 + y2;
                    q0 *= 86.3602421390890590575 + y2;
                    q0 *= -225.462687854119370527 + y2;
                    q0 *= 200.260212380060660359 + y2;
                    q0 *= -82.0372256168333339912 + y2;
                    q0 *= 15.9056225126211695515 + y2;
                    q0 *= -1.18331621121330003142 + y2;
                    return (y + y * y2 * p0 / q0) * lc_s2pi;
                }

                var x  = Sqrt(-2 * Log(y));
                var x0 = x - Log(x) / x;
                var z  = 1 / x;

                double x1;
                if (x < 8)
                {
                    var p1 = 4.05544892305962419923;
                    p1 *= 31.5251094599893866154 + z;
                    p1 *= 57.1628192246421288162 + z;
                    p1 *= 44.0805073893200834700 + z;
                    p1 *= 14.6849561928858024014 + z;
                    p1 *= 2.18663306850790267539 + z;
                    p1 *= -(1.40256079171354495875 * .1) + z;
                    p1 *= -(3.50424626827848203418 * .01) + z;
                    p1 *= -(8.57456785154685413611 * .0001) + z;

                    var q1 = 1.0;
                    q1 *= 15.7799883256466749731 + z;
                    q1 *= 45.3907635128879210584 + z;
                    q1 *= 41.3172038254672030440 + z;
                    q1 *= 15.0425385692907503408 + z;
                    q1 *= 2.50464946208309415979 + z;
                    q1 *= -(1.42182922854787788574 * .1) + z;
                    q1 *= -(3.80806407691578277194 * .01) + z;
                    q1 *= -(9.33259480895457427372 * .0001) + z;
                    x1 =  z * p1 / q1;
                }
                else
                {
                    var p2 = 3.23774891776946035970;
                    p2 *= 6.91522889068984211695 + z;
                    p2 *= 3.93881025292474443415 + z;
                    p2 *= 1.33303460815807542389 + z;
                    p2 *= 2.01485389549179081538 * .1 + z;
                    p2 *= 1.23716634817820021358 * .01 + z;
                    p2 *= 3.01581553508235416007 * .0001 + z;
                    p2 *= 2.65806974686737550832 * .000001 + z;
                    p2 *= 6.23974539184983293730 * .000000001 + z;

                    var q2 = 1d;
                    q2 *= 6.02427039364742014255 + z;
                    q2 *= 3.67983563856160859403 + z;
                    q2 *= 1.37702099489081330271 + z;
                    q2 *= 2.16236993594496635890 * .1 + z;
                    q2 *= 1.34204006088543189037 * .01 + z;
                    q2 *= 3.28014464682127739104 * .0001 + z;
                    q2 *= 2.89247864745380683936 * .000001 + z;
                    q2 *= 6.79019408009981274425 * .000000001 + z;
                    x1 =  z * p2 / q2;
                }
                return code == 0 ? x0 - x1 : x1 - x0;
            }
        }

        public static class Student
        {
            [DST]
            public static double StudentDistribution(int k, double t)
            {
                if (Abs(t - 0) < Eps) return .5;
                if (t < -2)
                    return .5 * IncompleteBeta.IncompleteBetaValue(.5 * k, .5, k / (k + t * t));

                var x = t < 0 ? -t : t;

                double rk = k;
                var    z  = 1 + x * x / rk;
                double tz;
                double f;
                double p;
                int    j;
                if (k % 2 != 0)
                {
                    var x_sqk = x / Sqrt(rk);
                    p = Atan(x_sqk);
                    if (k > 1)
                    {
                        f  = 1;
                        tz = 1;
                        j  = 3;
                        while (j <= k - 2 & tz / f > Eps)
                        {
                            tz *= (j - 1) / (z * j);
                            f  += tz;
                            j  += 2;
                        }
                        p += f * x_sqk / z;
                    }
                    p *= 2 / Consts.pi;
                }
                else
                {
                    f = tz = 1;
                    j = 2;
                    while (j <= k - 2 & tz / f > Eps)
                    {
                        tz *= (j - 1) / (z * j);
                        f  += tz;
                        j  += 2;
                    }
                    p = f * x / Sqrt(z * rk);
                }

                return .5 + .5 * (t < 0 ? -p : p);
            }

            [DST]
            public static double StudentDistributionInversed(int k, double p)
            {
                double z;

                double rk = k;
                if (p is > .25 and < .75)
                {
                    if (Abs(p - .5) < Eps) return 0;
                    z = IncompleteBeta.IncompleteBetaInversed(.5, .5 * rk, Abs(1 - 2 * p));
                    var t = Sqrt(rk * z / (1 - z));
                    return p < 0 ? -t : t;
                }

                var r_flg = -1;
                if (p >= .5)
                {
                    p     = 1 - p;
                    r_flg = 1;
                }

                z = IncompleteBeta.IncompleteBetaInversed(.5 * rk, .5, 2 * p);
                return __MaxRealNumber * z < rk ? r_flg * __MaxRealNumber : r_flg * Sqrt(rk / z - rk);
            }

            /// <summary>Квантиль Хи-квадрат</summary>
            ///  /// <param name="alpha">Квантиль [0..1]</param>
            /// <param name="n">Число степеней свободы</param>
            /// <returns>Квантиль</returns>
            public static double QuantileHi2(double alpha, int n) => Gamma.LowerRegularizedInv(n / 2d, alpha) / 0.5;

            // ReSharper disable CommentTypo
            /// <summary>Квантиль Хи-квадрат (Аппроксимация Корниша-Фишера)</summary>
            /// <remarks>http://ru.wikipedia.org/wiki/Квантили_распределения_хи-квадрат</remarks>
            /// <remarks>https://projecteuclid.org/download/pdf_1/euclid.aoms/1177730982</remarks>
            /// <param name="alpha">Квантиль [0..1] (0.95 - 95% вероятности попадания значения в выбранный диапазон)</param>
            /// <param name="n">Число степеней свободы (число элементов гистограммы, минус число параметров распределения: [mu, sgm = 2])</param>
            /// <returns>Квантиль</returns>
            // ReSharper restore CommentTypo
            public static double QuantileHi2Approximation(double alpha, int n)
            {
                if (alpha is < .001 or > .999)
                    throw new ArgumentOutOfRangeException(nameof(alpha), alpha, "Значения alpha < 0.001 и > 0.999 не поддерживаются");

                var d0 = alpha >= .5
                    ? 2.0637 * Pow(Log(1 / (1 - alpha)) - .16, .4274) - 1.5774
                    : -2.0637 * Pow(Log(1 / alpha) - .16, .4274) + 1.5774;

                var d2 = d0 * d0;
                var d4 = d2 * d2;

                var a = d0 * Consts.sqrt_2;
                var b = 2 * (d2 - 1) / 3;
                var c = d0 * (d2 - 7) / (9 * Consts.sqrt_2);
                var d = -(6 * d4 + 14 * d2 - 32) / 405;
                var e = d0 * (9 * d4 + 256 * d2 - 433) / (4860 * Consts.sqrt_2);

                var sqrt_n = Sqrt(n);
                return n + a * sqrt_n + b + c / sqrt_n + d / n + e / n / sqrt_n;
            }
        }
    }
}