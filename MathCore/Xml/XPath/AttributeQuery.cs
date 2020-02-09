// ReSharper disable once CheckNamespace
namespace System.Xml.XPath
{
    internal sealed class AttributeQuery : BaseAxisQuery
    {
        #region Constructors

        internal AttributeQuery(Query QyParent, string name, string prefix, XPathNodeType type) : base(QyParent, name, prefix, type) { }

        #endregion

        #region Methods

        //
        // we need to walk the attribute for
        // query like e/@a and position the reader to that
        // attribute node.

        // example: e/attribute::node()
        //          e/attribute::text() //no return
        //          e/@a
        //
        // There are two situations to match the attributes
        // 1). user has moved the reader to an attribute in the current element context
        // 2). user still in the element context, since it's an attribute query, we
        //     need to move to the attribute our self.

        internal override bool MatchNode(XPathReader reader)
        {
            var ret = true;

            if(NodeType == XPathNodeType.All) return ret;
            if(!MatchType(NodeType, reader.NodeType))
                ret = false;
            else if(Name != string.Empty && (Name != reader.Name || Prefix != reader.Prefix))
                ret = false;

            return ret;
        }

        // The reader will be the current element node
        // We need to restore the
        internal override object GetValue(XPathReader reader)
        {
            var lv_BaseReader = reader.BaseReader;

            object ret = null;

            if(lv_BaseReader.MoveToAttribute(Name))
                ret = reader.Value;

            //Move back to the parent
            lv_BaseReader.MoveToElement();
            return ret;
        }

        #endregion
    }
}