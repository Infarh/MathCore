using System;
using System.Collections.Generic;
using System.Text;

namespace MathCore
{
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

            private static readonly double[] An = { -0.000508781949658280665617, -0.00836874819741736770379, 0.0334806625409744615033, -0.0126926147662974029034, -0.0365637971411762664006, 0.0219878681111168899165, 0.00822687874676915743155, -0.00538772965071242932965 };
            private static readonly double[] Ad = { 1, -0.970005043303290640362, -1.56574558234175846809, 1.56221558398423026363, 0.662328840472002992063, -0.71228902341542847553, -0.0527396382340099713954, 0.0795283687341571680018, -0.00233393759374190016776, 0.000886216390456424707504 };
            private static readonly double[] Bn = { -0.202433508355938759655, 0.105264680699391713268, 8.37050328343119927838, 17.6447298408374015486, -18.8510648058714251895, -44.6382324441786960818, 17.445385985570866523, 21.1294655448340526258, -3.67192254707729348546 };
            private static readonly double[] Bd = { 1, 6.24264124854247537712, 3.9713437953343869095, -28.6608180499800029974, -20.1432634680485188801, 48.5609213108739935468, 10.8268667355460159008, -22.6436933413139721736, 1.72114765761200282724 };
            private static readonly double[] Cn = { -0.131102781679951906451, -0.163794047193317060787, 0.117030156341995252019, 0.387079738972604337464, 0.337785538912035898924, 0.142869534408157156766, 0.0290157910005329060432, 0.00214558995388805277169, -0.679465575181126350155e-6, 0.285225331782217055858e-7, -0.681149956853776992068e-9 };
            private static readonly double[] Cd = { 1, 3.46625407242567245975, 5.38168345707006855425, 4.77846592945843778382, 2.59301921623620271374, 0.848854343457902036425, 0.152264338295331783612, 0.01105924229346489121 };
            private static readonly double[] Dn = { -0.0350353787183177984712, -0.00222426529213447927281, 0.0185573306514231072324, 0.00950804701325919603619, 0.00187123492819559223345, 0.000157544617424960554631, 0.460469890584317994083e-5, -0.230404776911882601748e-9, 0.266339227425782031962e-11 };
            private static readonly double[] Dd = { 1, 1.3653349817554063097, 0.762059164553623404043, 0.220091105764131249824, 0.0341589143670947727934, 0.00263861676657015992959, 0.764675292302794483503e-4 };
            private static readonly double[] En = { -0.0167431005076633737133, -0.00112951438745580278863, 0.00105628862152492910091, 0.000209386317487588078668, 0.149624783758342370182e-4, 0.449696789927706453732e-6, 0.462596163522878599135e-8, -0.281128735628831791805e-13, 0.99055709973310326855e-16 };
            private static readonly double[] Ed = { 1, 0.591429344886417493481, 0.138151865749083321638, 0.0160746087093676504695, 0.000964011807005165528527, 0.275335474764726041141e-4, 0.282243172016108031869e-6 };
            private static readonly double[] Fn = { -0.0024978212791898131227, -0.779190719229053954292e-5, 0.254723037413027451751e-4, 0.162397777342510920873e-5, 0.396341011304801168516e-7, 0.411632831190944208473e-9, 0.145596286718675035587e-11, -0.116765012397184275695e-17 };
            private static readonly double[] Fd = { 1, 0.207123112214422517181, 0.0169410838120975906478, 0.000690538265622684595676, 0.145007359818232637924e-4, 0.144437756628144157666e-6, 0.509761276599778486139e-9 };
            private static readonly double[] Gn = { -0.000539042911019078575891, -0.28398759004727721098e-6, 0.899465114892291446442e-6, 0.229345859265920864296e-7, 0.225561444863500149219e-9, 0.947846627503022684216e-12, 0.135880130108924861008e-14, -0.348890393399948882918e-21 };
            private static readonly double[] Gd = { 1, 0.0845746234001899436914, 0.00282092984726264681981, 0.468292921940894236786e-4, 0.399968812193862100054e-6, 0.161809290887904476097e-8, 0.231558608310259605225e-11 };

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
                    var g = p * (p + 10);
                    var r = Polynom.Array.GetValue(p, An) / Polynom.Array.GetValue(p, Ad);
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
                    const float y = 2.249481201171875f;
                    var g = Math.Sqrt(-2 * Math.Log(q));
                    var xs = q - 0.25;
                    var r = Polynom.Array.GetValue(xs, Bn) / Polynom.Array.GetValue(xs, Bd);
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
                    var x = Math.Sqrt(-Math.Log(q));
                    if (x < 3)
                    {
                        // Max error found: 1.089051e-20
                        const float y = 0.807220458984375f;
                        var xs = x - 1.125;
                        var r = Polynom.Array.GetValue(xs, Cn) / Polynom.Array.GetValue(xs, Cd);
                        result = y * x + r * x;
                    }
                    else if (x < 6)
                    {
                        // Max error found: 8.389174e-21
                        const float y = 0.93995571136474609375f;
                        var xs = x - 3;
                        var r = Polynom.Array.GetValue(xs, Dn) / Polynom.Array.GetValue(xs, Dd);
                        result = y * x + r * x;
                    }
                    else if (x < 18)
                    {
                        // Max error found: 1.481312e-19
                        const float y = 0.98362827301025390625f;
                        var xs = x - 6;
                        var r = Polynom.Array.GetValue(xs, En) / Polynom.Array.GetValue(xs, Ed);
                        result = y * x + r * x;
                    }
                    else if (x < 44)
                    {
                        // Max error found: 5.697761e-20
                        const float y = 0.99714565277099609375f;
                        var xs = x - 18;
                        var r = Polynom.Array.GetValue(xs, Fn) / Polynom.Array.GetValue(xs, Fd);
                        result = y * x + r * x;
                    }
                    else
                    {
                        // Max error found: 1.279746e-20
                        const float y = 0.99941349029541015625f;
                        var xs = x - 44;
                        var r = Polynom.Array.GetValue(xs, Gn) / Polynom.Array.GetValue(xs, Gd);
                        result = y * x + r * x;
                    }
                }

                return s * result;
            }
        }
    }
}
