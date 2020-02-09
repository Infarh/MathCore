using System;
using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evaluations
{
    /// <summary>Вычисление преобразования типов</summary>
    /// <typeparam name="TInput">Тип входного значения</typeparam>
    /// <typeparam name="TOutput">Тип выходного значения</typeparam>
    public class ConvertEvaluation<TInput, TOutput> : Evaluation<TOutput>
    {
        /// <summary>Вычисление входного значения</summary>
        public Evaluation<TInput> InputEvaluation { get; set; }

        /// <summary>Функция-преобразователь типов входного в выходное значение</summary>
        public Func<TInput, TOutput> Converter { get; set; }

        /// <summary>Инициализация нового вычисления преобразования типов</summary>
        public ConvertEvaluation() { }

        /// <summary>Инициализация нового вычисления преобразования типов</summary>
        /// <param name="Converter">Метод преобразования входного значения в выходное</param>
        public ConvertEvaluation(Func<TInput, TOutput> Converter) => this.Converter = Converter;

        /// <summary>Инициализация нового вычисления преобразования типов</summary>
        /// <param name="InputEvaluation">Вычисление входного значения</param>
        /// <param name="Converter">Метод преобразования входного значения в выходное</param>
        public ConvertEvaluation(Evaluation<TInput> InputEvaluation, Func<TInput, TOutput> Converter) : this(Converter) => this.InputEvaluation = InputEvaluation;

        /// <inheritdoc />
        public override TOutput GetValue() => Converter(InputEvaluation.GetValue());

        /// <inheritdoc />
        public override Ex GetExpression() => Ex.Call(Converter.Target?.ToExpression(), Converter.Method, InputEvaluation.GetExpression());
    }
}