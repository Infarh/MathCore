//------------------------------------------------------------------------------
// <copyright file="Function.cs" company="Microsoft">
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

namespace System.Xml.XPath
{
    internal class Function : AstNode
    {
        #region Types

        internal enum FunctionType
        {
            FuncLast = 0,
            FuncPosition,
            FuncCount,
            FuncLocalName,
            FuncNameSpaceUri,
            FuncName,
            FuncString,
            FuncBoolean,
            FuncNumber,
            FuncTrue,
            FuncFalse,
            FuncNot,
            FuncID,
            FuncConcat,
            FuncStartsWith,
            FuncContains,
            FuncSubstringBefore,
            FuncSubstringAfter,
            FuncSubstring,
            FuncStringLength,
            FuncNormalize,
            FuncTranslate,
            FuncLang,
            FuncSum,
            FuncFloor,
            FuncCeiling,
            FuncRound,
            FuncUserDefined,
            Error
        }

        #endregion

        #region Static

        private static readonly string[] _Str =
        {
            "last()",
            "position()",
            "count()",
            "localname()",
            "namespaceuri()",
            "name()",
            "string()",
            "boolean()",
            "number()",
            "true()",
            "false()",
            "not()",
            "id()",
            "concat()",
            "starts-with()",
            "contains()",
            "substring-before()",
            "substring-after()",
            "substring()",
            "string-length()",
            "normalize-space()",
            "translate()",
            "lang()",
            "sum()",
            "floor()",
            "celing()",
            "round()"
        };

        #endregion

        #region Fields

        private readonly string _Name;

        #endregion

        #region Properties

        internal override QueryType TypeOfAst => QueryType.Function;

        internal override XPathResultType ReturnType
        {
            get
            {
                switch(TypeOfFunction)
                {
                    case FunctionType.FuncLast:
                        return XPathResultType.Number;
                    case FunctionType.FuncPosition:
                        return XPathResultType.Number;
                    case FunctionType.FuncCount:
                        return XPathResultType.Number;
                    case FunctionType.FuncID:
                        return XPathResultType.NodeSet;
                    case FunctionType.FuncLocalName:
                        return XPathResultType.String;
                    case FunctionType.FuncNameSpaceUri:
                        return XPathResultType.String;
                    case FunctionType.FuncName:
                        return XPathResultType.String;
                    case FunctionType.FuncString:
                        return XPathResultType.String;
                    case FunctionType.FuncBoolean:
                        return XPathResultType.Boolean;
                    case FunctionType.FuncNumber:
                        return XPathResultType.Number;
                    case FunctionType.FuncTrue:
                        return XPathResultType.Boolean;
                    case FunctionType.FuncFalse:
                        return XPathResultType.Boolean;
                    case FunctionType.FuncNot:
                        return XPathResultType.Boolean;
                    case FunctionType.FuncConcat:
                        return XPathResultType.String;
                    case FunctionType.FuncStartsWith:
                        return XPathResultType.Boolean;
                    case FunctionType.FuncContains:
                        return XPathResultType.Boolean;
                    case FunctionType.FuncSubstringBefore:
                        return XPathResultType.String;
                    case FunctionType.FuncSubstringAfter:
                        return XPathResultType.String;
                    case FunctionType.FuncSubstring:
                        return XPathResultType.String;
                    case FunctionType.FuncStringLength:
                        return XPathResultType.Number;
                    case FunctionType.FuncNormalize:
                        return XPathResultType.String;
                    case FunctionType.FuncTranslate:
                        return XPathResultType.String;
                    case FunctionType.FuncLang:
                        return XPathResultType.Boolean;
                    case FunctionType.FuncSum:
                        return XPathResultType.Number;
                    case FunctionType.FuncFloor:
                        return XPathResultType.Number;
                    case FunctionType.FuncCeiling:
                        return XPathResultType.Number;
                    case FunctionType.FuncRound:
                        return XPathResultType.Number;
                    case FunctionType.FuncUserDefined:
                        return XPathResultType.Error;
                }
                return XPathResultType.Error;
            }
        }

        internal FunctionType TypeOfFunction { get; } = FunctionType.Error;

        internal ArrayList ArgumentList { get; }

        internal string Prefix { get; }

        internal string Name => TypeOfFunction == FunctionType.FuncUserDefined ? _Name : _Str[(int)TypeOfFunction];

        #endregion

        #region Constructors

        internal Function(FunctionType ftype, ArrayList argumentList)
        {
            TypeOfFunction = ftype;
            ArgumentList = new ArrayList(argumentList);
        }

        internal Function(string prefix, string name, ArrayList argumentList)
        {
            TypeOfFunction = FunctionType.FuncUserDefined;
            Prefix = prefix;
            _Name = name;
            ArgumentList = new ArrayList(argumentList);
        }

        internal Function(FunctionType ftype) => TypeOfFunction = ftype;

        internal Function(FunctionType ftype, AstNode arg)
        {
            TypeOfFunction = ftype;
            ArgumentList = new ArrayList { arg };
        }

        #endregion
    }
}