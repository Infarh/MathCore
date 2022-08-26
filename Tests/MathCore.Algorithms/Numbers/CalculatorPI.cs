using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathCore.Algorithms.Numbers;

public static class CalculatorPI
{
    private const long B = 10000;
    private const long LB = 4;
    private const long MaxDiv = 450;

    public static void Calculate(int N)
    {
        var timer = Stopwatch.StartNew();

        var NbDigits = (long)N;

        var size = 1 + NbDigits / LB;

        var Pi = new long[size];
        var arctan = new long[size];
        var buffer1 = new long[size];
        var buffer2 = new long[size];

        long[] m = { 12, 8, -5 };
        long[] p = { 18, 57, 239 };

        //SetToInteger(Pi, 0);

        // Pi/4 = 12*arctan(1/18) + 8*arctan(1/57) - 5*arctan(1/239)
        // Pi/4 = Sum(i) [m[i]*arctan(1/p[i])]

        for (long i = 0; i < m.Length; i++)
        {
            //cout << i << endl;
            Console.WriteLine(i);
            arccot(p[i], arctan, buffer1, buffer2);
            Mul(arctan, Math.Abs(m[i]));

            if (m[i] > 0)
                Add(Pi, arctan);
            else
                Sub(Pi, arctan);
        }

        Mul(Pi, 4);
        timer.Stop();

        Print(Pi, timer.Elapsed.TotalSeconds);
    }

    private static void Print(long[] x, double Time)
    {
        Console.Write(x[0]);
        Console.Write('.');
        for (var i = 0; i < x.Length; i++)
            Console.Write(x[i]);

    }

    /// <summary>Перевод большого действительного числа к малому целому</summary>
    /// <param name="x"></param>
    /// <param name="I"></param>
    private static void SetToInteger(long[] x, long I)
    {
        x[0] = I;
        for (long i = 1; i < x.Length; i++)
            x[i] = 0;
    }

    private static bool IsZero(long[] x)
    {
        for (var i = 0; i < x.Length; i++)
            if (x[i] != 0)
                return false;
        return true;
    }

    /// <summary>x = x + y</summary>
    private static void Add(long[] x, long[] y)
    {
        long carry = 0;
        for (long i = x.Length - 1; i >= 0; i--)
        {
            x[i] += y[i] + carry;
            if (x[i] < B) carry = 0;
            else
            {
                carry = 1;
                x[i] -= B;
            }
        }
    }

    private static void Sub(long[] x, long[] y)
    {
        for (long i = x.Length - 1; i >= 0; i--)
        {
            x[i] -= y[i];
            if (x[i] < 0 && i != 0)
            {
                x[i] += B;
                x[i - 1]--;
            }
        }
    }

    private static void Mul(long[] x, long q)
    {
        long carry = 0;
        for (long i = x.Length - 1; i >= 0; i--)
        {
            var xi = x[i] * q;
            xi += carry;
            if (xi >= B)
            {
                carry = xi / B;
                xi -= carry * B;
            }
            else
                carry = 0;
            x[i] = xi;
        }
    }

    private static void Div(long[] x, long d, long[] y)
    {
        long carry = 0;
        var n = x.Length;
        for (long i = 0; i < n; i++)
        {
            var xi = x[i] + carry * B;
            var q = xi / d;
            carry = xi - q * d;
            y[i] = q;
        }
    }

    private static int j0 = 0;

    /// <summary>
    /// Вычисление котангенса целого числа р (это тангенс (1/p))<br/>
    /// Результат - большое действительное число размера n
    /// </summary>
    /// <param name="p"></param>
    /// <param name="x"></param>
    /// <param name="buf1"></param>
    /// <param name="buf2"></param>
    private static void arccot(long p, long[] x, long[] buf1, long[] buf2)
    {
        var n = x.Length;

        var p2 = p * p;
        long k = 3;
        long sign = 0;
        var uk = buf1;
        var vk = buf2;

        SetToInteger(x, 0);
        SetToInteger(uk, 1);

        Div(uk, p, uk);            /* uk = 1/p */
        Add(x, uk);                /* x  = uk */

        int j = 0, k0 = 1;
        while (!IsZero(uk))
        {
            if (p < MaxDiv)
                Div(uk, p2, uk);  /* Один шаг малого p */
            else
            {
                Div(uk, p, uk);   /* Два шага большого p (смотри деление) */
                Div(uk, p, uk);
            }
            /* uk = u(k-1)/(p^2) */
            Div(uk, k, vk); /* vk = uk/k  */
            if (sign != 0)
                Add(x, vk);  /* x = x+vk   */
            else
                Sub(x, vk);  /* x = x-vk   */
            k += 2;
            sign = 1 - sign;
            j++;
            if ((j % k0) == 0)
            {
                Console.WriteLine(" .");
                //cout << " ." << j << endl;
                k0 *= 2;
            }
        }
        Console.WriteLine(" -");
        //cout << " -" << j << endl;
        j0 += j;
        if (j != j0)
        {
            //cout << " +" << j0 << endl;
            Console.WriteLine(" +{0}", j0);
        }
    }
}
