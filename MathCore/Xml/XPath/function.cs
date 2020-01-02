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

        internal override XPathResultType ReturnType =>
            TypeOfFunction switch
            {
                FunctionType.FuncLast => XPathResultType.Number,
                FunctionType.FuncPosition => XPathResultType.Number,
                FunctionType.FuncCount => XPathResultType.Number,
                FunctionType.FuncID => XPathResultType.NodeSet,
                FunctionType.FuncLocalName => XPathResultType.String,
                FunctionType.FuncNameSpaceUri => XPathResultType.String,
                FunctionType.FuncName => XPathResultType.String,
                FunctionType.FuncString => XPathResultType.String,
                FunctionType.FuncBoolean => XPathResultType.Boolean,
                FunctionType.FuncNumber => XPathResultType.Number,
                FunctionType.FuncTrue => XPathResultType.Boolean,
                FunctionType.FuncFalse => XPathResultType.Boolean,
                FunctionType.FuncNot => XPathResultType.Boolean,
                FunctionType.FuncConcat => XPathResultType.String,
                FunctionType.FuncStartsWith => XPathResultType.Boolean,
                FunctionType.FuncContains => XPathResultType.Boolean,
                FunctionType.FuncSubstringBefore => XPathResultType.String,
                FunctionType.FuncSubstringAfter => XPathResultType.String,
                FunctionType.FuncSubstring => XPathResultType.String,
                FunctionType.FuncStringLength => XPathResultType.Number,
                FunctionType.FuncNormalize => XPathResultType.String,
                FunctionType.FuncTranslate => XPathResultType.String,
                FunctionType.FuncLang => XPathResultType.Boolean,
                FunctionType.FuncSum => XPathResultType.Number,
                FunctionType.FuncFloor => XPathResultType.Number,
                FunctionType.FuncCeiling => XPathResultType.Number,
                FunctionType.FuncRound => XPathResultType.Number,
                FunctionType.FuncUserDefined => XPathResultType.Error,
                _ => XPathResultType.Error
            };

        internal FunctionType TypeOfFunction { get; }

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