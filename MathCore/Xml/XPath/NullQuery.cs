//------------------------------------------------------------------------------
// <copyright file="AxisQuery.cs" company="Microsoft">
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
namespace System.Xml.XPath
{
    internal sealed class NullQuery : BaseAxisQuery
    {
        #region Methods

        internal override bool MatchNode(XPathReader reader) => false;

        #endregion
    }


    //
    // handles the child axis in the following situation:
    //  foo:bar
    //  child::*
    //  child::node() //element, text, comment, PI, no attribute
    //  child::Text()
    //  child::ProcessingInstruction()
    //  child::comment();
}