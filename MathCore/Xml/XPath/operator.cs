//------------------------------------------------------------------------------
// <copyright file="Operator.cs" company="Microsoft">
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

// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

internal class Operator : AstNode
{
    #region Types

    internal enum Op
    {
        Plus = 1,
        Minus = 2,
        Mul = 3,
        Mod = 4,
        Div = 5,
        Negate = 6,
        Lt = 7,
        Gt = 8,
        Le = 9,
        Ge = 10,
        Eq = 11,
        Ne = 12,
        Or = 13,
        And = 14,
        Union = 15,
        Invalid
    }

    #endregion

    #region Fields

    private readonly string[] _Str =
    [
        "+",
        "-",
        "multiply",
        "mod",
        "divide",
        "negate",
        "<",
        ">",
        "<=",
        ">=",
        "=",
        "!=",
        "or",
        "and",
        "union"
    ];

    #endregion

    #region Properties

    internal override QueryType TypeOfAst => QueryType.Operator;

    internal override XPathResultType ReturnType
    {
        get
        {
            if(OperatorType < Op.Lt)
                return XPathResultType.Number;
            if(OperatorType < Op.Union)
                return XPathResultType.Boolean;
            return XPathResultType.NodeSet;
        }
    }

    internal Op OperatorType { get; }

    internal AstNode Operand1 { get; }

    internal AstNode Operand2 { get; }

    internal string OperatorTypeName => _Str[(int)OperatorType - 1];

    internal override double DefaultPriority
    {
        get
        {
            if(OperatorType != Op.Union) return 0.5;
            var pri1 = Operand1.DefaultPriority;
            var pri2 = Operand2.DefaultPriority;

            return pri1 > pri2 ? pri1 : pri2;
        }
    }

    #endregion

    #region Constructors

    internal Operator(Op op, AstNode operand1, AstNode operand2)
    {
        OperatorType = op;
        Operand1     = operand1;
        Operand2     = operand2;
    }

    #endregion
}