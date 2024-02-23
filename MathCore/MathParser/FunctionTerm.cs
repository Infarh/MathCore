#nullable enable
using MathCore.MathParser.ExpressionTrees.Nodes;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.MathParser;

/// <summary>Функциональный элемент выражения</summary>
internal class FunctionTerm(string Name, BlockTerm Block) : StringTerm(Name)
{
    /// <summary>Блок со скобками</summary>
    public BlockTerm Block { get; set; } = Block;

    /// <summary>Новый функциональный элемент выражения</summary>
    /// <param name="StrTerm">Строковый элемент выражения</param>
    /// <param name="Block">Блок выражения</param>
    public FunctionTerm(StringTerm StrTerm, BlockTerm Block) : this(StrTerm.Name, Block) { }

    /// <summary>Получить поддерево</summary>
    /// <param name="Parser">Парсер</param>
    /// <param name="Expression">Математическое выражение</param>
    /// <returns>Узел функции</returns>
    public override ExpressionTreeNode GetSubTree(ExpressionParser Parser, MathExpression Expression)
        => new FunctionNode(this, Parser, Expression);

    /// <summary>Преобразование в строковую форму</summary>
    /// <returns>Строковое представление элемента</returns>
    public override string ToString() => $"{Name}{Block}";
}