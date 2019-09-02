using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using MathCore.Annotations;

namespace MathCore.MathParser.ExpressionTrees.Nodes
{
    /// <summary>Вычислимый узел дерева математического выражения</summary>
    [ContractClass(typeof(ComputedNodeContract))]
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

    [ContractClassFor(typeof(ComputedNode)), ExcludeFromCodeCoverage]
    internal abstract class ComputedNodeContract : ComputedNode
    {
        private ComputedNodeContract() { }

        public override Expression Compile()
        {
            Contract.Ensures(Contract.Result<Expression>() != null);
            throw new System.NotImplementedException();
        }

        public override Expression Compile([NotNull] params ParameterExpression[] Parameters)
        {
            Contract.Requires(Parameters != null);
            Contract.Ensures(Contract.Result<Expression>() != null);
            throw new System.NotImplementedException();
        }
    }
}