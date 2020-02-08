using System;
using MathCore.Annotations;
using Ex = System.Linq.Expressions.Expression;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.Evaluations
{
    /// <summary>Вычисление конкретного значения</summary>
    /// <typeparam name="T">Тип возвращаемого значения</typeparam>
    public class ValueEvaluation<T> : Evaluation<T>
    {
        /// <summary>Возвращаемое значение</summary>
        public T Value { get; set; }

        /// <summary>Инициализация нового вычисления конкретного значения</summary>
        public ValueEvaluation() { }

        /// <summary>Инициализация нового вычисления конкретного значения</summary>
        /// <param name="value">Возвращаемое значение</param>
        public ValueEvaluation(T value) => Value = value;

        /// <inheritdoc />
        public override T GetValue() => Value;

        /// <inheritdoc />
        [NotNull]
        public override Ex GetExpression() => Value.ToExpression();

        /// <inheritdoc />
        public override string ToString() => Value.ToString();

        /// <summary>Оператор неявного преобразования типа значения в тип вычисления этого значения</summary>
        /// <param name="Value">Оборачиваемое значение</param>
        [NotNull]
        public static implicit operator ValueEvaluation<T>(T Value) => new ValueEvaluation<T>(Value);
    }
}