using MathCore.Statistic;

const    int seed   = 1395601201;
const double sigma = 3;
const double mu     = 5;
const    int count  = 100000;

var rnd = new Random(seed);
var xx = rnd.NextNormal(count, sigma, mu);

var interval = xx.GetMinMax();
var min = interval.Min;
var interval_length = interval.Length;

var dx = interval_length / (1 + 3.32 * Math.Log10(count));
var segments_count = (int)Math.Round(interval_length / dx);
var histogram_dx = interval_length / segments_count;

var histogram = new int[segments_count];
foreach (var x in xx)
{
    var i = Math.Min(histogram.Length - 1, (int)Math.Floor((x - min) / dx));
    histogram[i]++;
}

var average = 0d;
for (var i = 0; i < segments_count; i++)
    average += (min + (i + 0.5) * dx) * histogram[i];
average /= count;

var variance = 0d;
for (var i = 0; i < segments_count; i++)
    variance += (average - (min + (i + 0.5) * dx)).Pow2() * histogram[i];
variance /= count;
//var sko = variance.Sqrt();

var variance_restored = variance * count / (count - 1);
var sko_restored = variance_restored.Sqrt();

var freedom_degree = segments_count - 1;

var distribution = Distributions.NormalGauss(sigma, mu);

var criteria = 0d;
for (var i = 0; i < segments_count; i++)
{
    var x = min + (i + 0.5) * dx;
    var p0 = histogram_dx * distribution(x);
    var p = (double)histogram[i] / count;
    criteria += (p0 - p).Pow2() / p0;

    Console.WriteLine("{0,2} [{1,5}] - p:{2,-8:0.00000#} - p0:{3,-10:0.0000000#} - d:{4,-10:0.########} - d0:{5,-10:0.########} - h:{6}", 
        i, histogram[i], p, p0, (p - p0).Abs(), (p0 - p).Pow2() / p0, criteria);
}

var quantile = SpecialFunctions.Distribution.Student.QuantileHi2(0.99, freedom_degree);

Console.WriteLine("chi:{0} - q:{1}", criteria, quantile);
Console.ReadLine();