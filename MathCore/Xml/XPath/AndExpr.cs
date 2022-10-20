// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

internal sealed class AndExpr : Query
{
    #region Fields

    private readonly BooleanFunctions _Operand1;
    private readonly BooleanFunctions _Operand2;

    #endregion

    #region Constructors

    private AndExpr() { }

    internal AndExpr(Query operand1, Query operand2)
    {
        _Operand1 = new BooleanFunctions(operand1);
        _Operand2 = new BooleanFunctions(operand2);
    }

    #endregion

    #region Methods

    internal override object GetValue(XPathReader reader)
    {
        var ret = _Operand1.GetValue(reader);
        return !Convert.ToBoolean(ret) ? ret : _Operand2.GetValue(reader);
    }

    internal override XPathResultType ReturnType() => XPathResultType.Boolean;

    #endregion
}