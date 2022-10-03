using System;
using System.Collections.Generic;
using MathCore.Annotations;
// ReSharper disable ConvertToAutoPropertyWhenPossible

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMember.Global

namespace MathCore.Data;

/// <summary>Структура для отображения размера данных в Байтах, кБ, МБ, ГБ, ТБ ...</summary>
public readonly struct DataLength : IFormattable, IEquatable<DataLength>
{
    //private const double __Base = 1024;
    /// <summary>Порог перехода между множителями Log(1024 * 0.7, __Base = 1024) - 70%</summary>
    private const double __Threshold = 0.948964729348844D;

    /// <summary>Множители единиц измерения</summary>
    [NotNull] private static readonly string[] __Units = { string.Empty, "k", "M", "G", "T", "P", "E", "Z", "Y" };

    /// <summary>Множители единиц измерения</summary>
    [NotNull] public static IReadOnlyList<string> Units => __Units;

    /// <summary>Преобразовать значение в представление размера с множителем</summary>
    /// <param name="value">Преобразуемое значение</param>
    /// <param name="Base">Основание</param>
    /// <param name="unit">Имя множителя единицы измерения</param>
    /// <param name="index">Индекс множителя</param>
    /// <returns>Значение X в представлении value = Base^index</returns>
    public static double Value(ulong value, double Base, out string unit, out int index)
    {
        if(value == 0)
        {
            unit  = __Units[0];
            index = 0;
            return 0;
        }

        var v_base = Math.Log(value, Base);
        var u      = (int)Math.Truncate(v_base);
        var d      = v_base - u;
        if(d > __Threshold)
        {
            u++;
            d--;
        }

        index = Math.Min(u, __Units.Length - 1);
        unit  = __Units[index];
        return Math.Pow(Base, d);
    }

    /// <summary>Преобразовать значение в представление размера с множителем</summary>
    /// <param name="value">Преобразуемое значение</param>
    /// <param name="Base">Основание</param>
    /// <param name="unit">Имя множителя единицы измерения</param>
    /// <param name="index">Индекс множителя</param>
    /// <exception cref="ArgumentOutOfRangeException">Если значение <paramref name="value"/> меньше 0</exception>
    /// <returns>Значение X в представлении value = Base^index</returns>
    public static double Value(double value, double Base, out string unit, out int index)
    {
        if(value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, "Значение должно быть больше 0");
        if(value == 0)
        {
            unit  = __Units[0];
            index = 0;
            return 0;
        }

        var v_base = Math.Log(value, Base);
        var u      = (int)Math.Truncate(v_base);
        var d      = v_base - u;
        if(d > __Threshold)
        {
            u++;
            d--;
        }

        index = Math.Min(u, __Units.Length - 1);
        unit  = __Units[index];
        return Math.Pow(Base, d);
    }

    /// <summary>Исходное значение размера</summary>
    private readonly double _Length;

    /// <summary>Основание системы счисления для выделения множителя (1024 б, либо 1000 для физических значений)</summary>
    private readonly double _Base;

    /// <summary>Количественный размер данных с множителем</summary>
    private readonly double _FormattedLength;

    /// <summary>Множитель размера данных</summary>
    private readonly string _Unit;

    /// <summary>Исходное значение размера</summary>
    public double Length => _Length;

    /// <summary>Основание системы счисления для выделения множителя (1024 б, либо 1000 для физических значений)</summary>
    public double Base => _Base;

    /// <summary>Количественный размер данных с множителем</summary>
    public double FormattedLength => _FormattedLength;

    /// <summary>Множитель размера данных</summary>
    public string Unit => _Unit;

    /// <summary>Инициализация нового экземпляра <see cref="DataLength"/></summary>
    /// <param name="Length">Количественный размер данных</param>
    /// <param name="Base">Основание системы счисления (на пример 1024 = 1кб, либо 1000 для физических значений)</param>
    public DataLength(double Length, double Base) => _FormattedLength = Value(_Length = Length, _Base = Base, out _Unit, out _);

    /// <inheritdoc />
    [NotNull] public override string ToString() => $"{_FormattedLength}{_Unit}";

    /// <summary>Представление значения в строковом виде с форматированием числовых данных</summary>
    /// <param name="format">Строка формата числовых значений</param>
    /// <returns>Строковое представление с форматированием</returns>
    [NotNull] public string ToString(string format) => $"{_FormattedLength.ToString(format)}{_Unit}";

    /// <inheritdoc />
    public string ToString(string format, IFormatProvider FormatProvider) => $"{_FormattedLength.ToString(format, FormatProvider)}{_Unit}";

    /// <inheritdoc />
    public bool Equals(DataLength other) => _Length.Equals(other._Length) && _Unit == other._Unit && _FormattedLength.Equals(other._FormattedLength) && _Base.Equals(other._Base);

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is DataLength other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hash_code = _Length.GetHashCode();
            hash_code = (hash_code * 397) ^ (_Unit != null ? _Unit.GetHashCode() : 0);
            hash_code = (hash_code * 397) ^ _FormattedLength.GetHashCode();
            hash_code = (hash_code * 397) ^ _Base.GetHashCode();
            return hash_code;
        }
    }

    /// <summary>Оператор определения равенства между двумя экземплярами <see cref="DataLength"/></summary>
    /// <returns>Истина, если все поля экземпляров совпадают</returns>
    public static bool operator ==(DataLength left, DataLength right) => left.Equals(right);

    /// <summary>Оператор определения неравенства между двумя экземплярами <see cref="DataLength"/></summary>
    /// <returns>Истина, если хотя бы одно поле экземпляров отличается</returns>
    public static bool operator !=(DataLength left, DataLength right) => !left.Equals(right);
}