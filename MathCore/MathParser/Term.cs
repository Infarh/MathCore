using MathCore.Annotations;
using MathCore.MathParser.ExpressionTrees.Nodes;

namespace MathCore.MathParser
{
    /// <summary>Элемент математического выражения</summary>
    internal abstract class Term
    {
        /// <summary>Строковое содержимое</summary>
        protected string _Value;

        /// <summary>Конструктор элемента математического выражения</summary>
        /// <param name="Value">Строковое содержимое</param>
        protected Term(string Value) => _Value = Value;

        /// <summary>Метод извлечения поддерева для данного элемента математического выражения</summary>
        /// <param name="Parser">Парсер математического выражения</param>
        /// <param name="Expression">Математическое выражение</param>
        /// <returns>Узел дерева мат.выражения, являющийся поддеревом для данного элемента мат.выражения</returns>
        [NotNull]
        public abstract ExpressionTreeNode GetSubTree([NotNull] ExpressionParser Parser, [NotNull] MathExpression Expression);

        /// <summary>Строковое представление элемента мат.выражения</summary>
        /// <returns>Строковое содержимое элемета мат.выражения</returns>
        public override string ToString() => _Value;
    }
}