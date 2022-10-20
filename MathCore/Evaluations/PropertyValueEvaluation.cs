﻿using Ex = System.Linq.Expressions.Expression;
// ReSharper disable UnusedMember.Global

namespace MathCore.Evaluations;

/// <summary>Вычисление значения свойства объекта</summary>
/// <typeparam name="TObject">Тип объекта, свойство которого надо получить</typeparam>
/// <typeparam name="TValue">Тип значения свойства</typeparam>
public class PropertyValueEvaluation<TObject, TValue> : UnaryOperatorEvaluation<TObject, TValue>
{
    /// <summary>Инициализация нового вычисления значения свойства объекта</summary>
    /// <param name="PropertyName">Имя свойства</param>
    public PropertyValueEvaluation(string PropertyName) : base(e => Ex.Property(e, PropertyName)) { }

    /// <summary>Инициализация нового вычисления свойства объекта</summary>
    /// <param name="obj">Вычисление объекта, свойство которого надо получить</param>
    /// <param name="PropertyName">Имя свойства объекта</param>
    public PropertyValueEvaluation(Evaluation<TObject> obj, string PropertyName) : base(e => Ex.Property(e, PropertyName), obj) { }
}