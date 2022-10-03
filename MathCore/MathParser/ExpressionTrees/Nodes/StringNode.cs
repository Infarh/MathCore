﻿#nullable enable
// ReSharper disable UnusedMember.Global


// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Строковый узел дерева математического выражения</summary>
public class StringNode : ParsedNode
{
    /// <summary>Значение узла</summary>
    public string Value { get; set; }

    /// <summary>Инициализация нового строкового узла</summary>
    public StringNode() { }

    /// <summary>Инициализация нового строкового узла</summary>
    /// <param name="value">Значение узла</param>
    public StringNode(string value) => Value = value;

    /// <summary>Клонирование узла</summary>
    /// <returns>Клон узла</returns>
    public override ExpressionTreeNode Clone() => new StringNode(Value)
    {
        Left  = Left?.Clone(),
        Right = Right?.Clone()
    };

    /// <summary>Строковое представление узла</summary>
    /// <returns>Строковое представление узла</returns>
    public override string ToString() => $"{(Left is null ? string.Empty : $"{Left}")}{Value}{(Right is null ? string.Empty : $"{Right}")}";

    /// <summary>Оператор неявного преобразования строки к типу строкового узла</summary>
    /// <param name="value">Строковое значение</param>
    /// <returns>Строковый узел</returns>
    public static implicit operator StringNode(string value) => new(value);

    /// <summary>Оператор неявного преобразования строкового узла к строковому типу</summary>
    /// <param name="node">Строковый узел</param>
    /// <returns>Значение строкового узла</returns>
    public static implicit operator string(StringNode node) => node.Value;
}