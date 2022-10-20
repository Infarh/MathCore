//------------------------------------------------------------------------------
// <copyright file="ExprQuery.cs" company="Microsoft">
//     
//      Copyright (c) 2002 Microsoft Corporation.  All rights reserved.
//     
//      The use and distribution terms for this software are contained in the file
//      named license.txt, which can be found in the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by the
//      terms of this license.
//     
//      You must not remove this notice, or any other, from this software.
//     
// </copyright>
//------------------------------------------------------------------------------

// ReSharper disable IdentifierTypo
// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;
// ReSharper disable CommentTypo
//
// Logical Expression should result in Boolean
// Rules:
// 1). Opnd1: node-set, opnd2: node-set
//  string(opnd1) op string(opnd1)
//
// 2). Opnd1: node-set
//  opnd2: string
//  string(opnd1) op opnd2
//
//  opnd2: number
//  number(string(opnd1)) op opnd2
//
//  opnd2: boolean
//  boolean(opnd1) op opnd2
//
// 3). Opnd convert sequence: boolean, number, string ( =, !=)
//     (<=, <, >=, >) convert to number
//
// Example: <Root><e a1='1' a2='2'/></Root>
// 1). /Root/e[@a1<@a2]: string(@a) < string(@b)
// 2). /Root/e[@a1 = true]: boolean(@a) = true
// 3). /Root/e["a" = true]: boolean("a") = true

// opnd1 = opnd2
// opnd1 != opnd2
// opnd1 > opnd2
// opnd1 < opnd2
// opnd1 >= opnd2
// opnd1 <= opnd2
// ReSharper restore CommentTypo

internal sealed class LogicalExpr : Query
{
    #region Fields

    private readonly Query _Operand1;
    private readonly Query _Operand2;

    internal readonly Operator.Op _Operator;

    #endregion

    #region Constructors

    internal LogicalExpr(Operator.Op Operator, Query Operand1, Query Operand2)
    {
        _Operand1 = Operand1;
        _Operand2 = Operand2;
        _Operator = Operator;
    }

    #endregion

    #region Methods

    private bool CompareAsNumber(XPathReader reader)
    {
        double n1, n2;

        var operand_var1 = _Operand1.GetValue(reader);
        var operand_var2 = _Operand2.GetValue(reader);

        try
        {
            n1 = Convert.ToDouble(operand_var1);
            n2 = Convert.ToDouble(operand_var2);
        }
        catch (Exception)
        {
            return false;
        }

        return _Operator switch
        {
            Operator.Op.Lt when n1 < n2        => true,
            Operator.Op.Gt when n1 > n2        => true,
            Operator.Op.Le when n1 <= n2       => true,
            Operator.Op.Ge when n1 >= n2       => true,
            Operator.Op.Eq when n1.Equals(n2)  => true,
            Operator.Op.Ne when !n1.Equals(n2) => true,
            _                                  => false
        };
    }

    private bool CompareAsString(XPathReader reader)
    {
        var opnd_var1 = _Operand1.GetValue(reader);
        var opnd_var2 = _Operand2.GetValue(reader);
        if (opnd_var1 is null || opnd_var2 is null)
            return false;
        var s1 = opnd_var1.ToString();
        var s2 = opnd_var2.ToString();

        if (_Operator > Operator.Op.Ge)
            return (Operator.Op.Eq == _Operator && s1.Equals(s2)) || (Operator.Op.Ne == _Operator && !s1.Equals(s2));
        else
        {
            //need to covert the string to the number and compare the numbers.
            double n1 = 0, n2 = 0;
            try
            {
                n1 = Convert.ToDouble(s1);
                n2 = Convert.ToDouble(s2);
            }
            catch
            {
                // ignored
            }

            return _Operator switch
            {
                Operator.Op.Lt when n1 < n2  => true,
                Operator.Op.Gt when n1 > n2  => true,
                Operator.Op.Le when n1 <= n2 => true,
                Operator.Op.Ge when n1 >= n2 => true,
                _                            => false
            };
        }
    }

    private bool CompareAsBoolean(XPathReader reader)
    {
        bool b1, b2;

        var operand_var1 = _Operand1.GetValue(reader);
        var operand_var2 = _Operand2.GetValue(reader);

        b1 = _Operand1.ReturnType() == XPathResultType.NodeSet
            ? operand_var1 != null
            : Convert.ToBoolean(operand_var1);

        b2 = _Operand1.ReturnType() == XPathResultType.NodeSet
            ? operand_var2 != null
            : Convert.ToBoolean(operand_var2);

        if (_Operator > Operator.Op.Ge)
            return (Operator.Op.Eq == _Operator && b1 == b2) || (Operator.Op.Ne == _Operator && b1 != b2);
        //need to covert the string to the number and compare the numbers.
        var n1 = b1 ? 1d : 0;
        var n2 = b2 ? 1d : 0;

        return _Operator switch
        {
            Operator.Op.Lt when n1 < n2  => true,
            Operator.Op.Gt when n1 > n2  => true,
            Operator.Op.Le when n1 <= n2 => true,
            Operator.Op.Ge when n1 >= n2 => true,
            _                            => false
        };
    }

    internal override object GetValue(XPathReader reader)
    {
        var type1 = _Operand1.ReturnType();
        var type2 = _Operand2.ReturnType();

        return type1 == XPathResultType.Boolean || type2 == XPathResultType.Boolean
            ? CompareAsBoolean(reader)
            : type1 == XPathResultType.Number || type2 == XPathResultType.Number
                ? CompareAsNumber(reader)
                : CompareAsString(reader);
    }

    internal override XPathResultType ReturnType() => XPathResultType.Boolean;

    #endregion
}