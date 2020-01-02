namespace System.Xml.XPath
{
    internal class XPathSelfQuery : BaseAxisQuery
    {
        #region Constructors

        internal XPathSelfQuery() { }

        internal XPathSelfQuery(Query queryInput, string name, string prefix, XPathNodeType type)
            : base(queryInput, name, prefix, type)
        { }

        #endregion

        #region Methods

        internal override bool MatchNode(XPathReader reader)
        {

            if(NodeType == XPathNodeType.All) return true;
            var ret = true;
            if(!MatchType(NodeType, reader.NodeType))
                ret = false;
            else if(Name != null && (Name != reader.Name || Prefix != reader.Prefix))
                ret = false;
            return ret;
        }

        internal override object GetValue(XPathReader reader)
        {
            if(reader.HasValue) return reader.Value;
            throw new XPathReaderException("Can't get the element value");
        }

        #endregion
    }
}