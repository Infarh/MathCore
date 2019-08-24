using System;
using System.Linq.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева математического выражения, реализующий оператор возведения в степень</summary>
    public class PowerOperatorNode : OperatorNode
    {
        /// <summary>Инициализация нового узла оператора возведения в степень</summary>
        public PowerOperatorNode() : base("^", 20) { }

        /// <summary>Вычисление узла выражения</summary>
        /// <returns>Возведение значения корня левого поддерева в степень значения корня правого поддерева</returns>
        public override double Compute() => Math.Pow(((ComputedNode)Left).Compute(), ((ComputedNode)Right).Compute());

        /// <summary>Компиляция выражения узла</summary>
        /// <returns>Скомпилированное выражение узла</returns>
        public override Expression Compile() => Expression.Power(((ComputedNode)Left).Compile(), ((ComputedNode)Right).Compile());

        /// <summary>Компиляция выражения узла</summary>
        /// <param name="Parameters">Массив параметров выражения</param>
        /// <returns>Скомпилированное выражение узла</returns>
        public override Expression Compile(ParameterExpression[] Parameters) => 
            Expression.Power(((ComputedNode)Left).Compile(Parameters), ((ComputedNode)Right).Compile(Parameters));

        /// <summary>Клонирование узла</summary>
        /// <returns>Клон узла</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<PowerOperatorNode>();
    }
}