#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable NotAccessedField.Local

namespace MathCore.MathParser;

/// <summary>Коллекция констант</summary>
[System.Diagnostics.DebuggerDisplay("Количество зафиксированных констант = {" + nameof(Count) + "}"), DST]
public sealed class ConstantsCollection : IEnumerable<ExpressionVariable>
{
    /// <summary>Элементы коллекции</summary>
    private readonly List<ExpressionVariable> _Items = new();

    /// <summary>Количество элементов коллекции</summary>
    public int Count => _Items.Count;

    /// <summary>Итератор констант по имени</summary>
    /// <param name="Name">Имя константы</param>
    /// <returns>Константа с указанным именем</returns>
    public ExpressionVariable this[string Name] => Name.NotNull().Length == 0
        ? throw new ArgumentOutOfRangeException(nameof(Name))
        : _Items.Find(v => v.Name == Name)
        ?? throw new ArgumentException($"Константа с именем {Name} не найдена");

    /// <summary>Добавить элемент в коллекцию</summary>
    /// <param name="Constant">Добавляемое значение, как константа</param>
    public bool Add(ExpressionVariable Constant)
    {
        if(_Items.Contains(v => v.Name == Constant.Name)) return false;
        Constant.IsConstant = true;
        _Items.Add(Constant);
        return true;
    }

    /// <summary>Получить имена констант коллекции</summary>
    /// <returns>Перечисление имён констант коллекции</returns>
    public IEnumerable<string> GetNames() => _Items.Select(v => v.Name);

    /// <summary>Получить перечислитеь констант коллекции</summary>
    /// <returns>Перечислитель констант</returns>
    IEnumerator<ExpressionVariable> IEnumerable<ExpressionVariable>.GetEnumerator() => _Items.GetEnumerator();

    /// <summary>Получить перечислитеь констант коллекции</summary>
    /// <returns>Перечислитель констант</returns>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<ExpressionVariable>)this).GetEnumerator();
}