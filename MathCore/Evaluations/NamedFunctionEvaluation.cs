using System;

namespace MathCore.Evaluations
{
    /// <summary>Именованное вычисление функции</summary>
    /// <typeparam name="T">Тип значения функции</typeparam>
    public class NamedFunctionEvaluation<T> : FunctionEvaluation<T>
    {
        /// <summary>Имя функции</summary>
        public string Name { get; set; }

        /// <summary>Инициализация нового вычисления значения функции</summary>
        public NamedFunctionEvaluation() { }

        /// <summary>Инициализация нового вычисления значения функции</summary>
        /// <param name="Function">Вычисляемая функция</param>
        public NamedFunctionEvaluation(Func<T> Function) : base(Function) { }

        /// <summary>Инициализация нового вычисления значения функции</summary>
        /// <param name="Function">Вычисляемая функция</param>
        /// <param name="Name">Имя функции</param>
        public NamedFunctionEvaluation(Func<T> Function, string Name) : base(Function) => this.Name = Name;

        /// <inheritdoc />
        public override string ToString() => $"{(Name.IsNullOrWhiteSpace() ? "_" : Name)}()";
    }
}