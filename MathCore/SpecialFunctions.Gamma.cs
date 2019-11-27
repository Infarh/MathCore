using System;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable UnusedMember.Global

namespace MathCore
{
    public static partial class SpecialFunctions
    {
        /// <summary>Гамма-функция</summary>
        public static class Gamma
        {
            [DST]
            public static double G(double x)
            {
                double z;
                var sgngam = 1;
                var q = Math.Abs(x);
                if (q > 33)
                {
                    if (x >= 0) z = GStir(x);
                    else
                    {
                        double p = (int)Math.Floor(q);
                        var i = (int)Math.Round(p);
                        if (i % 2 == 0) sgngam = -1;
                        z = q - p;
                        if (z > .5) z = q - ++p;
                        z = q * Math.Sin(Consts.pi * z);
                        z = Math.Abs(z);
                        z = Math.PI / (z * GStir(q));
                    }
                    return sgngam * z;
                }
                z = 1;

                while (x >= 3) z *= --x;

                while (x < 0)
                    if (x <= -.000000001) z /= x++;
                    else
                        return z / ((1 + .5772156649015329 * x) * x);

                while (x < 2)
                    if (x >= .000000001) z /= x++;
                    else
                        return z / ((1 + .5772156649015329 * x) * x);

                if (Math.Abs(x - 2) < Eps) return z;
                x -= 2;

                var pp = 1.60119522476751861407E-4;
                pp = 1.19135147006586384913E-3 + x * pp;
                pp = 1.04213797561761569935E-2 + x * pp;
                pp = 4.76367800457137231464E-2 + x * pp;
                pp = 2.07448227648435975150E-1 + x * pp;
                pp = 4.94214826801497100753E-1 + x * pp;
                pp = 9.99999999999999996796E-1 + x * pp;

                var qq = -2.31581873324120129819E-5;
                qq = 5.39605580493303397842E-4 + x * qq;
                qq = -4.45641913851797240494E-3 + x * qq;
                qq = 1.18139785222060435552E-2 + x * qq;
                qq = 3.58236398605498653373E-2 + x * qq;
                qq = -2.34591795718243348568E-1 + x * qq;
                qq = 7.14304917030273074085E-2 + x * qq;
                qq = 1.00000000000000000320 + x * qq;

                return z * pp / qq;
            }

            [DST]
            private static double GStir(double x)
            {
                var w = 1 / x;
                var stir = 7.87311395793093628397E-4;
                stir = -2.29549961613378126380E-4 + w * stir;
                stir = -2.68132617805781232825E-3 + w * stir;
                stir = 3.47222221605458667310E-3 + w * stir;
                stir = 8.33333333333482257126E-2 + w * stir;
                w = 1 + w * stir;
                var y = Math.Exp(x);

                if (x <= 143.01608) y = Math.Pow(x, x - .5) / y;
                else
                {
                    var v = Math.Pow(x, .5 * x - .25);
                    y = v * v / y;
                }
                return 2.50662827463100050242 * y * w;
            }

            [DST]
            public static double LnG(double x) => LnG(x, out _);

            [DST]
            public static double LnG(double x, out int sign)
            {
                double p;
                double q;
                double z;
                sign = 1;
                const double logpi = 1.14472988584940017414;
                const double lc_Ls2Pi = .91893853320467274178;
                if (x < -34)
                {
                    q = -x;
                    var w = LnG(q, out _);
                    p = (int)Math.Floor(q);
                    var i = (int)Math.Round(p);
                    sign = i % 2 == 0 ? -1 : 1;
                    z = q - p;
                    if (z > .5) z = ++p - q;
                    return logpi - Math.Log(q * Math.Sin(Consts.pi * z)) - w;
                }
                if (x < 13)
                {
                    z = 1;
                    p = 0;
                    var u = x;

                    while (u >= 3)
                    {
                        u = x + --p;
                        z *= u;
                    }

                    while (u < 2)
                    {
                        z /= u;
                        u = x + ++p;
                    }

                    if (z >= 0) sign = 1;
                    else
                    {
                        sign = -1;
                        z = -z;
                    }

                    if (Math.Abs(u - 2) < Eps) return Math.Log(z);

                    p -= 2;
                    x += p;

                    var b = -1378.25152569120859100;
                    b = -38801.6315134637840924 + x * b;
                    b = -331612.992738871184744 + x * b;
                    b = -1162370.97492762307383 + x * b;
                    b = -1721737.00820839662146 + x * b;
                    b = -853555.664245765465627 + x * b;

                    var c = 1.0;
                    c = -351.815701436523470549 + x * c;
                    c = -17064.2106651881159223 + x * c;
                    c = -220528.590553854454839 + x * c;
                    c = -1139334.44367982507207 + x * c;
                    c = -2532523.07177582951285 + x * c;
                    c = -2018891.41433532773231 + x * c;

                    return Math.Log(z) + x * b / c;
                }
                q = (x - .5) * Math.Log(x) - x + lc_Ls2Pi;
                if (x > 100000000) return q;

                p = 1 / (x * x);
                if (x >= 1000)
                    q += ((7.9365079365079365079365 * .0001 * p - 2.7777777777777777777778 * .001) * p
                          + .0833333333333333333333) / x;
                else
                {
                    var a = 8.11614167470508450300 * .0001;
                    a = -(5.95061904284301438324 * .0001) + p * a;
                    a = 7.93650340457716943945 * .0001 + p * a;
                    a = -(2.77777777730099687205 * .001) + p * a;
                    a = 8.33333333333331927722 * .01 + p * a;
                    q += a / x;
                }
                return q;
            }
        }
    }
}