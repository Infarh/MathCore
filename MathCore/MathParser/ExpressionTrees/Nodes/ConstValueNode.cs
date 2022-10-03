#nullable enable
using System;
using System.Linq.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Узел дерева, хранящий константное значение</summary>
public class ConstValueNode : ValueNode
{
    /// <summary>Значение узла</summary>
    private readonly double _Value;

    /// <summary>Флаг возможности получения значения без вычисления. Всегда = true</summary>
    public override bool IsPrecomputable => true;

    /// <summary>Значение узла. Не поддерживает присвоение</summary>
    public override double Value { [DST] get => _Value; [DST] set => throw new NotSupportedException(); }

    /// <summary>Инициализация константного узла</summary>
    [DST]
    public ConstValueNode() { }

    /// <summary>Инициализация константного узла</summary>
    /// <param name="Value">Значение узла</param>
    [DST]
    public ConstValueNode(double Value) => _Value = Value;

    /// <summary>Инициализация константного узла</summary>
    /// <param name="Value">Значение узла</param>
    [DST]
    public ConstValueNode(int Value) : this((double)Value) { }

    /// <summary>Вычислить значение поддерева</summary>
    /// <returns>Численное значение поддерева</returns>
    [DST]
    public override double Compute() => _Value;

    /// <summary>Скомпилировать в выражение</summary>
    /// <returns>Скомпилированное выражение System.Linq.Expressions</returns>
    [DST]
    public override Expression Compile() => _Value.ToExpression();

    /// <summary>Скомпилировать в выражение</summary>
    /// <param name="Args">Массив параметров</param>
    /// <returns>Скомпилированное выражение System.Linq.Expressions</returns>
    public override Expression Compile(params ParameterExpression[] Args) => Compile();

    /// <summary>Клонирование поддерева</summary>
    /// <returns>Клон поддерева</returns>
    public override ExpressionTreeNode Clone() => new ConstValueNode(Value)
    {
        Left  = Left?.Clone(),
        Right = Right?.Clone()
    };
}