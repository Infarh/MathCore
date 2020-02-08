using System;
using System.Linq.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, реализующий оператор отрицания</summary>
    public class NotOperatorNode : LogicOperatorNode
    {
        /// <summary>Инициализация нового узла оператора меньше</summary>
        public NotOperatorNode() : base("!", -20) { }

        /// <summary>Инициализация нового узла оператора меньше</summary>
        /// <param name="Left">Левое поддерево выражения</param>
        /// <param name="Right">Правое поддерево выражения</param>
        public NotOperatorNode(ExpressionTreeNode Left, ExpressionTreeNode Right)
            : this()
        {
            this.Left = Left;
            this.Right = Right;
        }

        /// <summary>Метод отрицания</summary>
        /// <param name="x">Первое значение</param>
        /// <param name="y">Второе значение</param>
        /// <returns>1 - если x меньше y и 0 - в противном случае</returns>
        private static double Comparer(double x, double y) => Math.Abs(x - y) >= EqualityOperatorNode.__Epsilon ? 1 : 0;

        /// <summary>Метод отрицания</summary>
        /// <param name="x">Значение</param>
        /// <returns>1 - если x меньше y и 0 - в противном случае</returns>
        private static double ComparerSingle(double x) => Math.Abs(x) < EqualityOperatorNode.__Epsilon ? 1 : 0;

        /// <summary>Вычисление значения узла</summary>
        /// <returns>0 - если разность между x и y по модулю меньше Epsilon и 1 во всех остальных случаях</returns>
        public override double Compute() => Left is null
                    ? ComparerSingle(((ComputedNode)Right).Compute())
                    : Comparer(((ComputedNode)Left).Compute(), ((ComputedNode)Right).Compute());

        /// <summary>Компиляция логики узла</summary>
        /// <returns>Скомпилированное логическое выражение, реализующее операцию отрицания НЕ</returns>
        public override Expression LogicCompile() =>
            Left is null
                ? Right is LogicOperatorNode node
                    ? (Expression) Expression.Not(node.LogicCompile())
                    : Expression.LessThan
                    (
                        EqualityOperatorNode.GetAbsMethodCall(((ComputedNode) Right).Compile()),
                        Expression.Constant(EqualityOperatorNode.__Epsilon)
                    )
                : Left is LogicOperatorNode operator_node && Right is LogicOperatorNode logic_operator_node
                    ? Expression.NotEqual(operator_node.LogicCompile(), logic_operator_node.LogicCompile())
                    : Expression.GreaterThanOrEqual
                    (
                        EqualityOperatorNode.GetAbsMethodCall(Expression.Subtract
                            (
                                ((ComputedNode) Left).Compile(),
                                ((ComputedNode) Right).Compile())
                        ),
                        Expression.Constant(EqualityOperatorNode.__Epsilon)
                    );

        /// <summary>Компиляция логики узла</summary>
        /// <param name="Parameters">Параметры компиляции</param>
        /// <returns>Скомпилированное логическое выражение, реализующее операцию отрицания НЕ</returns>
        public override Expression LogicCompile(ParameterExpression[] Parameters) =>
            Left is null
                ? Right is LogicOperatorNode node
                    ? (Expression) Expression.Not(node.LogicCompile(Parameters))
                    : Expression.LessThan
                    (
                        EqualityOperatorNode.GetAbsMethodCall(((ComputedNode) Right).Compile(Parameters)),
                        Expression.Constant(EqualityOperatorNode.__Epsilon)
                    )
                : Left is LogicOperatorNode operator_node && Right is LogicOperatorNode logic_operator_node
                    ? Expression.NotEqual(operator_node.LogicCompile(Parameters),
                        logic_operator_node.LogicCompile(Parameters))
                    : Expression.GreaterThanOrEqual
                    (
                        EqualityOperatorNode.GetAbsMethodCall(Expression.Subtract
                        (
                                ((ComputedNode) Left).Compile(Parameters),
                                ((ComputedNode) Right).Compile(Parameters))
                        ),
                        Expression.Constant(EqualityOperatorNode.__Epsilon)
                    );

        /// <summary>Строковое представление узла</summary>
        /// <returns>Строковое представление узла</returns>
        public override string ToString() => Right is null ? base.ToString() : $"{Left}≠{Right}";

        /// <summary>Клонирование узла</summary>
        /// <returns>Клон узла</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<NotOperatorNode>();
    }
}