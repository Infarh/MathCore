using MathCore.Statistic;

const    int seed   = 2;
const double sigma  = 7;
const double mu     = 5;
const    int count  = 100_000;

var rnd = new Random(seed);
var xx = GetRandom(rnd, count, sigma, mu);

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

var average = 0d;
for (var i = 0; i < segments_count; i++)
    average += (min + (i + 0.5) * histogram_dx) * histogram[i];
average /= count;

var variance = 0d;
for (var i = 0; i < segments_count; i++)
    variance += (average - (min + (i + 0.5) * histogram_dx)).Pow2() * histogram[i];
variance /= count;
var sko = variance.Sqrt();

var variance_restored = variance * count / (count - 1);
var sko_restored = variance_restored.Sqrt();

var freedom_degree = segments_count - 1;

Console.WriteLine("avg:{0:0.000} var:{1:0.000}({2:0.000}) sko:{3}({4:0.000})", average, variance, variance_restored, sko, sko_restored);

var distribution = Distributions.NormalGauss(sko_restored, average);

var criteria_p = 0d;
var criteria_n = 0d;
for (var i = 0; i < segments_count; i++)
{
    var x = min + (i + 0.5) * histogram_dx;
    var expected_p = histogram_dx * distribution(x);
    var expected_n = expected_p * count;

    var actual_n = histogram[i];
    var actual_p = (double)actual_n / count;

    criteria_p += (expected_p - actual_p).Pow2() / expected_p;
    criteria_n += (expected_n - actual_n).Pow2() / expected_n;

    Console.WriteLine("{0,2} [{1,8}] - p:{2,-8:0.00000#} - [{3,8:0}]p0:{4,-10:0.0000000#} - [{5,7:0}]d:{6,-10:0.########} - [{7,8:0.00}]d0:{8,-10:0.########} - h:{9:0.00}", 
        i, 
        histogram[i], actual_p, 
        expected_n, expected_p, 
        (expected_n - actual_n).Abs(), (actual_p - expected_p).Abs(),
        (expected_n - actual_n).Pow2() / expected_n, (expected_p - actual_p).Pow2() / expected_p, 
        criteria_p * count);
}

criteria_p *= count;

var quantile = SpecialFunctions.Distribution.Student.QuantileHi2(0.3, freedom_degree);

Console.WriteLine("chi_p:{0:0.00} - q:{1:0.00}", criteria_p, quantile);
Console.WriteLine("chi_n:{0:0.00} - q:{1:0.00}", criteria_n, quantile);
Console.ReadLine();

static double[] GetRandom(Random rnd, int Count, double Sgm, double Mu) => rnd.NextNormal(Count, Sgm, Mu);

static double[] GetRandom2(Random rnd, int Count, double Sgm, double Mu)
{
    var xx = new double[count];

    for(var i = 0; i < count; i++)
        xx[i] = GetRandom2(rnd, Sgm, Mu);

    return xx;

    static double GetRandom2(Random rnd, double Sgm, double Mu)
    {
        var r2 = 0d;
        var x = rnd.NextDouble() * 2 - 1;
        for (var y = rnd.NextDouble() * 2 - 1; r2 is > 1 or 0d; y = x, x = rnd.NextDouble() * 2 - 1)
            r2 = x * x + y * y;

        return Math.Sqrt(-2 * Math.Log(r2) / r2) * x * Sgm + Mu;
    }
}

static double[] GetRandom3(Random rnd, int Count, double Sgm, double Mu)
{
    var xx = new double[count];

    for (var i = 0; i < count; i++)
        xx[i] = GetRandom3(rnd, Sgm, Mu, 5);

    return xx;

    static double GetRandom3(Random rnd, double Sgm, double Mu, int n)
    {
        var s = 1d;
        for (var i = 0; i < n; i++)
            s *= rnd.NextDouble();

        return (n * s * 2 - 1) * Sgm + Mu;
    }
}

static double[] GetRandom4(Random rnd, int Count, double Sgm, double Mu)
{
    var xx = EnumRandom4(rnd, Sgm, Mu).Take(count).ToArray();

    return xx;

    static IEnumerable<double> EnumRandom4(Random rnd, double Sgm, double Mu)
    {
        while (true)
        {
            var u1 = 1 - rnd.NextDouble();
            var u2 = 1 - rnd.NextDouble();

            var sqrt = Math.Sqrt(-2 * Math.Log(u1));

            var z1 = sqrt * Math.Cos(2 * Math.PI * u2);
            yield return z1 * Sgm + Mu;

            var z2 = sqrt * Math.Sin(2 * Math.PI * u2);
            yield return z2 * Sgm + Mu;
        }
    }
}