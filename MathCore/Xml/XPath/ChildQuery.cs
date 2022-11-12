﻿// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

internal class ChildQuery : BaseAxisQuery
{
    #region Constructors

    internal ChildQuery(Query QyInput, string name, string prefix, XPathNodeType type) : base(QyInput, name, prefix, type) { }

    #endregion

    #region Methods

    // try to match the node
    internal override bool MatchNode(XPathReader reader)
    {
        if(NodeType == XPathNodeType.All) return true;
        if(!MatchType(NodeType, reader.NodeType))
            return false;
        if(Name != string.Empty && (Name != reader.LocalName || (Prefix.Length != 0 && !reader.MapPrefixWithNamespace(Prefix))))
            return false; //currently the AstNode build initial the name as String.Empty
        return true;
    }


    //
    // children query return value when it's in the
    // predicates
    // for example: <e><e1>1</e1><e1>2<e1/></e>
    // e[e1 = 1]
    // Can't move the reader forward.
    /// <exception cref="XPathReaderException">Can't get the child value</exception>
    internal override object GetValue(XPathReader reader) => throw new XPathReaderException("Can't get the child value");

    #endregion
}