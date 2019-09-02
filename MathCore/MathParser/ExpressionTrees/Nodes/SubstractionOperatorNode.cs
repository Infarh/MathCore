using System.Linq.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева выражения, релизующий оператор вычитания</summary>
    public class SubstractionOperatorNode : OperatorNode
    {
        /// <summary>Инициализация нового оператора вычитания</summary>
        public SubstractionOperatorNode() : base("-", 5) { }

        /// <summary>Вычисление значение узла</summary>
        /// <returns>Значение разности значений правого и левого поддеревьев</returns>
        public override double Compute() => (((ComputedNode)Left)?.Compute() ?? 0) - (((ComputedNode)Right)?.Compute() ?? 0);

        /// <summary>Компиляция выражения узла</summary>
        /// <returns>Скомпилированное выражение узла</returns>
        public override Expression Compile()
            => Expression.Subtract
            (
                ((ComputedNode)Left)?.Compile() ?? Expression.Constant(0.0),
                ((ComputedNode)Right)?.Compile() ?? Expression.Constant(0.0)
            );

        /// <summary>Компиляция выражения узла</summary>
        /// <param name="Parameters">Список параметров выражения</param>
        /// <returns>Скомпилированное выражение узла</returns>
        /// <returns></returns>
        public override Expression Compile(ParameterExpression[] Parameters) => Left is null
                    ? (Expression)Expression.Negate(((ComputedNode)Right).Compile(Parameters))
                    : Expression.Subtract(((ComputedNode)Left).Compile(Parameters), ((ComputedNode)Right).Compile(Parameters));

        /// <summary>Клонирование узла</summary>
        /// <returns>Клон узла</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<SubstractionOperatorNode>();
    }
}