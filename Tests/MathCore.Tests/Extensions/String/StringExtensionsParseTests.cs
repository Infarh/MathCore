namespace MathCore.Tests.Extensions.String;

[TestClass]
public class StringExtensionsParseTests
{
    [TestMethod]
    public void ParseDoubleInvariant()
    {
        const string str_en = "3.1415";
        const string str_ru = "3,1415";

        const double expected = 3.1415;

        var value_en = str_en.ToDoubleInvariant();
        var value_ru = str_ru.ToDoubleInvariant();

        value_en.AssertEquals(expected);
        value_ru.AssertEquals(expected);
    }
}