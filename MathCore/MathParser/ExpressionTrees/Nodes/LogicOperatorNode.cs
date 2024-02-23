#nullable enable
using System.Linq.Expressions;

using MathCore.Extensions.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Узел дерева мат.выражения, реализующий логическую операцию</summary>
public abstract class LogicOperatorNode(string Name, int Priority) : OperatorNode(Name, Priority)
{
    /// <summary>Компиляция логики узла</summary>
    /// <returns>Скомпилированное логическое выражение, реализующее логику оператора</returns>
    public abstract Expression LogicCompile();

    /// <summary>Компиляция логики узла</summary>
    /// <param name="Args">Параметры компиляции</param>
    /// <returns>Скомпилированное логическое выражение, реализующее логику оператора</returns>
    public abstract Expression LogicCompile(params ParameterExpression[] Args);

    /// <summary>Компиляция узла</summary>
    /// <returns>Скомпилированное выражение произведения узлов поддеревьев</returns>
    public override Expression Compile() => LogicCompile().ConditionWithResult(1d, 0d);

    /// <summary>Компиляция узла</summary>
    /// <param name="Args">Массив параметров выражения</param>
    /// <returns>Скомпилированное выражение произведения узлов поддеревьев</returns>
    public override Expression Compile(params ParameterExpression[] Args) => LogicCompile(Args).ConditionWithResult(1d, 0d);
}