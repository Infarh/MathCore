﻿using System;
using Ex = System.Linq.Expressions.Expression;
using bEx = System.Linq.Expressions.BinaryExpression;
// ReSharper disable UnusedMember.Global

namespace MathCore.Evaluations;

/// <summary>Вычисление функционального бинарного оператора между двумя вычислениями</summary>
/// <typeparam name="T">Тип значения вычисления</typeparam>
public class BinaryFunctionOperatorEvaluation<T> : BinaryFunctionOperationEvaluation<T>
{
    /// <summary>Метод получения функции, рассчитывающей значение оператора для двух известных значений вычислений</summary>
    /// <param name="OP">Функция, преобразующая два выражения в бинарный оператор, позволяющий рассчитать значение операции между двумя значениями операндов</param>
    /// <returns>Метод вычисления значения оператора на основе значений двух его операндов</returns>
    protected static Func<T, T, T> GetOperatorFunction(Func<Ex, Ex, bEx> OP)
    {
        var type = typeof(T);
        var pa   = Ex.Parameter(type, "a");
        var pb   = Ex.Parameter(type, "b");
        return Ex.Lambda<Func<T, T, T>>(OP(pa, pb), pa, pb).Compile();
    }

    /// <summary>Функция объединения двух выражений в бинарный оператор расчёта значения</summary>
    private readonly Func<Ex, Ex, bEx> _Operator;

    /// <summary>Инициализация нового функционального бинарного оператора вычисления</summary>
    /// <param name="Operator">Функция, определяющая как объединить два выражения операндов в бинарный оператор</param>
    public BinaryFunctionOperatorEvaluation(Func<Ex, Ex, bEx> Operator) : base(GetOperatorFunction(Operator)) => _Operator = Operator;

    /// <summary>Инициализация нового функционального бинарного оператора на основе функции генерации бинарного оператора и двух вычислений операндов</summary>
    /// <param name="Operator">Функция, определяющая как объединить два выражения операндов в бинарный оператор</param>
    /// <param name="a">Вычисление первого операнда</param>
    /// <param name="b">Вычисление второго операнда</param>
    public BinaryFunctionOperatorEvaluation(Func<Ex, Ex, bEx> Operator, Evaluation<T> a, Evaluation<T> b)
        : base(GetOperatorFunction(Operator), a, b) => _Operator = Operator;

    /// <inheritdoc />
    public override Ex GetExpression() => _Operator(A.GetExpression(), B.GetExpression());
}