using System;

using MathCore.Annotations;

using Ex = System.Linq.Expressions.Expression;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.Evaluations
{
    /// <summary>Вычисление функции без переменных</summary>
    /// <typeparam name="T"></typeparam>
    public class FunctionEvaluation<T> : Evulation<T>
    {
        /// <summary>Вычисляемая функция</summary>
        public Func<T> Function { get; set; }

        /// <summary>Инициализация нового вычисления функции</summary>
        public FunctionEvaluation() { }

        /// <summary>Инициализация нового вычисления функции</summary>
        /// <param name="Function">Вычисляемая функция</param>
        public FunctionEvaluation(Func<T> Function) => this.Function = Function;

        /// <inheritdoc />
        public override T GetValue() => Function();

        /// <inheritdoc />
        [NotNull]
        public override Ex GetExpression() => Ex.Call(
            Function.Target is null ? null : Ex.Constant(Function.Target),
            Function.Method);

        /// <inheritdoc />
        [NotNull]
        public override string ToString() => "f()";

        /// <summary>Оператор неявного преобразования типа функции к типу вычисления функции</summary>
        /// <param name="Function">Оборачиваемая функция</param>
        [NotNull]
        public static implicit operator FunctionEvaluation<T>(Func<T> Function) => new FunctionEvaluation<T>(Function);
    }
}