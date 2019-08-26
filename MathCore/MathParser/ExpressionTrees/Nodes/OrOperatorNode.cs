using System.Linq.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, реазлиующий оператор ленивого ИЛИ</summary>
    public class OrOperatorNode : LogicOperatorNode
    {
        /// <summary>Инициализация нового узла оператора ленивого ИЛИ</summary>
        public OrOperatorNode() : base("|", -15) { }

        /// <summary>Вычислить значение поддерева</summary>
        /// <returns>Численное значение поддерева</returns>
        public override double Compute() => (((ComputedNode)Left).Compute() > 0) || (((ComputedNode)Right).Compute() > 0) ? 1 : 0;

        /// <summary>Компиляция логики узла</summary>
        /// <returns>Скомпилированное логическое выражение, реализующее операцию ИЛИ</returns>
        public override Expression LogicCompile()
        {
            var left = Left is LogicOperatorNode left_operation
                        ? left_operation.LogicCompile()
                        : Expression.GreaterThan(EqualityOperatorNode.GetAbsMethodCall(((ComputedNode)Left).Compile()), Expression.Constant(0.0));
            var right = Right is LogicOperatorNode right_operation
                        ? right_operation.LogicCompile()
                        : Expression.GreaterThan(EqualityOperatorNode.GetAbsMethodCall(((ComputedNode)Right).Compile()), Expression.Constant(0.0));
            return Expression.OrElse(left, right);
        }

        /// <summary>Компиляция логики узла</summary>
        /// <param name="Parameters">Параметры компиляции</param>
        /// <returns>Скомпилированное логическое выражение, реализующее операцию ИЛИ</returns>
        public override Expression LogicCompile(ParameterExpression[] Parameters)
        {
            var left = Left is LogicOperatorNode left_operation
                        ? left_operation.LogicCompile(Parameters)
                        : Expression.GreaterThan(EqualityOperatorNode.GetAbsMethodCall(((ComputedNode)Left).Compile(Parameters)), Expression.Constant(0.0));
            var right = Right is LogicOperatorNode right_operation
                        ? right_operation.LogicCompile(Parameters)
                        : Expression.GreaterThan(EqualityOperatorNode.GetAbsMethodCall(((ComputedNode)Right).Compile(Parameters)), Expression.Constant(0.0));
            return Expression.OrElse(left, right);
        }

        /// <summary>Клонирование поддерева</summary>
        /// <returns>Клон поддерева</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<OrOperatorNode>();
    }
}