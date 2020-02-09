using System.Linq.Expressions;
using MathCore.Annotations;
using MathCore.Extensions.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, реализующий оператор ленивого ИЛИ</summary>
    public class OrOperatorNode : LogicOperatorNode
    {
        /// <summary>Инициализация нового узла оператора ленивого ИЛИ</summary>
        public OrOperatorNode() : base("|", -15) { }

        /// <summary>Вычислить значение поддерева</summary>
        /// <returns>Численное значение поддерева</returns>
        public override double Compute() => LeftCompute() > 0 || RightCompute() > 0 ? 1 : 0;

        /// <summary>Компиляция логики узла</summary>
        /// <returns>Скомпилированное логическое выражение, реализующее операцию ИЛИ</returns>
        [NotNull]
        public override Expression LogicCompile()
        {
            var left = Left is LogicOperatorNode left_operation 
                ? left_operation.LogicCompile()
                : LeftCompile().GetAbs().IsGreaterThan(0d);

            var right = Right is LogicOperatorNode right_operation 
                ? right_operation.LogicCompile() 
                : RightCompile().GetAbs().IsGreaterThan(0d);

            return left.OrLazy(right);
        }

        /// <summary>Компиляция логики узла</summary>
        /// <param name="Args">Параметры компиляции</param>
        /// <returns>Скомпилированное логическое выражение, реализующее операцию ИЛИ</returns>
        [NotNull]
        public override Expression LogicCompile(params ParameterExpression[] Args)
        {
            var left = Left is LogicOperatorNode left_operation
                        ? left_operation.LogicCompile(Args)
                        : LeftCompile(Args).GetAbs().IsGreaterThan(0d);
            var right = Right is LogicOperatorNode right_operation
                        ? right_operation.LogicCompile(Args)
                        : RightCompile(Args).GetAbs().IsGreaterThan(0d);

            return left.OrLazy(right);
        }

        /// <summary>Клонирование поддерева</summary>
        /// <returns>Клон поддерева</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<OrOperatorNode>();
    }
}