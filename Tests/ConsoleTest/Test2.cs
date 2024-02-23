using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.ImageSharp;
using OxyPlot.Series;

namespace ConsoleTest;
public class Test2
{
    public static void Run()
    {
        var model = new PlotModel
        {
            Background = OxyColors.White,
            Series =
            {
                new LineSeries
                {
                    ItemsSource = Enumerable.Range(1, 100).Select(i => new DataPoint())
                }
            }
        };

        model.Background = OxyColors.White;

        model.Axes.Add(new LinearAxis()
        {
            Position = AxisPosition.Left
        });

        var series = new LineSeries();

        series.Color = OxyColors.Red;
        series.StrokeThickness = 2;

        model.Series.Add(series);

        var exporter = new PngExporter(800, 600);
        using var stream = File.Create("plot.png");
        exporter.Export(model, stream);


        var svg_export = new SvgExporter();
        svg_export.Export(model, stream);
    }
}
