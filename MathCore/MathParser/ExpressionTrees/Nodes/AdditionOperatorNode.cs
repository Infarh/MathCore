using System;
using System.Linq.Expressions;
using MathCore.Extensions.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева выражений, реализующий оператор сложения</summary>
    public class AdditionOperatorNode : OperatorNode
    {
        public const string NodeName = "+";

        /// <summary>Новый оператор сложения</summary>
        public AdditionOperatorNode() : base(NodeName, 0) { }

        /// <summary>Вычисление узла</summary>
        /// <returns>Сумма поддеревьев</returns>
        public override double Compute() => (((ComputedNode)Left)?.Compute() ?? 0) + ((ComputedNode)Right)?.Compute() ?? 0;

        /// <summary>Компиляция узла</summary>
        /// <returns>Linq.Expression.Add()</returns>
        public override Expression Compile() =>
            (((ComputedNode)Left)?.Compile() ?? 0d.ToExpression())
                .Add(((ComputedNode)Right)?.Compile() ?? 0d.ToExpression());

        /// <summary>Компиляция узла</summary>
        /// <param name="Parameters">Массив параметров выражения</param>
        /// <returns>Linq.Expression.Add()</returns>
        public override Expression Compile(params ParameterExpression[] Parameters) =>
            (((ComputedNode)Left)?.Compile(Parameters) ?? 0d.ToExpression())
                .Add(((ComputedNode)Right)?.Compile(Parameters) ?? 0d.ToExpression());

        /// <summary>Клонирование узла</summary>
        /// <returns>Полный клон узла с клонами поддеревьев</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<AdditionOperatorNode>();
    }
}