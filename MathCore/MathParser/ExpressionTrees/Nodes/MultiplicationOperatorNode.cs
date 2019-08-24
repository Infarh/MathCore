using System.Linq.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, реазлиующий оператор умножения "*"</summary>
    public class MultiplicationOperatorNode : OperatorNode
    {
        public const string NodeName = "·";

        /// <summary>Инициализация нового узла оператора произведения</summary>
        public MultiplicationOperatorNode() : base(NodeName, 10) { }

        /// <summary>Инициализация нового узла оператора произведения</summary>
        /// <param name="Left">Левое поддерево произведения</param>
        /// <param name="Right">Правое поддерево произведения</param>
        public MultiplicationOperatorNode(ExpressionTreeNode Left, ExpressionTreeNode Right)
            : this()
        {
            this.Left = Left;
            this.Right = Right;
        }

        /// <summary>Вычисление значения узла</summary>
        /// <returns>Произведение значений корней правого и левого поддеревьев</returns>
        public override double Compute() => (Left == null ? 1 : ((ComputedNode)Left).Compute()) * ((ComputedNode)Right).Compute();

        /// <summary>Компиляция узла</summary>
        /// <returns>Скомпилированное выражение произведения узлов поддеревьев</returns>
        public override Expression Compile() => Expression.Multiply(((ComputedNode) Left)?.Compile() ?? Expression.Constant(1.0), ((ComputedNode)Right).Compile());

        /// <summary>Компиляция узла</summary>
        /// <param name="Parameters">Массив параметров выражения</param>
        /// <returns>Скомпилированное выражение произведения узлов поддеревьев</returns>
        public override Expression Compile(ParameterExpression[] Parameters) => 
            Expression.Multiply(((ComputedNode) Left)?.Compile(Parameters) ?? Expression.Constant(1.0), ((ComputedNode)Right).Compile(Parameters));

        /// <summary>Клонирование узла</summary>
        /// <returns>Клон узла</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<MultiplicationOperatorNode>();
    }
}