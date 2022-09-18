#nullable enable
//------------------------------------------------------------------------------
// <copyright file="XPathScanner.cs" company="Microsoft">
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

using System.Diagnostics;
using System.Globalization;

// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

internal sealed class XPathScanner
{
    #region Types

    public enum LexKind
    {
        Comma = ',',
        Slash = '/',
        At = '@',
        Dot = '.',
        LParens = '(',
        RParens = ')',
        LBracket = '[',
        RBracket = ']',
        Star = '*',
        Plus = '+',
        Minus = '-',
        Eq = '=',
        Lt = '<',
        Gt = '>',
        Bang = '!',
        Dollar = '$',
        // ReSharper disable once IdentifierTypo
        Apos = '\'',
        Quote = '"',
        Union = '|',
        Ne = 'N',         // !=
        Le = 'L',         // <=
        Ge = 'G',         // >=
        And = 'A',        // &&
        Or = 'O',         // ||
        DotDot = 'D',     // ..
        SlashSlash = 'S', // //
        Name = 'n',       // XML _Name
        String = 's',     // Quoted string constant
        Number = 'd',     // _Number constant
        Axe = 'a',        // Axe (like child::)
        Eof = 'E'
    }

    #endregion

    #region Fields

    private bool _CanBeFunction;
    private string _Name = null!;
    private double _NumberValue = double.NaN;
    private string _Prefix = null!;
    private string _StringValue = null!;
    private int _XpathExprIndex;

    #endregion

    #region Properties

    public string SourceText { get; }

    private char CurrentChar { get; set; }

    public LexKind Kind { get; private set; }

    public string Name
    {
        get
        {
            Debug.Assert(Kind is LexKind.Name or LexKind.Axe);
            Debug.Assert(_Name != null);
            return _Name!;
        }
    }

    public string Prefix
    {
        get
        {
            Debug.Assert(Kind == LexKind.Name);
            Debug.Assert(_Prefix != null);
            return _Prefix!;
        }
    }

    public string StringValue
    {
        get
        {
            Debug.Assert(Kind == LexKind.String);
            Debug.Assert(_StringValue != null);
            return _StringValue!;
        }
    }

    public double NumberValue
    {
        get
        {
            Debug.Assert(Kind == LexKind.Number);
            Debug.Assert(!double.IsNaN(_NumberValue));
            return _NumberValue;
        }
    }

    // To parse PathExpr we need a way to distinct name from function. 
    // THis distinction can't be done without context: "or (1 != 0)" this this a function or 'or' in OrExp 
    public bool CanBeFunction
    {
        get
        {
            Debug.Assert(Kind == LexKind.Name);
            return _CanBeFunction;
        }
    }

    #endregion

    #region Constructors

    public XPathScanner(string XPathExpr)
    {
        SourceText = XPathExpr ?? throw new XPathException($"'{string.Empty}' is an invalid expression.");
        NextChar();
        NextLex();
    }

    #endregion

    #region Methods

    private bool NextChar()
    {
        Debug.Assert(0 <= _XpathExprIndex && _XpathExprIndex <= SourceText.Length);
        if(_XpathExprIndex < SourceText.Length)
        {
            CurrentChar = SourceText[_XpathExprIndex++];
            return true;
        }
        CurrentChar = '\0';
        return false;
    }

    private void SkipSpace()
    {
        while(XmlCharType.IsWhiteSpace(CurrentChar) && NextChar()) { }
    }

    public bool NextLex()
    {
        SkipSpace();
        switch(CurrentChar)
        {
            case '\0':
                Kind = LexKind.Eof;
                return false;
            case ',':
            case '@':
            case '(':
            case ')':
            case '|':
            case '*':
            case '[':
            case ']':
            case '+':
            case '-':
            case '=':
            case '#':
            case '$':
                Kind = (LexKind)Convert.ToInt32(CurrentChar);
                NextChar();
                break;
            case '<':
                Kind = LexKind.Lt;
                NextChar();
                if(CurrentChar == '=')
                {
                    Kind = LexKind.Le;
                    NextChar();
                }
                break;
            case '>':
                Kind = LexKind.Gt;
                NextChar();
                if(CurrentChar == '=')
                {
                    Kind = LexKind.Ge;
                    NextChar();
                }
                break;
            case '!':
                Kind = LexKind.Bang;
                NextChar();
                if(CurrentChar == '=')
                {
                    Kind = LexKind.Ne;
                    NextChar();
                }
                break;
            case '.':
                Kind = LexKind.Dot;
                NextChar();
                if(CurrentChar == '.')
                {
                    Kind = LexKind.DotDot;
                    NextChar();
                }
                else if(XmlCharType.IsDigit(CurrentChar))
                {
                    Kind         = LexKind.Number;
                    _NumberValue = ScanFraction();
                }
                break;
            case '/':
                Kind = LexKind.Slash;
                NextChar();
                if(CurrentChar == '/')
                {
                    Kind = LexKind.SlashSlash;
                    NextChar();
                }
                break;
            case '"':
            case '\'':
                Kind         = LexKind.String;
                _StringValue = ScanString();
                break;
            default:
                if(XmlCharType.IsDigit(CurrentChar))
                {
                    Kind         = LexKind.Number;
                    _NumberValue = ScanNumber();
                }
                else if(XmlCharType.IsStartNcNameChar(CurrentChar))
                {
                    Kind    = LexKind.Name;
                    _Name   = ScanName();
                    _Prefix = string.Empty;
                    // "foo:bar" is one lexem not three because it doesn't allow spaces in between
                    // We should distinct it from "foo::" and need process "foo ::" as well
                    if(CurrentChar == ':')
                    {
                        NextChar();
                        // can be "foo:bar" or "foo::"
                        if(CurrentChar == ':')
                        {
                            // "foo::"
                            NextChar();
                            Kind = LexKind.Axe;
                        }
                        else
                        {
                            // "foo:*", "foo:bar" or "foo: "
                            _Prefix = _Name;
                            if(CurrentChar == '*')
                            {
                                NextChar();
                                _Name = "*";
                            }
                            else if(XmlCharType.IsStartNcNameChar(CurrentChar))
                                _Name = ScanName();
                            else
                                throw new XPathException($"'{SourceText}' has an invalid qualified name.");
                        }
                    }
                    else
                    {
                        SkipSpace();
                        if(CurrentChar == ':')
                        {
                            NextChar();
                            // it can be "foo ::" or just "foo :"
                            if(CurrentChar == ':')
                            {
                                NextChar();
                                Kind = LexKind.Axe;
                            }
                            else
                                throw new XPathException($"'{SourceText}' has an invalid qualified name.");
                        }
                    }
                    SkipSpace();
                    _CanBeFunction = CurrentChar == '(';
                }
                else
                    throw new XPathException($"'{SourceText}' has an invalid token.");
                break;
        }
        return true;
    }

    private double ScanNumber()
    {
        Debug.Assert(CurrentChar == '.' || XmlCharType.IsDigit(CurrentChar));
        var start = _XpathExprIndex - 1;
        var len   = 0;
        while(XmlCharType.IsDigit(CurrentChar))
        {
            NextChar();
            len++;
        }
        if(CurrentChar != '.') return ToXPathDouble(SourceText.Substring(start, len));
        NextChar();
        len++;
        while(XmlCharType.IsDigit(CurrentChar))
        {
            NextChar();
            len++;
        }
        return ToXPathDouble(SourceText.Substring(start, len));
    }

    private double ScanFraction()
    {
        Debug.Assert(XmlCharType.IsDigit(CurrentChar));
        var start = _XpathExprIndex - 2;
        Debug.Assert(0 <= start && SourceText[start] == '.');
        var len = 1; // '.'
        while(XmlCharType.IsDigit(CurrentChar))
        {
            NextChar();
            len++;
        }
        return ToXPathDouble(SourceText.Substring(start, len));
    }

    private string ScanString()
    {
        var end_char = CurrentChar;
        NextChar();
        var start = _XpathExprIndex - 1;
        var len   = 0;
        while(CurrentChar != end_char)
        {
            if(!NextChar())
                throw new XPathException("This is an unclosed string.");
            len++;
        }
        Debug.Assert(CurrentChar == end_char);
        NextChar();
        return SourceText.Substring(start, len);
    }

    private string ScanName()
    {
        Debug.Assert(XmlCharType.IsStartNcNameChar(CurrentChar));
        var start = _XpathExprIndex - 1;
        var len   = 0;
        while(XmlCharType.IsNcNameChar(CurrentChar))
        {
            NextChar();
            len++;
        }
        return SourceText.Substring(start, len);
    }

    internal static double ToXPathDouble(object s)
    {
        try
        {
            switch(Type.GetTypeCode(s.GetType()))
            {
                case TypeCode.String:
                    try
                    {
                        var str = ((string)s).TrimStart();
                        if(str[0] != '+')
                            return double.Parse(str,
                                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint |
                                NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
                    }
                    catch
                    {
                        // ignored
                    }

                    return double.NaN;
                case TypeCode.Double:
                    return (double)s;
                case TypeCode.Boolean:
                    return (bool)s ? 1.0 : 0.0;
                default:
                    // Script functions can fead us with Int32 & Co.
                    return Convert.ToDouble(s, NumberFormatInfo.InvariantInfo);
            }
        }
        catch
        {
            // ignored
        }

        return double.NaN;
    }

    #endregion
}