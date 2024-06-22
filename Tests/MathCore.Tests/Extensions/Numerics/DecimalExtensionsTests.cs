namespace MathCore.Tests.Extensions.Numerics;

[TestClass]
public class DecimalExtensionsTests
{
    [TestMethod]
    public void RoundAdaptive_Value0_Num3()
    {
        const decimal value = 0;
        const int digits = 3;

        var actual_value = value.RoundAdaptive(digits);
    }
}
