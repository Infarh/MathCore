using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathCore.Tests;

[TestClass]
public class StringPtrTests
{
    private static readonly CultureInfo __Culture = CultureInfo.InvariantCulture;

    [TestMethod]
    public void TryParseDouble_Integer_value()
    {
        const string str            = "123";
        var          expected_value = double.Parse(str, __Culture);

        var s         = str.AsStringPtr();
        var is_parsed = s.TryParseDouble(__Culture, out var actual_value);

        is_parsed.AssertEquals(true);
        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void TryParseDouble_Fractional_value()
    {
        const string str            = "123.456";
        var          expected_value = double.Parse(str, __Culture);

        var s         = str.AsStringPtr();
        var is_parsed = s.TryParseDouble(__Culture, out var actual_value);

        is_parsed.AssertEquals(true);
        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void TryParseDouble_Exponential_integer_value()
    {
        const string str            = "123E-010";
        var          expected_value = double.Parse(str, __Culture);

        var s         = str.AsStringPtr();
        var is_parsed = s.TryParseDouble(__Culture, out var actual_value);

        is_parsed.AssertEquals(true);
        actual_value.AssertEquals(expected_value, 2e-16);
    }

    [TestMethod]
    public void TryParseDouble_Exponential_fractional_value()
    {
        const string str            = "0.000123E+010";
        var          expected_value = double.Parse(str, __Culture);

        var s         = str.AsStringPtr();
        var is_parsed = s.TryParseDouble(__Culture, out var actual_value);

        is_parsed.AssertEquals(true);
        actual_value.AssertEquals(expected_value);
    }
}
