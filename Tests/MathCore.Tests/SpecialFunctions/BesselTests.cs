using Microsoft.VisualStudio.TestTools.UnitTesting.Extensions;

namespace MathCore.Tests.SpecialFunctions;

[TestClass]
public class BesselTests
{
    [TestMethod]
    public void Polynom3()
    {
        const int n = 3;

        var polynom = MathCore.SpecialFunctions.Bessel.Polynom(n);

        //polynom.ToDebug();
        polynom.Coefficients.AssertEquals(1, 6, 15, 15);
    }

    [TestMethod]
    public void Polynom3Inverted()
    {
        const int n = 3;

        var polynom = MathCore.SpecialFunctions.Bessel.Polynom(-n);

        //polynom.ToDebug();
        polynom.Coefficients.AssertEquals(15, 15, 6, 1);
    }
}
