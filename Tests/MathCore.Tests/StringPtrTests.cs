﻿using System.Globalization;

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

    [TestMethod]
    public void ParseInt32_ZeroValueString()
    {
        const string str = "0";
        const int expected = 0;

        var str_ptr = str.AsStringPtr();

        var actual = str_ptr.ParseInt32();

        actual.AssertEquals(expected);
    }

    [TestMethod]
    public void LastIndexOf_Test()
    {
        const string str = "-01234:::56789-";

        var ptr = str.AsStringPtr();

        //var ptr2 = ptr.Substring(1, -1);
        var ptr2 = ptr[1, -1];

        var actual_index = ptr2.LastIndexOf(':');

        actual_index.AssertEquals(7);
    }

    [TestMethod]
    public void LastIndexOf_Test2()
    {
        const string str = "(0,1..127):8";
        const int expected = 10;

        var pos = 5;

        var qq = str.LastIndexOf(':', pos + (str.Length - 1 - pos), (str.Length - pos));

        var str_ptr = str.AsStringPtr();

        var actual = str_ptr.LastIndexOf(':');

        actual.AssertEquals(expected);
    }

    [TestMethod]
    public void SubstringBefore_Test()
    {
        const string str = "Value=123";
        var ptr = str.AsStringPtr();
        var name = ptr.SubstringBefore('=');

        name.ToString().AssertEquals("Value");
    }

    [TestMethod]
    public void SubstringAfter_Test()
    {
        const string str = "Value=123";
        var ptr = str.AsStringPtr();
        var name = ptr.SubstringAfter('=');

        name.ToString().AssertEquals("123");
    }

    [TestMethod]
    public void TrimStart_Test()
    {
        const string str = ">Value='''123''';";
        var ptr = str.AsStringPtr(1, -1);

        var value = ptr.SubstringAfter('=');

        var trimmed = value.TrimStart('\'');

        trimmed.ToString().AssertEquals("123'''");
    }

    [TestMethod]
    public void TrimEnd_Test()
    {
        const string str = ">Value='''123''';";
        var ptr = str.AsStringPtr(1, -1);

        var value = ptr.SubstringAfter('=');

        var trimmed = value.TrimEnd('\'');

        trimmed.ToString().AssertEquals("'''123");
    }

    [TestMethod]
    public void Trim_Test()
    {
        const string str = ">Value='''123''';";
        var ptr = str.AsStringPtr(1, -1);

        var value = ptr.SubstringAfter('=');

        var trimmed = value.Trim('\'');

        trimmed.ToString().AssertEquals("123");
    }
}
