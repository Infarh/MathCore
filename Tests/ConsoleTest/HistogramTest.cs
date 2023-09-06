using ConsoleTest.Extensions;

using MathCore.Statistic;
using OxyPlot.Annotations;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Axes;

namespace ConsoleTest;

public class HistogramTest
{
    public static void Run()
    {
        var rnd = new Random(5);

        const int count = 1000_000;
        const double sigma = 1;
        const double mu = 0;

        var samples = new double[count];

        const int n = 12;
        for (var i = 0; i < count; i++)
        {
            var x = 0d;
            for (var j = 0; j < n; j++)
                x += rnd.NextDouble() - 0.5;
            samples[i] = x;
        }

        var avg = samples.Average();
        var sgm = samples.Dispersion().Sqrt();
        Console.WriteLine("avg:{0}, sgm:{1}", avg, sgm);

        //var values = rnd.NextNormal(count, sigma, m);

        //var values = Enumerable.Range(0, count).ToArray(_ => (rnd.NextDouble() - 0.5) + (rnd.NextDouble() - 0.5) + (rnd.NextDouble() - 0.5) + (rnd.NextDouble() - 0.5) + (rnd.NextDouble() - 0.5));
        var gauss = Distributions.NormalGauss(sigma, mu);
        var gauss0 = Distributions.NormalGauss(sigma, mu + 0.1);

        const int intervals_count = 60;
        var histogram = new Histogram(samples, intervals_count);

        var pirson = histogram.GetPirsonsCriteria(gauss);
        //var pirson0 = histogram.GetPirsonsCriteria(gauss0);

        //var q1 = SpecialFunctions.Distribution.Student.QuantileHi2Approximation(0.95, intervals_count - 2);
        //var q2 = SpecialFunctions.Distribution.Student.QuantileHi2(0.1, intervals_count - 2);
        var interval = histogram.Interval;
        const int function_points_count = 1000;
        var model = new PlotModel
        {
            Title = $"Pirson {pirson}",
            Background = OxyColors.White,
            Axes =
            {
                new LinearAxis
                {
                    Position = AxisPosition.Left,
                    MajorGridlineColor = OxyColors.Gray,
                    MinorGridlineColor = OxyColors.LightGray,
                    MajorGridlineStyle = LineStyle.Dash,
                    MinorGridlineStyle = LineStyle.Dot,
                },
                new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    MajorGridlineColor = OxyColors.Gray,
                    MinorGridlineColor = OxyColors.LightGray,
                    MajorGridlineStyle = LineStyle.Dash,
                    MinorGridlineStyle = LineStyle.Dot,
                }
            },
            Series =
            {
                new HistogramSeries
                {
                    FillColor = OxyColors.Blue,
                    StrokeColor = OxyColors.DarkBlue,
                    StrokeThickness = 1,
                    ItemsSource = histogram,
                    Mapping = o =>
                    {
                        var ((min, max), n, value, normal_value) = (Histogram.HistogramValue)o;
                        return new HistogramItem(min, max, value, 0);
                    },
                },
                new FunctionSeries(gauss, interval.Min, interval.Max, interval.Length / function_points_count)
                {
                    Color = OxyColors.Red
                },
            },
            Annotations =
            {
                new LineAnnotation
                {
                    X = histogram.Mean,
                    Type = LineAnnotationType.Vertical,
                    Color = OxyColors.DarkRed,
                    StrokeThickness = 2,
                    LineStyle = LineStyle.Solid,
                    Text = $"μ = {avg:0.###}"
                },
                new LineAnnotation
                {
                    X = histogram.StdDev + histogram.Mean,
                    Type = LineAnnotationType.Vertical,
                    Color = OxyColors.Red,
                    StrokeThickness = 2,
                    LineStyle = LineStyle.Dash,
                    Text = $"σ = {sgm:0.###}"
                },
                new LineAnnotation
                {
                    X = -histogram.StdDev + histogram.Mean,
                    Type = LineAnnotationType.Vertical,
                    Color = OxyColors.Red,
                    StrokeThickness = 2,
                    LineStyle = LineStyle.Dash,
                    Text = $"σ = {sgm:0.###}"
                },
                new LineAnnotation
                {
                    X = histogram.StdDev * 3 + histogram.Mean,
                    Type = LineAnnotationType.Vertical,
                    Color = OxyColors.Red,
                    StrokeThickness = 2,
                    LineStyle = LineStyle.Dash,
                    Text = "3σ"
                },
                new LineAnnotation
                {
                    X = -histogram.StdDev * 3 + histogram.Mean,
                    Type = LineAnnotationType.Vertical,
                    Color = OxyColors.Red,
                    StrokeThickness = 2,
                    LineStyle = LineStyle.Dash,
                    Text = "3σ"
                },
            }
        };


        //var result = histogram.CheckDistribution(gauss);

        model.ToPNG("image.png", 1024, 768, 125)
            .Execute()
        //.ShowInExplorer()
        ;
    }

    public static void RunIteration()
    {
        var rnd = new Random(5);

        const double D = 5;
        const double m = 3;
        const int count = 1000000;
        //var values = rnd.NextNormal(count, D, m);

        var values = Enumerable.Range(0, count).ToArray(_ => (rnd.NextDouble() - 0.5) + (rnd.NextDouble() - 0.5) + (rnd.NextDouble() - 0.5) + (rnd.NextDouble() - 0.5) + (rnd.NextDouble() - 0.5));

        var gauss = Distributions.NormalGauss(D, m);
        var gauss0 = Distributions.NormalGauss(D, m + 0.1);

        const int intervals_count = 30;
        var histogram = new Histogram(values, intervals_count);

        while (true)
        {
            Console.Clear();

            histogram.Print(Console.Out, (int)(Console.BufferWidth * 0.8));

            Console.ReadLine();
        }
    }
}