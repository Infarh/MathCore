//------------------------------------------------------------------------------
// <copyright file="Filter.cs" company="Microsoft">
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

// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

internal class Filter : AstNode
{
    #region Properties

    internal override QueryType TypeOfAst => QueryType.Filter;

    internal override XPathResultType ReturnType => XPathResultType.NodeSet;

    internal AstNode Input { get; }

    internal AstNode Condition { get; }

    #endregion

    #region Constructors

    internal Filter(AstNode input, AstNode condition)
    {
        Input     = input;
        Condition = condition;
    }

    #endregion
}