#nullable enable
using System.Linq.Expressions;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Вычислимый узел дерева математического выражения</summary>
public abstract class ComputedNode : ExpressionTreeNode
{
    /// <summary>Левый узел, приведённый к виду вычислимого узла (допускающий пустуй ссылку)</summary>
    public ComputedNode? LeftComputedOrNull => (ComputedNode?)Left;

    /// <summary>Левый узел, приведённый к виду вычислимого узла</summary>
    public ComputedNode LeftComputed => LeftComputedOrNull ?? throw new InvalidOperationException("Отсутствует ссылка на левый узел - операнд бинарной операции");

    /// <summary>Правый узел, приведённый к виду вычислимого узла (допускающий пустуй ссылку)</summary>
    public ComputedNode? RightComputedOrNull => (ComputedNode?)Right;

    /// <summary>Правый узел, приведённый к виду вычислимого узла</summary>
    public ComputedNode RightComputed => RightComputedOrNull ?? throw new InvalidOperationException("Отсутствует ссылка на правый узел - операнд бинарной операции");

    public double LeftCompute() => LeftComputed.Compute();

    public double LeftCompute(double DefaultValue) => LeftComputedOrNull?.Compute() ?? DefaultValue;

    public double RightCompute() => RightComputed.Compute();

    public double RightCompute(double DefaultValue) => RightComputedOrNull?.Compute() ?? DefaultValue;

    public Expression LeftCompile() => LeftComputed.Compile();
    public Expression LeftCompile(double DefaultValue) => LeftComputedOrNull?.Compile() ?? DefaultValue.ToExpression();
    public Expression LeftCompile(ParameterExpression[] Parameters) => LeftComputed.Compile(Parameters);
    public Expression LeftCompile(ParameterExpression[] Parameters, double DefaultValue) => LeftComputedOrNull?.Compile(Parameters) ?? DefaultValue.ToExpression();

    public Expression RightCompile() => RightComputed.Compile();
    public Expression RightCompile(double DefaultValue) => RightComputedOrNull?.Compile() ?? DefaultValue.ToExpression();
    public Expression RightCompile(ParameterExpression[] Parameters) => RightComputed.Compile(Parameters);
    public Expression RightCompile(ParameterExpression[] Parameters, double DefaultValue) => RightComputedOrNull?.Compile(Parameters) ?? DefaultValue.ToExpression();

    /// <summary>Вычислить значение поддерева</summary>
    /// <returns>Численное значение поддерева</returns>
    public abstract double Compute();

    /// <summary>Скомпилировать в выражение</summary>
    /// <returns>Скомпилированное выражение System.Linq.Expressions</returns>
    public abstract Expression Compile();

    /// <summary>Скомпилировать в выражение</summary>
    /// <param name="Args">Массив параметров</param>
    /// <returns>Скомпилированное выражение System.Linq.Expressions</returns>
    public abstract Expression Compile(params ParameterExpression[] Args);
}