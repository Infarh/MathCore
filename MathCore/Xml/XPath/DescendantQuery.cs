// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

internal class DescendantQuery(Query QyInput, string name, string prefix, XPathNodeType type) : BaseAxisQuery(QyInput, name, prefix, type)
{
    #region Methods

    internal override bool MatchNode(XPathReader reader)
    {
        var ret = true;

        if(NodeType == XPathNodeType.All) return ret;
        if(!MatchType(NodeType, reader.NodeType))
            ret = false;
        else if(Name != null && (Name != reader.Name || Prefix != reader.Prefix))
            ret = false;

        return ret;
    }


    //
    // Descendant query value
    //
    // <e><e1>1</e1><e2>2</e2></e>
    //
    // e[descendant::node()=1]
    //
    // current context node need to be saved if
    // we need to solve the case for future.
    /// <exception cref="XPathReaderException">Can't get the decedent nodes value</exception>
    internal override object GetValue(XPathReader reader) => throw new XPathReaderException("Can't get the descendent nodes value");

    #endregion
}