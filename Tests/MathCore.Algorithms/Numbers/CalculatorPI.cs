using System.Diagnostics;
using System.Threading;

namespace MathCore.Algorithms.Numbers;

public static class CalculatorPI
{
    /// <summary>Основание системы исчисления</summary>
    private const long __B = 10000;

    /// <summary>Число разрядов основания системы исчисления в десятичной системе = Log10(B)</summary>
    private const long __Lb = 4;

    private const long __MaxDiv = 450;

    public static void Calculate(int N)
    {
        Console.WriteLine("Вычисление числа pi с числом знаков: {0}", N);

        var timer = Stopwatch.StartNew();

        var size = N / __Lb + 1;

        var pi = new long[size];
        var arctan = new long[size];
        var buffer1 = new long[size];
        var buffer2 = new long[size];

        // Pi/4 = 12*arctan(1/18) + 8*arctan(1/57) - 5*arctan(1/239)
        Span<long> m = stackalloc long[] { 12, 8, -5 };
        Span<long> p = stackalloc long[] { 18, 57, 239 };

        Console.WriteLine("Формула вычисления:");
        Console.Write("    pi/4 =");
        for (var i = 0; i < m.Length; i++)
        {
            Console.Write(' ');
            if (m[i] >= 0) Console.Write('+');
            Console.Write("{0} * atan(1/{1})", m[i], p[i]);
        }
        Console.WriteLine();
        Console.WriteLine();

        // Pi/4 = Sum(i) [m[i]*arctan(1/p[i])]
        var iterations = 0;
        for (var i = 0; i < m.Length; i++)
        {
            Console.WriteLine("{0} * atan(1/{1})", m[i], p[i]);
            iterations += ArcCot(p[i], arctan, buffer1, buffer2);
            Multiply(arctan, Math.Abs(m[i]));

            if (m[i] > 0)
                Add(pi, arctan);
            else
                Substrate(pi, arctan);

            Console.WriteLine("Итераций: {0}", iterations);
        }

        Multiply(pi, 4);

        timer.Stop();

        //Print(pi);

        Console.WriteLine();
        Console.WriteLine("Знаков в секунду : {0:f1}", pi.Length / timer.Elapsed.TotalSeconds);
        Console.WriteLine("  Прошло времени : {0}", timer.Elapsed);
        Console.WriteLine("        Итераций : {0}", iterations);
    }

    public static async Task CalculateParallelAsync(int N)
    {
        Console.WriteLine("Асинхронное параллельное вычисление числа pi с числом знаков: {0}", N);

        var timer = Stopwatch.StartNew();

        // Pi/4 = 12*arctan(1/18) + 8*arctan(1/57) - 5*arctan(1/239)
        long[] m = { 12, 8, -5 };
        long[] p = { 18, 57, 239 };

        Console.WriteLine("Формула вычисления:");
        Console.Write("    pi/4 =");
        for (var i = 0; i < m.Length; i++)
        {
            Console.Write(' ');
            if (m[i] >= 0) Console.Write('+');
            Console.Write("{0} * atan(1/{1})", m[i], p[i]);
        }
        Console.WriteLine();
        Console.WriteLine();

        var size = N / __Lb + 1;
        var tasks = new List<Task<(long[] Result, int Iterations, bool Sign)>>();
        for (var i = 0; i < m.Length; i++)
        {
            var p_i = p[i];
            var m_i = m[i];

            var task = Task.Run(() =>
            {
                Debug.WriteLine($"Задача запущена {m_i} * atan(1 / {p_i})");
                Console.WriteLine($"Задача запущена {m_i} * atan(1 / {p_i})");
                var task_timer = Stopwatch.StartNew();

                var arctan = new long[size];
                var iters = ArcCot(p_i, arctan, new long[size], new long[size]);
                Multiply(arctan, Math.Abs(m_i));

                task_timer.Stop();

                Debug.WriteLine($"Задача завершена {m_i,3} * atan(1 / {p_i}) итераций:{iters} время:{task_timer.Elapsed}");
                Console.WriteLine($"Задача завершена {m_i,3} * atan(1 / {p_i}) итераций:{iters} время:{task_timer.Elapsed}");

                return (Result: arctan, Iterations: iters, Sign: m_i < 0);
            });

            tasks.Add(task);
        }

        var result = await Task.WhenAll(tasks);

        // Pi/4 = Sum(i) [m[i]*arctan(1/p[i])]
        var pi = new long[size];
        var iterations = 0;
        foreach (var (atan, result_iterations, sign) in result)
        {
            iterations += result_iterations;
            if (sign)
                Substrate(pi, atan);
            else
                Add(pi, atan);
        }

        Multiply(pi, 4);

        timer.Stop();

        //Print(pi);

        Console.WriteLine();
        Console.WriteLine("Знаков в секунду : {0:f1}", pi.Length / timer.Elapsed.TotalSeconds);
        Console.WriteLine("  Прошло времени : {0}", timer.Elapsed);
        Console.WriteLine("        Итераций : {0}", iterations);
    }


    private static void Print(long[] X)
    {
        Console.Write(X[0]);
        Console.Write('.');
        for (var i = 1; i < X.Length; i++)
            Console.Write(X[i]);
        Console.WriteLine();
    }

    public static void Clear(long[] X) => Array.Clear(X);

    /// <summary>Перевод большого действительного числа к малому целому</summary>
    /// <param name="x"></param>
    /// <param name="I"></param>
    private static void SetToInteger(long[] x, long I)
    {
        //for (long i = 1; i < x.Length; i++)
        //    x[i] = 0;
        Array.Clear(x);
        x[0] = I;
    }

    private static bool IsEmpty(long[] X)
    {
        for (var i = 0; i < X.Length; i++)
            if (X[i] != 0)
                return false;
        return true;
    }

    private static bool IsEmpty(long[] X, out int i0, out long x0)
    {
        for (var i = 0; i < X.Length; i++)
            if (X[i] != 0)
            {
                i0 = i;
                x0 = X[i];
                return false;
            }

        i0 = -1;
        x0 = -1;
        return true;
    }

    /// <summary>x = x + y</summary>
    private static void Add(long[] x, long[] y)
    {
        long carry = 0;
        for (long i = x.Length - 1; i >= 0; i--)
        {
            x[i] += y[i] + carry;
            if (x[i] < __B) carry = 0;
            else
            {
                carry = 1;
                x[i] -= __B;
            }
        }
    }

    private static void Substrate(long[] X, long[] Y)
    {
        for (long i = X.Length - 1; i >= 0; i--)
        {
            X[i] -= Y[i];
            if (X[i] < 0 && i != 0)
            {
                X[i] += __B;
                X[i - 1]--;
            }
        }
    }

    /// <summary>X *= q</summary>
    private static void Multiply(long[] X, long q)
    {
        long carry = 0;
        for (long i = X.Length - 1; i >= 0; i--)
        {
            var x = X[i] * q + carry;
            if (x < __B)
                carry = 0;
            else
            {
                //(carry, x) = (x / __B, x - carry * __B);
                carry = x / __B;
                x -= carry * __B;
            }

            X[i] = x;
        }
    }

    /// <summary>Y = X / d</summary>
    private static void Divide(long[] X, long d, long[] Y)
    {
        long carry = 0;
        var n = X.Length;
        for (long i = 0; i < n; i++)
        {
            var x = X[i] + carry * __B;
            var q = x / d;
            carry = x - q * d;
            Y[i] = q;
        }
    }

    /// <summary>
    /// Вычисление котангенса целого числа р (это тангенс (1/p))<br/>
    /// Результат - большое действительное число размера n
    /// </summary>
    /// <param name="p"></param>
    /// <param name="result"></param>
    /// <param name="uk"></param>
    /// <param name="vk"></param>
    private static int ArcCot(long p, long[] result, long[] uk, long[] vk)
    {
        var p2 = p * p;
        long k = 3;
        var sign = false;

        Clear(result);
        SetToInteger(uk, 1);

        Divide(uk, p, uk);              // uk = 1/p
        Add(result, uk);                // x  = uk

        var j = 0;
        var last_i0 = -1;
        while (!IsEmpty(uk, out var i0, out var x0))
        {
            //Debug.WriteLine("atan(1/{0}) {1,5:f1}% X=[{2}]:{3,-5}", p, (double)(i0 + 1) / uk.Length * 100, i0, x0);

            if (p < __MaxDiv)
                Divide(uk, p2, uk);     // Один шаг малого p
            else
            {
                Divide(uk, p, uk);      // Два шага большого p (смотри деление)
                Divide(uk, p, uk);
            }

            // uk = u(k-1) / p^2
            Divide(uk, k, vk);          // vk = uk / k
            if (sign)
                Add(result, vk);        // x = x + vk
            else
                Substrate(result, vk);  // x = x - vk

            k += 2;
            sign = !sign;
            j++;

            if (i0 == last_i0) continue;

        }

        return j;
    }

    private static int ArcCot(long p, long[] result, long[] uk, long[] vk, IProgress<double> Progress, CancellationToken Cancel)
    {
        var p2 = p * p;
        long k = 3;
        var sign = false;

        Clear(result);
        SetToInteger(uk, 1);

        Divide(uk, p, uk);              // uk = 1/p
        Add(result, uk);                // x  = uk

        var j = 0;
        var last_i0 = -1;
        while (!IsEmpty(uk, out var i0, out var x0))
        {
            Cancel.ThrowIfCancellationRequested();
            //Debug.WriteLine("atan(1/{0}) {1,5:f1}% X[{2}]:{3,-5}", p, (double)(i0 + 1) / uk.Length * 100, i0, x0);

            if (p < __MaxDiv)
                Divide(uk, p2, uk);     // Один шаг малого p
            else
            {
                Divide(uk, p, uk);      // Два шага большого p (смотри деление)
                Divide(uk, p, uk);
            }

            // uk = u(k-1) / p^2
            Divide(uk, k, vk);          // vk = uk / k
            if (sign)
                Add(result, vk);        // x = x + vk
            else
                Substrate(result, vk);  // x = x - vk

            k += 2;
            sign = !sign;
            j++;

            if (i0 == last_i0) continue;
            Progress.Report((double)(i0 + 1) / uk.Length);
            last_i0 = i0;
        }

        Progress.Report(1);

        return j;
    }
}
