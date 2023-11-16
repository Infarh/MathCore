#nullable enable
using System.Collections;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable ClassCanBeSealed.Global

namespace MathCore.MathParser;

/// <summary>Коллекция функционалов</summary>
public class FunctionalsCollection : IEnumerable<Functional>
{
    /// <summary>Список функционалов</summary>
    private readonly List<Functional> _Operators = new();

    /// <summary>Количество функционалов в коллекции</summary>
    public int Count => _Operators.Count;

    /// <summary>Добавить функционал в коллекцию</summary>
    /// <param name="Operator">Добавляемый функционал</param>
    /// <returns>Истина, если добавление прошло успешно</returns>
    public bool Add(Functional Operator)
    {
        if(_Operators.Exists(o => o.Name == Operator.Name)) return false;
        _Operators.Add(Operator);
        return true;
    }

    /// <inheritdoc />
    IEnumerator<Functional> IEnumerable<Functional>.GetEnumerator() => _Operators.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Operators).GetEnumerator();

    /// <inheritdoc />
    public override string ToString() => $"Complex operator collection > count = {Count}";
}