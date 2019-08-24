using System.Linq.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, реазлиующий оператор меньше</summary>
    public class LessThenOperatorNode : LogicOperatorNode
    {
        /// <summary>Инициализация нового узла оператора меньше</summary>
        public LessThenOperatorNode() : base("<", -10) { }

        /// <summary>Инициализация нового узла оператора меньше</summary>
        /// <param name="Left">Левое поддерево выражения</param>
        /// <param name="Right">Правое поддерево выражения</param>
        public LessThenOperatorNode(ExpressionTreeNode Left, ExpressionTreeNode Right)
            : this()
        {
            this.Left = Left;
            this.Right = Right;
        }

        /// <summary>Метод сравнения</summary>
        /// <param name="x">Первое значение</param>
        /// <param name="y">Второе значение</param>
        /// <returns>1 - если x меньше y и 0 - в противном случае</returns>
        private static double Comparer(double x, double y) => x < y ? 1 : 0;

        /// <summary>Вычисление значения узла</summary>
        /// <returns>0 - если разность между x и y по модулю меньше Epsilon и 1 во всех остальных случаях</returns>
        public override double Compute() => Comparer(((ComputedNode)Left).Compute(), ((ComputedNode)Right).Compute());

        /// <summary>Компиляция логики узла</summary>
        /// <returns>Скомпилированное логическое выражение, реализующее операцию сравнения Меньше</returns>
        public override Expression LogicCompile() => Expression.LessThan(((ComputedNode)Left).Compile(), ((ComputedNode)Right).Compile());

        /// <summary>Компиляция логики узла</summary>
        /// <param name="Parameters">Параметры компиляции</param>
        /// <returns>Скомпилированное логическое выражение, реализующее операцию сравнения Меньше</returns>
        public override Expression LogicCompile(ParameterExpression[] Parameters) => Expression.LessThan(((ComputedNode)Left).Compile(Parameters), ((ComputedNode)Right).Compile(Parameters));

        /// <summary>Клонирование узла</summary>
        /// <returns>Клон узла</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<LessThenOperatorNode>();
    }
}