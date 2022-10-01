#nullable enable
using System;
using System.Linq.Expressions;

namespace MathCore.MathParser;

/// <summary>Функционал</summary>
public abstract class Functional : ExpressionItem
{
    /// <summary>Инициализация нового функционала</summary>
    /// <param name="Name">Имя функционала</param>
    protected Functional(string Name) : base(Name) { }

    /// <summary>Метод определения значения</summary>
    /// <returns>Численное значение элемента выражения</returns>
    [NotSupported("Функционал не поддерживает метод получения численного значения")]
    public override double GetValue() => throw new NotSupportedException();

    /// <summary>Метод определения значения</summary>
    /// <param name="ParametersExpression">Выражение параметров</param>
    /// <param name="Function">Ядро функционала</param>
    /// <returns>Численное значение вычисленного выражения</returns>
    public abstract double GetValue(MathExpression ParametersExpression, MathExpression Function);

    /// <summary>Скомпилировать в выражение</summary>
    /// <returns>Скомпилированное выражение <see cref="System.Linq.Expressions"/></returns>
    public abstract Expression Compile(MathExpression ParametersExpression, MathExpression Function);

    /// <summary>Скомпилировать в выражение</summary>
    /// <param name="Function">Выражение ядра функции</param>
    /// <param name="Parameters">Массив параметров</param>
    /// <param name="ParametersExpression">Выражение параметров</param>
    /// <returns>Скомпилированное выражение <see cref="System.Linq.Expressions"/></returns>
    public abstract Expression Compile
    (
        MathExpression ParametersExpression,
        MathExpression Function,
        ParameterExpression[] Parameters
    );

    /// <summary>Инициализация оператора</summary>
    /// <param name="Parameters">Блок параметров</param>
    /// <param name="Function">Блок ядра функции</param>
    /// <param name="Parser">Парсер мат.выражения</param>
    /// <param name="Expression">Внешнее мат.выражение</param>
    public virtual void Initialize
    (
        MathExpression Parameters,
        MathExpression Function,
        ExpressionParser Parser,
        MathExpression Expression
    )
    {

    }
}