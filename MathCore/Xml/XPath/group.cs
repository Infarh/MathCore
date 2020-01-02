//------------------------------------------------------------------------------
// <copyright file="Group.cs" company="Microsoft">
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
    internal class Group : AstNode
    {
        #region Properties

        internal override QueryType TypeOfAst => QueryType.Group;

        internal override XPathResultType ReturnType => XPathResultType.NodeSet;

        internal AstNode GroupNode { get; }

        internal override double DefaultPriority => 0;

        #endregion

        #region Constructors

        internal Group(AstNode groupNode) => GroupNode = groupNode;

        #endregion
    }
}