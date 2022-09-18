using System;

// ReSharper disable IdentifierTypo

namespace MathCore;

public static partial class SpecialFunctions
{
    /// <summary>Класс функций Бесселя</summary>
    // ReSharper disable once CommentTypo
    //[Copyright("1984, 1987, 1989, 2000 by Stephen L. Moshier")]
    public static class Bessel
    {
        public static Polynom Polynom(int N)
        {
            if (N == 0) throw new ArgumentOutOfRangeException(nameof(N), 0, "Порядок полинома не должен быть равен 0");
            //if (N < 0) return PolynomInverted(-N);

            var inversed = N < 0;
            N = Math.Abs(N);

            var a = new double[N + 1];
            if (inversed)
                a[N] = 1;
            else
                a[0] = 1;

            //https/en.wikipedia.org/wiki/Bessel_polynomials
            static double Ratio(int n, int k) => (n * n + n + k - k * k) / 2d / k;

            if (inversed)
                for (var k = 1; k <= N; k++)
                    a[N - k] = a[N - k + 1] * Ratio(N, k);
            else
                for (var k = 1; k <= N; k++)
                    a[k] = a[k - 1] * Ratio(N, k);

            // y3(x) = 15x3 + 15x2 + 6x + 1
            // inv_y3(x) = x3 + 6x2 + 15x + 15
            return new(a);
        }

        private static void BesselAsymptote0(double x, out double P, out double Q)
        {
            var xsq = 64 / (x * x);
            var p2  = .0;
            p2 = 2485.271928957404011288128951 + xsq * p2;
            p2 = 153982.6532623911470917825993 + xsq * p2;
            p2 = 2016135.283049983642487182349 + xsq * p2;
            p2 = 8413041.456550439208464315611 + xsq * p2;
            p2 = 12332384.76817638145232406055 + xsq * p2;
            p2 = 5393485.083869438325262122897 + xsq * p2;
            var q2 = 1.0;
            q2 = 2615.700736920839685159081813 + xsq * q2;
            q2 = 156001.7276940030940592769933 + xsq * q2;
            q2 = 2025066.801570134013891035236 + xsq * q2;
            q2 = 8426449.050629797331554404810 + xsq * q2;
            q2 = 12338310.22786324960844856182 + xsq * q2;
            q2 = 5393485.083869438325560444960 + xsq * q2;
            var p3 = -.0;
            p3 = -4.887199395841261531199129300 + xsq * p3;
            p3 = -226.2630641933704113967255053 + xsq * p3;
            p3 = -2365.956170779108192723612816 + xsq * p3;
            p3 = -8239.066313485606568803548860 + xsq * p3;
            p3 = -10381.41698748464093880530341 + xsq * p3;
            p3 = -3984.617357595222463506790588 + xsq * p3;
            var q3 = 1.0;
            q3 = 408.7714673983499223402830260 + xsq * q3;
            q3 = 15704.89191515395519392882766 + xsq * q3;
            q3 = 156021.3206679291652539287109 + xsq * q3;
            q3 = 533291.3634216897168722255057 + xsq * q3;
            q3 = 666745.4239319826986004038103 + xsq * q3;
            q3 = 255015.5108860942382983170882 + xsq * q3;
            P  = p2 / q2;
            Q  = 8 * p3 / q3 / x;
        }

        private static void BesselAsymptote1(double x, out double P, out double Q)
        {
            var xsq = 64 / (x * x);
            var p2  = -1611.616644324610116477412898;
            p2 = -109824.0554345934672737413139 + xsq * p2;
            p2 = -1523529.351181137383255105722 + xsq * p2;
            p2 = -6603373.248364939109255245434 + xsq * p2;
            p2 = -9942246.505077641195658377899 + xsq * p2;
            p2 = -4435757.816794127857114720794 + xsq * p2;

            var q2 = 1.0;
            q2 = -1455.009440190496182453565068 + xsq * q2;
            q2 = -107263.8599110382011903063867 + xsq * q2;
            q2 = -1511809.506634160881644546358 + xsq * q2;
            q2 = -6585339.479723087072826915069 + xsq * q2;
            q2 = -9934124.389934585658967556309 + xsq * q2;
            q2 = -4435757.816794127856828016962 + xsq * q2;

            var p3 = 35.26513384663603218592175580;
            p3 = 1706.375429020768002061283546 + xsq * p3;
            p3 = 18494.26287322386679652009819 + xsq * p3;
            p3 = 66178.83658127083517939992166 + xsq * p3;
            p3 = 85145.16067533570196555001171 + xsq * p3;
            p3 = 33220.91340985722351859704442 + xsq * p3;

            var q3 = 1.0;
            q3 = 863.8367769604990967475517183 + xsq * q3;
            q3 = 37890.22974577220264142952256 + xsq * q3;
            q3 = 400294.4358226697511708610813 + xsq * q3;
            q3 = 1419460.669603720892855755253 + xsq * q3;
            q3 = 1819458.042243997298924553839 + xsq * q3;
            q3 = 708712.8194102874357377502472 + xsq * q3;

            P = p2 / q2;
            Q = 8 * p3 / q3 / x;
        }

        private static void BesselMFirstCheb(double c, ref double b0, ref double b1, ref double b2) { b0 = c; b1 = 0; b2 = 0; }

        private static void BesselMnExtCheb(double x, double c, ref double b0, ref double b1, ref double b2)
        {
            b2 = b1;
            b1 = b0;
            b0 = x * b1 - b2 + c;
        }

        private static void BesselM1FirstCheb(double c, ref double b0, ref double b1, ref double b2) { b0 = c; b1 = 0; b2 = 0; }

        private static void BesselM1NextCheb(double x, double c, ref double b0, ref double b1, ref double b2)
        {
            b2 = b1;
            b1 = b0;
            b0 = x * b1 - b2 + c;
        }

        /// <summary>Функция Бесселя 0 порядка</summary>
        /// <param name="x">Аргумент</param>
        /// <returns>Значение функции Бесселя нулевого порядка</returns>
        [DST]
        public static double J0(double x)
        {
            x = Math.Abs(x);

            if (x > 8)
            {
                BesselAsymptote0(x, out var p0, out var q0);
                var nn = x - Consts.pi / 4;
                return Math.Sqrt(2 / Consts.pi / x) * (p0 * Math.Cos(nn) - q0 * Math.Sin(nn));
            }

            var xsq = x * x;

            var p1 = 26857.86856980014981415848441;
            p1 = -40504123.71833132706360663322 + xsq * p1;
            p1 = 25071582855.36881945555156435 + xsq * p1;
            p1 = -8085222034853.793871199468171 + xsq * p1;
            p1 = 1434354939140344.111664316553 + xsq * p1;
            p1 = -136762035308817138.6865416609 + xsq * p1;
            p1 = 6382059341072356562.289432465 + xsq * p1;
            p1 = -117915762910761053603.8440800 + xsq * p1;
            p1 = 493378725179413356181.6813446 + xsq * p1;

            var q1 = 1.0;
            q1 = 1363.063652328970604442810507 + xsq * q1;
            q1 = 1114636.098462985378182402543 + xsq * q1;
            q1 = 669998767.2982239671814028660 + xsq * q1;
            q1 = 312304311494.1213172572469442 + xsq * q1;
            q1 = 112775673967979.8507056031594 + xsq * q1;
            q1 = 30246356167094626.98627330784 + xsq * q1;
            q1 = 5428918384092285160.200195092 + xsq * q1;
            q1 = 493378725179413356211.3278438 + xsq * q1;

            return p1 / q1;
        }

        /// <summary>Функция Бесселя 1 порядка</summary>
        /// <param name="x">Аргумент функции</param>
        /// <returns>Значение функции Бесселя первого порядка</returns>
        [DST]
        public static double J1(double x)
        {
            double s = Math.Sign(x);

            x = Math.Abs(x);

            if (x > 8)
            {
                BesselAsymptote1(x, out var p0, out var q0);
                var nn            = x - 3 * Math.PI / 4;
                var result        = Math.Sqrt(2 / Consts.pi / x) * (p0 * Math.Cos(nn) - q0 * Math.Sin(nn));
                if (s < 0) result = -result;
                return result;
            }

            var xsq = x * x;

            var p1 = 2701.122710892323414856790990;
            p1 = -4695753.530642995859767162166 + xsq * p1;
            p1 = 3413234182.301700539091292655 + xsq * p1;
            p1 = -1322983480332.126453125473247 + xsq * p1;
            p1 = 290879526383477.5409737601689 + xsq * p1;
            p1 = -35888175699101060.50743641413 + xsq * p1;
            p1 = 2316433580634002297.931815435 + xsq * p1;
            p1 = -66721065689249162980.20941484 + xsq * p1;
            p1 = 581199354001606143928.050809 + xsq * p1;

            var q1 = 1.0;
            q1 = 1606.931573481487801970916749 + xsq * q1;
            q1 = 1501793.594998585505921097578 + xsq * q1;
            q1 = 1013863514.358673989967045588 + xsq * q1;
            q1 = 524371026216.7649715406728642 + xsq * q1;
            q1 = 208166122130760.7351240184229 + xsq * q1;
            q1 = 60920613989175217.46105196863 + xsq * q1;
            q1 = 11857707121903209998.37113348 + xsq * q1;
            q1 = 1162398708003212287858.529400 + xsq * q1;

            return s * x * p1 / q1;
        }

        /// <summary>Функция Бесселя n порядка</summary>
        /// <param name="n">Порядок функции Бесселя</param>
        /// <param name="x">Аргумент Функции Бесселя</param>
        /// <returns>Значение функции Бесселя n порядка</returns>
        [DST]
        public static double Jn(int n, double x)
        {
            int sg;

            if (n >= 0) sg = 1;
            else
            {
                n  = -n;
                sg = n % 2 == 0 ? 1 : -1;
            }

            if (x < 0)
            {
                if (n % 2 != 0) sg = -sg;
                x = -x;
            }

            if (n == 0) return sg * J0(x);

            if (n == 1) return sg * J1(x);

            if (n == 2) return Math.Abs(x - 0) < Eps ? 0 : sg * 2 * J1(x) / x - J0(x);

            if (x < Eps) return 0;

            var k   = 53;
            var pk  = 2d * (n + k);
            var ans = pk;
            var xk  = x * x;

            do
            {
                pk  -= 2;
                ans =  pk - xk / ans;
                k--;
            } while (k != 0);

            ans = x / ans;
            pk  = 1;
            var pkm1 = 1 / ans;
            k = n - 1;
            var r = 2d * k;

            do
            {
                var pkm2 = (pkm1 * r - pk * x) / x;
                pk   =  pkm1;
                pkm1 =  pkm2;
                r    -= 2;
                k--;
            } while (k != 0);

            return sg * (Math.Abs(pk) > Math.Abs(pkm1) ? J1(x) / pk : J0(x) / pkm1);
        }

        /// <summary>Функция Бесселя второго типа, нулевого порядка</summary>
        /// <param name="x">Аргумент функции Бесселя второго типа, нулевого порядка</param>
        /// <returns>Значение функции Бесселя второго типа, нулевого порядка</returns>
        [DST]
        public static double Y0(double x)
        {
            if (x > 8)
            {
                BesselAsymptote0(x, out var p0, out var q0);
                var nn = x - Math.PI / 4;
                return Math.Sqrt(2 / Consts.pi / x) * (p0 * Math.Sin(nn) + q0 * Math.Cos(nn));
            }

            var xsq = x * x;

            var p4 = -41370.35497933148554125235152;
            p4 = 59152134.65686889654273830069 + xsq * p4;
            p4 = -34363712229.79040378171030138 + xsq * p4;
            p4 = 10255208596863.94284509167421 + xsq * p4;
            p4 = -1648605817185729.473122082537 + xsq * p4;
            p4 = 137562431639934407.8571335453 + xsq * p4;
            p4 = -5247065581112764941.297350814 + xsq * p4;
            p4 = 65874732757195549259.99402049 + xsq * p4;
            p4 = -27502866786291095837.01933175 + xsq * p4;

            var q4 = 1.0;
            q4 = 1282.452772478993804176329391 + xsq * q4;
            q4 = 1001702.641288906265666651753 + xsq * q4;
            q4 = 579512264.0700729537480087915 + xsq * q4;
            q4 = 261306575504.1081249568482092 + xsq * q4;
            q4 = 91620380340751.85262489147968 + xsq * q4;
            q4 = 23928830434997818.57439356652 + xsq * q4;
            q4 = 4192417043410839973.904769661 + xsq * q4;
            q4 = 372645883898616588198.9980 + xsq * q4;

            return p4 / q4 + 2 / Consts.pi * J0(x) * Math.Log(x);
        }

        /// <summary>Функция Бесселя второго типа, первого порядка</summary>
        /// <param name="x">Аргумент функции Бесселя второго типа, первого порядка</param>
        /// <returns>Значение функции Бесселя второго типа, первого порядка</returns>
        [DST]
        public static double Y1(double x)
        {
            if (x > 8)
            {
                BesselAsymptote1(x, out var p0, out var q0);
                var nn = x - 3 * Consts.pi / 4;
                return Math.Sqrt(2 / Consts.pi / x) * (p0 * Math.Sin(nn) + q0 * Math.Cos(nn));
            }

            var xsq = x * x;

            var p4 = -2108847.540133123652824139923;
            p4 = 3639488548.124002058278999428 + xsq * p4;
            p4 = -2580681702194.450950541426399 + xsq * p4;
            p4 = 956993023992168.3481121552788 + xsq * p4;
            p4 = -196588746272214065.8820322248 + xsq * p4;
            p4 = 21931073399177975921.11427556 + xsq * p4;
            p4 = -1212297555414509577913.561535 + xsq * p4;
            p4 = 26554738314348543268942.48968 + xsq * p4;
            p4 = -99637534243069222259967.44354 + xsq * p4;

            var q4 = 1.0;
            q4 = 1612.361029677000859332072312 + xsq * q4;
            q4 = 1563282.754899580604737366452 + xsq * q4;
            q4 = 1128686837.169442121732366891 + xsq * q4;
            q4 = 646534088126.5275571961681500 + xsq * q4;
            q4 = 297663212564727.6729292742282 + xsq * q4;
            q4 = 108225825940881955.2553850180 + xsq * q4;
            q4 = 29549879358971486742.90758119 + xsq * q4;
            q4 = 5435310377188854170800.653097 + xsq * q4;
            q4 = 508206736694124324531442.4152 + xsq * q4;

            return x * p4 / q4 + 2 / Consts.pi * (J1(x) * Math.Log(x) - 1 / x);
        }

        /// <summary>Функция Бесселя второго типа, n порядка</summary>
        /// <param name="n">Порядок функции Бесселя второго типа</param>
        /// <param name="x">Аргумент функции Бесселя второго типа, n порядка</param>
        /// <returns>Значение функции Бесселя второго типа, n порядка</returns>
        [DST]
        public static double Yn(int n, double x)
        {
            double s = 1;

            if (n < 0)
                if ((n = -n) % 2 != 0)
                    s = -1;

            if (n == 0)
                return Y0(x);

            if (n == 1)
                return s * Y1(x);

            var a = Y0(x);
            var b = Y1(x);
            for (var i = 1; i <= n - 1; i++)
            {
                var tmp = b;
                b = 2 * i / x * b - a;
                a = tmp;
            }
            return s * b;
        }

        /// <summary>Модифицированная функция Бесселя нулевого порядка </summary>
        /// <param name="x">Аргумент модифицированной функции Бесселя нулевого порядка</param>
        /// <returns>Значение модифицированной функции Бесселя нулевого порядка</returns>
        [DST]
        public static double I0(double x)
        {
            if (x < 0) x = -x;

            double b0 = 0;
            double b1 = 0;
            double b2 = 0;

            if (x <= 8)
            {
                var y = .5 * x - 2;
                BesselMFirstCheb(-4.41534164647933937950E-18, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 3.33079451882223809783E-17, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, -2.43127984654795469359E-16, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 1.71539128555513303061E-15, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, -1.16853328779934516808E-14, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 7.67618549860493561688E-14, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, -4.85644678311192946090E-13, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 2.95505266312963983461E-12, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, -1.72682629144155570723E-11, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 9.67580903537323691224E-11, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, -5.18979560163526290666E-10, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 2.65982372468238665035E-9, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, -1.30002500998624804212E-8, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 6.04699502254191894932E-8, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, -2.67079385394061173391E-7, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 1.11738753912010371815E-6, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, -4.41673835845875056359E-6, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 1.64484480707288970893E-5, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, -5.75419501008210370398E-5, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 1.88502885095841655729E-4, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, -5.76375574538582365885E-4, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 1.63947561694133579842E-3, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, -4.32430999505057594430E-3, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 1.05464603945949983183E-2, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, -2.37374148058994688156E-2, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 4.93052842396707084878E-2, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, -9.49010970480476444210E-2, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 1.71620901522208775349E-1, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, -3.04682672343198398683E-1, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 6.76795274409476084995E-1, ref b0, ref b1, ref b2);
                return Math.Exp(x) * (.5 * (b0 - b2));
            }

            var z = 32 / x - 2;
            BesselMFirstCheb(-7.23318048787475395456E-18, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -4.83050448594418207126E-18, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 4.46562142029675999901E-17, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 3.46122286769746109310E-17, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -2.82762398051658348494E-16, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -3.42548561967721913462E-16, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 1.77256013305652638360E-15, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 3.81168066935262242075E-15, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -9.55484669882830764870E-15, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -4.15056934728722208663E-14, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 1.54008621752140982691E-14, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 3.85277838274214270114E-13, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 7.18012445138366623367E-13, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -1.79417853150680611778E-12, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -1.32158118404477131188E-11, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -3.14991652796324136454E-11, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 1.18891471078464383424E-11, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 4.94060238822496958910E-10, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 3.39623202570838634515E-9, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 2.26666899049817806459E-8, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 2.04891858946906374183E-7, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 2.89137052083475648297E-6, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 6.88975834691682398426E-5, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 3.36911647825569408990E-3, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 8.04490411014108831608E-1, ref b0, ref b1, ref b2);
            return Math.Exp(x) * (.5 * (b0 - b2)) / Math.Sqrt(x);
        }

        /// <summary>Модифицированная функция Бесселя первого порядка </summary>
        /// <param name="x">Аргумент модифицированной функции Бесселя первого порядка</param>
        /// <returns>Значение модифицированной функции Бесселя первого порядка</returns>
        [DST]
        public static double I1(double x)
        {
            double y;
            double b0 = 0;
            double b1 = 0;
            double b2 = 0;

            var z = Math.Abs(x);
            if (z <= 8)
            {
                y = z / 2 - 2;
                BesselM1FirstCheb(2.77791411276104639959E-18, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -2.11142121435816608115E-17, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 1.55363195773620046921E-16, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -1.10559694773538630805E-15, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 7.60068429473540693410E-15, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -5.04218550472791168711E-14, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 3.22379336594557470981E-13, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -1.98397439776494371520E-12, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 1.17361862988909016308E-11, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -6.66348972350202774223E-11, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 3.62559028155211703701E-10, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -1.88724975172282928790E-9, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 9.38153738649577178388E-9, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -4.44505912879632808065E-8, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 2.00329475355213526229E-7, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -8.56872026469545474066E-7, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 3.47025130813767847674E-6, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -1.32731636560394358279E-5, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 4.78156510755005422638E-5, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -1.61760815825896745588E-4, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 5.12285956168575772895E-4, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -1.51357245063125314899E-3, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 4.15642294431288815669E-3, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -1.05640848946261981558E-2, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 2.47264490306265168283E-2, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -5.29459812080949914269E-2, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 1.02643658689847095384E-1, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -1.76416518357834055153E-1, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 2.52587186443633654823E-1, ref b0, ref b1, ref b2);
                z = (.5 * (b0 - b2)) * z * Math.Exp(z);
            }
            else
            {
                y = 32 / z - 2;
                BesselM1FirstCheb(7.51729631084210481353E-18, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 4.41434832307170791151E-18, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -4.65030536848935832153E-17, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -3.20952592199342395980E-17, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 2.96262899764595013876E-16, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 3.30820231092092828324E-16, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -1.88035477551078244854E-15, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -3.81440307243700780478E-15, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 1.04202769841288027642E-14, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 4.27244001671195135429E-14, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -2.10154184277266431302E-14, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -4.08355111109219731823E-13, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -7.19855177624590851209E-13, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 2.03562854414708950722E-12, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 1.41258074366137813316E-11, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 3.25260358301548823856E-11, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -1.89749581235054123450E-11, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -5.58974346219658380687E-10, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -3.83538038596423702205E-9, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -2.63146884688951950684E-8, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -2.51223623787020892529E-7, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -3.88256480887769039346E-6, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -1.10588938762623716291E-4, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -9.76109749136146840777E-3, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 7.78576235018280120474E-1, ref b0, ref b1, ref b2);
                z = (.5 * (b0 - b2)) * Math.Exp(z) / Math.Sqrt(z);
            }

            return x < 0 ? -z : z;
        }

        /// <summary>Модифицированная функция Бесселя второго типа, нулевого порядка </summary>
        /// <param name="x">Аргумент модифицированной функции Бесселя второго типа, нулевого порядка</param>
        /// <returns>Значение модифицированной функции Бесселя второго типа, нулевого порядка</returns>
        [DST]
        public static double K0(double x)
        {
            double b0 = 0;
            double b1 = 0;
            double b2 = 0;

            if (x <= 2)
            {
                var y = x * x - 2;
                BesselMFirstCheb(1.37446543561352307156E-16, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 4.25981614279661018399E-14, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 1.03496952576338420167E-11, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 1.90451637722020886025E-9, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 2.53479107902614945675E-7, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 2.28621210311945178607E-5, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 1.26461541144692592338E-3, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 3.59799365153615016266E-2, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, 3.44289899924628486886E-1, ref b0, ref b1, ref b2);
                BesselMnExtCheb(y, -5.35327393233902768720E-1, ref b0, ref b1, ref b2);
                return (.5 * (b0 - b2)) - Math.Log(.5 * x) * I0(x);
            }

            var z = 8 / x - 2;
            BesselMFirstCheb(5.30043377268626276149E-18, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -1.64758043015242134646E-17, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 5.21039150503902756861E-17, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -1.67823109680541210385E-16, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 5.51205597852431940784E-16, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -1.84859337734377901440E-15, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 6.34007647740507060557E-15, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -2.22751332699166985548E-14, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 8.03289077536357521100E-14, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -2.98009692317273043925E-13, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 1.14034058820847496303E-12, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -4.51459788337394416547E-12, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 1.85594911495471785253E-11, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -7.95748924447710747776E-11, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 3.57739728140030116597E-10, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -1.69753450938905987466E-9, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 8.57403401741422608519E-9, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -4.66048989768794782956E-8, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 2.76681363944501510342E-7, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -1.83175552271911948767E-6, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 1.39498137188764993662E-5, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -1.28495495816278026384E-4, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 1.56988388573005337491E-3, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, -3.14481013119645005427E-2, ref b0, ref b1, ref b2);
            BesselMnExtCheb(z, 2.44030308206595545468E0, ref b0, ref b1, ref b2);
            return (.5 * (b0 - b2)) * Math.Exp(-x) / Math.Sqrt(x);
        }

        /// <summary>Модифицированная функция Бесселя второго типа, первого порядка </summary>
        /// <param name="x">Аргумент модифицированной функции Бесселя второго типа, первого порядка</param>
        /// <returns>Значение модифицированной функции Бесселя второго типа, первого порядка</returns>
        /// <exception cref="ArgumentOutOfRangeException">x меньше 0</exception>
        [DST]
        public static double K1(double x)
        {
            double b0 = 0;
            double b1 = 0;
            double b2 = 0;

            if (x <= 2)
            {
                var y = x * x - 2;
                BesselM1FirstCheb(-7.02386347938628759343E-18, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -2.42744985051936593393E-15, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -6.66690169419932900609E-13, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -1.41148839263352776110E-10, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -2.21338763073472585583E-8, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -2.43340614156596823496E-6, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -1.73028895751305206302E-4, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -6.97572385963986435018E-3, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -1.22611180822657148235E-1, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, -3.53155960776544875667E-1, ref b0, ref b1, ref b2);
                BesselM1NextCheb(y, 1.52530022733894777053E0, ref b0, ref b1, ref b2);
                return Math.Log(.5 * x) * I1(x) + (.5 * (b0 - b2)) / x;
            }

            var z = 8 / x - 2;
            BesselM1FirstCheb(-5.75674448366501715755E-18, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, 1.79405087314755922667E-17, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, -5.68946255844285935196E-17, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, 1.83809354436663880070E-16, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, -6.05704724837331885336E-16, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, 2.03870316562433424052E-15, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, -7.01983709041831346144E-15, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, 2.47715442448130437068E-14, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, -8.97670518232499435011E-14, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, 3.34841966607842919884E-13, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, -1.28917396095102890680E-12, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, 5.13963967348173025100E-12, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, -2.12996783842756842877E-11, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, 9.21831518760500529508E-11, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, -4.19035475934189648750E-10, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, 2.01504975519703286596E-9, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, -1.03457624656780970260E-8, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, 5.74108412545004946722E-8, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, -3.50196060308781257119E-7, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, 2.40648494783721712015E-6, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, -1.93619797416608296024E-5, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, 1.95215518471351631108E-4, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, -2.85781685962277938680E-3, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, 1.03923736576817238437E-1, ref b0, ref b1, ref b2);
            BesselM1NextCheb(z, 2.72062619048444266945E0, ref b0, ref b1, ref b2);
            return Math.Exp(-x) * (.5 * (b0 - b2)) / Math.Sqrt(x);
        }

        /// <summary>Модифицированная функция Бесселя второго типа, n порядка </summary>
        /// <param name="n">Порядок модифицированной функции Бесселя второго типа</param>
        /// <param name="x">Аргумент модифицированной функции Бесселя второго типа, n порядка</param>
        /// <returns>Значение модифицированной функции Бесселя второго типа, n порядка</returns>
        /// <exception cref="ArgumentOutOfRangeException">при x меньше, либо = 0</exception>
        /// <exception cref="ArgumentOutOfRangeException">|n| больше 31</exception>
        /// <exception cref="OverflowException"></exception>
        [DST]
        public static double Kn(int n, double x)
        {
            var    nn = Math.Abs(n);
            double k;
            double nk1f;
            double t;
            double s;
            double z0;
            double z;
            double fn;
            double pn;
            double pk;
            int    i;

            const double eul = 5.772156649015328606065e-1;


            if (x <= 9.55)
            {
                var ans = .0;
                z0 = .25 * x * x;
                fn = 1;
                pn = 0;
                var zmn = 1.0;
                var tox = 2 / x;

                if (nn > 0)
                {
                    pn = -eul;
                    k  = 1.0;
                    for (i = 1; i < nn; i++)
                    {
                        pn += 1 / k;
                        k++;
                        fn *= k;
                    }

                    zmn = tox;
                    if (nn == 1)
                        ans = 1 / x;
                    else
                    {
                        nk1f = fn / nn;
                        var kf = 1.0;
                        s = nk1f;
                        z = -z0;
                        var zn = 1.0;

                        for (i = 1; i < nn; i++)
                        {
                            nk1f /= nn - i;
                            kf   *= i;
                            zn   *= z;
                            t    =  nk1f * zn / kf;
                            s    += t;

                            if (__MaxRealNumber - Math.Abs(t) <= Math.Abs(s))
                                throw new OverflowException();

                            if (tox > 1 & __MaxRealNumber / tox < zmn)
                                throw new OverflowException();

                            zmn *= tox;
                        }

                        s *= .5;
                        t =  Math.Abs(s);

                        if (zmn > 1 & __MaxRealNumber / zmn < t)
                            throw new OverflowException();

                        if (t > 1 && __MaxRealNumber / t < zmn)
                            throw new OverflowException();

                        ans = s * zmn;
                    }
                }

                var tlg = 2 * Math.Log(.5 * x);
                pk = -eul;

                if (nn == 0)
                {
                    pn = pk;
                    t  = 1;
                }
                else
                {
                    pn += 1.0 / nn;
                    t  =  1 / fn;
                }

                s = (pk + pn - tlg) * t;
                k = 1;

                do
                {
                    t  *= z0 / (k * (k + nn));
                    pk += 1 / k;
                    pn += 1 / (k + nn);
                    s  += (pk + pn - tlg) * t;
                    k++;
                } while (Math.Abs(t / s) > Eps);

                s *= .5 / zmn;
                if (nn % 2 != 0)
                    s = -s;

                return ans + s;
            }
            if (x > Math.Log(__MaxRealNumber))
                return 0;

            k  = nn;
            pn = 4 * k * k;
            z0 = 8 * x;
            pk = fn = s = t = 1;
            var nkf = __MaxRealNumber;
            i = 0;

            do
            {
                z    =  pn - pk * pk;
                t    *= z / (fn * z0);
                nk1f =  Math.Abs(t);

                if (i >= nn & nk1f > nkf)
                    break;

                nkf =  nk1f;
                s   += t;
                fn++;
                pk += 2;
                i++;
            } while (Math.Abs(t / s) > Eps);

            return Math.Exp(-x) * Math.Sqrt(Consts.pi / (2 * x)) * s;
        }
    }
}