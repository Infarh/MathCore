﻿#nullable enable
using System.ComponentModel;
using System.Text;

// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

namespace MathCore;

/// <summary>Интервал сравнимых величин</summary>
/// <typeparam name="T">Тип сравнимых величин</typeparam>
/// <remarks>Интервал</remarks>
/// <param name="Min">Нижняя граница интервала</param>
/// <param name="MinInclude">Включена ли нижняя граница интервала?</param>
/// <param name="Max">Верхняя граница интервала</param>
/// <param name="MaxInclude">Включена ли верхняя граница интервала</param>
[method: DST]
/// <summary>Интервал сравнимых величин</summary>
/// <typeparam name="T">Тип сравнимых величин</typeparam>
public readonly struct Interval<T>(T Min, bool MinInclude, T Max, bool MaxInclude) : IEquatable<Interval<T>>, IEquatable<(T Min, T Max)>, ICloneable<Interval<T>> 
    where T : IComparable<T>
{
    /* ------------------------------------------------------------------------------------------ */

    #region Конструкторы

    /// <summary>Интервал</summary>
    /// <param name="Min">Нижняя граница интервала</param>
    /// <param name="Max">Верхняя граница интервала</param>
    [DST]
    public Interval(T Min, T Max) : this(Min, true, Max, true) { }

    /// <summary>Интервал</summary>
    /// <param name="Min">Нижняя граница интервала</param>
    /// <param name="Max">Верхняя граница интервала</param>
    /// <param name="IncludeLimits">Включать пределы? (default:true)</param>
    [DST]
    public Interval(T Min, T Max, bool IncludeLimits) : this(Min, IncludeLimits, Max, IncludeLimits) { }

    #endregion

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Определение нового интервала</summary>
    /// <param name="Min">Минимальное значение</param>
    /// <param name="Max">Максимальное значение</param>
    /// <param name="IncludeLimits">Границы интервала входят?</param>
    /// <returns>Новый интервал в указанных границах</returns>
    public static Interval<T> Value(T Min, T Max, bool IncludeLimits = true) => new(Min, Max, IncludeLimits);

    /* ------------------------------------------------------------------------------------------ */

    #region Поля

    /// <summary>Включена ли нижняя граница интервала?</summary>
    private readonly bool _MinInclude = MinInclude;
    /// <summary>Включена ли верхняя граница интервала?</summary>
    private readonly bool _MaxInclude = MaxInclude;
    /// <summary>Нижняя граница интервала</summary>
    private readonly T _Min = Min;
    /// <summary>Верхняя граница интервала</summary>
    private readonly T _Max = Max;

    #endregion

    /* ------------------------------------------------------------------------------------------ */

    #region Свойства

    /// <summary>Включена ли нижняя граница интервала?</summary>
    public bool MinInclude { get => _MinInclude; init => _MinInclude = value; }

    /// <summary>Включена ли верхняя граница интервала?</summary>
    public bool MaxInclude { get => _MaxInclude; init => _MaxInclude = value; }

    /// <summary>Нижняя граница интервала</summary>
    public T Min { get => _Min; init => _Min = value; }

    /// <summary>Верхняя граница интервала</summary>
    public T Max { get => _Max; init => _Max = value; }

    #endregion

    /* ------------------------------------------------------------------------------------------ */

    public Interval<T> IncludeMax(bool Include) => new(_Min, _MinInclude, _Max, Include);
    public Interval<T> IncludeMin(bool Include) => new(_Min, Include, _Max, _MaxInclude);
    public Interval<T> Include(bool IncludeMin, bool IncludeMax) => new(_Min, IncludeMin, _Max, IncludeMax);
    public Interval<T> Include(bool Include) => new(_Min, Include, _Max, Include);

    public Interval<T> SetMin(T Value) => new(Value, _MinInclude, _Max, _MaxInclude);
    public Interval<T> SetMin(T Value, bool IncludeMin) => new(Value, IncludeMin, _Max, _MaxInclude);
    public Interval<T> SetMax(T Value) => new(_Min, _MinInclude, Value, _MaxInclude);
    public Interval<T> SetMax(T Value, bool IncludeMax) => new(_Min, _MinInclude, Value, IncludeMax);

    /// <summary>
    /// Метод возвращает указанное значение, если оно находится внутри интервала,
    /// либо соответствующую его границу, если значение входит за его пределы
    /// </summary>
    /// <param name="Value">Нормализуемое значение</param>
    /// <returns>
    /// Значение, переданное в качестве аргумента, если оно входит в интервал,
    /// иначе соответствующая граница интервала
    /// </returns>
    [DST]
    public T Normalize(T Value) => Value.CompareTo(_Max) > 0 ? _Max : (Value.CompareTo(_Min) < 0 ? _Min : Value);

    /// <summary>Замена значения ссылки на значение границы интервала, если значение не входит в интервал</summary>
    /// <param name="Value">Проверяемое значение</param>
    public void Normalize(ref T Value)
    {
        if (Value.CompareTo(_Max) > 0)
            Value = _Max;
        else if (Value.CompareTo(_Min) < 0)
            Value = _Min;
    }

    /// <summary>Проверка на вхождение значения в интервал</summary>
    /// <param name="Value">Проверяемое значение</param>
    /// <returns>Истина, если значение входит в интервал</returns>
    [DST]
    public bool Check(T Value) =>
        (_MinInclude && _Min.CompareTo(Value) == 0)
        || (_MaxInclude && _Max.CompareTo(Value) == 0)
        || (Value.CompareTo(_Min) > 0 && Value.CompareTo(_Max) < 0);

    /// <summary>Проверка - входит ли указанный интервал в текущий</summary>
    /// <param name="I">Проверяемый интервал</param>
    /// <returns>Истина, если проверяемый интервал находится в границах текущего</returns>
    [DST]
    public bool IsInclude(Interval<T> I) => Check(I.Min) && Check(I.Max);

    [DST]
    public bool IsIntersect(Interval<T> I)
    {
        var min_to_min_compare = I._Min.CompareTo(_Min);
        var min_to_max_compare = I._Min.CompareTo(_Max);

        var max_to_min_compare = I._Max.CompareTo(_Min);
        var max_to_max_compare = I._Max.CompareTo(_Max);


        if ((max_to_min_compare < 0 && min_to_min_compare < 0) || (min_to_max_compare > 0 && max_to_max_compare > 0)) return false;

        if (min_to_min_compare < 0)
            return max_to_min_compare > 0 || MinInclude && I.MaxInclude;

        if (max_to_max_compare > 0)
            return min_to_max_compare < 0 || MaxInclude && I.MinInclude;

        throw new NotSupportedException($"Ошибка реализации метода проверки на пересечение интервалов {this}|{I}");
    }

    public Interval<T> GetInvertedInterval() => new(Max, MaxInclude, Min, MinInclude);

    public void WhileInInterval(T start, Action<T> Do, Func<T, T> Pos)
    {
        var x = start;
        while (Check(x))
        {
            Do(x);
            x = Pos(x);
        }
    }

    /* ------------------------------------------------------------------------------------------ */

    #region Базовые методы

    public void Deconstruct(out T min, out T max)
    {
        min = _Min;
        max = _Max;
    }

    public void Deconstruct(out T min, out bool IncludeMin, out T max, out bool IncludeMax)
    {
        min        = _Min;
        max        = _Max;
        IncludeMin = _MinInclude;
        IncludeMax = _MaxInclude;
    }

    /// <summary>Играет роль хэш-функции для определенного типа. </summary>
    /// <returns>Хэш-код для текущего объекта <see cref="T:System.Object"/>.</returns>
    /// <filterpriority>2</filterpriority>
    [DST]
    public override int GetHashCode()
    {
        unchecked
        {
            var result = _MinInclude.GetHashCode();
            result = (result * 397) ^ _MaxInclude.GetHashCode();
            result = (result * 397) ^ _Min.GetHashCode();
            result = (result * 397) ^ _Max.GetHashCode();
            return result;
        }
    }

    /// <summary>Указывает, равен ли текущий объект другому объекту того же типа.</summary>
    /// <returns>true, если текущий объект равен параметру <paramref name="other"/>, в противном случае — false.</returns>
    /// <param name="other">Объект, который требуется сравнить с данным объектом.</param>
    [DST]
    public bool Equals(Interval<T> other)
    {
        var comparer = EqualityComparer<T>.Default;
        return other._MinInclude == _MinInclude
            && other._MaxInclude == _MaxInclude
            && comparer.Equals(other._Min, _Min)
            && comparer.Equals(other._Max, _Max);
    }

    /// <summary>Указывает, равен ли текущий объект другому объекту того же типа.</summary>
    /// <returns>true, если текущий объект равен параметру <paramref name="other"/>, в противном случае — false.</returns>
    /// <param name="other">Объект, который требуется сравнить с данным объектом.</param>
    [DST]
    public bool Equals((T Min, T Max) other)
    {
        var comparer = EqualityComparer<T>.Default;
        return comparer.Equals(other.Min, _Min) && comparer.Equals(other.Max, _Max);
    }

    /// <summary>
    /// Определяет, равен ли заданный объект <see cref="T:System.Object"/> текущему объекту <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// true, если указанный объект <see cref="T:System.Object"/> равен текущему объекту <see cref="T:System.Object"/>; в противном случае — false.
    /// </returns>
    /// <param name="obj">Объект <see cref="T:System.Object"/>, который требуется сравнить с текущим объектом <see cref="T:System.Object"/>.</param>
    /// <exception cref="T:System.NullReferenceException">Параметр <paramref name="obj"/> имеет значение null.</exception><filterpriority>2</filterpriority>
    [DST]
    public override bool Equals(object obj) => obj is Interval<T> I && Equals(I);

    /// <inheritdoc />
    object ICloneable.Clone() => Clone();

    /// <inheritdoc />
    public Interval<T> Clone() => new(_Min, _MinInclude, _Max, _MaxInclude);

    /// <inheritdoc />
    [DST]
    public override string ToString() => string.Format(
        "{0}{2};{3}{1}",
        _MinInclude ? "[" : "(",
        _MaxInclude ? "]" : ")",
        _Min, _Max);

    #endregion

    /* ------------------------------------------------------------------------------------------ */

    #region Операторы

    [DST]
    public static bool operator ==(Interval<T> left, Interval<T> right) => left.Equals(right);

    [DST]
    public static bool operator !=(Interval<T> left, Interval<T> right) => !(left == right);
    [DST]
    public static bool operator ==(Interval<T> left, (T Min, T Max) right) => left.Equals(right);

    [DST]
    public static bool operator !=(Interval<T> left, (T Min, T Max) right) => !(left == right);
    [DST]
    public static bool operator ==((T Min, T Max) left, Interval<T> right) => right.Equals(left);

    [DST]
    public static bool operator !=((T Min, T Max) left, Interval<T> right) => !(left == right);

    public static implicit operator (T Min, T Max)(Interval<T> v) => (v._Min, v._Max);

    /// <summary>Оператор неявного приведения типа к предикату</summary>
    /// <param name="I">Интервал</param>
    /// <returns>Предикат от вещественного типа двойной точности</returns>
    [DST]
    public static implicit operator Predicate<T>(Interval<T> I) => I.Check;

    /// <summary>Оператор проверки на вхождение величины в интервал</summary>
    /// <param name="Value">Проверяемая величина</param>
    /// <param name="I">Интервал</param>
    /// <returns>Истина, если величина внутри интервала</returns>
    [DST]
    public static bool operator ^(T Value, Interval<T> I) => I.Check(Value);

    /// <summary>Оператор проверки на вхождение величины в интервал</summary>
    /// <param name="Value">Проверяемая величина</param>
    /// <param name="I">Интервал</param>
    /// <returns>Истина, если величина внутри интервала</returns>
    [DST]
    public static bool operator ^(Interval<T> I, T Value) => Value ^ I;

    [DST]
    public static bool operator >(Interval<T> I, T Value)
    {
        var result = I._Min.CompareTo(Value);
        return (result == 0 && !I._MinInclude) || result > 0;
    }

    [DST]
    public static bool operator <(Interval<T> I, T Value)
    {
        var result = I._Max.CompareTo(Value);
        return (result == 0 && !I._MaxInclude) || result < 0;
    }

    [DST]
    public static bool operator >(T Value, Interval<T> I)
    {
        var result = Value.CompareTo(I._Max);
        return (result == 0 && !I._MaxInclude) || result > 0;
    }

    [DST]
    public static bool operator <(T Value, Interval<T> I)
    {
        var result = Value.CompareTo(I._Min);
        return (result == 0 && !I._MinInclude) || result < 0;
    }

    #endregion

    /* ------------------------------------------------------------------------------------------ */
}

/// <summary>Интервал вещественных значений двойной точности</summary>
[Serializable]
[TypeConverter(typeof(IntervalConverter))]
public readonly struct Interval : IComparable<double>, IFormattable, 
                                  IEquatable<Interval>, 
                                  IEquatable<(double Min, double Max)>,
                                  IEquatable<(int Min, double Max)>,
                                  IEquatable<(double Min, int Max)>
{
    /* -------------------------------------------------------------------------------------------- */

    public static Interval Width(double Center, double Length, bool IncludeLimits = true) =>
        Width(Center, Length, IncludeLimits, IncludeLimits);

    public static Interval Width(double Center, double Length, bool MinInclude, bool MaxInclude) =>
        new(Center - Length / 2, MinInclude, Center + Length / 2, MaxInclude);

    public static IEnumerable<double> Range(double Min, double Max, double Step)
    {
        if (Min > Max)
            throw new InvalidOperationException("Минимум должен быть меньше максимума");
        if (Step == 0)
            throw new ArgumentException("Шаг не может быть равен 0", nameof(Step));
        if (Step < 0)
            throw new ArgumentOutOfRangeException(nameof(Step), Step, "Шаг не может быть меньше 0");

        while (Min <= Max)
        {
            yield return Min;
            Min += Step;
        }
    }

    public static IEnumerable<double> RangeN(double Min, double Max, int Count) => Range
    (
        Min:  Min,
        Max:  Max,
        Step: (Max - Min) / (Count - 1)
    );

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Метод сравнения двух интервалов</summary>
    /// <param name="a">Первый сравниваемый интервал</param>
    /// <param name="b">Второй сравниваемый интервал</param>
    /// <returns>1 - если первый интервал больше второго, -1 - если первый интервал меньше второго, 0 - если интервалы равны</returns>
    public static int Comparer_Length(Interval a, Interval b)
    {
        var l1 = a.Length;
        var l2 = b.Length;
        return l1 > l2 ? 1 : (l1 < l2 ? -1 : 0);
    }

    private static Random? __Random;

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Включена ли нижняя граница интервала?</summary>
    private readonly bool _MinInclude;

    /// <summary>Включена ли верхняя граница интервала?</summary>
    private readonly bool _MaxInclude;

    /// <summary>Нижняя граница интервала</summary>
    private readonly double _Min;

    /// <summary>Верхняя граница интервала</summary>
    private readonly double _Max;

    /* -------------------------------------------------------------------------------------------- */

    #region Свойства

    public double RandomValue => (__Random ??= new()).NextDouble() * Length + _Min;

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

    #endregion

    /* -------------------------------------------------------------------------------------------- */

    #region Конструкторы

    /// <summary>Интервал</summary>
    /// <param name="Min">Нижняя граница интервала</param>
    /// <param name="Max">Верхняя граница интервала</param>
    public Interval(double Min, double Max) : this(Min, true, Max, true) { }

    /// <summary>Интервал</summary>
    /// <param name="Min">Нижняя граница интервала</param>
    /// <param name="Max">Верхняя граница интервала</param>
    /// <param name="IncludeLimits">Включать пределы?</param>
    public Interval(double Min, double Max, bool IncludeLimits) : this(Min, IncludeLimits, Max, IncludeLimits) { }

    /// <summary>Интервал</summary>
    /// <param name="Min">Нижняя граница интервала</param>
    /// <param name="MinInclude">Включена ли нижняя граница интервала?</param>
    /// <param name="Max">Верхняя граница интервала</param>
    /// <param name="MaxInclude">Включена ли верхняя граница интервала</param>
    public Interval(double Min, bool MinInclude, double Max, bool MaxInclude)
    {
        _Min        = Min;
        _Max        = Max;
        _MinInclude = MinInclude;
        _MaxInclude = MaxInclude;
    }

    #endregion

    /* -------------------------------------------------------------------------------------------- */

    #region Интервальные функции

    public Interval IncludeMax(bool Include) => new(_Min, _MinInclude, _Max, Include);
    public Interval IncludeMin(bool Include) => new(_Min, Include, _Max, _MaxInclude);
    public Interval Include(bool IncludeMin, bool IncludeMax) => new(_Min, IncludeMin, _Max, IncludeMax);
    public Interval Include(bool Include) => new(_Min, Include, _Max, Include);

    public Interval SetMin(double Value) => new(Value, _MinInclude, _Max, _MaxInclude);
    public Interval SetMin(double Value, bool IncludeMin) => new(Value, IncludeMin, _Max, _MaxInclude);
    public Interval SetMax(double Value) => new(_Min, _MinInclude, Value, _MaxInclude);
    public Interval SetMax(double Value, bool IncludeMax) => new(_Min, _MinInclude, Value, IncludeMax);

    public void Deconstruct(out double min, out double max)
    {
        min = _Min;
        max = _Max;
    }

    public void Deconstruct(out double min, out bool IncludeMinMin, out double max, out bool IncludeMinMax)
    {
        min           = _Min;
        max           = _Max;
        IncludeMinMin = _MinInclude;
        IncludeMinMax = _MaxInclude;
    }

    /// <summary>
    /// Метод возвращает указанное значение, если оно находится внутри интервала,
    /// либо соответствующую его границу, если значение входит за его пределы
    /// </summary>
    /// <param name="Value">Нормализуемое значение</param>
    /// <returns>
    /// Значение, переданное в качестве аргумента, если оно входит в интервал,
    /// иначе соответствующая граница интервала
    /// </returns>
    [DST]
    public double Normalize(double Value) => Math.Max(_Min, Math.Min(Value, _Max));

    /// <summary>Замена значения ссылки на значение границы интервала, если значение не входит в интервал</summary>
    /// <param name="Value">Проверяемое значение</param>
    public void Normalize(ref double Value)
    {
        if (Value > _Max) Value      = _Max;
        else if (Value < _Min) Value = _Min;
    }

    public bool Check(double X, double MinOffset, double MaxOffset)
    {
        var min = _Min + MinOffset;
        var max = _Max + MaxOffset;

        return (_MinInclude && Math.Abs(X - min) < double.Epsilon)
            || (_MaxInclude && Math.Abs(X - max) < double.Epsilon)
            || (X > min && X < max);
    }

    /// <summary>Проверка на вхождение значения в интервал</summary>
    /// <param name="Value">Проверяемое значение</param>
    /// <returns>Истина, если значение входит в интервал</returns>
    [DST]
    public bool Check(double Value) =>
        (_MinInclude && _Min.CompareTo(Value) == 0) || 
        (_MaxInclude && _Max.CompareTo(Value) == 0) || 
        (Value.CompareTo(_Min) > 0 && Value.CompareTo(_Max) < 0);

    public bool Check(double X, double Offset) => Check(X, Offset, -Offset);

    public double FitValue(double Value, double ValueMax) => Min + Value * Length / ValueMax;
    public double FitValue(double Value, double ValueMin, double ValueMax) => Min + (Value - ValueMin) * Length / (ValueMax - ValueMin);
    public double FitValue(double Value, Interval ValueInterval) => Min + (Value - ValueInterval.Min) * Length / ValueInterval.Length;

    public bool IsExclude(Interval I) => !IsInclude(I);

    public bool IsInclude(Interval I) =>
        Check(I._MinInclude ? I._Min : I._Min + double.Epsilon) && 
        Check(I._MaxInclude ? I._Max : I._Max - double.Epsilon);

    public bool IsIntersect(Interval I)
    {
        if (Math.Abs(I._Min - _Min) < double.Epsilon && Math.Abs(I._Max - _Max) < double.Epsilon) return true;

        var min_include = Check(I._Min) || Math.Abs(I._Min - _Max) > double.Epsilon;
        var max_include = Check(I._Max) || Math.Abs(I._Max - _Min) > double.Epsilon;

        return min_include || max_include;
    }

    public bool IsIntersect((double Min, double Max) I)
    {
        if (Math.Abs(I.Min - _Min) < double.Epsilon && Math.Abs(I.Max - _Max) < double.Epsilon) return true;

        var min_include = Check(I.Min) || Math.Abs(I.Min - _Max) > double.Epsilon;
        var max_include = Check(I.Max) || Math.Abs(I.Max - _Min) > double.Epsilon;

        return min_include || max_include;
    }

    public bool IsIntersect((int Min, double Max) I)
    {
        if (Math.Abs(I.Min - _Min) < double.Epsilon && Math.Abs(I.Max - _Max) < double.Epsilon) return true;

        var min_include = Check(I.Min) || Math.Abs(I.Min - _Max) > double.Epsilon;
        var max_include = Check(I.Max) || Math.Abs(I.Max - _Min) > double.Epsilon;

        return min_include || max_include;
    }

    public bool IsIntersect((double Min, int Max) I)
    {
        if (Math.Abs(I.Min - _Min) < double.Epsilon && Math.Abs(I.Max - _Max) < double.Epsilon) return true;

        var min_include = Check(I.Min) || Math.Abs(I.Min - _Max) > double.Epsilon;
        var max_include = Check(I.Max) || Math.Abs(I.Max - _Min) > double.Epsilon;

        return min_include || max_include;
    }

    #endregion

    public int CompareTo(double x) =>
        (x > _Min && x < _Max) || 
        (_MinInclude && Math.Abs(Min - x) < double.Epsilon) || 
        (_MaxInclude && Math.Abs(Max - x) < double.Epsilon)
            ? 0
            : x < _Min ? -1 : 1;

    #region Цыклы

    public void For(int samples, Action<double> Do)
    {
        var len               = Length;
        var min               = _Min;
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
        var len               = Length;
        var min               = _Min;
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

    public IEnumerable<double> GetValues(int Count)
    {
        var len               = Length;
        var min               = _Min;
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

    public Interval GetInvertedInterval() => new(_Max, _MaxInclude, _Min, _MinInclude);

    public IEnumerable<double> GetValues(double Step)
    {
        var position = _Min;
        do
        {
            yield return position;
            position += Step;
        } while ((_MaxInclude && position <= _Max) || (!_MaxInclude && position < Max));
    }

    public IEnumerable<Interval> GetSubIntervals(int Count)
    {
        var last = _MinInclude ? _Min : _Min - double.Epsilon;
        return GetValues(Count)
           .Skip(1)
           .ForeachLazyLast(v => last = v)
           .Select(v => new Interval(last, v, true));
    }

    /// <inheritdoc />
    [DST]
    public override string ToString() => new StringBuilder()
       .Append(_MinInclude ? '[' : '(')
       .Append(_Min)
       .Append(", ")
       .Append(_Max)
       .Append(_MaxInclude ? ']' : ')')
       .ToString();

    //public override string ToString() => string.Format(
    //    "{0}{2};{3}{1}",
    //    _MinInclude ? "[" : "(",
    //    _MaxInclude ? "]" : ")",
    //    _Min, _Max);

    public string ToString(string Format) => new StringBuilder()
       .Append(_MinInclude ? '[' : '(')
       .Append(_Min.ToString(Format))
       .Append(", ")
       .Append(_Max.ToString(Format))
       .Append(_MaxInclude ? ']' : ')')
       .ToString();

    public string ToString(IFormatProvider FormatProvider) => new StringBuilder()
       .Append(_MinInclude ? '[' : '(')
       .Append(_Min.ToString(FormatProvider))
       .Append(", ")
       .Append(_Max.ToString(FormatProvider))
       .Append(_MaxInclude ? ']' : ')')
       .ToString();

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
    public string ToString(string Format, IFormatProvider FormatProvider) => new StringBuilder()
       .Append(_MinInclude ? '[' : '(')
       .Append(_Min.ToString(Format, FormatProvider))
       .Append(", ")
       .Append(_Max.ToString(Format, FormatProvider))
       .Append(_MaxInclude ? ']' : ')')
       .ToString();

    /// <inheritdoc />
    [DST]
    public override int GetHashCode() => new HashBuilder()
       .Append(_MinInclude)
       .Append(_MaxInclude)
       .Append(_Min)
       .Append(_Max)
       .Hash;

    /// <inheritdoc />
    [DST]
    public bool Equals(Interval other) =>
        other._MinInclude == _MinInclude
        && other._MaxInclude == _MaxInclude
        && other._Min.Equals(_Min)
        && other._Max.Equals(Max);

    /// <inheritdoc />
    [DST]
    public bool Equals((double Min, double Max) other) =>
        _Min.Equals(other.Min) && 
        _Max.Equals(other.Max);

    /// <inheritdoc />
    [DST]
    public bool Equals((int Min, double Max) other) =>
        _Min.Equals(other.Min) && 
        _Max.Equals(other.Max);

    /// <inheritdoc />
    [DST]
    public bool Equals((double Min, int Max) other) =>
        _Min.Equals(other.Min) && 
        _Max.Equals(other.Max);

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is Interval I && Equals(I);

    /* ------------------------------------------------------------------------------------------ */

    public static implicit operator (double Min, double Max)(Interval I) => (I.Min, I.Max);

    public static implicit operator Interval((double, double) V) => new(V.Item1, V.Item2);

    public static implicit operator Interval((double, double, bool) V) => new(V.Item1, V.Item2, V.Item3);

    public static implicit operator Interval((double, bool, double) V) => new(V.Item1, V.Item2, V.Item3, false);

    public static implicit operator Interval((double, bool, double, bool) V) => new(V.Item1, V.Item2, V.Item3, V.Item4);

    public static implicit operator double(Interval I) => I.Length;

    public static explicit operator Interval(double V) => new(0, true, V, true);

    public static Interval operator +(Interval I, double x) => new(I._Min + x, I._MinInclude, I._Max + x, I._MaxInclude);

    public static Interval operator -(Interval I, double x) => new(I._Min - x, I._MinInclude, I._Max - x, I._MaxInclude);

    public static Interval operator *(Interval I, double x) => new(I._Min * x, I._MinInclude, I._Max * x, I._MaxInclude);

    public static Interval operator /(Interval I, double x) => new(I._Min / x, I._MinInclude, I._Max / x, I._MaxInclude);

    /* ------------------------------------------------------------------------------------------ */
}