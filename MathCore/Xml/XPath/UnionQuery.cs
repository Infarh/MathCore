namespace System.Xml.XPath
{
    internal sealed class UnionQuery : BaseAxisQuery
    {
        #region Fields

        private readonly Query _Query1;
        private readonly Query _Query2;

        #endregion

        #region Constructors

        internal UnionQuery(Query query1, Query query2)
        {
            _Query1 = query1;
            _Query2 = query2;
        }

        #endregion

        #region Methods

        internal override bool MatchNode(XPathReader reader) => _Query1.MatchNode(reader) || _Query2.MatchNode(reader);

        //
        // <e><e1>1</e1><e2>2</e2></e>
        //
        // e[ e1 | e2 = 1]
        //
        // results:
        // e
        //
        // Union query needs to return two objects
        //
        // The only success situation is the two attribute situations
        // because only the attribute within current scope are cached.
        //
        internal override object GetValue(XPathReader reader)
        {
            var lv_ObjArray = new object[2];
            lv_ObjArray[0] = _Query1.GetValue(reader);
            lv_ObjArray[1] = _Query2.GetValue(reader);
            return lv_ObjArray;
        }

        #endregion
    }
}