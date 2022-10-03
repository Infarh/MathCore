#nullable enable
using System;
using System.Linq.Expressions;
using MathCore.Extensions.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Узел дерева математического выражения, реализующий оператор возведения в степень</summary>
public class PowerOperatorNode : OperatorNode
{
    /// <summary>Инициализация нового узла оператора возведения в степень</summary>
    public PowerOperatorNode() : base("^", 20) { }

    /// <summary>Вычисление узла выражения</summary>
    /// <returns>Возведение значения корня левого поддерева в степень значения корня правого поддерева</returns>
    public override double Compute() => Math.Pow(LeftCompute(), RightCompute());

    /// <summary>Компиляция выражения узла</summary>
    /// <returns>Скомпилированное выражение узла</returns>
    public override Expression Compile() => LeftCompile().Power(RightCompile());

    /// <summary>Компиляция выражения узла</summary>
    /// <param name="Args">Массив параметров выражения</param>
    /// <returns>Скомпилированное выражение узла</returns>
    public override Expression Compile(params ParameterExpression[] Args) => LeftCompile(Args).Power(RightCompile(Args));

    /// <summary>Клонирование узла</summary>
    /// <returns>Клон узла</returns>
    public override ExpressionTreeNode Clone() => CloneOperatorNode<PowerOperatorNode>();
}