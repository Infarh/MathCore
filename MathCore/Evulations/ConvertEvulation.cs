using System;
using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evulations
{
    /// <summary>Вычисление преобразования типов</summary>
    /// <typeparam name="TInput">Тип входного значения</typeparam>
    /// <typeparam name="TOutput">Тип выходного значения</typeparam>
    public class ConvertEvulation<TInput, TOutput> : Evulation<TOutput>
    {
        /// <summary>Вычисление входного значения</summary>
        public Evulation<TInput> InputEvulation { get; set; }

        /// <summary>Функция-преобразователь типов входного в выходное значение</summary>
        public Func<TInput, TOutput> Converter { get; set; }

        /// <summary>Инициализация нового вычисления преобразования типов</summary>
        public ConvertEvulation() { }

        /// <summary>Инициализация нового вычисления преобразования типов</summary>
        /// <param name="Converter">Метод преобразования входного значения в выходное</param>
        public ConvertEvulation(Func<TInput, TOutput> Converter) => this.Converter = Converter;

        /// <summary>Инициализация нового вычисления преобразования типов</summary>
        /// <param name="InputEvulation">Вычисление входного значения</param>
        /// <param name="Converter">Метод преобразования входного значения в выходное</param>
        public ConvertEvulation(Evulation<TInput> InputEvulation, Func<TInput, TOutput> Converter) : this(Converter) => this.InputEvulation = InputEvulation;

        /// <inheritdoc />
        public override TOutput GetValue() => Converter(InputEvulation.GetValue());

        /// <inheritdoc />
        public override Ex GetExpression() => Ex.Call(Converter.Target?.ToExpression(), Converter.Method, InputEvulation.GetExpression());
    }
}