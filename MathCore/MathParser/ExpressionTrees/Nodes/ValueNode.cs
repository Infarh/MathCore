#nullable enable
// ReSharper disable UnusedMember.Global
namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Узел дерева мат.выражения, хранящий значение</summary>
public abstract class ValueNode : ComputedNode
{
    /// <summary>Значение узла</summary>
    public abstract double Value { get; set; }

    /// <summary>Преобразование в строковую форму</summary>
    /// <returns>Строковое представление</returns>
    public override string ToString()
    {
        const string format = "{1}{0}{2}";
        static string Convert(ExpressionTreeNode? n) => n?.ToString() ?? string.Empty;
        return string.Format(format, Value, Convert(Left), Convert(Right));
    }
}