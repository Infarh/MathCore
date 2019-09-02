//------------------------------------------------------------------------------
// <copyright file="IQuery.cs" company="Microsoft">
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
    internal abstract class Query
    {
        #region Properties

        internal virtual int PositionCount { get; set; }

        #endregion

        #region Methods

        internal virtual object GetValue(XPathReader reader) => null;

        //the default always not matched
        internal virtual bool MatchNode(XPathReader reader) => false;

        internal abstract XPathResultType ReturnType();

        #endregion
    }
}