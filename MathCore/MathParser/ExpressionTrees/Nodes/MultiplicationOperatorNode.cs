using System.Linq.Expressions;
using MathCore.Extensions.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, реализующий оператор умножения "*"</summary>
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
        public override double Compute() => LeftCompute(1) * RightCompute();

        /// <summary>Компиляция узла</summary>
        /// <returns>Скомпилированное выражение произведения узлов поддеревьев</returns>
        public override Expression Compile() => LeftCompile(1).Multiply(RightCompile());

        /// <summary>Компиляция узла</summary>
        /// <param name="Args">Массив параметров выражения</param>
        /// <returns>Скомпилированное выражение произведения узлов поддеревьев</returns>
        public override Expression Compile(params ParameterExpression[] Args) => LeftCompile(Args, 1).Multiply(RightCompile(Args));

        /// <summary>Клонирование узла</summary>
        /// <returns>Клон узла</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<MultiplicationOperatorNode>();
    }
}