//------------------------------------------------------------------------------
// <copyright file="Operand.cs" company="Microsoft">
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

namespace System.Xml.XPath
{
    internal class Operand : AstNode
    {
        #region Fields

        private readonly XPathResultType _Type;

        #endregion

        #region Properties

        internal override QueryType TypeOfAst => QueryType.ConstantOperand;

        internal override XPathResultType ReturnType => _Type;

        internal string OperandType =>
            _Type switch
            {
                XPathResultType.Number => "number",
                XPathResultType.String => "string",
                XPathResultType.Boolean => "boolean",
                _ => null
            };

        internal object OperandValue { get; }

        internal string Prefix { get; } = string.Empty;

        #endregion

        #region Constructors

        internal Operand(string var)
        {
            OperandValue = var;
            _Type = XPathResultType.String;
        }

        internal Operand(double var)
        {
            OperandValue = var;
            _Type = XPathResultType.Number;
        }

        internal Operand(bool var)
        {
            OperandValue = var;
            _Type = XPathResultType.Boolean;
        }

        #endregion
    }
}