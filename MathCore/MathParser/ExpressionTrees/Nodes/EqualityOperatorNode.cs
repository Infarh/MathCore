#nullable enable
using System;
using System.Linq.Expressions;

using MathCore.Extensions.Expressions;
// ReSharper disable ConvertToAutoPropertyWhenPossible

// ReSharper disable UnusedMember.Global

namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Узел дерева мат.выражения, реализующий оператор равенства</summary>
public class EqualityOperatorNode : LogicOperatorNode
{
    /// <summary>Точность вычисления равенства для чисел с плавающей точкой</summary>
    private static double __Epsilon = 1e-12;

    /// <summary>Точность вычисления равенства для чисел с плавающей точкой</summary>
    public static double Epsilon { get => __Epsilon; set => __Epsilon = value; }

    /// <summary>Инициализация нового узла оператора равенства</summary>
    public EqualityOperatorNode() : base("=", -30) { }

    /// <summary>Инициализация нового узла оператора равенства</summary>
    /// <param name="Left">Левое поддерево выражения</param>
    /// <param name="Right">Правое поддерево выражения</param>
    public EqualityOperatorNode(ExpressionTreeNode Left, ExpressionTreeNode Right)
        : this()
    {
        this.Left  = Left;
        this.Right = Right;
    }

    /// <summary>Метод сравнения</summary>
    /// <param name="x">Первое значение</param>
    /// <param name="y">Второе значение</param>
    /// <returns>0 - если разность между x и y по модулю меньше Epsilon и 1 во всех остальных случаях</returns>
    private static double Comparer(double x, double y) => Math.Abs(x - y) < __Epsilon ? 1 : 0;

    /// <summary>Вычисление значения узла</summary>
    /// <returns>0 - если разность между x и y по модулю меньше Epsilon и 1 во всех остальных случаях</returns>
    public override double Compute() => Comparer(LeftCompute(), RightCompute());

    /// <summary>Компиляция логики узла</summary>
    /// <returns>Скомпилированное логическое выражение, реализующее операцию сравнения Равенство</returns>
    public override Expression LogicCompile() => 
        Left is LogicOperatorNode left_node && Right is LogicOperatorNode right_node
            ? left_node.LogicCompile().IsEqual(right_node.LogicCompile())
            : LeftCompile().Subtract(RightCompile()).GetAbs().IsLessThan(__Epsilon);

    /// <summary>Компиляция логики узла</summary>
    /// <param name="Args">Параметры компиляции</param>
    /// <returns>Скомпилированное логическое выражение, реализующее операцию сравнения Равенство</returns>
    public override Expression LogicCompile(params ParameterExpression[] Args) => 
        Left is LogicOperatorNode left_node && Right is LogicOperatorNode right_node
            ? left_node.LogicCompile(Args).IsEqual(right_node.LogicCompile(Args))
            : LeftCompile(Args).Subtract(RightCompile(Args)).GetAbs().IsLessThan(__Epsilon);

    /// <summary>Клонирование узла</summary>
    /// <returns>Клон узла</returns>
    public override ExpressionTreeNode Clone() => CloneOperatorNode<EqualityOperatorNode>();
}