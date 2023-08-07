using System.Diagnostics;
using System.Numerics;

using MathCore.Algorithms.Numbers;

const int x0 = 2;

//const int n = 2 * 2 * 2 * 3 * 3 * 5 * 5 * 5 * 13 * 13;
//const int n = 2 * 2 * 3 * 5 * 5 * 5 * 7 * 13 * 13;
const int n = 2 * 2 * 3 * 5 * 7;


Console.WriteLine("End.");
//Console.ReadLine();


static BigInteger PowSimple(BigInteger x, uint n)
{
    Console.WriteLine("simple {0}^{1}", x, n);
    var timer = Stopwatch.StartNew();
    try
    {
        var result = new BigInteger(1);

        for (var i = 0; i < n; i++)
        {
            result *= x;
            //Console.WriteLine(result);
        }

        return result;
    }
    finally
    {
        timer.Stop();
        Console.WriteLine("Calculation of SIMPLE x^{0} completed at time {1}s", n, timer.Elapsed.TotalSeconds);
    }
}

static BigInteger PowFast(BigInteger x, uint n)
{
    var timer = Stopwatch.StartNew();
    try
    {
        if (n == 0) return new(1);
        //if (n == 1) return x;
        //if (n == 2) return x * x;
        //if (n == 3) return x * x * x;

        var result = x;

        var power = n;
        while (power > 0 && power % 2 == 0)
        {
            result *= result;
            power >>= 1;
        }

        while (power > 0 && power % 3 == 0)
        {
            result *= result * result;
            power /= 3;
        }

        return power > 1 ? result * PowFast(result, power - 1) : result;
    }
    finally
    {
        timer.Stop();
        Console.WriteLine("Calculation of FAST x^{0} completed at time {1}s", n, timer.Elapsed.TotalSeconds);
    }
}

static double PowSimpleD(double x, uint n)
{
    var result = 1d;

    for (var i = 0; i < n; i++)
        result *= x;

    return result;
}

static double PowFastD(double x, uint n)
{
    if (n == 0) return 1;
    if (n == 1) return x;
    if (n == 2) return x * x;
    if (n == 3) return x * x * x;

    var result = x;

    var power = n;
    while (power > 0 && power % 2 == 0)
    {
        result *= result;
        power >>= 1;
    }

    while (power > 0 && power % 3 == 0)
    {
        result *= result * result;
        power /= 3;
    }

    return power > 1 ? result * PowFastD(result, power - 1) : result;
}