using System;

using static System.Math;

// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace MathCore;

public static partial class SpecialFunctions
{
    public static class IncompleteBeta
    {
        // ReSharper disable CommentTypo
        /*************************************************************************
        Incomplete beta integral

        Returns incomplete beta integral of the arguments, evaluated
        from zero to x.  The function is defined as

                         x
            -            -
           | (a+b)      | |  a-1     b-1
         -----------    |   t   (1-t)   dt.
          -     -     | |
         | (a) | (b)   -
                        0

        The domain of definition is 0 <= x <= 1.  In this
        implementation a and b are restricted to positive values.
        The integral from x to 1 may be obtained by the symmetry
        relation

           1 - incbet( a, b, x )  =  incbet( b, a, 1-x ).

        The integral is evaluated by a continued fraction expansion
        or, when b*x is small, by a power series.

        ACCURACY:

        Tested at uniformly distributed random points (a,b,x) with a and b
        in "domain" and x between 0 and 1.
                                               Relative error
        arithmetic   domain     # trials      peak         rms
           IEEE      0,5         10000       6.9e-15     4.5e-16
           IEEE      0,85       250000       2.2e-13     1.7e-14
           IEEE      0,1000      30000       5.3e-12     6.3e-13
           IEEE      0,10000    250000       9.3e-11     7.1e-12
           IEEE      0,100000    10000       8.7e-10     4.8e-11
        Outputs smaller than the IEEE gradual underflow threshold
        were excluded from these statistics.

        Cephes Math Library, Release 2.8:  June, 2000
        Copyright 1984, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        // ReSharper restore CommentTypo
        [DST]
        public static double IncompleteBetaValue(double a, double b, double x)
        {
            if(a <= 0) throw new ArgumentOutOfRangeException(nameof(a), "a должна быть > 0");
            if(b <= 0) throw new ArgumentOutOfRangeException(nameof(b), "b должна быть > 0");
            if(x is < 0 or > 1) throw new ArgumentOutOfRangeException(nameof(x), "a должна быть в пределах [0;1]");

            const double big     = 4.503599627370496e15;
            const double big_inv = 2.22044604925031308085e-16;
            const double max_gam = 171.624376956302725;
            const double min_log = __LogMinRealNumber;
            const double max_log = __LogMaxRealNumber;

            if(Abs(x - 0) < Eps) return 0;
            if(Abs(x - 1) < Eps) return 1;
            var flag = 0;

            if(b * x <= 1 && x <= .95)
                return IncompleteBetaPowerSeries(a, b, x, max_gam);

            var w = 1 - x;

            double t;
            double xc;
            if (x <= a / (a + b)) xc = w;
            else
            {
                flag = 1;
                t    = a;
                a    = b;
                b    = t;
                xc   = x;
                x    = w;
            }

            if(flag == 1 && b * x <= 1 && x <= .95)
            {
                t = IncompleteBetaPowerSeries(a, b, x, max_gam);
                return 1 - (t <= Eps ? Eps : t);
            }

            var y = x * (a + b - 2) - (a - 1);
            w = y < 0 
                ? IncompleteBetaFractionExpansion(a, b, x, big, big_inv) 
                : IncompleteBetaFractionExpansion2(a, b, x, big, big_inv) / xc;

            y = a * Log(x);
            t = b * Log(xc);

            if(a + b < max_gam && Abs(y) < max_log && Abs(t) < max_log)
            {
                t = Pow(xc, b) * Pow(x, a) / a * w * Gamma.G(a + b) / (Gamma.G(a) * Gamma.G(b));
                return flag == 1 ? 1 - (t <= Eps ? Eps : t) : t;
            }

            y += t + Gamma.LnG(a + b, out _) - Gamma.LnG(a, out _) - Gamma.LnG(b, out _);
            y += Log(w / a);
            t =  y < min_log ? 0 : Exp(y);
            if(flag == 1)
                t = 1 - (t <= Eps ? Eps : t);
            return t;
        }


        // ReSharper disable CommentTypo
        /*************************************************************************
        Inverse of incomplete beta integral

        Given y, the function finds x such that

         incbet( a, b, x ) = y .

        The routine performs interval halving or Newton iterations to find the
        root of incbet(a,b,x) - y = 0.


        ACCURACY:

                             Relative error:
                       x     a,b
        arithmetic   domain  domain  # trials    peak       rms
           IEEE      0,1    .5,10000   50000    5.8e-12   1.3e-13
           IEEE      0,1   .25,100    100000    1.8e-13   3.9e-15
           IEEE      0,1     0,5       50000    1.1e-12   5.5e-15
        With a and b constrained to half-integer or integer values:
           IEEE      0,1    .5,10000   50000    5.8e-12   1.1e-13
           IEEE      0,1    .5,100    100000    1.7e-14   7.9e-16
        With a = .5, b constrained to half-integer or integer values:
           IEEE      0,1    .5,10000   10000    8.3e-11   1.0e-11

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1996, 2000 by Stephen L. Moshier
        *************************************************************************/
        // ReSharper restore CommentTypo

        [DST]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Стиль", "IDE0059:Ненужное присваивание значения", Justification = "<Ожидание>")]
        public static double IncompleteBetaInversed(double a, double b, double y)
        {
            //
            // special cases
            //
            if(Abs(y) < Eps) return 0;
            if(Abs(y - 1) < Eps) return 1;

            //
            // these initializations are not really necessary,
            // but without them compiler complains about 'possibly uninitialized variables'.
            //
            double dithresh = 0;
            var    rflg     = 0;
            double aaa      = 0;
            double bbb      = 0;
            double y0       = 0;
            double x        = 0;
            double yyy      = 0;
            double lgm      = 0;
            var    dir      = 0;
            double di       = 0;

            //
            // normal initializations
            //
            double    x0               = 0;
            double    yl               = 0;
            double    x1               = 1;
            double    yh               = 1;
            var       nflg             = 0;
            var       mainlooppos      = 0;
            const int ihalve           = 1;
            const int ihalvecycle      = 2;
            const int newt             = 3;
            const int newtcycle        = 4;
            const int breaknewtcycle   = 5;
            const int breakihalvecycle = 6;

            //
            // main loop
            //
            while(true)
            {
                //
                // start
                //
                double d;
                double yp;
                if (mainlooppos == 0)
                {
                    if(a <= 1 || b <= 1)
                    {
                        dithresh    = 1e-6;
                        rflg        = 0;
                        aaa         = a;
                        bbb         = b;
                        y0          = y;
                        x           = aaa / (aaa + bbb);
                        yyy         = IncompleteBetaValue(aaa, bbb, x);
                        mainlooppos = ihalve;
                        continue;
                    }

                    dithresh = 1e-4;
                    yp       = -Distribution.Normal.NormalDistributionInversed(y);

                    if(y > .5)
                    {
                        rflg = 1;
                        aaa  = b;
                        bbb  = a;
                        y0   = 1 - y;
                        yp   = -yp;
                    }
                    else
                    {
                        rflg = 0;
                        aaa  = a;
                        bbb  = b;
                        y0   = y;
                    }

                    lgm = (yp * yp - 3) / 6;
                    x   = 2 / (1 / (2 * aaa - 1) + 1 / (2 * bbb - 1));
                    d = yp * Sqrt(x + lgm) / x 
                        - (1 / (2 * bbb - 1) - 1 / (2 * aaa - 1)) 
                        * (lgm + 5d / 6 - 2 / (3 * x));

                    d *= 2;

                    if(d < Log(__MinRealNumber))
                    {
                        x = 0;
                        break;
                    }

                    x   = aaa / (aaa + bbb * Exp(d));
                    yyy = IncompleteBetaValue(aaa, bbb, x);
                    yp  = (yyy - y0) / y0;

                    if(Abs(yp) < .2)
                    {
                        mainlooppos = newt;
                        continue;
                    }
                    mainlooppos = ihalve;
                    continue;
                }

                //
                // ihalve
                //
                var i = 0;
                if(mainlooppos == ihalve)
                {
                    dir         = 0;
                    di          = .5;
                    mainlooppos = ihalvecycle;
                    continue;
                }

                //
                // ihalvecycle
                //
                if(mainlooppos == ihalvecycle)
                {
                    if(i <= 99)
                    {
                        if(i != 0)
                        {
                            x = x0 + di * (x1 - x0);

                            if(Abs(x - 1) < Eps)
                                x = 1 - Eps;

                            if(Abs(x) < Eps)
                            {
                                di = .5;
                                x  = x0 + di * (x1 - x0);

                                if(Abs(x) < Eps) break;
                            }

                            yyy = IncompleteBetaValue(aaa, bbb, x);
                            yp  = (x1 - x0) / (x1 + x0);

                            if(Abs(yp) < dithresh)
                            {
                                mainlooppos = newt;
                                continue;
                            }

                            yp = (yyy - y0) / y0;

                            if(Abs(yp) < dithresh)
                            {
                                mainlooppos = newt;
                                continue;
                            }
                        }

                        if(yyy < y0)
                        {
                            x0 = x;
                            yl = yyy;
                            if(dir >= 0)
                                di = dir > 3
                                    ? 1 - (1 - di) * (1 - di)
                                    : (dir > 1 ? .5 * di + .5 : (y0 - yyy) / (yh - yl));
                            else
                            {
                                dir = 0;
                                di  = .5;
                            }
                            dir++;
                            if(x0 > .75)
                            {
                                if(rflg == 1)
                                {
                                    rflg = 0;
                                    aaa  = a;
                                    bbb  = b;
                                    y0   = y;
                                }
                                else
                                {
                                    rflg = 1;
                                    aaa  = b;
                                    bbb  = a;
                                    y0   = 1 - y;
                                }
                                x           = 1 - x;
                                yyy         = IncompleteBetaValue(aaa, bbb, x);
                                x0          = 0;
                                yl          = 0;
                                x1          = 1;
                                yh          = 1;
                                mainlooppos = ihalve;
                                continue;
                            }
                        }
                        else
                        {
                            x1 = x;
                            if(rflg == 1 && x1 < Eps)
                            {
                                x = 0;
                                break;
                            }
                            yh = yyy;

                            if(dir <= 0) 
                                di = dir < -3 
                                    ? di * di 
                                    : (dir < -1 
                                        ? di * .5 
                                        : (yyy - y0) / (yh - yl));
                            else
                            {
                                dir = 0;
                                di  = .5;
                            }

                            dir--;
                        }
                        i++;
                        mainlooppos = ihalvecycle;
                        continue;
                    }
                    mainlooppos = breakihalvecycle;
                    continue;
                }

                // breakihalvecycle
                if(mainlooppos == breakihalvecycle)
                {
                    if(x0 >= 1)
                    {
                        x = 1 - Eps;
                        break;
                    }
                    if(x <= 0)
                    {
                        x = 0;
                        break;
                    }
                    mainlooppos = newt;
                    continue;
                }

                // newt
                if(mainlooppos == newt)
                {
                    if(nflg != 0) break;

                    nflg        = 1;
                    lgm         = Gamma.LnG(aaa + bbb, out _) 
                        - Gamma.LnG(aaa, out _) 
                        - Gamma.LnG(bbb, out _);
                    mainlooppos = newtcycle;
                    continue;
                }

                // newtcycle
                if(mainlooppos == newtcycle)
                {
                    if(i <= 7)
                    {
                        if(i != 0) yyy = IncompleteBetaValue(aaa, bbb, x);

                        if(yyy < yl)
                        {
                            x   = x0;
                            yyy = yl;
                        }
                        else
                        {
                            if(yyy > yh)
                            {
                                x   = x1;
                                yyy = yh;
                            }
                            else
                            {
                                if(yyy < y0)
                                {
                                    x0 = x;
                                    yl = yyy;
                                }
                                else
                                {
                                    x1 = x;
                                    yh = yyy;
                                }
                            }
                        }
                        if(Abs(x - 1) < Eps || Abs(x) < Eps)
                        {
                            mainlooppos = breaknewtcycle;
                            continue;
                        }

                        d = (aaa - 1) * Log(x) + (bbb - 1) * Log(1 - x) + lgm;

                        if(d < Log(__MinRealNumber)) break;

                        if(d > Log(__MaxRealNumber))
                        {
                            mainlooppos = breaknewtcycle;
                            continue;
                        }
                        d = (yyy - y0) / Exp(d);
                        var xt = x - d;

                        if(xt <= x0)
                        {
                            yyy = (x - x0) / (x1 - x0);
                            xt  = x0 + .5 * yyy * (x - x0);

                            if(xt <= 0)
                            {
                                mainlooppos = breaknewtcycle;
                                continue;
                            }
                        }

                        if(xt >= x1)
                        {
                            yyy = (x1 - x) / (x1 - x0);
                            xt  = x1 - .5 * yyy * (x1 - x);
                            if(xt >= 1)
                            {
                                mainlooppos = breaknewtcycle;
                                continue;
                            }
                        }

                        x = xt;
                        if(Abs(d / x) < 128 * Eps) break;
                        i++;
                        mainlooppos = newtcycle;
                        continue;
                    }
                    mainlooppos = breaknewtcycle;
                    continue;
                }

                //
                // breaknewtcycle
                //
                if(mainlooppos != breaknewtcycle) continue;

                dithresh    = 256 * Eps;
                mainlooppos = ihalve;
            }

            //
            // done
            //
            return rflg == 0 ? x : (x <= Eps ? 1 - Eps : 1 - x);
        }


        /*************************************************************************
        Continued fraction expansion #1 for incomplete beta integral

        Cephes Math Library, Release 2.8:  June, 2000
        Copyright 1984, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/

        private static double IncompleteBetaFractionExpansion(double a, double b, double x, double big, double biginv)
        {
            var k1   = a;
            var k2   = a + b;
            var k3   = a;
            var k4   = a + 1;
            var k5   = 1.0;
            var k6   = b - 1;
            var k7   = k4;
            var k8   = a + 2;
            var pkm2 = 0.0;
            var qkm2 = 1.0;
            var pkm1 = 1.0;
            var qkm1 = 1.0;
            var ans  = 1.0;
            var r    = 1.0;
            var n    = 0;

            const double thresh = 3 * Eps;
            do
            {
                var xk = -(x * k1 * k2 / (k3 * k4));
                var pk = pkm1 + pkm2 * xk;
                var qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                xk   = x * k5 * k6 / (k7 * k8);
                pk   = pkm1 + pkm2 * xk;
                qk   = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                if(Abs(qk) > Eps)
                    r = pk / qk;
                double t;
                if(Abs(r) < Eps) t = 1;
                else
                {
                    t   = Abs((ans - r) / r);
                    ans = r;
                }

                if(t < thresh) break;

                k1 += 1;
                k2 += 1;
                k3 += 2;
                k4 += 2;
                k5 += 1;
                k6 += 1;
                k7 += 2;
                k8 += 2;

                if(Abs(qk) + Abs(pk) > big)
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }

                if(Abs(qk) >= biginv && Abs(pk) >= biginv) continue;

                pkm2 *= big;
                pkm1 *= big;
                qkm2 *= big;
                qkm1 *= big;
            } while(++n != 300);
            return ans;
        }


        /*************************************************************************
        Continued fraction expansion #2
        for incomplete beta integral

        Cephes Math Library, Release 2.8:  June, 2000
        Copyright 1984, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        private static double IncompleteBetaFractionExpansion2(double a, double b, double x, double big, double biginv)
        {
            var          k1     = a;
            var          k2     = b - 1;
            var          k3     = a;
            var          k4     = a + 1;
            var          k5     = 1.0;
            var          k6     = a + b;
            var          k7     = a + 2;
            var          k8     = a + 2;
            var          pkm2   = 0.0;
            var          qkm2   = 1.0;
            var          pkm1   = 1.0;
            var          qkm1   = 1.0;
            var          z      = x / (1 - x);
            var          ans    = 1.0;
            var          r      = 1.0;
            var          n      = 0;
            const double thresh = 3 * Eps;

            do
            {
                var xk = -(z * k1 * k2 / (k3 * k4));
                var pk = pkm1 + pkm2 * xk;
                var qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                xk   = z * k5 * k6 / (k7 * k8);
                pk   = pkm1 + pkm2 * xk;
                qk   = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                if(Abs(qk) > Eps) r = pk / qk;
                double t;

                if(Abs(r) < Eps) t = 1;
                else
                {
                    t   = Abs((ans - r) / r);
                    ans = r;
                }

                if(t < thresh) break;

                k1 += 1;
                k2 += 1;
                k3 += 2;
                k4 += 2;
                k5 += 1;
                k6 += 1;
                k7 += 2;
                k8 += 2;

                if(Abs(qk) + Abs(pk) > big)
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }
                if(Abs(qk) >= biginv && Abs(pk) >= biginv) continue;

                pkm2 *= big;
                pkm1 *= big;
                qkm2 *= big;
                qkm1 *= big;
            } while(++n != 300);
            return ans;
        }


        /*************************************************************************
        GetPower series for incomplete beta integral.
        Use when b*x is small and x not too close to 1.

        Cephes Math Library, Release 2.8:  June, 2000
        Copyright 1984, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        private static double IncompleteBetaPowerSeries(double a, double b, double x, double maxgam)
        {
            var ai = 1 / a;
            var u  = (1 - b) * x;
            var v  = u / (a + 1);
            var t1 = v;
            var t  = u;
            var n  = 2.0;
            var s  = 0.0;
            var z  = Eps * ai;
            while(Abs(v) > z)
            {
                u =  (n - b) * x / n;
                t *= u;
                v =  t / (a + n);
                s += v;
                n++;
            }
            s += t1;
            s += ai;
            u =  a * Log(x);
            if((a + b) < maxgam && Abs(u) < Log(__MaxRealNumber))
            {
                t =  Gamma.G(a + b) / (Gamma.G(a) * Gamma.G(b));
                s *= t * Pow(x, a);
            }
            else
            {
                t = Gamma.LnG(a + b, out _) - Gamma.LnG(a, out _) - Gamma.LnG(b, out _) + u + Log(s);
                s = t < Log(__MinRealNumber) ? 0 : Exp(t);
            }
            return s;
        }
    }
}