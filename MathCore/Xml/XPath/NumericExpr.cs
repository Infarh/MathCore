// ReSharper disable once CheckNamespace
namespace System.Xml.XPath
{
    internal sealed class NumericExpr : Query
    {
        #region Fields

        internal Operator.Op Op;
        internal Query Operand1;
        internal Query Operand2;

        #endregion

        #region Constructors

        //
        // The operand needs to use the number function
        // to covert to numbers
        internal NumericExpr(Operator.Op op, Query operand1, Query operand2)
        {
            Operand1 = operand1.ReturnType() != XPathResultType.Number ? new NumberFunctions(operand1) : operand1;

            if(operand2 != null && (operand2.ReturnType() != XPathResultType.Number))
                Operand2 = new NumberFunctions(operand2);
            else
                Operand2 = operand2;

            Op = op;
        }

        #endregion

        #region Methods

        internal override object GetValue(XPathReader reader)
        {
            var n1 = Convert.ToDouble(Operand1.GetValue(reader));

            var n2 = 0d;
            if(Op != Operator.Op.Negate)
                n2 = Convert.ToDouble(Operand2.GetValue(reader));

            return Op switch
            {
                Operator.Op.Plus => (n1 + n2),
                Operator.Op.Minus => (n1 - n2),
                Operator.Op.Mod => (n1 % n2),
                Operator.Op.Div => (n1 / n2),
                Operator.Op.Mul => (n1 * n2),
                Operator.Op.Negate => -n1,
                _ => (object)null
            };
        }

        internal override XPathResultType ReturnType() => XPathResultType.Number;

        #endregion
    }
}