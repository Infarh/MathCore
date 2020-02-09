using System.Linq.Expressions;
using MathCore.Annotations;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, реализующий оператор больше</summary>
    // todo: добавить логику разбора конструкций типа 5>x>-5
    public class GreaterThenOperatorNode : LogicOperatorNode
    {
        /// <summary>Инициализация нового узла оператора больше</summary>
        public GreaterThenOperatorNode() : base(">", -10) { }

        /// <summary>Инициализация нового узла оператора больше</summary>
        /// <param name="Left">Левое поддерево выражения</param>
        /// <param name="Right">Правое поддерево выражения</param>
        public GreaterThenOperatorNode(ExpressionTreeNode Left, ExpressionTreeNode Right)
            : this()
        {
            this.Left = Left;
            this.Right = Right;
        }

        /// <summary>Метод сравнения</summary>
        /// <param name="x">Первое значение</param>
        /// <param name="y">Второе значение</param>
        /// <returns>1 - если x больше y и 0 - в противном случае</returns>
        private static double Comparer(double x, double y) => x > y ? 1 : 0;

        /// <summary>Вычисление значения узла</summary>
        /// <returns>0 - если разность между x и y по модулю меньше Epsilon и 1 во всех остальных случаях</returns>
        public override double Compute() => Comparer(((ComputedNode)Left).Compute(), ((ComputedNode)Right).Compute());

        /// <summary>Компиляция логики узла</summary>
        /// <returns>Скомпилированное логическое выражение, реализующее операцию сравнения Больше</returns>
        [NotNull]
        public override Expression LogicCompile() => Expression.GreaterThan(((ComputedNode)Left).Compile(), ((ComputedNode)Right).Compile());

        /// <summary>Компиляция логики узла</summary>
        /// <param name="Args">Параметры компиляции</param>
        /// <returns>Скомпилированное логическое выражение, реализующее операцию сравнения Больше</returns>
        [NotNull]
        public override Expression LogicCompile(ParameterExpression[] Args) => Expression.GreaterThan(((ComputedNode)Left).Compile(Args), ((ComputedNode)Right).Compile(Args));

        /// <summary>Клонирование узла</summary>
        /// <returns>Клон узла</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<GreaterThenOperatorNode>();
    }
}