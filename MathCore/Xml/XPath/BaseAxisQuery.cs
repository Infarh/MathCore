//------------------------------------------------------------------------------
// <copyright file="BaseQuery.cs" company="Microsoft">
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

internal class BaseAxisQuery : Query
{
    #region Properties

    internal string Name { get; } = string.Empty;

    internal string Prefix { get; } = string.Empty;

    internal XPathNodeType NodeType { get; }

    internal int Depth { get; set; } = -1;

    internal Query QueryInput { get; set; }

    #endregion

    #region Constructors

    internal BaseAxisQuery() { }

    internal BaseAxisQuery(Query QueryInput) => this.QueryInput = QueryInput;

    internal BaseAxisQuery(Query QueryInput, string name, string prefix, XPathNodeType NodeType)
    {
        Prefix = prefix;
        this.QueryInput = QueryInput;
        Name = name;
        this.NodeType = NodeType;
    }

    #endregion

    #region Methods

    internal override XPathResultType ReturnType() => XPathResultType.NodeSet;

    internal static bool MatchType(XPathNodeType XType, XmlNodeType type) => XType switch
    {
        XPathNodeType.Element when type is XmlNodeType.Element or XmlNodeType.EndElement   => true,
        XPathNodeType.Attribute when type is XmlNodeType.Attribute                         => true,
        XPathNodeType.Text when type is XmlNodeType.Text                                   => true,
        XPathNodeType.ProcessingInstruction when type is XmlNodeType.ProcessingInstruction => true,
        XPathNodeType.Comment when type is XmlNodeType.Comment                             => true,

        _ => throw new XPathReaderException("Unknown nodeType")
    };

    #endregion
}