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

namespace System.Xml.XPath
{
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

    internal sealed class LogicalExpr : Query
    {
        #region Fields

        private readonly Query _Opnd1;
        private readonly Query _Opnd2;

        internal Operator.Op Op;

        #endregion

        #region Constructors

        internal LogicalExpr(Operator.Op op, Query opnd1, Query opnd2)
        {
            _Opnd1 = opnd1;
            _Opnd2 = opnd2;
            Op = op;
        }

        #endregion

        #region Methods

        private bool CompareAsNumber(XPathReader reader)
        {
            double n1, n2;

            var lv_OpndVar1 = _Opnd1.GetValue(reader);
            var lv_OpndVar2 = _Opnd2.GetValue(reader);

            try
            {
                n1 = Convert.ToDouble(lv_OpndVar1);
                n2 = Convert.ToDouble(lv_OpndVar2);
            } catch(Exception)
            {
                return false;
            }

            switch(Op)
            {
                case Operator.Op.Lt:
                    if(n1 < n2)
                        return true;
                    break;
                case Operator.Op.Gt:
                    if(n1 > n2)
                        return true;
                    break;
                case Operator.Op.Le:
                    if(n1 <= n2)
                        return true;
                    break;
                case Operator.Op.Ge:
                    if(n1 >= n2)
                        return true;
                    break;
                case Operator.Op.Eq:
                    if(n1.Equals(n2))
                        return true;
                    break;
                case Operator.Op.Ne:
                    if(!n1.Equals(n2))
                        return true;
                    break;
            }

            return false;
        }

        private bool CompareAsString(XPathReader reader)
        {
            var ret = false;

            var lv_OpndVar1 = _Opnd1.GetValue(reader);
            var lv_OpndVar2 = _Opnd2.GetValue(reader);
            if(lv_OpndVar1 is null || lv_OpndVar2 is null)
                return false;
            var s1 = lv_OpndVar1.ToString();
            var s2 = lv_OpndVar2.ToString();

            if(Op > Operator.Op.Ge)
            {
                if((Operator.Op.Eq == Op && s1.Equals(s2)) || (Operator.Op.Ne == Op && !s1.Equals(s2)))
                    ret = true;
            }
            else
            {
                //need to covert the string to the number and compare the numbers.
                double n1 = 0, n2 = 0;
                try
                {
                    n1 = Convert.ToDouble(s1);
                    n2 = Convert.ToDouble(s2);
                } catch
                {
                }

                switch(Op)
                {
                    case Operator.Op.Lt:
                        if(n1 < n2) ret = true;
                        break;
                    case Operator.Op.Gt:
                        if(n1 > n2) ret = true;
                        break;
                    case Operator.Op.Le:
                        if(n1 <= n2) ret = true;
                        break;
                    case Operator.Op.Ge:
                        if(n1 >= n2) ret = true;
                        break;
                }
            }
#if DEBUG1
            Console.WriteLine("s1 {0}, s2 {1}, op {2}, ret: {3}", s1, s2, op, ret);
#endif
            return ret;
        }

        private bool CompareAsBoolean(XPathReader reader)
        {
            bool b1, b2;

            var lv_OpndVar1 = _Opnd1.GetValue(reader);
            var lv_OpndVar2 = _Opnd2.GetValue(reader);

            if(_Opnd1.ReturnType() == XPathResultType.NodeSet)
                b1 = lv_OpndVar1 != null;
            else
                b1 = Convert.ToBoolean(lv_OpndVar1);

            if(_Opnd1.ReturnType() == XPathResultType.NodeSet)
                b2 = lv_OpndVar2 != null;
            else
                b2 = Convert.ToBoolean(lv_OpndVar2);

            if(Op > Operator.Op.Ge)
                return (Operator.Op.Eq == Op && b1 == b2) || (Operator.Op.Ne == Op && b1 != b2);
            //need to covert the string to the number and compare the numbers.
            double n1, n2;
            try
            {
                n1 = Convert.ToDouble(b1);
                n2 = Convert.ToDouble(b2);
            } catch
            {
                return false;
            }

            switch(Op)
            {
                case Operator.Op.Lt:
                    if(n1 < n2) return true;
                    break;
                case Operator.Op.Gt:
                    if(n1 > n2) return true;
                    break;
                case Operator.Op.Le:
                    if(n1 <= n2) return true;
                    break;
                case Operator.Op.Ge:
                    if(n1 >= n2) return true;
                    break;
            }
            return false;
        }

        internal override object GetValue(XPathReader reader)
        {
            var type1 = _Opnd1.ReturnType();
            var type2 = _Opnd2.ReturnType();

            if(type1 == XPathResultType.Boolean || type2 == XPathResultType.Boolean)
                return CompareAsBoolean(reader);
            if(type1 == XPathResultType.Number || type2 == XPathResultType.Number)
                return CompareAsNumber(reader);
            return CompareAsString(reader);
        }

        internal override XPathResultType ReturnType() => XPathResultType.Boolean;

        #endregion
    }

    //
    // Get Value and result in number
    //

    //
    // Process Or expression
    //
    // Data type between the two operations
    //
    // LogicalExpression or LogicalExpression
    //
    //
    // for example:
    //     @a > 1 or @b < 2
    //     @a or @b
    //
    //   @a | @b or @c
}