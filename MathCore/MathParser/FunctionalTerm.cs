#nullable enable
using MathCore.MathParser.ExpressionTrees.Nodes;

namespace MathCore.MathParser;

/// <summary>Блок определения функции</summary>
internal sealed class FunctionalTerm : FunctionTerm
{
    /// <summary>Параметры оператора</summary>
    public BlockTerm Parameters { get; }

    /// <summary>Инициализация блока комплексного оператора</summary>
    /// <param name="Header">Заголовок блока</param>
    /// <param name="Body">Тело блока</param>
    public FunctionalTerm(FunctionTerm Header, BlockTerm Body) : base(Header.Name, Body) => Parameters = Header.Block;

    /// <summary>Получить поддерево комплексного оператора</summary>
    /// <param name="Parser">Парсер</param>
    /// <param name="Expression">Математическое выражение</param>
    /// <returns>Узел комплексного оператора</returns>
    public override ExpressionTreeNode GetSubTree(ExpressionParser Parser, MathExpression Expression)
        => new FunctionalNode(this, Parser, Expression);

    /// <summary>Преобразование в строковую форму</summary>
    /// <returns>Строковое представление элемента</returns>
    public override string ToString() => $"{Name}{Parameters}{Block}";
}