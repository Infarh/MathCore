#nullable enable
using System.Linq.Expressions;

using MathCore.Extensions.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Узел дерева мат.выражения, реализующий оператор меньше</summary>
public class LessThenOperatorNode : LogicOperatorNode
{
    /// <summary>Инициализация нового узла оператора меньше</summary>
    public LessThenOperatorNode() : base("<", -10) { }

    /// <summary>Инициализация нового узла оператора меньше</summary>
    /// <param name="Left">Левое поддерево выражения</param>
    /// <param name="Right">Правое поддерево выражения</param>
    public LessThenOperatorNode(ExpressionTreeNode Left, ExpressionTreeNode Right)
        : this()
    {
        this.Left  = Left;
        this.Right = Right;
    }

    /// <summary>Метод сравнения</summary>
    /// <param name="x">Первое значение</param>
    /// <param name="y">Второе значение</param>
    /// <returns>1 - если x меньше y и 0 - в противном случае</returns>
    private static double Comparer(double x, double y) => x < y ? 1 : 0;

    /// <summary>Вычисление значения узла</summary>
    /// <returns>0 - если разность между x и y по модулю меньше Epsilon и 1 во всех остальных случаях</returns>
    public override double Compute() => Comparer(LeftCompute(), RightCompute());

    /// <summary>Компиляция логики узла</summary>
    /// <returns>Скомпилированное логическое выражение, реализующее операцию сравнения Меньше</returns>
    public override Expression LogicCompile() => LeftCompile().IsLessThan(RightCompile());

    /// <summary>Компиляция логики узла</summary>
    /// <param name="Args">Параметры компиляции</param>
    /// <returns>Скомпилированное логическое выражение, реализующее операцию сравнения Меньше</returns>
    public override Expression LogicCompile(params ParameterExpression[] Args) => LeftCompile(Args).IsLessThan(RightCompile(Args));

    /// <summary>Клонирование узла</summary>
    /// <returns>Клон узла</returns>
    public override ExpressionTreeNode Clone() => CloneOperatorNode<LessThenOperatorNode>();
}