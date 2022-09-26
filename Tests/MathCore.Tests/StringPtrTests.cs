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

        is_parsed.AssertTrue();
        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void TryParseDouble_Fractional_value()
    {
        const string str            = "123.456";
        var          expected_value = double.Parse(str, __Culture);

        var s         = str.AsStringPtr();
        var is_parsed = s.TryParseDouble(__Culture, out var actual_value);

        is_parsed.AssertTrue();
        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void TryParseDouble_Exponential_integer_value()
    {
        const string str            = "123E-010";
        var          expected_value = double.Parse(str, __Culture);

        var s         = str.AsStringPtr();
        var is_parsed = s.TryParseDouble(__Culture, out var actual_value);

        is_parsed.AssertTrue();
        actual_value.AssertEquals(expected_value, 2e-16);
    }

    [TestMethod]
    public void TryParseDouble_Exponential_fractional_value()
    {   
        const string str            = "0.000123E+010";
        var          expected_value = double.Parse(str, __Culture);

        var s         = str.AsStringPtr();
        var is_parsed = s.TryParseDouble(__Culture, out var actual_value);

        is_parsed.AssertTrue();
        actual_value.AssertEquals(expected_value, 1e-9);
    }

    [TestMethod]
    public void TokenizerSingleChar_Count()
    {
        const string str = "1;6;9";

        var tokenizer = str.AsStringPtr().Split(';');
        var count = tokenizer.Count;

        count.AssertEquals(3);
    }

    [TestMethod]
    public void TokenizerSingleChar_Count_SkipEmpty()
    {
        const string str = "1;;9";

        var tokenizer = str.AsStringPtr().Split(';').SkipEmpty();
        var count = tokenizer.Count;

        count.AssertEquals(2);
    }

    [TestMethod]
    public void TokenizerSingleChar_Indexer()
    {
        const string str = "   1;6;9   ";

        var str_ptr   = str.AsStringPtr().Trim();
        var tokenizer = str_ptr.Split(';');

        tokenizer.Count.AssertEquals(3);

        tokenizer[0].TryParseInt32().AssertEquals(1);
        tokenizer[1].TryParseInt32().AssertEquals(6);
        tokenizer[2].TryParseInt32().AssertEquals(9);

        if (tokenizer is [var a1, var a2, var a3] 
            && a1 == 1
            && a2 == 6
            && a3 == 9)
        {
            a1.ParseInt32().AssertEquals(1);
            a2.ParseInt32().AssertEquals(6);
            a3.ParseInt32().AssertEquals(9);
        }
        else
            Assert.Fail();
    }
}
