using System;
using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evulations
{
    /// <summary>Вычисление конкретного значения</summary>
    /// <typeparam name="T">Тип возвращаемого значения</typeparam>
    public class ValueEvaluation<T> : Evulation<T>
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
        public override Ex GetExpression() => Value.ToExpression();

        /// <inheritdoc />
        public override string ToString() => Value.ToString();

        /// <summary>ОПератор неявного преобразования типа значения в тип вычисления этого значения</summary>
        /// <param name="Value">Оборачиваемое значение</param>
        public static implicit operator ValueEvaluation<T>(T Value) => new ValueEvaluation<T>(Value);
    }

    /// <summary>Именованное вычисление конкретного значения</summary>
    /// <typeparam name="T">Тип возвращаемого значения</typeparam>
    public class NamedValueEvaluation<T> : ValueEvaluation<T>
    {
        /// <summary>Имя вычисления</summary>
        public string Name { get; set; }

        /// <summary>Признак того, что данное вычисление является именованным параметром</summary>
        public bool IsParameter { get; set; }

        /// <summary>Инициализация нового именованного вычисления конкретного значения</summary>
        public NamedValueEvaluation() { }

        /// <summary>Инициализация нового именованного вычисления конкретного значения</summary>
        /// <param name="value">Возвращаемое значение</param>
        public NamedValueEvaluation(T value) : base(value) { }

        /// <summary>Инициализация нового именованного вычисления конкретного значения</summary>
        /// <param name="value">Возвращаемое значение</param>
        /// <param name="name">Имя вычисления</param>
        public NamedValueEvaluation(T value, string name) : base(value) => Name = name;

        /// <summary>Если вычисление является параметром, то возвращается выражение параметра, иначе возвращается вычисление значения</summary>
        /// <returns>Выражение, соответствующее данному вычислению</returns>
        public override Ex GetExpression() => IsParameter
            ? Ex.Parameter(typeof(T), Name)
            : base.GetExpression();

        /// <inheritdoc />
        public override string ToString() => IsParameter
            ? Name.IsNullOrWhiteSpace()
                ? $"({typeof(T)})p"
                : Name
            : Name.IsNullOrWhiteSpace()
                ? Value.ToString()
                : $"{Name}={Value}";
    }
}