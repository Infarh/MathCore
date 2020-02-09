using System.Linq.Expressions;
using MathCore.Extensions.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева выражений, реализующий оператор сложения</summary>
    public class AdditionOperatorNode : OperatorNode
    {
        /// <summary>Имя узла операции "+"</summary>
        public const string NodeName = "+";

        /// <summary>Новый оператор сложения</summary>
        public AdditionOperatorNode() : base(NodeName, 0) { }

        /// <summary>Вычисление узла</summary>
        /// <returns>Сумма поддеревьев</returns>
        public override double Compute() => LeftCompute(0) + RightCompute(0);

        /// <summary>Компиляция узла</summary>
        /// <returns>Linq.Expression.Add()</returns>
        public override Expression Compile() => LeftCompile(0).Add(RightCompile(0));

        /// <summary>Компиляция узла</summary>
        /// <param name="Args">Массив параметров выражения</param>
        /// <returns>Linq.Expression.Add()</returns>
        public override Expression Compile(params ParameterExpression[] Args) => LeftCompile(Args, 0).Add(RightCompile(Args, 0));

        /// <summary>Клонирование узла</summary>
        /// <returns>Полный клон узла с клонами поддеревьев</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<AdditionOperatorNode>();
    }
}