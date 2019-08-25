namespace System.Xml.XPath
{
    internal sealed class MethodOperand : Query
    {
        #region Fields

        private readonly Function.FunctionType _FuncType;
        //private Query _Opnd;

        #endregion

        #region Constructors

        internal MethodOperand(Query opnd, Function.FunctionType FuncType) => _FuncType = FuncType;//_Opnd = opnd;

        #endregion

        #region Methods

        //
        // The function context node has to be the axis selected node
        //  <E a='1' xmlns='test'> <E1/> </E>
        //
        //  /E/E1[namespaceuri(../E)= 'test']
        internal override object GetValue(XPathReader reader)
        {
            object ret = null;

            // the Opnd must be attribute, otherwise it will be in error
            switch(_FuncType)
            {
                case Function.FunctionType.FuncCount:
                    ret = reader.AttributeCount;
                    break;

                case Function.FunctionType.FuncPosition:
                    //we need to go back to the fileter query to get count
                    ret = PositionCount;
                    break;

                case Function.FunctionType.FuncNameSpaceUri:
                    ret = reader.NamespaceURI;
                    break;

                case Function.FunctionType.FuncLocalName:
                    ret = reader.LocalName;
                    break;

                case Function.FunctionType.FuncName:
                    ret = reader.Name;
                    break;
            }

            return ret;
        }

        internal override XPathResultType ReturnType() => _FuncType <= Function.FunctionType.FuncCount
            ? XPathResultType.Number
            : XPathResultType.String;

        #endregion
    }
}