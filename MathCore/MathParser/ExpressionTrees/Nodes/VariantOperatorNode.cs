using System.Linq.Expressions;
// ReSharper disable UnusedMember.Global

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, реазлиующий оператор определения вариантов</summary>
    public class VariantOperatorNode : OperatorNode
    {
        /// <summary>Инициализация нового узла оператора определения вариантов</summary>
        public VariantOperatorNode() : base(":", -16) { }

        /// <summary>Инициализация нового узла оператора определения вариантов</summary>
        /// <param name="Left">Левое поддерево выражения</param>
        /// <param name="Right">Правое поддерево выражения</param>
        public VariantOperatorNode(ExpressionTreeNode Left, ExpressionTreeNode Right)
            : this()
        {
            this.Left = Left;
            this.Right = Right;
        }

        /// <summary>Вычисление значения узла</summary>
        /// <returns></returns>
        public override double Compute() => ((ComputedNode)Left).Compute();

        /// <summary>Компиляция узла</summary>
        /// <returns>Скомпилированное выражение произведения узлов поддеревьев</returns>
        public override Expression Compile() => ((ComputedNode)Left).Compile();

        /// <summary>Компиляция узла</summary>
        /// <param name="Parameters">Массив параметров выражения</param>
        /// <returns>Скомпилированное выражение произведения узлов поддеревьев</returns>
        public override Expression Compile(ParameterExpression[] Parameters) => ((ComputedNode)Left).Compile(Parameters);

        /// <summary>Клонирование узла</summary>
        /// <returns>Клон узла</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<VariantOperatorNode>();
    }
}