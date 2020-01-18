// ReSharper disable once CheckNamespace
namespace System.Xml.XPath
{
    internal sealed class GroupQuery : BaseAxisQuery
    {
        #region Constructors

        internal GroupQuery(Query queryInput) : base(queryInput) { }

        #endregion

        #region Methods

        internal override object GetValue(XPathReader reader) => QueryInput.GetValue(reader);

        internal override XPathResultType ReturnType() => QueryInput.ReturnType();

        internal override bool MatchNode(XPathReader reader) => base.MatchNode(reader);

        #endregion
    }
}