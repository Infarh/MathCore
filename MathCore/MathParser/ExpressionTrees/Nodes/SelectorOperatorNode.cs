#nullable enable
using System;
using System.Linq.Expressions;

using MathCore.Extensions.Expressions;

// ReSharper disable UnusedMember.Global

namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Узел дерева мат.выражения, реализующий оператор выбора</summary>
public class SelectorOperatorNode : OperatorNode
{
    public VariantOperatorNode Variants => (VariantOperatorNode?)Right ?? throw new InvalidOperationException("Узел вариантов (правое поддерево) не определён");

    /// <summary>Инициализация нового узла оператора выбора</summary>
    public SelectorOperatorNode() : base("?", -17) { }

    /// <summary>Инициализация нового узла оператора выбора</summary>
    /// <param name="Left">Левое поддерево выражения</param>
    /// <param name="Right">Правое поддерево выражения</param>
    public SelectorOperatorNode(ExpressionTreeNode Left, ExpressionTreeNode Right)
        : this()
    {
        this.Left  = Left;
        this.Right = Right;
    }

    /// <summary>Вычисление значения узла</summary>
    /// <returns></returns>
    public override double Compute() => Math.Abs(LeftCompute()) > 0 
        ? Variants.LeftCompute() 
        : Variants.RightCompute();

    /// <summary>Компиляция узла</summary>
    /// <returns>Скомпилированное выражение произведения узлов поддеревьев</returns>
    public override Expression Compile()
    {
        var        variants = Variants;
        Expression selector;
        if(Left is LogicOperatorNode node)
            selector = node.LogicCompile();
        else
        {
            var comparer = LeftCompile().GetAbs();
            selector = comparer.IsGreaterThan(0d);
        }
        return selector.Condition(variants.LeftCompile(), variants.RightCompile());
    }

    /// <summary>Компиляция узла</summary>
    /// <param name="Args">Массив параметров выражения</param>
    /// <returns>Скомпилированное выражение произведения узлов поддеревьев</returns>
    public override Expression Compile(ParameterExpression[] Args)
    {
        var        variants = Variants;
        Expression selector;
        if(Left is LogicOperatorNode node)
            selector = node.LogicCompile(Args);
        else
        {
            var comparer = LeftCompile(Args).GetAbs();
            selector = comparer.IsGreaterThan(0d);
        }
        return selector.Condition(variants.LeftCompile(Args), variants.RightCompile(Args));
    }

    /// <summary>Клонирование узла</summary>
    /// <returns>Клон узла</returns>
    public override ExpressionTreeNode Clone() => CloneOperatorNode<SelectorOperatorNode>();
}