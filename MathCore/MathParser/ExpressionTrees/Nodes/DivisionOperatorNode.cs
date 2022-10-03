#nullable enable
using System.Linq.Expressions;
using MathCore.Extensions.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Узел дерева мат.выражения, реализующий оператор деления</summary>
public class DivisionOperatorNode : OperatorNode
{
    public const string NodeName = "/";

    /// <summary>Инициализация узла оператора деления</summary>
    public DivisionOperatorNode() : base(NodeName, 15) { }

    /// <summary>Вычисление значения узла</summary>
    /// <returns>Значение узла</returns>
    public override double Compute() => LeftCompute(1) / RightCompute();

    /// <summary>Компиляция узла</summary>
    /// <returns>Скомпилированное выражение узла</returns>
    public override Expression Compile() =>
        LeftCompile(1).Divide(RightCompile());

    /// <summary>Компиляция узла с параметрами</summary>
    /// <param name="Args">Список параметров выражения</param>
    /// <returns>Скомпилированное выражение узла</returns>
    public override Expression Compile(params ParameterExpression[] Args) => LeftCompile(Args, 1).Divide(RightCompile(Args));

    /// <summary>Строковое представление узла</summary>
    /// <returns>Строковое представление</returns>
    public override ExpressionTreeNode Clone() => CloneOperatorNode<DivisionOperatorNode>();
}