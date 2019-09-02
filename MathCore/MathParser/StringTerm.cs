using System.Diagnostics.Contracts;
using MathCore.Annotations;
using MathCore.MathParser.ExpressionTrees.Nodes;

namespace MathCore.MathParser
{
    /// <summary>Строковый элемент выражения</summary>
    internal class StringTerm : Term
    {
        /// <summary>Имя строкового элемента</summary>
        [NotNull]
        public string Name => _Value;

        /// <summary>Новый строковый элемент</summary>
        /// <param name="Name">Имя строкового элемента</param>
        public StringTerm([NotNull] string Name) : base(Name) => Contract.Requires(!string.IsNullOrEmpty(Name));

        /// <summary>Поддерево элемента, состоящее из узла-переменной</summary>
        /// <param name="Parser">Парсер</param>
        /// <param name="Expression">Математическое выражение</param>
        /// <returns>Узел дерева с переменной, полученной из Expression.Variable[Name]</returns>
        public override ExpressionTreeNode GetSubTree(ExpressionParser Parser, MathExpression Expression)
            => new VariableValueNode(Expression.Variable[Name]);
    }
}