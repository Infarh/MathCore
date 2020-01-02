//------------------------------------------------------------------------------
// <copyright file="Variable.cs" company="Microsoft">
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
    internal class Variable : AstNode
    {
        #region Properties

        internal override QueryType TypeOfAst => QueryType.Variable;

        internal override XPathResultType ReturnType => XPathResultType.Error;

        internal string Name => Prefix != string.Empty ? $"{Prefix}:{Localname}" : Localname;

        internal string Localname { get; }

        internal string Prefix { get; } = string.Empty;

        #endregion

        #region Constructors

        internal Variable(string name, string prefix)
        {
            Localname = name;
            Prefix = prefix;
        }

        #endregion
    }
}