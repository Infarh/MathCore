using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.CSV;

/// <summary>Строка данных</summary>
public readonly struct CSVQueryRow : IEnumerable<KeyValuePair<string, string>>
{
    /// <summary>Массив элементов данных строки</summary>
    private readonly string[] _Items;

    /// <summary>Словарь столбцов</summary>
    private readonly IDictionary<string, int> _Header;

    private readonly CultureInfo _Culture;

    /// <summary>Число элементов данных строки</summary>
    public int ItemsCount => _Items.Length;

    /// <summary>Индекс (номер) строки (отсчёт от 0)</summary>
    public int Index { get; }

    /// <summary>Начальное положение в источнике</summary>
    public long StartPos { get; }

    /// <summary>Конечное положение в источнике</summary>
    public long EndPos { get; }

    public string SourceLine { get; }

    /// <summary>Заголовки строки</summary>
    [NotNull] public IReadOnlyDictionary<string, int> Headers => new ReadOnlyDictionary<string, int>(_Header);

    /// <summary>Строковое значение по указанному индексу колонки в строке данных</summary>
    /// <param name="ValueIndex">Индекс колонки</param>
    /// <returns>Строковое значение</returns>
    public Value this[int ValueIndex] => new(ValueIndex >= 0 ? _Items[ValueIndex] : _Items[_Items.Length + ValueIndex], _Culture);

    public ref string Value(int ValueIndex) => ref ValueIndex >= 0
        ? ref _Items[ValueIndex]
        : ref _Items[_Items.Length + ValueIndex];

    /// <summary>Строковое значение по имени колонки</summary>
    /// <param name="ValueName">Имя колонки в массиве данных</param>
    /// <returns>Строковое значение</returns>
    public Value this[[NotNull] string ValueName] => new(_Items[_Header[ValueName]], _Culture);

    public ref string Value([NotNull] string ValueName) => ref _Items[_Header[ValueName]];

    /// <summary>Инициализация нового экземпляра строки данных <see cref="CSVQueryRow"/></summary>
    /// <param name="SourceLine">Исходная строка</param>
    /// <param name="Index">Индекс строки</param>
    /// <param name="Items">Массив строковых значений</param>
    /// <param name="Header">Словарь заголовков</param>
    /// <param name="StartPos">Начальное положение в источнике</param>
    /// <param name="EndPos">Конечное положение в источнике</param>
    /// <param name="Culture">Сведения о культуре для выполнения преобразования данных</param>
    public CSVQueryRow(
        string SourceLine, 
        int Index, 
        string[] Items, 
        IDictionary<string, int> Header, 
        long StartPos, long EndPos, 
        CultureInfo Culture)
    {
        this.SourceLine = SourceLine;
        this.Index      = Index;
        this.StartPos   = StartPos;
        this.EndPos     = EndPos;
        _Items          = Items;
        _Header         = Header;
        _Culture        = Culture;
    }

    /// <summary>Представление значения в указанном типе</summary>
    /// <typeparam name="T">Требуемый тип значения</typeparam>
    /// <param name="ValueIndex">Индекс колонки</param>
    /// <returns>Представление значения в указанном типе</returns>
    public T ValueAs<T>(int ValueIndex) => (T)Convert.ChangeType(Value(ValueIndex), typeof(T));

    /// <summary>Представление значения в указанном типе</summary>
    /// <typeparam name="T">Требуемый тип значения</typeparam>
    /// <param name="ValueName">Имя столбца</param>
    /// <returns>Представление значения в указанном типе</returns>
    public T ValueAs<T>([NotNull] string ValueName) => ValueAs<T>(_Header[ValueName]);

    /// <summary>Первое строковое значение</summary>
    public ref string FirstValue() => ref Value(0);

    /// <summary>Последнее строковое значение</summary>
    public ref string LastValue() => ref Value(-1);

    /// <summary>Первое значение</summary>
    public T FirstValue<T>() => ValueAs<T>(0);

    /// <summary>Последнее значение</summary>
    public T LastValue<T>() => ValueAs<T>(-1);

    [NotNull]
    public IDictionary<string, string> ToDictionary()
    {
        var values = _Items;
        return _Header.ToDictionary(h => h.Key, h => values[h.Value]);
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        foreach (var (header, index) in _Header.OrderBy(h => h.Value).ThenBy(h => h.Key))
            yield return new(header, _Items[index]);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}