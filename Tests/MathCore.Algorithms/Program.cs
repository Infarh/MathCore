using System.Diagnostics;
using System.Numerics;


const int x0 = 2;
var x = new BigInteger(x0);

//const int n = 2 * 2 * 2 * 3 * 3 * 5 * 5 * 5 * 13 * 13;
const int n = 2 * 2 * 3 * 5 * 5 * 5 * 7 * 13 * 13;
//const int n = 2 * 3 * 5;
//const int n = 2 * 2 * 7;
Console.WriteLine("pow n: {0} - 38,6275725s", n);

BigInteger expected;
if (false && File.Exists($"expected({x0}pow{n}).bin"))
{
    using var file = File.OpenRead($"expected({x0}pow{n}).bin");
    var buffer = new byte[file.Length];
    file.Read(buffer);
    expected = new BigInteger(buffer);
}
else
{
    expected = PowSimple(x, n);
    using var file = File.Create($"expected({x0}pow{n}).bin");
    file.Write(expected.ToByteArray());
}

//var expected_str = expected.ToString();
//Console.WriteLine(expected_str);

var actual = PowFast(x, n);

var equals = expected == actual;
Console.WriteLine("Equal: {0}", equals);

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