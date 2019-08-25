namespace System.Xml.XPath
{
    internal sealed class AndExpr : Query
    {
        #region Fields

        private readonly BooleanFunctions _Opnd1;
        private readonly BooleanFunctions _Opnd2;

        #endregion

        #region Constructors

        private AndExpr() { }

        internal AndExpr(Query opnd1, Query opnd2)
        {
            _Opnd1 = new BooleanFunctions(opnd1);
            _Opnd2 = new BooleanFunctions(opnd2);
        }

        #endregion

        #region Methods

        internal override object GetValue(XPathReader reader)
        {
            var ret = _Opnd1.GetValue(reader);
            return !Convert.ToBoolean(ret) ? ret : _Opnd2.GetValue(reader);
        }

        internal override XPathResultType ReturnType() => XPathResultType.Boolean;

        #endregion
    }
}