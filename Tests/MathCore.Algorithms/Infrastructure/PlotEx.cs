#nullable enable
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.ImageSharp;
using OxyPlot.Series;

namespace MathCore.Algorithms.Infrastructure;

internal static class Plot
{
    public static PlotModel Cartesian
    {
        get
        {
            var model = new PlotModel
            {
                Axes =
            {
                new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Title = "X",
                    MajorGridlineStyle = LineStyle.Dash,
                    MinorGridlineStyle = LineStyle.Dot,
                    MajorGridlineColor = OxyColors.DimGray,
                    MinorGridlineColor = OxyColors.LightGray,
                },
                new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Y",
                    MajorGridlineStyle = LineStyle.Dash,
                    MinorGridlineStyle = LineStyle.Dot,
                    MajorGridlineColor = OxyColors.DimGray,
                    MinorGridlineColor = OxyColors.LightGray,
                },
            },
                Background = OxyColors.White,
            };

            return model;
        }
    }
    

    public static PlotModel Line(this PlotModel plot, IEnumerable<(double X, double Y)> Points, OxyColor? Color = null, string? Title = null, double Thickness = 2)
    {
        var line = new LineSeries 
        {
            StrokeThickness = Thickness,
        };
        
        line.Points.AddRange(Points.Select(p => new DataPoint(p.X, p.Y)));

        if (Color is { } color)
            line.Color = color;

        if (Title is { Length: > 0 } title)
            line.Title = title;

        plot.Series.Add(line);

        return plot;
    }

    public static FileInfo ToPNG(this PlotModel plot, string FilePath = "plot.png", int Width = 800, int Height = 600, double Resolution = 96)
    {
        var exporter = new PngExporter(Width, Height, Resolution);
        using var stream = File.Create(FilePath);
        exporter.Export(plot, stream);
        return new(FilePath);
    }
}