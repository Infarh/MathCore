using System.Linq.Expressions;
using MathCore.Extensions.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Узел дерева выражения, реализующий оператор вычитания</summary>
    public class subtractionOperatorNode : OperatorNode
    {
        /// <summary>Инициализация нового оператора вычитания</summary>
        public subtractionOperatorNode() : base("-", 5) { }

        /// <summary>Вычисление значение узла</summary>
        /// <returns>Значение разности значений правого и левого поддеревьев</returns>
        public override double Compute() => LeftCompute(0) - RightCompute(0);

        /// <summary>Компиляция выражения узла</summary>
        /// <returns>Скомпилированное выражение узла</returns>
        public override Expression Compile() => LeftCompile(0).Subtract(RightCompile(0));

        /// <summary>Компиляция выражения узла</summary>
        /// <param name="Args">Список параметров выражения</param>
        /// <returns>Скомпилированное выражение узла</returns>
        /// <returns></returns>
        public override Expression Compile(params ParameterExpression[] Args) => Left is null
                    ? RightCompile(Args).Negate()
                    : LeftCompile(Args).Subtract( RightCompile(Args));

        /// <summary>Клонирование узла</summary>
        /// <returns>Клон узла</returns>
        public override ExpressionTreeNode Clone() => CloneOperatorNode<subtractionOperatorNode>();
    }
}