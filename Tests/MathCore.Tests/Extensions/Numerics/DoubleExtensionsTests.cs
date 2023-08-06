using System.Diagnostics;

namespace MathCore.Tests.Extensions.Numerics;

[TestClass]
public class DoubleExtensionsTests
{
    [TestMethod]
    public void PowIntTest()
    {
        const double x = 2;
        const int n = 2 * 2 * 3 * 3 * 5;

        var expected = Math.Pow(x, n);

        var actual = x.Pow(n);
        //var actual2 = x.PowFast(n);

        actual.AssertEquals(expected);
        //actual2.AssertEquals(expected);
        actual.ToDebug();
    }
}
