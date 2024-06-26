using System.Diagnostics;

using MathCore.Interpolation;

namespace MathCore.Tests.Interpolation;

[TestClass, Ignore]
public class InterpolatorNDLagrangeTests
{
    private const string __DataFilePath = "Interpolation/InterpolatorNDData.zip";

    [TestMethod]
    public void TestInterpolation()
    {
        var data_file = new FileInfo(__DataFilePath).ThrowIfNotFound();

        var interpolator = InterpolatorNDLagrange.LoadCSV(data_file);

        var i = 0;
        var time = Stopwatch.StartNew();
        var last_time = time.Elapsed.TotalSeconds;
        var time_per_operation = 0d;
        foreach (var (point, expected_value) in interpolator.Points)
        {
            i++;
            var actual_value = interpolator[point];

            actual_value.AssertEquals(expected_value);
            last_time = time.Elapsed.TotalSeconds;
            time_per_operation = last_time / i;

            if (i == 10)
                break;
        }
    }
}