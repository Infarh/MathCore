using static System.Math;

// ReSharper disable InconsistentNaming

namespace MathCore;

public static partial class SpecialFunctions
{
    public static class Erf
    {
        public static double Inv(double z)
        {
            if (z == 0.0) return 0.0;

            if (z >= 1.0) return double.PositiveInfinity;

            if (z <= -1.0) return double.NegativeInfinity;

            double p, q, s;
            if (z < 0)
            {
                p = -z;
                q = 1 - p;
                s = -1;
            }
            else
            {
                p = z;
                q = 1 - z;
                s = 1;
            }

            return InvImpl(p, q, s);
        }

        private static readonly double[] An = [-0.000508781949658280665617, -0.00836874819741736770379, 0.0334806625409744615033, -0.0126926147662974029034, -0.0365637971411762664006, 0.0219878681111168899165, 0.00822687874676915743155, -0.00538772965071242932965];
        private static readonly double[] Ad = [1, -0.970005043303290640362, -1.56574558234175846809, 1.56221558398423026363, 0.662328840472002992063, -0.71228902341542847553, -0.0527396382340099713954, 0.0795283687341571680018, -0.00233393759374190016776, 0.000886216390456424707504];
        private static readonly double[] Bn = [-0.202433508355938759655, 0.105264680699391713268, 8.37050328343119927838, 17.6447298408374015486, -18.8510648058714251895, -44.6382324441786960818, 17.445385985570866523, 21.1294655448340526258, -3.67192254707729348546];
        private static readonly double[] Bd = [1, 6.24264124854247537712, 3.9713437953343869095, -28.6608180499800029974, -20.1432634680485188801, 48.5609213108739935468, 10.8268667355460159008, -22.6436933413139721736, 1.72114765761200282724];
        private static readonly double[] Cn = [-0.131102781679951906451, -0.163794047193317060787, 0.117030156341995252019, 0.387079738972604337464, 0.337785538912035898924, 0.142869534408157156766, 0.0290157910005329060432, 0.00214558995388805277169, -0.679465575181126350155e-6, 0.285225331782217055858e-7, -0.681149956853776992068e-9];
        private static readonly double[] Cd = [1, 3.46625407242567245975, 5.38168345707006855425, 4.77846592945843778382, 2.59301921623620271374, 0.848854343457902036425, 0.152264338295331783612, 0.01105924229346489121];
        private static readonly double[] Dn = [-0.0350353787183177984712, -0.00222426529213447927281, 0.0185573306514231072324, 0.00950804701325919603619, 0.00187123492819559223345, 0.000157544617424960554631, 0.460469890584317994083e-5, -0.230404776911882601748e-9, 0.266339227425782031962e-11];
        private static readonly double[] Dd = [1, 1.3653349817554063097, 0.762059164553623404043, 0.220091105764131249824, 0.0341589143670947727934, 0.00263861676657015992959, 0.764675292302794483503e-4];
        private static readonly double[] En = [-0.0167431005076633737133, -0.00112951438745580278863, 0.00105628862152492910091, 0.000209386317487588078668, 0.149624783758342370182e-4, 0.449696789927706453732e-6, 0.462596163522878599135e-8, -0.281128735628831791805e-13, 0.99055709973310326855e-16];
        private static readonly double[] Ed = [1, 0.591429344886417493481, 0.138151865749083321638, 0.0160746087093676504695, 0.000964011807005165528527, 0.275335474764726041141e-4, 0.282243172016108031869e-6];
        private static readonly double[] Fn = [-0.0024978212791898131227, -0.779190719229053954292e-5, 0.254723037413027451751e-4, 0.162397777342510920873e-5, 0.396341011304801168516e-7, 0.411632831190944208473e-9, 0.145596286718675035587e-11, -0.116765012397184275695e-17];
        private static readonly double[] Fd = [1, 0.207123112214422517181, 0.0169410838120975906478, 0.000690538265622684595676, 0.145007359818232637924e-4, 0.144437756628144157666e-6, 0.509761276599778486139e-9];
        private static readonly double[] Gn = [-0.000539042911019078575891, -0.28398759004727721098e-6, 0.899465114892291446442e-6, 0.229345859265920864296e-7, 0.225561444863500149219e-9, 0.947846627503022684216e-12, 0.135880130108924861008e-14, -0.348890393399948882918e-21];
        private static readonly double[] Gd = [1, 0.0845746234001899436914, 0.00282092984726264681981, 0.468292921940894236786e-4, 0.399968812193862100054e-6, 0.161809290887904476097e-8, 0.231558608310259605225e-11];

        private static double InvImpl(double p, double q, double s)
        {
            double result;

            if (p <= 0.5)
            {
                // Evaluate inverse erf using the rational approximation:
                //
                // x = p(p+10)(Y+R(p))
                //
                // Where Y is a constant, and R(p) is optimized for a low
                // absolute error compared to |Y|.
                //
                // double: Max error found: 2.001849e-18
                // long double: Max error found: 1.017064e-20
                // Maximum Deviation Found (actual error term at infinite precision) 8.030e-21
                const float y = 0.0891314744949340820313f;
                var         g = p * (p + 10);
                var         r = Polynom.Array.GetValue(p, An) / Polynom.Array.GetValue(p, Ad);
                //var r = Polynom.Array.GetValue(p, ErvInvImpAn) / Polynom.Array.GetValue(p, ErvInvImpAd);
                result = g * y + g * r;
            }
            else if (q >= 0.25)
            {
                // Rational approximation for 0.5 > q >= 0.25
                //
                // x = sqrt(-2*log(q)) / (Y + R(q))
                //
                // Where Y is a constant, and R(q) is optimized for a low
                // absolute error compared to Y.
                //
                // double : Max error found: 7.403372e-17
                // long double : Max error found: 6.084616e-20
                // Maximum Deviation Found (error term) 4.811e-20
                const float y  = 2.249481201171875f;
                var         g  = Sqrt(-2 * Log(q));
                var         xs = q - 0.25;
                var         r  = Polynom.Array.GetValue(xs, Bn) / Polynom.Array.GetValue(xs, Bd);
                result = g / (y + r);
            }
            else
            {
                // For q < 0.25 we have a series of rational approximations all
                // of the general form:
                //
                // let: x = sqrt(-log(q))
                //
                // Then the result is given by:
                //
                // x(Y+R(x-B))
                //
                // where Y is a constant, B is the lowest value of x for which
                // the approximation is valid, and R(x-B) is optimized for a low
                // absolute error compared to Y.
                //
                // Note that almost all code will really go through the first
                // or maybe second approximation.  After than we're dealing with very
                // small input values indeed: 80 and 128 bit long double's go all the
                // way down to ~ 1e-5000 so the "tail" is rather long...
                var x = Sqrt(-Log(q));
                if (x < 3)
                {
                    // Max error found: 1.089051e-20
                    const float y  = 0.807220458984375f;
                    var         xs = x - 1.125;
                    var         r  = Polynom.Array.GetValue(xs, Cn) / Polynom.Array.GetValue(xs, Cd);
                    result = y * x + r * x;
                }
                else if (x < 6)
                {
                    // Max error found: 8.389174e-21
                    const float y  = 0.93995571136474609375f;
                    var         xs = x - 3;
                    var         r  = Polynom.Array.GetValue(xs, Dn) / Polynom.Array.GetValue(xs, Dd);
                    result = y * x + r * x;
                }
                else if (x < 18)
                {
                    // Max error found: 1.481312e-19
                    const float y  = 0.98362827301025390625f;
                    var         xs = x - 6;
                    var         r  = Polynom.Array.GetValue(xs, En) / Polynom.Array.GetValue(xs, Ed);
                    result = y * x + r * x;
                }
                else if (x < 44)
                {
                    // Max error found: 5.697761e-20
                    const float y  = 0.99714565277099609375f;
                    var         xs = x - 18;
                    var         r  = Polynom.Array.GetValue(xs, Fn) / Polynom.Array.GetValue(xs, Fd);
                    result = y * x + r * x;
                }
                else
                {
                    // Max error found: 1.279746e-20
                    const float y  = 0.99941349029541015625f;
                    var         xs = x - 44;
                    var         r  = Polynom.Array.GetValue(xs, Gn) / Polynom.Array.GetValue(xs, Gd);
                    result = y * x + r * x;
                }
            }

            return s * result;
        }

        public static double Value(double x)
        {
            // https://www.johndcook.com/blog/cpp_erf/
            // constants
            const double a1 = 0.254829592;
            const double a2 = -0.284496736;
            const double a3 = 1.421413741;
            const double a4 = -1.453152027;
            const double a5 = 1.061405429;
            const double p  = 0.3275911;

            // Save the sign of x
            var sign = Sign(x);
            x = Abs(x);

            // A&S formula 7.1.26
            var t = 1.0 / (1.0 + p * x);
            var y = 1.0 - ((((a5 * t + a4) * t + a3) * t + a2) * t + a1) * t * Exp(-x * x);

            return sign * y;
        }

        ///// <summary>
        ///// Returns the value of the gaussian error function at <paramref name="x"/>.
        ///// </summary>
        //public static double Value_2(double x)
        //{
        //    /*
        //     https://stackoverflow.com/questions/22834998/what-reference-should-i-use-to-use-erf-erfc-function
        //     https://math.stackexchange.com/questions/263216/error-function-erf-with-better-precision/1889960#1889960
        //    Copyright (C) 1993 by Sun Microsystems, Inc. All rights reserved.
        //    *
        //    * Developed at SunPro, a Sun Microsystems, Inc. business.
        //    * Permission to use, copy, modify, and distribute this
        //    * software is freely granted, provided that this notice
        //    * is preserved.
        //    */

        //    #region Constants

        //    const double tiny = 1e-300;
        //    const double erx = 8.45062911510467529297e-01;

        //    // Coefficients for approximation to erf on [0, 0.84375]
        //    const double efx = 1.28379167095512586316e-01; /* 0x3FC06EBA; 0x8214DB69 */
        //    const double efx8 = 1.02703333676410069053e+00; /* 0x3FF06EBA; 0x8214DB69 */
        //    const double pp0 = 1.28379167095512558561e-01; /* 0x3FC06EBA; 0x8214DB68 */
        //    const double pp1 = -3.25042107247001499370e-01; /* 0xBFD4CD7D; 0x691CB913 */
        //    const double pp2 = -2.84817495755985104766e-02; /* 0xBF9D2A51; 0xDBD7194F */
        //    const double pp3 = -5.77027029648944159157e-03; /* 0xBF77A291; 0x236668E4 */
        //    const double pp4 = -2.37630166566501626084e-05; /* 0xBEF8EAD6; 0x120016AC */
        //    const double qq1 = 3.97917223959155352819e-01; /* 0x3FD97779; 0xCDDADC09 */
        //    const double qq2 = 6.50222499887672944485e-02; /* 0x3FB0A54C; 0x5536CEBA */
        //    const double qq3 = 5.08130628187576562776e-03; /* 0x3F74D022; 0xC4D36B0F */
        //    const double qq4 = 1.32494738004321644526e-04; /* 0x3F215DC9; 0x221C1A10 */
        //    const double qq5 = -3.96022827877536812320e-06; /* 0xBED09C43; 0x42A26120 */

        //    // Coefficients for approximation to erf in [0.84375, 1.25]
        //    const double pa0 = -2.36211856075265944077e-03; /* 0xBF6359B8; 0xBEF77538 */
        //    const double pa1 = 4.14856118683748331666e-01; /* 0x3FDA8D00; 0xAD92B34D */
        //    const double pa2 = -3.72207876035701323847e-01; /* 0xBFD7D240; 0xFBB8C3F1 */
        //    const double pa3 = 3.18346619901161753674e-01; /* 0x3FD45FCA; 0x805120E4 */
        //    const double pa4 = -1.10894694282396677476e-01; /* 0xBFBC6398; 0x3D3E28EC */
        //    const double pa5 = 3.54783043256182359371e-02; /* 0x3FA22A36; 0x599795EB */
        //    const double pa6 = -2.16637559486879084300e-03; /* 0xBF61BF38; 0x0A96073F */
        //    const double qa1 = 1.06420880400844228286e-01; /* 0x3FBB3E66; 0x18EEE323 */
        //    const double qa2 = 5.40397917702171048937e-01; /* 0x3FE14AF0; 0x92EB6F33 */
        //    const double qa3 = 7.18286544141962662868e-02; /* 0x3FB2635C; 0xD99FE9A7 */
        //    const double qa4 = 1.26171219808761642112e-01; /* 0x3FC02660; 0xE763351F */
        //    const double qa5 = 1.36370839120290507362e-02; /* 0x3F8BEDC2; 0x6B51DD1C */
        //    const double qa6 = 1.19844998467991074170e-02; /* 0x3F888B54; 0x5735151D */

        //    // Coefficients for approximation to erfc in [1.25, 1/0.35]
        //    const double ra0 = -9.86494403484714822705e-03; /* 0xBF843412; 0x600D6435 */
        //    const double ra1 = -6.93858572707181764372e-01; /* 0xBFE63416; 0xE4BA7360 */
        //    const double ra2 = -1.05586262253232909814e+01; /* 0xC0251E04; 0x41B0E726 */
        //    const double ra3 = -6.23753324503260060396e+01; /* 0xC04F300A; 0xE4CBA38D */
        //    const double ra4 = -1.62396669462573470355e+02; /* 0xC0644CB1; 0x84282266 */
        //    const double ra5 = -1.84605092906711035994e+02; /* 0xC067135C; 0xEBCCABB2 */
        //    const double ra6 = -8.12874355063065934246e+01; /* 0xC0545265; 0x57E4D2F2 */
        //    const double ra7 = -9.81432934416914548592e+00; /* 0xC023A0EF; 0xC69AC25C */
        //    const double sa1 = 1.96512716674392571292e+01; /* 0x4033A6B9; 0xBD707687 */
        //    const double sa2 = 1.37657754143519042600e+02; /* 0x4061350C; 0x526AE721 */
        //    const double sa3 = 4.34565877475229228821e+02; /* 0x407B290D; 0xD58A1A71 */
        //    const double sa4 = 6.45387271733267880336e+02; /* 0x40842B19; 0x21EC2868 */
        //    const double sa5 = 4.29008140027567833386e+02; /* 0x407AD021; 0x57700314 */
        //    const double sa6 = 1.08635005541779435134e+02; /* 0x405B28A3; 0xEE48AE2C */
        //    const double sa7 = 6.57024977031928170135e+00; /* 0x401A47EF; 0x8E484A93 */
        //    const double sa8 = -6.04244152148580987438e-02; /* 0xBFAEEFF2; 0xEE749A62 */

        //    // Coefficients for approximation to erfc in [1/0.35, 28]
        //    const double rb0 = -9.86494292470009928597e-03; /* 0xBF843412; 0x39E86F4A */
        //    const double rb1 = -7.99283237680523006574e-01; /* 0xBFE993BA; 0x70C285DE */
        //    const double rb2 = -1.77579549177547519889e+01; /* 0xC031C209; 0x555F995A */
        //    const double rb3 = -1.60636384855821916062e+02; /* 0xC064145D; 0x43C5ED98 */
        //    const double rb4 = -6.37566443368389627722e+02; /* 0xC083EC88; 0x1375F228 */
        //    const double rb5 = -1.02509513161107724954e+03; /* 0xC0900461; 0x6A2E5992 */
        //    const double rb6 = -4.83519191608651397019e+02; /* 0xC07E384E; 0x9BDC383F */
        //    const double sb1 = 3.03380607434824582924e+01; /* 0x403E568B; 0x261D5190 */
        //    const double sb2 = 3.25792512996573918826e+02; /* 0x40745CAE; 0x221B9F0A */
        //    const double sb3 = 1.53672958608443695994e+03; /* 0x409802EB; 0x189D5118 */
        //    const double sb4 = 3.19985821950859553908e+03; /* 0x40A8FFB7; 0x688C246A */
        //    const double sb5 = 2.55305040643316442583e+03; /* 0x40A3F219; 0xCEDF3BE6 */
        //    const double sb6 = 4.74528541206955367215e+02; /* 0x407DA874; 0xE79FE763 */
        //    const double sb7 = -2.24409524465858183362e+01; /* 0xC03670E2; 0x42712D62 */

        //    #endregion

        //    if (double.IsNaN(x))
        //        return double.NaN;

        //    if (double.IsNegativeInfinity(x))
        //        return -1.0;

        //    if (double.IsPositiveInfinity(x))
        //        return 1.0;

        //    int hx, ix, i;
        //    double R, S, P, Q, s, y, z, r;
        //    //unsafe
        //    //{
        //    //    double one = 1.0;
        //    //    n0 = ((*(int*)&one) >> 29) ^ 1;
        //    //    hx = *(n0 + (int*)&x);
        //    //}
        //    var one = 1.0;
        //    var n0 = (one.ToIntBits() >> 29) ^ 1;
        //    hx = (int)(n0 + x.ToIntBits());

        //    ix = hx & 0x7F_FF_FF_FF;

        //    if (ix < 0x3F_EB_00_00) // |x| < 0.84375
        //    {
        //        if (ix < 0x3E300000) // |x| < 2**-28
        //            return ix < 0x00800000 
        //                ? 0.125 * (8.0 * x + efx8 * x) // avoid underflow
        //                : x + efx * x;

        //        z = x * x;
        //        r = pp0 + z * (pp1 + z * (pp2 + z * (pp3 + z * pp4)));
        //        s = 1.0 + z * (qq1 + z * (qq2 + z * (qq3 + z * (qq4 + z * qq5))));
        //        y = r / s;
        //        return x + x * y;
        //    }

        //    if (ix < 0x3FF40000) // 0.84375 <= |x| < 1.25
        //    {
        //        s = Math.Abs(x) - 1.0;
        //        P = pa0 + s * (pa1 + s * (pa2 + s * (pa3 + s * (pa4 + s * (pa5 + s * pa6)))));
        //        Q = 1.0 + s * (qa1 + s * (qa2 + s * (qa3 + s * (qa4 + s * (qa5 + s * qa6)))));
        //        return hx >= 0 
        //            ? erx + P / Q 
        //            : -erx - P / Q;
        //    }

        //    if (ix >= 0x40180000) // inf > |x| >= 6
        //        return hx >= 0 
        //            ? 1.0 - tiny 
        //            : tiny - 1.0;

        //    x = Math.Abs(x);
        //    s = 1.0 / (x * x);
        //    if (ix < 0x4006DB6E) // |x| < 1/0.35
        //    {
        //        R = ra0 + s * (ra1 + s * (ra2 + s * (ra3 + s * (ra4 + s * (ra5 + s * (ra6 + s * ra7))))));
        //        S = 1.0 + s * (sa1 + s * (sa2 + s * (sa3 + s * (sa4 + s * (sa5 + s * (sa6 + s * (sa7 + s * sa8)))))));
        //    }
        //    else // |x| >= 1/0.35
        //    {
        //        R = rb0 + s * (rb1 + s * (rb2 + s * (rb3 + s * (rb4 + s * (rb5 + s * rb6)))));
        //        S = 1.0 + s * (sb1 + s * (sb2 + s * (sb3 + s * (sb4 + s * (sb5 + s * (sb6 + s * sb7))))));
        //    }
        //    z = x;
        //    unsafe { *(1 - n0 + (int*)&z) = 0; }
        //    r = Math.Exp(-z * z - 0.5625) * Math.Exp((z - x) * (z + x) + R / S);

        //    return hx >= 0 
        //        ? 1.0 - r / x 
        //        : r / x - 1.0;
        //}

        ///// <summary>
        ///// Returns the value of the complementary error function at <paramref name="x"/>.
        ///// </summary>
        //public static double Value_compl(double x)
        //{
        //    /*
        //    Copyright (C) 1993 by Sun Microsystems, Inc. All rights reserved.
        //    *
        //    * Developed at SunPro, a Sun Microsystems, Inc. business.
        //    * Permission to use, copy, modify, and distribute this
        //    * software is freely granted, provided that this notice
        //    * is preserved.
        //    */

        //    #region Constants

        //    const double tiny = 1e-300;
        //    const double erx = 8.45062911510467529297e-01;

        //    // Coefficients for approximation to erf on [0, 0.84375]
        //    const double efx = 1.28379167095512586316e-01; /* 0x3FC06EBA; 0x8214DB69 */
        //    const double efx8 = 1.02703333676410069053e+00; /* 0x3FF06EBA; 0x8214DB69 */
        //    const double pp0 = 1.28379167095512558561e-01; /* 0x3FC06EBA; 0x8214DB68 */
        //    const double pp1 = -3.25042107247001499370e-01; /* 0xBFD4CD7D; 0x691CB913 */
        //    const double pp2 = -2.84817495755985104766e-02; /* 0xBF9D2A51; 0xDBD7194F */
        //    const double pp3 = -5.77027029648944159157e-03; /* 0xBF77A291; 0x236668E4 */
        //    const double pp4 = -2.37630166566501626084e-05; /* 0xBEF8EAD6; 0x120016AC */
        //    const double qq1 = 3.97917223959155352819e-01; /* 0x3FD97779; 0xCDDADC09 */
        //    const double qq2 = 6.50222499887672944485e-02; /* 0x3FB0A54C; 0x5536CEBA */
        //    const double qq3 = 5.08130628187576562776e-03; /* 0x3F74D022; 0xC4D36B0F */
        //    const double qq4 = 1.32494738004321644526e-04; /* 0x3F215DC9; 0x221C1A10 */
        //    const double qq5 = -3.96022827877536812320e-06; /* 0xBED09C43; 0x42A26120 */

        //    // Coefficients for approximation to erf in [0.84375, 1.25]
        //    const double pa0 = -2.36211856075265944077e-03; /* 0xBF6359B8; 0xBEF77538 */
        //    const double pa1 = 4.14856118683748331666e-01; /* 0x3FDA8D00; 0xAD92B34D */
        //    const double pa2 = -3.72207876035701323847e-01; /* 0xBFD7D240; 0xFBB8C3F1 */
        //    const double pa3 = 3.18346619901161753674e-01; /* 0x3FD45FCA; 0x805120E4 */
        //    const double pa4 = -1.10894694282396677476e-01; /* 0xBFBC6398; 0x3D3E28EC */
        //    const double pa5 = 3.54783043256182359371e-02; /* 0x3FA22A36; 0x599795EB */
        //    const double pa6 = -2.16637559486879084300e-03; /* 0xBF61BF38; 0x0A96073F */
        //    const double qa1 = 1.06420880400844228286e-01; /* 0x3FBB3E66; 0x18EEE323 */
        //    const double qa2 = 5.40397917702171048937e-01; /* 0x3FE14AF0; 0x92EB6F33 */
        //    const double qa3 = 7.18286544141962662868e-02; /* 0x3FB2635C; 0xD99FE9A7 */
        //    const double qa4 = 1.26171219808761642112e-01; /* 0x3FC02660; 0xE763351F */
        //    const double qa5 = 1.36370839120290507362e-02; /* 0x3F8BEDC2; 0x6B51DD1C */
        //    const double qa6 = 1.19844998467991074170e-02; /* 0x3F888B54; 0x5735151D */

        //    // Coefficients for approximation to erfc in [1.25, 1/0.35]
        //    const double ra0 = -9.86494403484714822705e-03; /* 0xBF843412; 0x600D6435 */
        //    const double ra1 = -6.93858572707181764372e-01; /* 0xBFE63416; 0xE4BA7360 */
        //    const double ra2 = -1.05586262253232909814e+01; /* 0xC0251E04; 0x41B0E726 */
        //    const double ra3 = -6.23753324503260060396e+01; /* 0xC04F300A; 0xE4CBA38D */
        //    const double ra4 = -1.62396669462573470355e+02; /* 0xC0644CB1; 0x84282266 */
        //    const double ra5 = -1.84605092906711035994e+02; /* 0xC067135C; 0xEBCCABB2 */
        //    const double ra6 = -8.12874355063065934246e+01; /* 0xC0545265; 0x57E4D2F2 */
        //    const double ra7 = -9.81432934416914548592e+00; /* 0xC023A0EF; 0xC69AC25C */
        //    const double sa1 = 1.96512716674392571292e+01; /* 0x4033A6B9; 0xBD707687 */
        //    const double sa2 = 1.37657754143519042600e+02; /* 0x4061350C; 0x526AE721 */
        //    const double sa3 = 4.34565877475229228821e+02; /* 0x407B290D; 0xD58A1A71 */
        //    const double sa4 = 6.45387271733267880336e+02; /* 0x40842B19; 0x21EC2868 */
        //    const double sa5 = 4.29008140027567833386e+02; /* 0x407AD021; 0x57700314 */
        //    const double sa6 = 1.08635005541779435134e+02; /* 0x405B28A3; 0xEE48AE2C */
        //    const double sa7 = 6.57024977031928170135e+00; /* 0x401A47EF; 0x8E484A93 */
        //    const double sa8 = -6.04244152148580987438e-02; /* 0xBFAEEFF2; 0xEE749A62 */

        //    // Coefficients for approximation to erfc in [1/0.35, 28]
        //    const double rb0 = -9.86494292470009928597e-03; /* 0xBF843412; 0x39E86F4A */
        //    const double rb1 = -7.99283237680523006574e-01; /* 0xBFE993BA; 0x70C285DE */
        //    const double rb2 = -1.77579549177547519889e+01; /* 0xC031C209; 0x555F995A */
        //    const double rb3 = -1.60636384855821916062e+02; /* 0xC064145D; 0x43C5ED98 */
        //    const double rb4 = -6.37566443368389627722e+02; /* 0xC083EC88; 0x1375F228 */
        //    const double rb5 = -1.02509513161107724954e+03; /* 0xC0900461; 0x6A2E5992 */
        //    const double rb6 = -4.83519191608651397019e+02; /* 0xC07E384E; 0x9BDC383F */
        //    const double sb1 = 3.03380607434824582924e+01; /* 0x403E568B; 0x261D5190 */
        //    const double sb2 = 3.25792512996573918826e+02; /* 0x40745CAE; 0x221B9F0A */
        //    const double sb3 = 1.53672958608443695994e+03; /* 0x409802EB; 0x189D5118 */
        //    const double sb4 = 3.19985821950859553908e+03; /* 0x40A8FFB7; 0x688C246A */
        //    const double sb5 = 2.55305040643316442583e+03; /* 0x40A3F219; 0xCEDF3BE6 */
        //    const double sb6 = 4.74528541206955367215e+02; /* 0x407DA874; 0xE79FE763 */
        //    const double sb7 = -2.24409524465858183362e+01; /* 0xC03670E2; 0x42712D62 */

        //    #endregion

        //    if (double.IsNaN(x))
        //        return double.NaN;

        //    if (double.IsNegativeInfinity(x))
        //        return 2.0;

        //    if (double.IsPositiveInfinity(x))
        //        return 0.0;

        //    int hx, ix;
        //    double R, S, P, Q, s, y, z, r;
        //    //unsafe
        //    //{
        //    //    double one = 1.0;
        //    //    n0 = ((*(int*)&one) >> 29) ^ 1;
        //    //    hx = *(n0 + (int*)&x);
        //    //}
        //    var one = 1.0;
        //    var n0 = (one.ToIntBits() >> 29) ^ 1;
        //    hx = (int)(n0 + x.ToIntBits());

        //    ix = hx & 0x7FFFFFFF;

        //    if (ix < 0x3FEB0000) // |x| < 0.84375
        //    {
        //        if (ix < 0x3C700000) // |x| < 2**-56
        //            return 1.0 - x;

        //        z = x * x;
        //        r = pp0 + z * (pp1 + z * (pp2 + z * (pp3 + z * pp4)));
        //        s = 1.0 + z * (qq1 + z * (qq2 + z * (qq3 + z * (qq4 + z * qq5))));
        //        y = r / s;
        //        if (hx < 0x3FD00000) // x < 1/4
        //            return 1.0 - (x + x * y);
        //        r = x * y;
        //        r += x - 0.5;
        //        return 0.5 - r;
        //    }

        //    if (ix < 0x3FF40000) // 0.84375 <= |x| < 1.25
        //    {
        //        s = Math.Abs(x) - 1.0;
        //        P = pa0 + s * (pa1 + s * (pa2 + s * (pa3 + s * (pa4 + s * (pa5 + s * pa6)))));
        //        Q = 1.0 + s * (qa1 + s * (qa2 + s * (qa3 + s * (qa4 + s * (qa5 + s * qa6)))));
        //        return hx >= 0 
        //            ? 1.0 - erx - P / Q 
        //            : 1.0 + (erx + P / Q);
        //    }

        //    if (ix < 0x403C0000) // |x| < 28
        //    {
        //        x = Math.Abs(x);
        //        s = 1.0 / (x * x);
        //        if (ix < 0x4006DB6D) // |x| < 1/.35 ~ 2.857143
        //        {
        //            R = ra0 + s * (ra1 + s * (ra2 + s * (ra3 + s * (ra4 + s * (ra5 + s * (ra6 + s * ra7))))));
        //            S = 1.0 + s * (sa1 + s * (sa2 + s * (sa3 + s * (sa4 + s * (sa5 + s * (sa6 + s * (sa7 + s * sa8)))))));
        //        }
        //        else // |x| >= 1/.35 ~ 2.857143
        //        {
        //            if (hx < 0 && ix >= 0x40180000)
        //                return 2.0 - tiny; // x < -6

        //            R = rb0 + s * (rb1 + s * (rb2 + s * (rb3 + s * (rb4 + s * (rb5 + s * rb6)))));
        //            S = 1.0 + s * (sb1 + s * (sb2 + s * (sb3 + s * (sb4 + s * (sb5 + s * (sb6 + s * sb7))))));
        //        }
        //        z = x;
        //        unsafe { *(1 - n0 + (int*)&z) = 0; }
        //        r = Math.Exp(-z * z - 0.5625) *
        //        Math.Exp((z - x) * (z + x) + R / S);
        //        if (hx > 0)
        //            return r / x;
        //        else
        //            return 2.0 - r / x;
        //    }
        //    else
        //    {
        //        if (hx > 0)
        //            return tiny * tiny;
        //        else
        //            return 2.0 - tiny;
        //    }
        //}
    }
}