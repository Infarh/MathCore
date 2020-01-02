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
        internal override object GetValue(XPathReader reader) =>
            _FuncType switch
            {
                Function.FunctionType.FuncCount => (object) reader.AttributeCount,
                Function.FunctionType.FuncPosition => PositionCount,
                Function.FunctionType.FuncNameSpaceUri => reader.NamespaceURI,
                Function.FunctionType.FuncLocalName => reader.LocalName,
                Function.FunctionType.FuncName => reader.Name,
                _ => null
            };

        internal override XPathResultType ReturnType() => _FuncType <= Function.FunctionType.FuncCount
            ? XPathResultType.Number
            : XPathResultType.String;

        #endregion
    }
}