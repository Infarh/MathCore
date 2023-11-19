﻿//------------------------------------------------------------------------------
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
namespace System.Xml.XPath;

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

#if !NET8_0_OR_GREATER
    public XPathReaderException(System.Runtime.Serialization.SerializationInfo Info, System.Runtime.Serialization.StreamingContext Context) : base(Info, Context) { } 
#endif

    #endregion
}