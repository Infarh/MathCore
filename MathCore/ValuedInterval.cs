#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

namespace MathCore;

/// <summary>Интервал вещественных значений двойной точности</summary>
[Serializable]
public readonly struct ValuedInterval<T> : IComparable<double>, IFormattable
{
    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Метод сравнения двух интервалов</summary>
    /// <param name="a">Первый сравниваемый интервал</param>
    /// <param name="b">Второй сравниваемый интервал</param>
    /// <returns>1 - если первый интервал больше второго, -1 - если первый интервал меньше второго, 0 - если интервалы равны</returns>
    public static int Comparer_Length(ValuedInterval<T> a, ValuedInterval<T> b)
    {
        var l1 = a.Length;
        var l2 = b.Length;
        return l1 > l2 ? 1 : (l1 < l2 ? -1 : 0);
    }

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Включена ли нижняя граница интервала?</summary>
    private readonly bool _MinInclude;
    /// <summary>Включена ли верхняя граница интервала?</summary>
    private readonly bool _MaxInclude;
    /// <summary>Нижняя граница интервала</summary>
    private readonly double _Min;
    /// <summary>Верхняя граница интервала</summary>
    private readonly double _Max;

    private readonly T _Value;

    /* -------------------------------------------------------------------------------------------- */

    #region Свойства

    /// <summary>Включена ли нижняя граница интервала?</summary>
    public bool MinInclude { get => _MinInclude; init => _MinInclude = value; }

    /// <summary>Включена ли верхняя граница интервала?</summary>
    public bool MaxInclude { get => _MaxInclude; init => _MaxInclude = value; }

    /// <summary>Нижняя граница интервала</summary>
    public double Min { get => _Min; init => _Min = value; }

    /// <summary>Верхняя граница интервала</summary>
    public double Max { get => _Max; init => _Max = value; }

    /// <summary>Длина интервала</summary>
    public double Length => _Max - _Min;

    /// <summary>Середина интервала</summary>
    public double Middle => (_Min + _Max) / 2;

    public T Value => _Value;

    public Interval Interval => new(_Min, _MinInclude, _Max, _MaxInclude);

    #endregion

    /* -------------------------------------------------------------------------------------------- */

    #region Конструкторы

    /// <summary>Интервал</summary>
    /// <param name="Min">Нижняя граница интервала</param>
    /// <param name="Max">Верхняя граница интервала</param>
    /// <param name="Value">Значение</param>
    public ValuedInterval(double Min, double Max, T Value) : this(Min, true, Max, true, Value) { }

    /// <summary>Интервал</summary>
    /// <param name="Min">Нижняя граница интервала</param>
    /// <param name="Max">Верхняя граница интервала</param>
    /// <param name="IncludeLimits">Включать пределы?</param>
    /// <param name="Value">Значение</param>
    public ValuedInterval(double Min, double Max, bool IncludeLimits, T Value) : this(Min, IncludeLimits, Max, IncludeLimits, Value) { }

    /// <summary>Интервал</summary>
    /// <param name="Min">Нижняя граница интервала</param>
    /// <param name="MinInclude">Включена ли нижняя граница интервала?</param>
    /// <param name="Max">Верхняя граница интервала</param>
    /// <param name="MaxInclude">Включена ли верхняя граница интервала</param>
    /// <param name="Value">Значение</param>
    public ValuedInterval(double Min, bool MinInclude, double Max, bool MaxInclude, T Value)
    {
        _Min = Min;
        _Max = Max;
        _MinInclude = MinInclude;
        _MaxInclude = MaxInclude;
        _Value = Value;
    }

    public ValuedInterval(Interval Interval, T Value)
    {
        Interval.Deconstruct(out _Min, out _MinInclude, out _Max, out _MaxInclude);
        _Value = Value;
    }

    #endregion

    /* -------------------------------------------------------------------------------------------- */

    #region Интервальные функции

    public ValuedInterval<T> IncludeMax(bool Include) => new(_Min, _MinInclude, _Max, Include, _Value);
    public ValuedInterval<T> IncludeMin(bool Include) => new(_Min, Include, _Max, _MaxInclude, _Value);
    public ValuedInterval<T> Include(bool IncludeMin, bool IncludeMax) => new(_Min, IncludeMin, _Max, IncludeMax, _Value);
    public ValuedInterval<T> Include(bool Include) => new(_Min, Include, _Max, Include, _Value);

    public ValuedInterval<T> SetMin(double value) => new(value, _MinInclude, _Max, _MaxInclude, _Value);
    public ValuedInterval<T> SetMin(double value, bool IncludeMin) => new(value, IncludeMin, _Max, _MaxInclude, _Value);
    public ValuedInterval<T> SetMax(double value) => new(_Min, _MinInclude, value, _MaxInclude, _Value);
    public ValuedInterval<T> SetMax(double value, bool IncludeMax) => new(_Min, _MinInclude, value, IncludeMax, _Value);

    public ValuedInterval<T> SetValue(T value) => new(_Min, _MinInclude, _Max, _MaxInclude, value);

    public void Deconstruct(out double min, out double max)
    {
        min = _Min;
        max = _Max;
    }

    public void Deconstruct(out double min, out double max, out T value)
    {
        min = _Min;
        max = _Max;
        value = _Value;
    }

    public void Deconstruct(out double min, out bool IncludeMin, out double max, out bool IncludeMax, out T value)
    {
        min = _Min;
        max = _Max;
        value = _Value;
        IncludeMin = _MinInclude;
        IncludeMax = _MaxInclude;
    }

    ///// <summary>Проверка на вхождение в интервал</summary>
    ///// <param name="X">Проверяемая величина</param>
    ///// <returns></returns>
    //public bool Check(double X)
    //{
    //    return (MinInclude && Math.Abs(X - Min) < double.Epsilon)
    //        || (MaxInclude && Math.Abs(X - Max) < double.Epsilon)
    //        || (X > Min && X < Max);
    //}

    public bool Check(double X, double MinOffset, double MaxOffset)
    {
        var min = _Min + MinOffset;
        var max = _Max + MaxOffset;

        return (_MinInclude && Math.Abs(X - min) < double.Epsilon)
            || (_MaxInclude && Math.Abs(X - max) < double.Epsilon)
            || (X > min && X < max);
    }

    /// <summary>Проверка на вхождение значения в интервал</summary>
    /// <param name="value">Проверяемое значение</param>
    /// <returns>Истина, если значение входит в интервал</returns>
    [DST]
    public bool Check(double value) =>
        (_MinInclude && _Min.CompareTo(value) == 0) || 
        (_MaxInclude && _Max.CompareTo(value) == 0) || 
        (value.CompareTo(_Min) > 0 && value.CompareTo(_Max) < 0);

    public bool Check(double X, double Offset) => Check(X, Offset, -Offset);

    public bool IsExclude(ValuedInterval<T> I) => !IsInclude(I);
    public bool IsExclude((double Min, double Max) I) => !IsInclude(I);
    public bool IsExclude((int Min, double Max) I) => !IsInclude(I);
    public bool IsExclude((double Min, int Max) I) => !IsInclude(I);

    public bool IsInclude(ValuedInterval<T> I) =>
        Check(I._MinInclude ? I._Min : I._Min + double.Epsilon) && 
        Check(I._MaxInclude ? I._Max : I._Max - double.Epsilon);

    public bool IsInclude((double Min, double Max) I) =>
        Check(I.Min) && 
        Check(I.Max);

    public bool IsInclude((int Min, double Max) I) =>
        Check(I.Min) && 
        Check(I.Max);

    public bool IsInclude((double Min, int Max) I) =>
        Check(I.Min) && 
        Check(I.Max);

    public bool IsIntersect(ValuedInterval<T> I)
    {
        if (Math.Abs(I.Min - Min) < double.Epsilon && Math.Abs(I._Max - _Max) < double.Epsilon) return true;

        var min_include = Check(I._Min) || Math.Abs(I._Min - _Max) > double.Epsilon;
        var max_include = Check(I._Max) || Math.Abs(I._Max - _Min) > double.Epsilon;

        return min_include || max_include;
    }

    public bool IsIntersect((double Min, double Max) I)
    {
        if (Math.Abs(I.Min - Min) < double.Epsilon && Math.Abs(I.Max - _Max) < double.Epsilon) return true;

        var min_include = Check(I.Min) || Math.Abs(I.Min - _Max) > double.Epsilon;
        var max_include = Check(I.Max) || Math.Abs(I.Max - _Min) > double.Epsilon;

        return min_include || max_include;
    }

    #endregion

    /// <inheritdoc />
    public int CompareTo(double x) =>
        (x > _Min && x < _Max) ||
        (_MinInclude && Math.Abs(_Min - x) < double.Epsilon) || 
        (_MaxInclude && Math.Abs(_Max - x) < double.Epsilon)
            ? 0
            : (x < _Min ? -1 : 1);

    #region Цыклы

    public void For(int samples, Action<double> Do)
    {
        var len = Length;
        var min = _Min;
        if (!_MaxInclude) len -= double.Epsilon;
        if (!_MinInclude)
        {
            len -= double.Epsilon;
            min += double.Epsilon;
        }
        var dx = len / (samples - 1);
        for (var i = 0; i < samples; i++)
            Do(min + i * dx);
    }

    public void For(int samples, Action<int, double> Do)
    {
        var len = Length;
        var min = _Min;
        if (!_MaxInclude) len -= double.Epsilon;
        if (!_MinInclude)
        {
            len -= double.Epsilon;
            min += double.Epsilon;
        }
        var dx = len / (samples - 1);
        for (var i = 0; i < samples; i++)
            Do(i, min + i * dx);
    }

    public void WhileInInterval(double step, Action<double> Do)
    {
        var min = Math.Min(_Max, _Min);
        step = _Max < _Min && step > 0 ? -step : step;
        var x = min + (_MinInclude ? 0 : double.Epsilon);
        while (Check(x)) Do(x += step);
    }

    #endregion

    //public double[] GetValues(int Count)
    //{
    //    var result = new double[Count];
    //    For(Count, (i, x) => result[i] = x);
    //    return result;
    //}

    public IEnumerable<double> GetValues(int Count)
    {
        var len = Length;
        var min = _Min;
        if (!_MaxInclude) len -= double.Epsilon;
        if (!_MinInclude)
        {
            len -= double.Epsilon;
            min += double.Epsilon;
        }
        var dx = len / (Count - 1);
        for (var i = 0; i < Count; i++)
            yield return min + i * dx;
    }

    public ValuedInterval<T> GetInvertedInterval() => new(_Max, _MaxInclude, _Min, _MinInclude, _Value);

    public IEnumerable<double> GetValues(double Step)
    {
        var position = _Min;
        do
        {
            yield return position;
            position += Step;
        } while ((_MaxInclude && position <= _Max) || (!_MaxInclude && position < _Max));
    }

    public IEnumerable<ValuedInterval<T>> GetSubIntervals(int Count)
    {
        var last = _MinInclude ? _Min : _Min - double.Epsilon;
        var value = _Value;
        return GetValues(Count)
           .Skip(1)
           .ForeachLazyLast(v => last = v)
           .Select(v => new ValuedInterval<T>(last, v, true, value));
    }

    public override string ToString() => new StringBuilder()
       .Append(_MinInclude ? "[" : "(")
       .Append(_Min)
       .Append(";")
       .Append(_Max)
       .Append(_MaxInclude ? "]" : ")")
       .Append(":")
       .Append(_Value)
       .ToString();

    public string ToString(string Format) => string.Format(
        "{0}{2};{3}{1}",
        _MinInclude ? "[" : "(",
        _MaxInclude ? "]" : ")",
        _Min.ToString(Format),
        _Max.ToString(Format));

    /// <summary>Форматирует значение текущего экземпляра с использованием заданного формата.</summary>
    /// <returns>Объект <see cref="T:System.String"/> содержит значение текущего экземпляра в заданном формате.</returns>
    /// <param name="Format">
    /// Объект <see cref="T:System.String"/>, задающий используемый формат.— или — 
    /// Значение null для использования формата по умолчанию, определенного для типа реализации 
    /// <see cref="T:System.IFormattable"/>. 
    /// </param>
    /// <param name="FormatProvider">
    /// Объект <see cref="T:System.IFormatProvider"/>, используемый для форматирования значения.— или — 
    /// Значение null для получения сведений о форматировании чисел на основе текущего значения параметра языкового 
    /// стандарта операционной системы. 
    /// </param>
    /// <filterpriority>2</filterpriority>
    public string ToString(string Format, IFormatProvider FormatProvider) => string.Format(
        "{0}{2};{3}{1}",
        _MinInclude ? "[" : "(",
        _MaxInclude ? "]" : ")",
        _Min.ToString(Format, FormatProvider),
        _Max.ToString(Format, FormatProvider));

    /* ------------------------------------------------------------------------------------------ */

    public static implicit operator double(ValuedInterval<T> I) => I.Length;

    public static explicit operator ValuedInterval<T>(double V) => new(0, true, V, true, default!);

    public static ValuedInterval<T> operator +(ValuedInterval<T> I, double x) => new(I._Min + x, I._MinInclude, I._Max + x, I._MaxInclude, I._Value);

    public static ValuedInterval<T> operator -(ValuedInterval<T> I, double x) => new(I._Min - x, I._MinInclude, I._Max - x, I._MaxInclude, I._Value);

    public static ValuedInterval<T> operator *(ValuedInterval<T> I, double x) => new(I._Min * x, I._MinInclude, I._Max * x, I._MaxInclude, I._Value);

    public static ValuedInterval<T> operator /(ValuedInterval<T> I, double x) => new(I._Min / x, I._MinInclude, I._Max / x, I._MaxInclude, I._Value);

    /* ------------------------------------------------------------------------------------------ */
}