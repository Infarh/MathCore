#nullable enable
using System.Linq.Expressions;

using MathCore.Extensions.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Узел дерева мат.выражения, реализующий оператор ленивого И</summary>
public class AndOperatorNode : LogicOperatorNode
{
    /// <summary>Инициализация нового узла оператора ленивого И</summary>
    public AndOperatorNode() : base("&", -15) { }

    /// <summary>Вычислить значение поддерева</summary>
    /// <returns>Численное значение поддерева</returns>
    public override double Compute() => (LeftCompute() > 0) && (RightCompute() > 0) ? 1 : 0;

    /// <summary>Компиляция логики узла</summary>
    /// <returns>Скомпилированное логическое выражение, реализующее операцию И</returns>
    public override Expression LogicCompile()
    {
        var left = Left is LogicOperatorNode left_node
            ? left_node.LogicCompile()
            : LeftCompile().GetAbs().IsGreaterThan(0d);

        var right = Right is LogicOperatorNode right_node
            ? right_node.LogicCompile()
            : RightCompile().GetAbs().IsGreaterThan(0d);

        return left.AndLazy(right);
    }

    /// <summary>Компиляция логики узла</summary>
    /// <param name="Args">Параметры компиляции</param>
    /// <returns>Скомпилированное логическое выражение, реализующее операцию И</returns>
    public override Expression LogicCompile(params ParameterExpression[] Args)
    {
        var left = Left is LogicOperatorNode left_node
            ? left_node.LogicCompile(Args)
            : LeftComputed.Compile(Args).GetAbs().IsGreaterThan(0d);

        var right = Right is LogicOperatorNode right_node
            ? right_node.LogicCompile(Args)
            : RightComputed.Compile(Args).GetAbs().IsGreaterThan(0d);

        return left.AndLazy(right);
    }

    /// <summary>Клонирование поддерева</summary>
    /// <returns>Клон поддерева</returns>
    public override ExpressionTreeNode Clone() => CloneOperatorNode<AndOperatorNode>();
}