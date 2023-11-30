using System.Linq;

using MathCore.Statistic;
using CommunityToolkit.HighPerformance;
using MathCore.Hash;
using MMD5 = System.Security.Cryptography.MD5;
using System.Globalization;
#pragma warning disable CS8321 // Local function is declared but never used

var bytes = new byte[60];
for (var i = 0; i < bytes.Length; i++)
    bytes[i] = (byte)i;

var hash0 = MMD5.HashData(bytes).ToStringHex();
var hash1 = MD5.Compute(bytes).ToStringHex();
//var hash2 = MD5.Compute(bytes.AsSpan()).ToStringHex();

//const    int seed   = 1395601201;
//const    int seed   = 3;
const double sigma = 3;
const double mu     = 5;
const    int count  = 10000;

var seed = (int)DateTime.Now.Ticks;
Console.WriteLine("Seed = {0}", seed);
var rnd = new Random(seed);

//var xx = rnd.NextNormal(count, sigma, mu);
var xx = EnumNormal(rnd).Take(count).ToArray(x => x * sigma + mu);

var avg = xx.Average();
var sgm = xx.Dispersion().Sqrt();

var interval = xx.GetMinMax();
var min = interval.Min;
var interval_length = interval.Length;

var dx = interval_length / (1 + 3.32 * Math.Log10(count));
var segments_count = (int)Math.Round(interval_length / dx);
var histogram_dx = interval_length / (segments_count - 0);

var histogram = new int[segments_count];
foreach (var x in xx)
{
    var i0 = Math.Min(segments_count - 1, (int)Math.Floor((x - min) / histogram_dx));
    histogram[i0]++;
}

var distribution = Distributions.NormalGauss(sigma, mu);
var expected = new double[segments_count];
var average = 0d;
for (var i = 0; i < segments_count; i++)
{
    var x = min + (i + 0.5) * dx;
    expected[i] = histogram_dx * distribution(x) * count;
    average += x * histogram[i];
}
average /= count;

var variance = 0d;
for (var i = 0; i < segments_count; i++)
    variance += (average - (min + (i + 0.5) * histogram_dx)).Pow2() * histogram[i];
variance /= count;
var sko = variance.Sqrt();

var variance_restored = variance * count / (count - 1);
var sko_restored = variance_restored.Sqrt();

var freedom_degree = segments_count - 3;

var criteria_p = 0d;
var criteria_n = 0d;
for (var i = 0; i < segments_count; i++)
{
    var n = histogram[i];

    var x = min + (i + 0.5) * dx;
    var p0 = histogram_dx * distribution(x);
    var p = (double)n / count;
    var n0 = p0 * count;

    var delta_p = p0 - p;
    var delta_p2 = delta_p.Pow2();
    var delta_pr = delta_p2 / p0;
    criteria_p += delta_pr;

    var delta_n = n0 - n;
    var delta_n2 = delta_n.Pow2();
    var delta_nr = delta_n2 / n0;
    criteria_n += delta_nr;

    Console.WriteLine("{0,2} [{1,6}]p:{2,-8:0.00000#} - [{3,6}]p0:{4,-10:0.0000000#} - d:{5,-14:0.########;0.########} - d0:{6,-12:0.########} - h:{7}", 
        i, histogram[i], p, (int)(p0 * count), p0, delta_n, delta_nr, criteria_n);
}

var quantile = SpecialFunctions.Distribution.Student.QuantileHi2(0.9, freedom_degree);

criteria_p *= count;

var yy = GetChi2(histogram, expected);

Console.WriteLine("chi_p:{0,-6:0.####} < q:{1,-7:0.####} - {2}", criteria_p, quantile, criteria_p < quantile);
Console.WriteLine("chi_n:{0,-6:0.####} < q:{1,-7:0.####} - {2}", criteria_n, quantile, criteria_n < quantile);
//Console.ReadLine();

static double GetChi2(int[] observed, double[] expected)
{
    var sum = 0.0;
    for (var i = 0; i < observed.Length; ++i) 
        sum += (observed[i] - expected[i]).Pow2() / expected[i];
    return sum;
}

static IEnumerable<double> EnumNormal(Random rnd)
{
    while (true)
    {
        double x, y, r;
        do
        {
            x = rnd.NextDouble() * 2 - 1;
            y = rnd.NextDouble() * 2 - 1;
            r = x * x + y * y;
        }
        while (r.Abs() < double.Epsilon || r > 1);

        var sqrt_log_r = (-2 * Math.Log(r) / r).Sqrt();

        yield return x * sqrt_log_r;
        yield return y * sqrt_log_r;
    }
}

static double ChiSquarePval(double x, int df)
{
    // x = a computed chi-square value.
    // df = degrees of freedom.
    // output = prob. x value occurred by chance.
    // ACM 299.
    if (x <= 0 || df < 1)
        throw new Exception("Bad arg in ChiSquarePval()");

    var y = 0.0;
    double ee; // change from e
    double c;
    var a = 0.5 * x; // 299 variable names
    var even = df % 2 == 0; // Is df even?

    if (df > 1) y = Exp(-a); // ACM update remark (4)

    var s = even ? y : 2 * Gauss(-Math.Sqrt(x));

    if (df <= 2) return s;

    x = 0.5 * (df - 1);
    var z = even ? 1 : 0.5;
    if (a > 40.0) // ACM remark (5)
    {
        ee = even ? 0 : 0.5723649429247000870717135;
        c = Math.Log(a); // log base e
        while (z <= x)
        {
            ee = Math.Log(z) + ee;
            s += Exp(c * z - a - ee); // ACM update remark (6)
            z++;
        }
        return s;
    }

    ee = even ? 1 : 0.5641895835477562869480795 / Math.Sqrt(a);

    c = 0;
    while (z <= x)
    {
        ee *= a / z; // ACM update remark (7)
        c += ee;
        z++;
    }
    return c * y + s;

} 

static double Exp(double x) => x < -40
    ? 0
    : Math.Exp(x);

static double Gauss(double z)
{
    // input = z-value (-inf to +inf)
    // output = p under Normal curve from -inf to z
    // ACM Algorithm #209
    double p; // result. called ‘z’ in 209
    if (z == 0)
        p = 0;
    else
    {
        var y = z.Abs() / 2; // 209 scratch variable
        switch (y)
        {
            case >= 3:
                p = 1;
                break;
            case < 1:
                var w = y * y; // 209 scratch variable
                p = ((((((((0.000124818987 * w
                            - 0.001075204047) * w + 0.005198775019) * w
                          - 0.019198292004) * w + 0.059054035642) * w
                        - 0.151968751364) * w + 0.319152932694) * w
                      - 0.531923007300) * w + 0.797884560593) * y
                                                              * 2;
                break;
            default:
                y -= 2;
                p = (((((((((((((-0.000045255659 * y
                                 + 0.000152529290) * y - 0.000019538132) * y
                               - 0.000676904986) * y + 0.001390604284) * y
                             - 0.000794620820) * y - 0.002034254874) * y
                           + 0.006549791214) * y - 0.010557625006) * y
                         + 0.011630447319) * y - 0.009279453341) * y
                       + 0.005353579108) * y - 0.002141268741) * y
                     + 0.000535310849) * y + 0.999936657524;
                break;
        }
    }

    return z > 0 
        ? (p + 1) / 2 
        : (1 - p) / 2;
}