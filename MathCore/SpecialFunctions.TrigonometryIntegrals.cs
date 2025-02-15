﻿using static System.Math;

// ReSharper disable IdentifierTypo

namespace MathCore;

public static partial class SpecialFunctions
{
    public class TrigonometryIntegrals
    {
        // ReSharper disable CommentTypo
        /*************************************************************************
        Sine and cosine integrals

        Evaluates the integrals

                                 x
                                 -
                                |  cos t - 1
          Ci(x) = eul + ln x +  |  --------- dt,
                                |      t
                               -
                                0
                    x
                    -
                   |  sin t
          Si(x) =  |  ----- dt
                   |    t
                  -
                   0

        where eul = 0.57721566490153286061 is Euler's constant.
        The integrals are approximated by rational functions.
        For x > 8 auxiliary functions f(x) and g(x) are employed
        such that

        Ci(x) = f(x) sin(x) - g(x) cos(x)
        Si(x) = pi/2 - f(x) cos(x) - g(x) sin(x)


        ACCURACY:
           Test interval = [0,50].
        Absolute error, except relative when > 1:
        arithmetic   function   # trials      peak         rms
           IEEE        Si        30000       4.4e-16     7.3e-17
           IEEE        Ci        30000       6.9e-16     5.1e-17

        Cephes Math Library Release 2.1:  January, 1989
        Copyright 1984, 1987, 1989 by Stephen L. Moshier
        *************************************************************************/
        // ReSharper restore CommentTypo

        /// <summary>Вычисляет интегралы синуса и косинуса для заданного значения x</summary>
        /// <param name="x">Входное значение, для которого вычисляются интегралы.</param>
        /// <param name="si">Выходное значение интеграла синуса (Si).</param>
        /// <param name="ci">Выходное значение интеграла косинуса (Ci).</param>
        /// <remarks>
        /// Функция вычисляет интегралы Si(x) и Ci(x) с использованием рациональных функций
        /// для приближения. Для x > 8 используются вспомогательные функции f(x) и g(x),
        /// которые позволяют вычислить значения интегралов с высокой точностью.
        /// </remarks>
        [DST]
        public static void SineCosineIntegrals(double x, out double si, out double ci)
        {

            int sg;
            if (x >= 0) sg = 0;
            else
            {
                sg = -1;
                x = -x;
            }

            if (Abs(x) < Eps)
            {
                si = 0;
                ci = -__MaxRealNumber;
                return;
            }

            if (x > 1E9)
            {
                si = Consts.pi05 - Cos(x) / x;
                ci = Sin(x) / x;
                return;
            }

            double s;
            double c;
            double z;
            if (x <= 4)
            {
                z = x * x;
                var sn = -8.39167827910303881427E-11;
                sn = sn * z + 4.62591714427012837309E-8;
                sn = sn * z - 9.75759303843632795789E-6;
                sn = sn * z + 9.76945438170435310816E-4;
                sn = sn * z - 4.13470316229406538752E-2;
                sn = sn * z + 1.00000000000000000302E0;

                var sd = 2.03269266195951942049E-12;
                sd = sd * z + 1.27997891179943299903E-9;
                sd = sd * z + 4.41827842801218905784E-7;
                sd = sd * z + 9.96412122043875552487E-5;
                sd = sd * z + 1.42085239326149893930E-2;
                sd = sd * z + 9.99999999999999996984E-1;
                s = x * sn / sd;

                var cn = 2.02524002389102268789E-11;
                cn = cn * z - 1.35249504915790756375E-8;
                cn = cn * z + 3.59325051419993077021E-6;
                cn = cn * z - 4.74007206873407909465E-4;
                cn = cn * z + 2.89159652607555242092E-2;
                cn = cn * z - 1.00000000000000000080E0;

                var cd = 4.07746040061880559506E-12;
                cd = cd * z + 3.06780997581887812692E-9;
                cd = cd * z + 1.23210355685883423679E-6;
                cd = cd * z + 3.17442024775032769882E-4;
                cd = cd * z + 5.10028056236446052392E-2;
                cd = cd * z + 4.00000000000000000080E0;
                c = z * cn / cd;
                si = sg == 0 ? s : -s;
                ci = 0.57721566490153286061 + Log(x) + c;
                return;
            }

            s = Sin(x);
            c = Cos(x);
            z = 1 / (x * x);

            double gd;
            double gn;
            double fd;
            double fn;
            double g;
            double f;
            if (x < 8)
            {
                fn = 4.23612862892216586994E0;
                fn = fn * z + 5.45937717161812843388E0;
                fn = fn * z + 1.62083287701538329132E0;
                fn = fn * z + 1.67006611831323023771E-1;
                fn = fn * z + 6.81020132472518137426E-3;
                fn = fn * z + 1.08936580650328664411E-4;
                fn = fn * z + 5.48900223421373614008E-7;

                fd = 1.00000000000000000000E0;
                fd = fd * z + 8.16496634205391016773E0;
                fd = fd * z + 7.30828822505564552187E0;
                fd = fd * z + 1.86792257950184183883E0;
                fd = fd * z + 1.78792052963149907262E-1;
                fd = fd * z + 7.01710668322789753610E-3;
                fd = fd * z + 1.10034357153915731354E-4;
                fd = fd * z + 5.48900252756255700982E-7;
                f = fn / (x * fd);

                gn = 8.71001698973114191777E-2;
                gn = gn * z + 6.11379109952219284151E-1;
                gn = gn * z + 3.97180296392337498885E-1;
                gn = gn * z + 7.48527737628469092119E-2;
                gn = gn * z + 5.38868681462177273157E-3;
                gn = gn * z + 1.61999794598934024525E-4;
                gn = gn * z + 1.97963874140963632189E-6;
                gn = gn * z + 7.82579040744090311069E-9;

                gd = 1.00000000000000000000E0;
                gd = gd * z + 1.64402202413355338886E0;
                gd = gd * z + 6.66296701268987968381E-1;
                gd = gd * z + 9.88771761277688796203E-2;
                gd = gd * z + 6.22396345441768420760E-3;
                gd = gd * z + 1.73221081474177119497E-4;
                gd = gd * z + 2.02659182086343991969E-6;
                gd = gd * z + 7.82579218933534490868E-9;
                g = z * gn / gd;
            }
            else
            {
                fn = 4.55880873470465315206E-1;
                fn = fn * z + 7.13715274100146711374E-1;
                fn = fn * z + 1.60300158222319456320E-1;
                fn = fn * z + 1.16064229408124407915E-2;
                fn = fn * z + 3.49556442447859055605E-4;
                fn = fn * z + 4.86215430826454749482E-6;
                fn = fn * z + 3.20092790091004902806E-8;
                fn = fn * z + 9.41779576128512936592E-11;
                fn = fn * z + 9.70507110881952024631E-14;

                fd = 1.00000000000000000000E0;
                fd = fd * z + 9.17463611873684053703E-1;
                fd = fd * z + 1.78685545332074536321E-1;
                fd = fd * z + 1.22253594771971293032E-2;
                fd = fd * z + 3.58696481881851580297E-4;
                fd = fd * z + 4.92435064317881464393E-6;
                fd = fd * z + 3.21956939101046018377E-8;
                fd = fd * z + 9.43720590350276732376E-11;
                fd = fd * z + 9.70507110881952025725E-14;
                f = fn / (x * fd);

                gn = 6.97359953443276214934E-1;
                gn = gn * z + 3.30410979305632063225E-1;
                gn = gn * z + 3.84878767649974295920E-2;
                gn = gn * z + 1.71718239052347903558E-3;
                gn = gn * z + 3.48941165502279436777E-5;
                gn = gn * z + 3.47131167084116673800E-7;
                gn = gn * z + 1.70404452782044526189E-9;
                gn = gn * z + 3.85945925430276600453E-12;
                gn = gn * z + 3.14040098946363334640E-15;

                gd = 1.00000000000000000000E0;
                gd = gd * z + 1.68548898811011640017E0;
                gd = gd * z + 4.87852258695304967486E-1;
                gd = gd * z + 4.67913194259625806320E-2;
                gd = gd * z + 1.90284426674399523638E-3;
                gd = gd * z + 3.68475504442561108162E-5;
                gd = gd * z + 3.57043223443740838771E-7;
                gd = gd * z + 1.72693748966316146736E-9;
                gd = gd * z + 3.87830166023954706752E-12;
                gd = gd * z + 3.14040098946363335242E-15;
                g = z * gn / gd;
            }
            si = Consts.pi05 - f * c - g * s;
            if (sg != 0) si = -si;
            ci = f * s - g * c;
        }


        // ReSharper disable CommentTypo
        /*************************************************************************
        Hyperbolic sine and cosine integrals

        Approximates the integrals

                                   x
                                   -
                                  | |   cosh t - 1
          Chi(x) = eul + ln x +   |    -----------  dt,
                                | |          t
                                 -
                                 0

                      x
                      -
                     | |  sinh t
          Shi(x) =   |    ------  dt
                   | |       t
                    -
                    0

        where eul = 0.57721566490153286061 is Euler's constant.
        The integrals are evaluated by power series for x < 8
        and by Chebyshev expansions for x between 8 and 88.
        For large x, both functions approach exp(x)/2x.
        Arguments greater than 88 in magnitude return MAXNUM.


        ACCURACY:

        Test interval 0 to 88.
                             Relative error:
        arithmetic   function  # trials      peak         rms
           IEEE         Shi      30000       6.9e-16     1.6e-16
               Absolute error, except relative when |Chi| > 1:
           IEEE         Chi      30000       8.4e-16     1.4e-16

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        *************************************************************************/
        // ReSharper restore CommentTypo

        /// <summary>Вычисляет гиперболические интегралы синуса и косинуса для заданного значения</summary>
        /// <param name="x">Значение, для которого вычисляются интегралы.</param>
        /// <param name="shi">Выходной параметр для гиперболического интеграла синуса.</param>
        /// <param name="chi">Выходной параметр для гиперболического интеграла косинуса.</param>
        /// <remarks>
        /// Функция вычисляет интегралы для значений меньше 8 с использованием степенных рядов,
        /// для значений от 8 до 88 - с использованием разложений в ряды Чебышева,
        /// а для больших значений - приближённо как exp(x)/2x.
        /// </remarks>
        [DST]
        public static void HyperbolicSineCosineIntegrals(double x, out double shi, out double chi)
        {
            int sg;
            if (x >= 0) sg = 0;
            else
            {
                sg = -1;
                x = -x;
            }

            if (Abs(x) < Eps)
            {
                shi = 0;
                chi = -__MaxRealNumber;
                return;
            }

            double k;
            double c;
            double s;
            double a;
            if (x < 8)
            {
                var z = x * x;
                a = 1;
                s = 1;
                c = 0;
                k = 2;
                do
                {
                    a *= z / k;
                    c += a / k;
                    k++;
                    a /= k;
                    s += a / k;
                    k++;
                } while (Abs(a / s) >= Eps);
                s *= x;
            }
            else
            {
                double b1;
                double b0;
                double b2;
                if (x < 18)
                {
                    a = (576 / x - 52) / 10;
                    k = Exp(x) / x;
                    b0 = 1.83889230173399459482E-17;
                    b1 = 0;
                    ChebIterationShiChi(a, -9.55485532279655569575E-17, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 2.04326105980879882648E-16, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 1.09896949074905343022E-15, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -1.31313534344092599234E-14, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 5.93976226264314278932E-14, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -3.47197010497749154755E-14, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -1.40059764613117131000E-12, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 9.49044626224223543299E-12, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -1.61596181145435454033E-11, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -1.77899784436430310321E-10, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 1.35455469767246947469E-9, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -1.03257121792819495123E-9, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -3.56699611114982536845E-8, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 1.44818877384267342057E-7, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 7.82018215184051295296E-7, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -5.39919118403805073710E-6, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -3.12458202168959833422E-5, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 8.90136741950727517826E-5, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 2.02558474743846862168E-3, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 2.96064440855633256972E-2, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 1.11847751047257036625E0, ref b0, ref b1, out b2);
                    s = k * .5 * (b0 - b2);

                    b0 = -8.12435385225864036372E-18;
                    b1 = 0;
                    ChebIterationShiChi(a, 2.17586413290339214377E-17, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 5.22624394924072204667E-17, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -9.48812110591690559363E-16, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 5.35546311647465209166E-15, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -1.21009970113732918701E-14, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -6.00865178553447437951E-14, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 7.16339649156028587775E-13, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -2.93496072607599856104E-12, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -1.40359438136491256904E-12, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 8.76302288609054966081E-11, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -4.40092476213282340617E-10, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -1.87992075640569295479E-10, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 1.31458150989474594064E-8, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -4.75513930924765465590E-8, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -2.21775018801848880741E-7, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 1.94635531373272490962E-6, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 4.33505889257316408893E-6, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -6.13387001076494349496E-5, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, -3.13085477492997465138E-4, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 4.97164789823116062801E-4, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 2.64347496031374526641E-2, ref b0, ref b1, out _);
                    ChebIterationShiChi(a, 1.11446150876699213025E0, ref b0, ref b1, out b2);
                    c = k * .5 * (b0 - b2);
                }
                else
                {
                    if (x <= 88)
                    {
                        a = (6336 / x - 212) / 70;
                        k = Exp(x) / x;
                        b0 = -1.05311574154850938805E-17;
                        b1 = 0;
                        ChebIterationShiChi(a, 2.62446095596355225821E-17, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 8.82090135625368160657E-17, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -3.38459811878103047136E-16, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -8.30608026366935789136E-16, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 3.93397875437050071776E-15, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 1.01765565969729044505E-14, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -4.21128170307640802703E-14, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -1.60818204519802480035E-13, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 3.34714954175994481761E-13, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 2.72600352129153073807E-12, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 1.66894954752839083608E-12, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -3.49278141024730899554E-11, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -1.58580661666482709598E-10, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -1.79289437183355633342E-10, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 1.76281629144264523277E-9, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 1.69050228879421288846E-8, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 1.25391771228487041649E-7, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 1.16229947068677338732E-6, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 1.61038260117376323993E-5, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 3.49810375601053973070E-4, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 1.28478065259647610779E-2, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 1.03665722588798326712E0, ref b0, ref b1, out b2);
                        s = k * .5 * (b0 - b2);

                        b0 = 8.06913408255155572081E-18;
                        b1 = 0;
                        ChebIterationShiChi(a, -2.08074168180148170312E-17, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -5.98111329658272336816E-17, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 2.68533951085945765591E-16, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 4.52313941698904694774E-16, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -3.10734917335299464535E-15, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -4.42823207332531972288E-15, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 3.49639695410806959872E-14, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 6.63406731718911586609E-14, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -3.71902448093119218395E-13, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -1.27135418132338309016E-12, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 2.74851141935315395333E-12, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 2.33781843985453438400E-11, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 2.71436006377612442764E-11, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -2.56600180000355990529E-10, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -1.61021375163803438552E-9, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -4.72543064876271773512E-9, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, -3.00095178028681682282E-9, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 7.79387474390914922337E-8, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 1.06942765566401507066E-6, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 1.59503164802313196374E-5, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 3.49592575153777996871E-4, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 1.28475387530065247392E-2, ref b0, ref b1, out _);
                        ChebIterationShiChi(a, 1.03665693917934275131E0, ref b0, ref b1, out b2);
                        c = k * .5 * (b0 - b2);
                    }
                    else
                    {
                        shi = sg == 0 ? __MaxRealNumber : -__MaxRealNumber;
                        chi = __MaxRealNumber;
                        return;
                    }
                }
            }
            shi = sg == 0 ? s : -s;
            chi = 0.57721566490153286061 + Log(x) + c;
        }

        /// <summary>Выполняет одну итерацию метода Чебышёва для вычисления интегралов гиперболических синусов и косинусов</summary>
        /// <param name="x">Аргумент функции.</param>
        /// <param name="c">Коэффициент Чебышёва.</param>
        /// <param name="b0">Ссылка на переменную, представляющую текущее значение b0, которое будет обновлено.</param>
        /// <param name="b1">Ссылка на переменную, представляющую текущее значение b1, которое будет обновлено.</param>
        /// <param name="b2">Выходной параметр, представляющий предыдущее значение b1, которое будет обновлено.</param>
        private static void ChebIterationShiChi(double x, double c, ref double b0, ref double b1, out double b2)
        {
            b2 = b1;
            b1 = b0;
            b0 = x * b1 - b2 + c;
        }
    }
}