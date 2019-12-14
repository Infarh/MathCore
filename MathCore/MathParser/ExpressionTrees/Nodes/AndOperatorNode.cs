using System.Linq.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, реазлиующий оператор ленивого И</summary>
    public class AndOperatorNode : LogicOperatorNode
    {
        /// <summary>Инициализация нового узла оператора ленивого И</summary>
        public AndOperatorNode() : base("&", -15) { }

        /// <summary>Вычислить значение поддерева</summary>
        /// <returns>Численное значение поддерева</returns>
        public override double Compute() => (((ComputedNode)Left).Compute() > 0) && (((ComputedNode)Right).Compute() > 0) ? 1 : 0;

        /// <summary>Компиляция логики узла</summary>
        /// <returns>Скомпилированное логическое выражение, реализующее операцию И</returns>
        public override Expression LogicCompile()
        {
            Expression left;
            if (Left is LogicOperatorNode left_node)
                left = left_node.LogicCompile();
            else
            {
                var value = EqualityOperatorNode.GetAbsMethodCall(((ComputedNode)Left).Compile());
                left = Expression.GreaterThan(value, Expression.Constant(0.0));
            }

            Expression right;
            if (Right is LogicOperatorNode right_node)
                right = right_node.LogicCompile();
            else
            {
                var value = EqualityOperatorNode.GetAbsMethodCall(((ComputedNode)Right).Compile());
                right = Expression.GreaterThan(value, Expression.Constant(0.0));
            }

            return Expression.AndAlso(left, right);
        }

        /// <summary>Компиляция логики узла</summary>
        /// <param name="Parameters">Параметры компиляции</param>
        /// <returns>Скомпилированное логическое выражение, реализующее операцию И</returns>
        public override Expression LogicCompile(params ParameterExpression[] Parameters)
        {
            var left = Left is LogicOperatorNode left_node
                        ? left_node.LogicCompile(Parameters)
                        : Expression.GreaterThan(EqualityOperatorNode.GetAbsMethodCall(((ComputedNode)Left).Compile(Parameters)), Expression.Constant(0.0));
            var right = Right is LogicOperatorNode right_node
                        ? right_node.LogicCompile(Parameters)
                        : Expression.GreaterThan(EqualityOperatorNode.GetAbsMethodCall(((ComputedNode)Right).Compile(Parameters)), Expression.Constant(0.0));
            return Expression.AndAlso(left, right);
        }

        /// <summary>Клонирование поддерева</summary>
        /// <returns>Клон поддерева</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<AndOperatorNode>();
    }
}