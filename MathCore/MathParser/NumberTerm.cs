﻿#nullable enable
using MathCore.MathParser.ExpressionTrees.Nodes;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.MathParser;

/// <summary>Числовой элемент математического выражения</summary>
internal sealed class NumberTerm : Term
{
    /// <summary>Численное значение элемента</summary>
    private int _IntValue;

    /// <summary>Численное значение элемента</summary>
    public int Value
    {
        get => _IntValue;
        set
        {
            _IntValue = value;
            _Value    = value.ToString();
        }
    }

    /// <summary>Новый численный элемент мат.выражения</summary>
    /// <param name="Str">Строковое значение элемента</param>
    public NumberTerm(string Str) : base(Str) => _IntValue = int.Parse(Str);

    public NumberTerm(int Value) : base(Value.ToString()) => _IntValue = Value;

    /// <summary>Извлечь поддерево</summary>
    /// <param name="Parser">Парсер</param>
    /// <param name="Expression">Математическое выражение</param>
    /// <returns>Узел константного значения</returns>
    public override ExpressionTreeNode GetSubTree(ExpressionParser Parser, MathExpression Expression)
        => new ConstValueNode(_IntValue);

    /// <summary>Попытаться добавить дробное значение числа</summary>
    /// <param name="node">Узел выражения</param>
    /// <param name="SeparatorTerm">Блок разделитель</param>
    /// <param name="DecimalSeparator">Блок с целой частью числа</param>
    /// <param name="FractionPartTerm">Блок с дробной частью числа</param>
    /// <returns>Истина, если действие совершено успешно. Ложь, если в последующих блоках не содержится нужной информации</returns>
    public static bool TryAddFractionPart(ref ExpressionTreeNode node, Term SeparatorTerm, char DecimalSeparator, Term FractionPartTerm)
    {
        if(node is not ConstValueNode value) throw new ArgumentException("Неверный тип узла дерева");
        if(SeparatorTerm is not CharTerm separator || separator.Value != DecimalSeparator) return false;
        if(FractionPartTerm is not NumberTerm fraction) return false;

        var v_value = fraction.Value;
        if(v_value == 0) return true;
        node = new ConstValueNode(value.Value + v_value / Math.Pow(10, Math.Truncate(Math.Log10(v_value)) + 1));
        return true;
    }
}