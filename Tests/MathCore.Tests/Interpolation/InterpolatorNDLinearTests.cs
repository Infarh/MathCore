using System.Diagnostics;

using MathCore.Interpolation;

namespace MathCore.Tests.Interpolation;

[TestClass]
public class InterpolatorNDLinearTests
{
    private const string __DataFilePath = "Interpolation/InterpolatorNDData.zip";

    public TestContext? TestContext { get; set; }

    private static FileInfo DataFile
    {
        get
        {
            var full_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, __DataFilePath);
            var file = new FileInfo(full_path);

            return file.ThrowIfNotFound(FullPathInMessage: true);
        }
    }

    [TestMethod]
    public void TestInterpolation_FromZip()
    {
        var file = DataFile;

        Debug.WriteLine($"Файл данных:{file.FullName} - exist:{file.Exists}");

        var interpolator = InterpolatorNDLinear.LoadCSV(file/*, ValueSelector: (_, v) => v > 0*/);

        //dT      H      G      M
        var value_480 = interpolator[0, 10668, 28000, 0.480]; //0.504628
        var value_490 = interpolator[0, 10668, 28000, 0.490]; //0.514852

        var value_485 = interpolator[0, 10668, 28000, 0.485]; //(0.504628 + 0.514852)/2 = 0.50974

        value_480.AssertEquals(0.504628);
        value_490.AssertEquals(0.514852);

        value_485.AssertEquals((0.504628 + 0.514852) / 2);
    }

    [TestMethod]
    public void TestInterpolation_OneDimensional()
    {
        using var reader = new StringReader("x;f\n0;0\n1;10\n2;20\n");
        var interpolator = InterpolatorNDLinear.LoadCSV(reader);

        var y0 = interpolator[0];
        var y1 = interpolator[1];
        var y05 = interpolator[0.5];

        y0.AssertEquals(0);
        y1.AssertEquals(10);
        y05.AssertEquals(5);
    }

    [TestMethod]
    public void TestInterpolation_TwoDimensional_Sequential()
    {
        // f(x,y) = x + 10*y на сетке 2x2
        using var reader = new StringReader("x;y;f\n0;0;0\n1;0;1\n0;1;10\n1;1;11\n");
        var interpolator = InterpolatorNDLinear.LoadCSV(reader);

        var f00 = interpolator[0, 0];
        var f10 = interpolator[1, 0];
        var f01 = interpolator[0, 1];
        var f11 = interpolator[1, 1];

        var f05_0 = interpolator[0.5, 0];      // интерполяция по x
        var f0_05 = interpolator[0, 0.5];      // интерполяция по y
        var f05_05 = interpolator[0.5, 0.5];   // последовательная интерполяция по x и затем по y

        f00.AssertEquals(0);
        f10.AssertEquals(1);
        f01.AssertEquals(10);
        f11.AssertEquals(11);

        f05_0.AssertEquals(0.5);
        f0_05.AssertEquals(5);
        f05_05.AssertEquals(5.5);
    }
}
