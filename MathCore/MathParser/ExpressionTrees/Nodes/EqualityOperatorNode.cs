using System;
using System.Linq.Expressions;
using MathCore.Annotations;

// ReSharper disable UnusedMember.Global

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева мат.выражения, реализующий оператор равенства</summary>
    public class EqualityOperatorNode : LogicOperatorNode
    {
        /// <summary>Получить выражение вызова метода определения модуля числа из класса Math</summary>
        /// <param name="x">Параметр выражения</param>
        /// <returns>Выражение <see cref="System.Linq.Expressions.Expression"/>, вызывающее метод Math.Abs с параметром x</returns>
        [NotNull]
        public static Expression GetAbsMethodCall([NotNull] Expression x) => Expression.Call(((Func<double, double>)Math.Abs).Method, x);

        /// <summary>Точность вычисления равенства для чисел с плавающей точкой</summary>
        internal static double __Epsilon = 1e-12;

        /// <summary>Точность вычисления равенства для чисел с плавающей точкой</summary>
        public static double Epsilon { get => __Epsilon; set => __Epsilon = value; }

        /// <summary>Инициализация нового узла оператора равенства</summary>
        public EqualityOperatorNode() : base("=", -30) { }

        /// <summary>Инициализация нового узла оператора равенства</summary>
        /// <param name="Left">Левое поддерево выражения</param>
        /// <param name="Right">Правое поддерево выражения</param>
        public EqualityOperatorNode(ExpressionTreeNode Left, ExpressionTreeNode Right)
            : this()
        {
            this.Left = Left;
            this.Right = Right;
        }

        /// <summary>Метод сравнения</summary>
        /// <param name="x">Первое значение</param>
        /// <param name="y">Второе значение</param>
        /// <returns>0 - если разность между x и y по модулю меньше Epsilon и 1 во всех остальных случаях</returns>
        private static double Comparer(double x, double y) => Math.Abs(x - y) < __Epsilon ? 1 : 0;

        /// <summary>Вычисление значения узла</summary>
        /// <returns>0 - если разность между x и y по модулю меньше Epsilon и 1 во всех остальных случаях</returns>
        public override double Compute() => Comparer(((ComputedNode)Left).Compute(), ((ComputedNode)Right).Compute());

        /// <summary>Компиляция логики узла</summary>
        /// <returns>Скомпилированное логическое выражение, реализующее операцию сравнения Равенство</returns>
        [NotNull]
        public override Expression LogicCompile() => 
            Left is LogicOperatorNode left_node && Right is LogicOperatorNode right_node
                ? Expression.Equal(left_node.LogicCompile(), right_node.LogicCompile())
                : Expression.LessThan
                (
                    GetAbsMethodCall(Expression.Subtract
                        (
                            ((ComputedNode)Left).Compile(),
                            ((ComputedNode)Right).Compile())
                    ),
                    Expression.Constant(__Epsilon)
                );

        /// <summary>Компиляция логики узла</summary>
        /// <param name="Parameters">Параметры компиляции</param>
        /// <returns>Скомпилированное логическое выражение, реализующее операцию сравнения Равенство</returns>
        [NotNull]
        public override Expression LogicCompile(ParameterExpression[] Parameters) => 
            Left is LogicOperatorNode left_node && Right is LogicOperatorNode right_node
                ? Expression.Equal(left_node.LogicCompile(Parameters), right_node.LogicCompile(Parameters))
                : Expression.LessThan
                (
                    GetAbsMethodCall(Expression.Subtract
                        (
                            ((ComputedNode)Left).Compile(Parameters),
                            ((ComputedNode)Right).Compile(Parameters))
                    ),
                    Expression.Constant(__Epsilon)
                );

        /// <summary>Клонирование узла</summary>
        /// <returns>Клон узла</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<EqualityOperatorNode>();
    }
}