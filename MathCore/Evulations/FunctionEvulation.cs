using System;
using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evulations
{
    /// <summary>Вычисление функции без переменных</summary>
    /// <typeparam name="T"></typeparam>
    public class FunctionEvulation<T> : Evulation<T>
    {
        /// <summary>Вычисляемая функция</summary>
        public Func<T> Function { get; set; }

        /// <summary>Инициализация нового вычисления функции</summary>
        public FunctionEvulation() { }

        /// <summary>Инициализация нового вычисления функции</summary>
        /// <param name="Function">Вычисляемая функция</param>
        public FunctionEvulation(Func<T> Function) { this.Function = Function; }

        /// <inheritdoc />
        public override T GetValue() => Function();

        /// <inheritdoc />
        public override Ex GetExpression() => Ex.Call(
            Function.Target == null ? null : Ex.Constant(Function.Target),
            Function.Method);

        /// <inheritdoc />
        public override string ToString() => "f()";

        /// <summary>Оператор неявного преобразования типа функции к типу вычисления функции</summary>
        /// <param name="Function">Оборачиваемая функция</param>
        public static implicit operator FunctionEvulation<T>(Func<T> Function) => new FunctionEvulation<T>(Function);
    }

    /// <summary>Именованное вычисление функции</summary>
    /// <typeparam name="T">Тип значения функции</typeparam>
    public class NamedFunctionEvulation<T> : FunctionEvulation<T>
    {
        /// <summary>Имя функции</summary>
        public string Name { get; set; }

        /// <summary>Инициализация нового вычисления значения функции</summary>
        public NamedFunctionEvulation() { }

        /// <summary>Инициализация нового вычисления значения функции</summary>
        /// <param name="Function">Вычисляемая функция</param>
        public NamedFunctionEvulation(Func<T> Function) : base(Function) { }

        /// <summary>Инициализация нового вычисления значения функции</summary>
        /// <param name="Function">Вычисляемая функция</param>
        /// <param name="Name">Имя функции</param>
        public NamedFunctionEvulation(Func<T> Function, string Name) : base(Function) { this.Name = Name; }

        /// <inheritdoc />
        public override string ToString() => $"{(Name.IsNullOrWhiteSpace() ? "_" : Name)}()";
    }
}