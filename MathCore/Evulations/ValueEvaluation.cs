using System;
using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evulations
{
    /// <summary>¬ычисление конкретного значени€</summary>
    /// <typeparam name="T">“ип возвращаемого значени€</typeparam>
    public class ValueEvaluation<T> : Evulation<T>
    {
        /// <summary>¬озвращаемое значение</summary>
        public T Value { get; set; }

        /// <summary>»нициализаци€ нового вычислени€ конкретного значени€</summary>
        public ValueEvaluation() { }

        /// <summary>»нициализаци€ нового вычислени€ конкретного значени€</summary>
        /// <param name="value">¬озвращаемое значение</param>
        public ValueEvaluation(T value) => Value = value;

        /// <inheritdoc />
        public override T GetValue() => Value;

        /// <inheritdoc />
        public override Ex GetExpression() => Value.ToExpression();

        /// <inheritdoc />
        public override string ToString() => Value.ToString();

        /// <summary>ќѕератор не€вного преобразовани€ типа значени€ в тип вычислени€ этого значени€</summary>
        /// <param name="Value">ќборачиваемое значение</param>
        public static implicit operator ValueEvaluation<T>(T Value) => new ValueEvaluation<T>(Value);
    }

    /// <summary>»менованное вычисление конкретного значени€</summary>
    /// <typeparam name="T">“ип возвращаемого значени€</typeparam>
    public class NamedValueEvaluation<T> : ValueEvaluation<T>
    {
        /// <summary>»м€ вычислени€</summary>
        public string Name { get; set; }

        /// <summary>ѕризнак того, что данное вычисление €вл€етс€ именованным параметром</summary>
        public bool IsParameter { get; set; }

        /// <summary>»нициализаци€ нового именованного вычислени€ конкретного значени€</summary>
        public NamedValueEvaluation() { }

        /// <summary>»нициализаци€ нового именованного вычислени€ конкретного значени€</summary>
        /// <param name="value">¬озвращаемое значение</param>
        public NamedValueEvaluation(T value) : base(value) { }

        /// <summary>»нициализаци€ нового именованного вычислени€ конкретного значени€</summary>
        /// <param name="value">¬озвращаемое значение</param>
        /// <param name="name">»м€ вычислени€</param>
        public NamedValueEvaluation(T value, string name) : base(value) => Name = name;

        /// <summary>≈сли вычисление €вл€етс€ параметром, то возвращаетс€ выражение параметра, иначе возвращаетс€ вычисление значени€</summary>
        /// <returns>¬ыражение, соответствующее данному вычислению</returns>
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