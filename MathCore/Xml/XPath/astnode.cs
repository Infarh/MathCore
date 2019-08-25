//------------------------------------------------------------------------------
// <copyright file="AstNode.cs" company="Microsoft">
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
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace System.Xml.XPath
{
    internal class AstNode
    {
        #region Types

        internal enum QueryType
        {
            Axis,
            Operator,
            Filter,
            ConstantOperand,
            Function,
            Group,
            Root,
            Variable,
            Error
        }

        #endregion

        #region Properties

        internal virtual QueryType TypeOfAst => QueryType.Error;

        internal virtual XPathResultType ReturnType => XPathResultType.Error;

        internal virtual double DefaultPriority => 0.5;

        #endregion

        #region Methods

        internal static AstNode NewAstNode(string parsestring)
        {
            try
            {
                return XPathParser.ParseXPathExpresion(parsestring);
            }
            catch (XPathException e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }

        #endregion
    }
}