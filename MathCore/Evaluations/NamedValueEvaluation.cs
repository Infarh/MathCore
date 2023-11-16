using System.Linq.Expressions;

namespace MathCore.Evaluations;

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
    public override Expression GetExpression() => IsParameter
        ? Expression.Parameter(typeof(T), Name)
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