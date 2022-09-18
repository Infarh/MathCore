using System.Diagnostics;

namespace MathCore.Tests.Extensions.String;

[TestClass]
public class StringExtensionsTrimTests
{
    [TestMethod]
    public void TrimByLengthTest()
    {
        const string str              = "1234567890";
        const string trimmed_expected = "12..90";

        var trimmed_actual = str.TrimByLength(6);

        Debug.WriteLine(trimmed_actual);
        trimmed_actual.AssertEquals(trimmed_expected);
    }
}
