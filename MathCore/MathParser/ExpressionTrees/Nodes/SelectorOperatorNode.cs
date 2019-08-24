using System;
using System.Linq.Expressions;
// ReSharper disable UnusedMember.Global

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, реазлиующий оператор выбора</summary>
    public class SelectorOperatorNode : OperatorNode
    {
        /// <summary>Инициализация нового узла оператора выбора</summary>
        public SelectorOperatorNode() : base("?", -17) { }

        /// <summary>Инициализация нового узла оператора выбора</summary>
        /// <param name="Left">Левое поддерево выражения</param>
        /// <param name="Right">Правое поддерево выражения</param>
        public SelectorOperatorNode(ExpressionTreeNode Left, ExpressionTreeNode Right)
            : this()
        {
            this.Left = Left;
            this.Right = Right;
        }

        /// <summary>Вычисление значения узла</summary>
        /// <returns></returns>
        public override double Compute()
        {
            var variants = (VariantOperatorNode)Right;
            return Math.Abs(((ComputedNode)Left).Compute()) > 0
                        ? ((ComputedNode)variants.Left).Compute()
                        : ((ComputedNode)variants.Right).Compute();
        }

        /// <summary>Компиляция узла</summary>
        /// <returns>Скомпилированное выражение произведения узлов поддеревьев</returns>
        public override Expression Compile()
        {
            var variants = (VariantOperatorNode)Right;
            Expression condition;
            if(Left is LogicOperatorNode node)
                condition = node.LogicCompile();
            else
            {
                var comparer = EqualityOperatorNode.GetAbsMethodCall(((ComputedNode)Left).Compile());
                condition = Expression.GreaterThan(comparer, Expression.Constant(0.0));
            }
            return Expression.Condition(condition, ((ComputedNode)variants.Left).Compile(), ((ComputedNode)variants.Right).Compile());
        }

        /// <summary>Компиляция узла</summary>
        /// <param name="Parameters">Массив параметров выражения</param>
        /// <returns>Скомпилированное выражение произведения узлов поддеревьев</returns>
        public override Expression Compile(ParameterExpression[] Parameters)
        {
            var variants = (VariantOperatorNode)Right;
            Expression condition;
            if(Left is LogicOperatorNode node)
                condition = node.LogicCompile(Parameters);
            else
            {
                var comparer = EqualityOperatorNode.GetAbsMethodCall(((ComputedNode)Left).Compile(Parameters));
                condition = Expression.GreaterThan(comparer, Expression.Constant(0.0));
            }
            return Expression.Condition(condition, ((ComputedNode)variants.Left).Compile(Parameters), ((ComputedNode)variants.Right).Compile(Parameters));
        }

        /// <summary>Клонирование узла</summary>
        /// <returns>Клон узла</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<SelectorOperatorNode>();
    }
}