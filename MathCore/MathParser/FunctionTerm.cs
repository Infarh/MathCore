using MathCore.Annotations;
using MathCore.MathParser.ExpressionTrees.Nodes;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.MathParser
{
    /// <summary>Функциональный элемент выражения</summary>
    internal class FunctionTerm : StringTerm
    {
        /// <summary>Блок со скобками</summary>
        [NotNull]
        public BlockTerm Block { get; set; }

        /// <summary>Новый функциональный элемент выражения</summary>
        /// <param name="StrTerm">Строковый элемент выражения</param>
        /// <param name="Block">Блок выражения</param>
        public FunctionTerm([NotNull] StringTerm StrTerm, [NotNull] BlockTerm Block) : this(StrTerm.Name, Block) { }

        public FunctionTerm([NotNull] string Name, [NotNull] BlockTerm Block) : base(Name) => this.Block = Block;

        /// <summary>Получить поддерево</summary>
        /// <param name="Parser">Парсер</param>
        /// <param name="Expression">Математическое выражение</param>
        /// <returns>Узел функции</returns>
        public override ExpressionTreeNode GetSubTree(ExpressionParser Parser, MathExpression Expression)
            => new FunctionNode(this, Parser, Expression);

        /// <summary>Преобразование в строковую форму</summary>
        /// <returns>Строковое представление элемента</returns>
        [NotNull]
        public override string ToString() => $"{Name}{Block.ToString() ?? string.Empty}";
    }
}