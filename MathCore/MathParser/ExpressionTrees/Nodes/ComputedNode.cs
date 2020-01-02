using System.Linq.Expressions;
using MathCore.Annotations;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Вычислимый узел дерева математического выражения</summary>
    public abstract class ComputedNode : ExpressionTreeNode
    {
        /// <summary>Вычислить значение поддерева</summary>
        /// <returns>Численное значение поддерева</returns>
        public abstract double Compute();

        /// <summary>Скомпилировать в выражение</summary>
        /// <returns>Скомпилированное выражение System.Linq.Expressions</returns>
        [NotNull]
        public abstract Expression Compile();

        /// <summary>Скомпилировать в выражение</summary>
        /// <param name="Parameters">Массив параметров</param>
        /// <returns>Скомпилированное выражение System.Linq.Expressions</returns>
        [NotNull]
        public abstract Expression Compile([NotNull] params ParameterExpression[] Parameters);
    }
}