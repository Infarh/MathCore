#nullable enable
namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Узел интервального значения</summary>
public class IntervalNode : ParsedNode
{
    /// <summary>Минимальное значение</summary>
    public ExpressionTreeNode? Min { get => Left; set => Left = value; }

    /// <summary>Максимальное значение</summary>
    public ExpressionTreeNode? Max { get => Right; set => Right = value; }

    public IntervalNode(double Min, double Max) : this(new ConstValueNode(Min), new ConstValueNode(Max)) { }

    public IntervalNode(ExpressionTreeNode Min, ExpressionTreeNode? Max = null) { Left = Min; Right = Max; }

    /// <summary>Клонирование поддерева</summary>
    /// <returns>Клон поддерева</returns>
    public override ExpressionTreeNode Clone() => new IntervalNode(Min, Max);

    /// <summary>Преобразование узла в строку</summary>
    /// <returns>Строковое представление узла</returns>
    public override string ToString() => $"{Left}..{Right}";
}