using OxyPlot;
using OxyPlot.ImageSharp;

namespace ConsoleTest.Extensions;

internal static class PlotModelEx
{
    public static FileInfo ToPNG(this IPlotModel Model, string FilePath, int Width = 800, int Height = 600, double Resolution = 96) =>
        Model.ToPNG(new FileInfo(FilePath), Width, Height, Resolution);

    public static FileInfo ToPNG(this IPlotModel Model, FileInfo File, int Width = 800, int Height = 600, double Resolution = 96)
    {
        var exporter = new PngExporter(Width, Height, Resolution);
        using var stream = File.Create();
        exporter.Export(Model, stream);
        return File;
    }
}
