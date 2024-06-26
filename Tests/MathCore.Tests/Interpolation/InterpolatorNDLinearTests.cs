using System.Diagnostics;

using MathCore.Interpolation;

namespace MathCore.Tests.Interpolation;

[TestClass]
public class InterpolatorNDLinearTests
{
    private const string __DataFilePath = "Interpolation/InterpolatorNDData.zip";

    private static FileInfo DataFile => new FileInfo(__DataFilePath).ThrowIfNotFound();

    [TestMethod]
    public void TestInterpolation()
    {
        var file = DataFile;

        var interpolator = InterpolatorNDLinear.LoadCSV(file, ValueSelector: (_, v) => v > 0);

        var i = 0;
        var time = Stopwatch.StartNew();
        var last_time = time.Elapsed.TotalSeconds;
        var time_per_operation = 0d;

        
    }
}
