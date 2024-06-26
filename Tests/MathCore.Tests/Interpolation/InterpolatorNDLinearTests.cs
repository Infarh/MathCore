using MathCore.Interpolation;

namespace MathCore.Tests.Interpolation;

[TestClass]
public class InterpolatorNDLinearTests
{
    private const string __DataFilePath = "Interpolation/InterpolatorNDData.zip";

    private static FileInfo DataFile => new FileInfo(__DataFilePath).ThrowIfNotFound();

    [TestMethod, Ignore]
    public void TestInterpolation()
    {
        var file = DataFile;

        var interpolator = InterpolatorNDLinear.LoadCSV(file/*, ValueSelector: (_, v) => v > 0*/);

        var value_480 = interpolator[0, 10668, 28000, 0.480]; //0.504628
        var value_490 = interpolator[0, 10668, 28000, 0.490]; //0.514852

        var value_485 = interpolator[0, 10668, 28000, 0.485]; //(0.504628 + 0.514852)/2 = 0.50974

        value_480.AssertEquals(0.504628);
        value_490.AssertEquals(0.514852);

        value_485.AssertEquals((0.504628 + 0.514852) / 2);
    }
}
