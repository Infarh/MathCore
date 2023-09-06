namespace ConsoleTest;

public static class PearsonCriteriaTest
{
    public static void Run()
    {
        const int seed = 13;
        const double sigma = 7;
        const double mu = 5;
        const int count = 100_000_000;

        var rnd = new Random(seed);

        var samples = GetSamples(rnd, count, sigma, mu);

        var (histogram, min, dx) = CreateHistogram(samples);

        var (avg, sgm) = samples.AverageAndSigma();
    }

    private static double[] GetSamples(Random rnd, int count, double sigma, double mu)
    {
        var samples = new double[count];
        for(var i = 0; i < count; i++)
            samples[i] = GetRandom(rnd, sigma, mu);
        return samples;
    }

    private static double GetRandom(Random rnd, double sigma, double mu) => rnd.NextUniform(sigma, mu);

    private static (int[] Samples, double Min, double dx) CreateHistogram(double[] samples)
    {
        static (int Segments, double Min, double dx) GetHistogramConfig(double[] samples)
        {
            var interval = samples.GetMinMax();
            var min = interval.Min;
            var interval_length = interval.Length;

            var dx = interval_length / (1 + 3.32 * Math.Log10(samples.Length));
            var segments_count = (int)Math.Round(interval_length / dx);
            var histogram_dx = interval_length / (segments_count - 0);
            return (segments_count, min, histogram_dx);
        }

        var (segments_count, min, dx) = GetHistogramConfig(samples);

        var histogram = new int[segments_count];
        foreach (var x in samples)
        {
            var i0 = Math.Min(segments_count - 1, (int)Math.Floor((x - min) / dx));
            histogram[i0]++;
        }

        return (histogram, min, dx);
    }
}
