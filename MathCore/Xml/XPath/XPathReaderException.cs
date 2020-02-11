//------------------------------------------------------------------------------
// <copyright file="XPathException.cs" company="Microsoft">
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

using System.Runtime.Serialization;

namespace System.Xml.XPath
{
    // The XPathException class contains XML parser errors.
    // 
    // The XPathReaderException class contains XML parser errors.
    // 
    /// <devdoc>
    ///     <para>
    ///         Represents the exception that is thrown when there is error processing an
    ///         XPath expression.
    ///     </para>
    /// </devdoc>
    [Serializable]
    public class XPathReaderException : XPathException
    {
        #region Constructors

        public XPathReaderException(string message) : base(message) { }

        public XPathReaderException(SerializationInfo Info, StreamingContext Context) : base(Info, Context) { }

        #endregion
    }
}