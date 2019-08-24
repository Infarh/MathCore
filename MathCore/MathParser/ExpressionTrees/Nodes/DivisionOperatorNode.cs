using System.Linq.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, реализующий оператор деления</summary>
    public class DivisionOperatorNode : OperatorNode
    {
        public const string NodeName = "/";

        /// <summary>Инициализация узла оператора деления</summary>
        public DivisionOperatorNode() : base(NodeName, 15) { }

        /// <summary>Вычисление значения узла</summary>
        /// <returns>Значение узла</returns>
        public override double Compute() => (((ComputedNode) Left)?.Compute() ?? 1) / ((ComputedNode)Right).Compute();

        /// <summary>Компиляция узла</summary>
        /// <returns>Скомпилированное выражение узла</returns>
        public override Expression Compile() => Expression.Divide(((ComputedNode) Left)?.Compile() ?? Expression.Constant(1.0), ((ComputedNode)Right).Compile());

        /// <summary>Компиляция узла с параметрами</summary>
        /// <param name="Parameters">Список параметров выражения</param>
        /// <returns>Скомпилированное выражение узла</returns>
        public override Expression Compile(params ParameterExpression[] Parameters) => Expression.Divide(((ComputedNode) Left)?.Compile(Parameters) ?? Expression.Constant(1.0), ((ComputedNode)Right).Compile(Parameters));

        /// <summary>Строковое представление узла</summary>
        /// <returns>Строковое представление</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<DivisionOperatorNode>();
    }
}