//------------------------------------------------------------------------------
// <copyright file="FunctionQuery.cs" company="Microsoft">
//     
//      Copyright (c) 2002 Microsoft Corporation.  All rights reserved.
//     
//      The use and distribution terms for this software are contained in the file
//      named license.txt, which can be found in the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by the
//      terms of this license.
//     
//      You must not remove this notice, or any other, from this software.
//     
// </copyright>
//------------------------------------------------------------------------------

using System.Collections;
using System.Text;

// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

using FT = Function.FunctionType;

internal sealed class StringFunctions : Query
{
    #region Fields

    private readonly ArrayList _ArgList;

    private readonly FT _FuncType;

    #endregion

    #region Constructors

    public StringFunctions() { }

    public StringFunctions(ArrayList qy, FT FuncType)
    {
        _ArgList  = qy;
        _FuncType = FuncType;
    }

    #endregion

    #region Methods

    internal override object GetValue(XPathReader reader) => _FuncType switch
    {
        FT.FuncString          => toString(reader),
        FT.FuncConcat          => Concat(reader),
        FT.FuncStartsWith      => StartsWith(reader),
        FT.FuncContains        => Contains(reader),
        FT.FuncSubstringBefore => SubstringBefore(reader),
        FT.FuncSubstringAfter  => SubstringAfter(reader),
        FT.FuncSubstring       => Substring(reader),
        FT.FuncStringLength    => StringLength(reader),
        FT.FuncNormalize       => Normalize(reader),
        FT.FuncTranslate       => Translate(reader),
        _                      => new object()
    };

    internal override XPathResultType ReturnType() => _FuncType switch
    {
        FT.FuncStringLength => XPathResultType.Number,
        FT.FuncStartsWith   => XPathResultType.Boolean,
        FT.FuncContains     => XPathResultType.Boolean,
        _                   => XPathResultType.String
    };

    private static string toString(double num) => Convert.ToString(num);

    private static string toString(bool b) => b ? "true" : "false";

    //
    // string string(object?)
    // object
    // node-set: string value of the first node
    //           node-set = null, String.Empty return
    // number: NaN -> "NaN"
    //         +0->"0", -0->"0",
    //         +infinity -> "Infinity" -infinity -> "Infinity"
    // boolean: true -> "true" false -> "false"
    //
    // Example: <Root><e a1='1' a2='2'/>text1</e>
    //                <e a='12'> text2</e>
    //          </Root>
    // /Root/e[string(self::node())="text"]
    // /Root/e[string(attribute::node())='1']
    // /Root[string(/e)="text"]

    private string toString(XPathReader reader) => _ArgList is { Count: > 0 }
        ? (((Query)_ArgList[0]).GetValue(reader)?.ToString() ?? string.Empty)
        : string.Empty;

    private string Concat(XPathReader reader)
    {
        var count = 0;
        var s     = new StringBuilder();
        while(count < _ArgList.Count)
            s.Append(((Query)_ArgList[count++]).GetValue(reader));
        return s.ToString();
    }

    private bool StartsWith(XPathReader reader)
    {
        var str1 = ((Query)_ArgList[0]).GetValue(reader).ToString();
        var str2 = ((Query)_ArgList[1]).GetValue(reader).ToString();

        return str1.StartsWith(str2);
    }

    private bool Contains(XPathReader reader)
    {
        var str1 = ((Query)_ArgList[0]).GetValue(reader).ToString();
        var str2 = ((Query)_ArgList[1]).GetValue(reader).ToString();
        return str1.IndexOf(str2, StringComparison.Ordinal) != -1;
    }

    private string SubstringBefore(XPathReader reader)
    {
        var str1  = ((Query)_ArgList[0]).GetValue(reader).ToString();
        var str2  = ((Query)_ArgList[1]).GetValue(reader).ToString();
        var index = str1.IndexOf(str2, StringComparison.Ordinal);
        return index != -1 ? str1.Substring(0, index) : string.Empty;
    }

    private string SubstringAfter(XPathReader reader)
    {
        var str1  = ((Query)_ArgList[0]).GetValue(reader).ToString();
        var str2  = ((Query)_ArgList[1]).GetValue(reader).ToString();
        var index = str1.IndexOf(str2, StringComparison.Ordinal);
        return index != -1 ? str1.Substring(index + str2.Length) : string.Empty;
    }

    private string Substring(XPathReader reader)
    {
        var str1 = ((Query)_ArgList[0]).GetValue(reader).ToString();
        var num  = Math.Round(Convert.ToDouble(((Query)_ArgList[1]).GetValue(reader))) - 1;
        if(double.IsNaN(num)) return string.Empty;
        if(_ArgList.Count == 3)
        {
            var num1 = Math.Round(Convert.ToDouble(((Query)_ArgList[2]).GetValue(reader)));
            if(double.IsNaN(num1)) return string.Empty;
            if(num < 0)
            {
                num1 = num + num1;
                if(num1 <= 0) return string.Empty;
                num = 0;
            }
            var maxlength             = str1.Length - num;
            if(num1 > maxlength) num1 = maxlength;
            return str1.Substring((int)num, (int)num1);
        }
        if(num < 0) num = 0;
        return str1.Substring((int)num);
    }

    private double StringLength(XPathReader reader) =>
        _ArgList is { Count: > 0 }
            ? ((Query)_ArgList[0]).GetValue(reader).ToString().Length
            : 0;

    private string Normalize(XPathReader reader)
    {
        var str1 = _ArgList is { Count: > 0 }
            ? ((Query) _ArgList[0]).GetValue(reader).ToString().Trim()
            : string.Empty;
        var count       = 0;
        var str2        = new StringBuilder();
        var first_space = true;
        while(count < str1.Length)
        {
            if(!XmlCharType.IsWhiteSpace(str1[count]))
            {
                first_space = true;
                str2.Append(str1[count]);
            }
            else if(first_space)
            {
                first_space = false;
                str2.Append(str1[count]);
            }
            count++;
        }
        return str2.ToString();
    }

    private string Translate(XPathReader reader)
    {
        var str1  = ((Query)_ArgList[0]).GetValue(reader).ToString();
        var str2  = ((Query)_ArgList[1]).GetValue(reader).ToString();
        var str3  = ((Query)_ArgList[2]).GetValue(reader).ToString();
        var str   = new StringBuilder();
        var count = 0;
        while(count < str1.Length)
        {
            var index = str2.IndexOf(str1[count]);
            if(index != -1)
            {
                if(index < str3.Length)
                    str.Append(str3[index]);
            }
            else
                str.Append(str1[count]);
            count++;
        }
        return str.ToString();
    }

    #endregion
}

//
// BooleanFunctions
//