//------------------------------------------------------------------------------
// <copyright file="OperandQuery.cs" company="Microsoft">
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
//
// Function which takes operand
//

//
// The leaf node for the expression
//
internal sealed class OperandQuery : Query
{
    #region Fields

    private readonly XPathResultType _Type;
    private readonly object _Variable;

    #endregion

    #region Constructors

    internal OperandQuery(object var, XPathResultType type)
    {
        _Variable = var;
        _Type     = type;
    }

    #endregion

    #region Methods

    internal override object GetValue(XPathReader reader) => _Variable;

    internal override XPathResultType ReturnType() => _Type;

    #endregion
}