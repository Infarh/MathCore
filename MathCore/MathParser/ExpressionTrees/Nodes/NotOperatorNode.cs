#nullable enable
using System.Linq.Expressions;

using MathCore.Extensions.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Узел дерева мат.выражения, реализующий оператор отрицания</summary>
public class NotOperatorNode : LogicOperatorNode
{
    /// <summary>Инициализация нового узла оператора меньше</summary>
    public NotOperatorNode() : base("!", -20) { }

    /// <summary>Инициализация нового узла оператора меньше</summary>
    /// <param name="Left">Левое поддерево выражения</param>
    /// <param name="Right">Правое поддерево выражения</param>
    public NotOperatorNode(ExpressionTreeNode Left, ExpressionTreeNode Right)
        : this()
    {
        this.Left  = Left;
        this.Right = Right;
    }

    /// <summary>Метод отрицания</summary>
    /// <param name="x">Первое значение</param>
    /// <param name="y">Второе значение</param>
    /// <returns>1 - если x меньше y и 0 - в противном случае</returns>
    private static double Comparer(double x, double y) => Math.Abs(x - y) >= EqualityOperatorNode.Epsilon ? 1 : 0;

    /// <summary>Метод отрицания</summary>
    /// <param name="x">Значение</param>
    /// <returns>1 - если x меньше y и 0 - в противном случае</returns>
    private static double ComparerSingle(double x) => Math.Abs(x) < EqualityOperatorNode.Epsilon ? 1 : 0;

    /// <summary>Вычисление значения узла</summary>
    /// <returns>0 - если разность между x и y по модулю меньше Epsilon и 1 во всех остальных случаях</returns>
    public override double Compute() => Left is null
        ? ComparerSingle(RightCompute())
        : Comparer(LeftCompute(), RightCompute());

    /// <summary>Компиляция логики узла</summary>
    /// <returns>Скомпилированное логическое выражение, реализующее операцию отрицания НЕ</returns>
    public override Expression LogicCompile() =>
        Left is null
            ? Right is LogicOperatorNode node
                ? node.LogicCompile().Not()
                : RightCompile().GetAbs().IsLessThan(EqualityOperatorNode.Epsilon)
            : Left is LogicOperatorNode operator_node && Right is LogicOperatorNode logic_operator_node
                ? operator_node.LogicCompile().IsNotEqual(logic_operator_node.LogicCompile())
                : LeftCompile().Subtract(RightCompile()).GetAbs().IsGreaterThanOrEqual(EqualityOperatorNode.Epsilon);

    /// <summary>Компиляция логики узла</summary>
    /// <param name="Args">Параметры компиляции</param>
    /// <returns>Скомпилированное логическое выражение, реализующее операцию отрицания НЕ</returns>
    public override Expression LogicCompile(ParameterExpression[] Args) =>
        Left is null
            ? Right is LogicOperatorNode node
                ? node.LogicCompile(Args).Not()
                : RightCompile(Args).GetAbs().IsLessThan(EqualityOperatorNode.Epsilon)
            : Left is LogicOperatorNode operator_node && Right is LogicOperatorNode logic_operator_node
                ? operator_node.LogicCompile(Args).IsNotEqual(logic_operator_node.LogicCompile(Args))
                : LeftCompile(Args).Subtract(RightCompile(Args)).GetAbs().IsGreaterThanOrEqual(EqualityOperatorNode.Epsilon);

    /// <summary>Строковое представление узла</summary>
    /// <returns>Строковое представление узла</returns>
    public override string ToString() => Right is null ? base.ToString() : $"{Left}≠{Right}";

    /// <summary>Клонирование узла</summary>
    /// <returns>Клон узла</returns>
    public override ExpressionTreeNode Clone() => CloneOperatorNode<NotOperatorNode>();
}