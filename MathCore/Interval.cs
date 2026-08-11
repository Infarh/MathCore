using System.ComponentModel;
using System.Runtime.InteropServices;
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
/// <summary>Интервал сравнимых величин</summary>
/// <typeparam name="T">Тип сравнимых величин</typeparam>
[StructLayout(LayoutKind.Sequential)]
[method: DST]
public readonly struct Interval<T>(T Min, bool MinInclude, T Max, bool MaxInclude, IComparer<T> Comparer)
    : IEquatable<Interval<T>>, IEquatable<(T Min, T Max)>, ICloneable<Interval<T>>
{
    /* ------------------------------------------------------------------------------------------ */

    #region Конструкторы

    /// <summary>Интервал</summary>
    /// <param name="Min">Нижняя граница интервала</param>
    /// <param name="Max">Верхняя граница интервала</param>
    [DST]
    public Interval(T Min, T Max) : this(Min, true, Max, true, Comparer<T>.Default) { }

    /// <summary>Интервал</summary>
    /// <param name="Min">Нижняя граница интервала</param>
    /// <param name="Max">Верхняя граница интервала</param>
    [DST]
    public Interval(T Min, bool MinInclude, T Max, bool MaxInclude) : this(Min, MinInclude, Max, MaxInclude, Comparer<T>.Default) { }

    /// <summary>Интервал</summary>
    /// <param name="Min">Нижняя граница интервала</param>
    /// <param name="Max">Верхняя граница интервала</param>
    [DST]
    public Interval(T Min, T Max, IComparer<T> Comparer) : this(Min, true, Max, true, Comparer) { }

    /// <summary>Интервал</summary>
    /// <param name="Min">Нижняя граница интервала</param>
    /// <param name="Max">Верхняя граница интервала</param>
    /// <param name="IncludeLimits">Включать пределы? (default:true)</param>
    [DST]
    public Interval(T Min, T Max, bool IncludeLimits) : this(Min, IncludeLimits, Max, IncludeLimits, Comparer<T>.Default) { }

    /// <summary>Интервал</summary>
    /// <param name="Min">Нижняя граница интервала</param>
    /// <param name="Max">Верхняя граница интервала</param>
    /// <param name="IncludeLimits">Включать пределы? (default:true)</param>
    [DST]
    public Interval(T Min, T Max, bool IncludeLimits, IComparer<T> Comparer) : this(Min, IncludeLimits, Max, IncludeLimits, Comparer) { }

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

    private readonly IComparer<T> Comparer = Comparer;

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

    /// <summary>Интервал является пустым - минимум совпадает с максимумом</summary>
    public bool IsEmpty => Equals(_Min, _Max);

    /// <summary>Границы интервала инвертированы (минимум больше максимума)</summary>
    public bool IsInverted => Comparer.Compare(_Min, _Max) > 0;

    #endregion

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Версия интервала с указанным состоянием включения верхней границы</summary>
    /// <param name="Include">Включать ли верхнюю границу</param>
    /// <returns>Новый интервал</returns>
    public Interval<T> IncludeMax(bool Include) => new(_Min, _MinInclude, _Max, Include, Comparer);
    /// <summary>Версия интервала с указанным состоянием включения нижней границы</summary>
    /// <param name="Include">Включать ли нижнюю границу</param>
    /// <returns>Новый интервал</returns>
    public Interval<T> IncludeMin(bool Include) => new(_Min, Include, _Max, _MaxInclude, Comparer);
    /// <summary>Версия интервала с указанными состояниями включения границ</summary>
    /// <param name="IncludeMin">Включать ли нижнюю границу</param>
    /// <param name="IncludeMax">Включать ли верхнюю границу</param>
    /// <returns>Новый интервал</returns>
    public Interval<T> Include(bool IncludeMin, bool IncludeMax) => new(_Min, IncludeMin, _Max, IncludeMax, Comparer);
    /// <summary>Версия интервала с указанным состоянием включения обеих границ</summary>
    /// <param name="Include">Включать ли границы</param>
    /// <returns>Новый интервал</returns>
    public Interval<T> Include(bool Include) => new(_Min, Include, _Max, Include, Comparer);

    /// <summary>Версия интервала с указанной нижней границей</summary>
    /// <param name="Value">Новая нижняя граница</param>
    /// <returns>Новый интервал</returns>
    public Interval<T> SetMin(T Value) => new(Value, _MinInclude, _Max, _MaxInclude, Comparer);
    /// <summary>Версия интервала с указанной нижней границей и её включением</summary>
    /// <param name="Value">Новая нижняя граница</param>
    /// <param name="IncludeMin">Включать ли нижнюю границу</param>
    /// <returns>Новый интервал</returns>
    public Interval<T> SetMin(T Value, bool IncludeMin) => new(Value, IncludeMin, _Max, _MaxInclude, Comparer);
    /// <summary>Версия интервала с указанной верхней границей</summary>
    /// <param name="Value">Новая верхняя граница</param>
    /// <returns>Новый интервал</returns>
    public Interval<T> SetMax(T Value) => new(_Min, _MinInclude, Value, _MaxInclude, Comparer);
    /// <summary>Версия интервала с указанной верхней границей и её включением</summary>
    /// <param name="Value">Новая верхняя граница</param>
    /// <param name="IncludeMax">Включать ли верхнюю границу</param>
    /// <returns>Новый интервал</returns>
    public Interval<T> SetMax(T Value, bool IncludeMax) => new(_Min, _MinInclude, Value, IncludeMax, Comparer);
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
    public T Normalize(T Value) => Comparer.Compare(Value, _Max) > 0 ? _Max : (Comparer.Compare(Value, _Min) < 0 ? _Min : Value);

    /// <summary>Замена значения ссылки на значение границы интервала, если значение не входит в интервал</summary>
    /// <param name="Value">Проверяемое значение</param>
    public void Normalize(ref T Value)
    {
        if (Comparer.Compare(Value, _Max) > 0)
            Value = _Max;
        else if (Comparer.Compare(Value, _Min) < 0)
            Value = _Min;
    }

    /// <summary>Проверка на вхождение значения в интервал</summary>
    /// <param name="Value">Проверяемое значение</param>
    /// <returns>Истина, если значение входит в интервал</returns>
    [DST]
    public bool Check(T Value) =>
        (_MinInclude && Comparer.Compare(_Min, Value) == 0)
        || (_MaxInclude && Comparer.Compare(_Max, Value) == 0)
        || (Comparer.Compare(Value, _Min) > 0 && Comparer.Compare(Value, _Max) < 0);

    /// <summary>Проверка - входит ли указанный интервал в текущий</summary>
    /// <param name="I">Проверяемый интервал</param>
    /// <returns>Истина, если проверяемый интервал находится в границах текущего</returns>
    [DST]
    public bool IsInclude(Interval<T> I) => Check(I.Min) && Check(I.Max);

    /// <summary>Проверка пересечения текущего интервала с указанным</summary>
    /// <param name="I">Проверяемый интервал</param>
    /// <returns>Истина, если интервалы пересекаются</returns>
    [DST]
    public bool IsIntersect(Interval<T> I)
    {
        var min_to_min_compare = Comparer.Compare(I._Min, _Min);
        var min_to_max_compare = Comparer.Compare(I._Min, _Max);

        var max_to_min_compare = Comparer.Compare(I._Max, _Min);
        var max_to_max_compare = Comparer.Compare(I._Max, _Max);


        if ((max_to_min_compare < 0 && min_to_min_compare < 0) || (min_to_max_compare > 0 && max_to_max_compare > 0)) return false;

        if (min_to_min_compare < 0)
            return max_to_min_compare > 0 || (MinInclude && I.MaxInclude);

        if (max_to_max_compare > 0)
            return min_to_max_compare < 0 || (MaxInclude && I.MinInclude);

        throw new NotSupportedException($"Ошибка реализации метода проверки на пересечение интервалов {this}|{I}");
    }

    /// <summary>Возвращает инвертированный интервал (границы меняются местами)</summary>
    /// <returns>Инвертированный интервал</returns>
    public Interval<T> GetInvertedInterval() => new(Max, MaxInclude, Min, MinInclude, Comparer);

    /// <summary>Выполняет действие для каждого значения, входящего в интервал, начиная с указанного</summary>
    /// <param name="start">Стартовое значение</param>
    /// <param name="Do">Выполняемое действие</param>
    /// <param name="Pos">Функция перехода к следующему значению</param>
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

    /// <summary>Деконструкция интервала на нижнюю и верхнюю границы</summary>
    /// <param name="min">Нижняя граница</param>
    /// <param name="max">Верхняя граница</param>
    public void Deconstruct(out T min, out T max)
    {
        min = _Min;
        max = _Max;
    }

    /// <summary>Деконструкция интервала на границы и их включения</summary>
    /// <param name="min">Нижняя граница</param>
    /// <param name="IncludeMin">Включена ли нижняя граница</param>
    /// <param name="max">Верхняя граница</param>
    /// <param name="IncludeMax">Включена ли верхняя граница</param>
    public void Deconstruct(out T min, out bool IncludeMin, out T max, out bool IncludeMax)
    {
        min = _Min;
        max = _Max;
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
            if (_Min is not null)
                result = (result * 397) ^ _Min.GetHashCode();
            if (_Max is not null)
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
    public override bool Equals(object? obj) => obj is Interval<T> I && Equals(I);

    /// <inheritdoc />
    object ICloneable.Clone() => Clone();

    /// <inheritdoc />
    public Interval<T> Clone() => new(_Min, _MinInclude, _Max, _MaxInclude, Comparer);

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

    /// <summary>Оператор равенства двух интервалов</summary>
    [DST]
    public static bool operator ==(Interval<T> left, Interval<T> right) => left.Equals(right);

    /// <summary>Оператор неравенства двух интервалов</summary>
    [DST]
    public static bool operator !=(Interval<T> left, Interval<T> right) => !(left == right);
    /// <summary>Оператор равенства интервала и кортежа границ</summary>
    [DST]
    public static bool operator ==(Interval<T> left, (T Min, T Max) right) => left.Equals(right);

    /// <summary>Оператор неравенства интервала и кортежа границ</summary>
    [DST]
    public static bool operator !=(Interval<T> left, (T Min, T Max) right) => !(left == right);
    /// <summary>Оператор равенства кортежа границ и интервала</summary>
    [DST]
    public static bool operator ==((T Min, T Max) left, Interval<T> right) => right.Equals(left);

    /// <summary>Оператор неравенства кортежа границ и интервала</summary>
    [DST]
    public static bool operator !=((T Min, T Max) left, Interval<T> right) => !(left == right);

    /// <summary>Неявное преобразование интервала в кортеж границ</summary>
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

    /// <summary>Проверка, что весь интервал находится ниже значения (строго или с учётом границы)</summary>
    [DST]
    public static bool operator >(Interval<T> I, T Value)
    {
        var result = I.Comparer.Compare(I._Min, Value);
        return (result == 0 && !I._MinInclude) || result > 0;
    }

    /// <summary>Проверка, что весь интервал находится выше значения (строго или с учётом границы)</summary>
    [DST]
    public static bool operator <(Interval<T> I, T Value)
    {
        var result = I.Comparer.Compare(I._Max, Value);
        return (result == 0 && !I._MaxInclude) || result < 0;
    }

    /// <summary>Проверка, что значение находится выше всего интервала (строго или с учётом границы)</summary>
    [DST]
    public static bool operator >(T Value, Interval<T> I)
    {
        var result = I.Comparer.Compare(Value, I._Max);
        return (result == 0 && !I._MaxInclude) || result > 0;
    }

    /// <summary>Проверка, что значение находится ниже всего интервала (строго или с учётом границы)</summary>
    [DST]
    public static bool operator <(T Value, Interval<T> I)
    {
        var result = I.Comparer.Compare(Value, I._Min);
        return (result == 0 && !I._MinInclude) || result < 0;
    }

    #endregion

    /* ------------------------------------------------------------------------------------------ */
}

/// <summary>Интервал вещественных значений двойной точности</summary>
/// <remarks>Интервал</remarks>
/// <param name="Min">Нижняя граница интервала</param>
/// <param name="MinInclude">Включена ли нижняя граница интервала?</param>
/// <param name="Max">Верхняя граница интервала</param>
/// <param name="MaxInclude">Включена ли верхняя граница интервала</param>
[Serializable]
[TypeConverter(typeof(IntervalConverter))]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
[method: DST]
public readonly struct Interval(double Min, bool MinInclude, double Max, bool MaxInclude) :
    IComparable<double>, IFormattable,
    IEquatable<Interval>,
    IEquatable<(double Min, double Max)>,
    IEquatable<(int Min, double Max)>,
    IEquatable<(double Min, int Max)>
{
    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Формирует интервал заданной ширины с центром в указанной точке</summary>
    /// <param name="Center">Центр интервала</param>
    /// <param name="Length">Ширина интервала</param>
    /// <param name="IncludeLimits">Включать ли границы (по умолчанию true)</param>
    /// <returns>Интервал</returns>
    public static Interval Width(double Center, double Length, bool IncludeLimits = true) =>
        Width(Center, Length, IncludeLimits, IncludeLimits);

    /// <summary>Формирует интервал заданной ширины с центром в указанной точке и указанными включениями границ</summary>
    /// <param name="Center">Центр интервала</param>
    /// <param name="Length">Ширина интервала</param>
    /// <param name="MinInclude">Включать ли нижнюю границу</param>
    /// <param name="MaxInclude">Включать ли верхнюю границу</param>
    /// <returns>Интервал</returns>
    public static Interval Width(double Center, double Length, bool MinInclude, bool MaxInclude) =>
        new(Center - Length / 2, MinInclude, Center + Length / 2, MaxInclude);

    /// <summary>Перечисляет значения от минимума до максимума с заданным шагом</summary>
    /// <param name="Min">Начальное значение</param>
    /// <param name="Max">Конечное значение</param>
    /// <param name="Step">Шаг перечисления</param>
    /// <returns>Последовательность значений</returns>
    /// <exception cref="InvalidOperationException">Минимум больше максимума</exception>
    /// <exception cref="ArgumentException">Шаг равен нулю</exception>
    /// <exception cref="ArgumentOutOfRangeException">Шаг меньше нуля</exception>
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

    /// <summary>Перечисляет заданное количество значений, равномерно распределённых по интервалу</summary>
    /// <param name="Min">Начальное значение</param>
    /// <param name="Max">Конечное значение</param>
    /// <param name="Count">Количество значений</param>
    /// <returns>Последовательность значений</returns>
    public static IEnumerable<double> RangeN(double Min, double Max, int Count) => Range
    (
        Min: Min,
        Max: Max,
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
    private readonly bool _MinInclude = MinInclude;

    /// <summary>Включена ли верхняя граница интервала?</summary>
    private readonly bool _MaxInclude = MaxInclude;

    /// <summary>Нижняя граница интервала</summary>
    private readonly double _Min = Min;

    /// <summary>Верхняя граница интервала</summary>
    private readonly double _Max = Max;

    /* -------------------------------------------------------------------------------------------- */

    #region Свойства

    /// <summary>Случайное значение внутри интервала</summary>
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

    /// <summary>Интервал является пустым - минимум совпадает с максимумом</summary>
    public bool IsEmpty => Equals(_Min, _Max);

    /// <summary>Границы интервала инвертированы (минимум больше максимума)</summary>
    public bool IsInverted => _Min > _Max;

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

    #endregion

    /* -------------------------------------------------------------------------------------------- */

    #region Интервальные функции

    /// <summary>Версия интервала с указанным состоянием включения верхней границы</summary>
    /// <param name="Include">Включать ли верхнюю границу</param>
    /// <returns>Новый интервал</returns>
    public Interval IncludeMax(bool Include) => new(_Min, _MinInclude, _Max, Include);
    /// <summary>Версия интервала с указанным состоянием включения нижней границы</summary>
    /// <param name="Include">Включать ли нижнюю границу</param>
    /// <returns>Новый интервал</returns>
    public Interval IncludeMin(bool Include) => new(_Min, Include, _Max, _MaxInclude);
    /// <summary>Версия интервала с указанными состояниями включения границ</summary>
    /// <param name="IncludeMin">Включать ли нижнюю границу</param>
    /// <param name="IncludeMax">Включать ли верхнюю границу</param>
    /// <returns>Новый интервал</returns>
    public Interval Include(bool IncludeMin, bool IncludeMax) => new(_Min, IncludeMin, _Max, IncludeMax);
    /// <summary>Версия интервала с указанным состоянием включения обеих границ</summary>
    /// <param name="Include">Включать ли границы</param>
    /// <returns>Новый интервал</returns>
    public Interval Include(bool Include) => new(_Min, Include, _Max, Include);

    /// <summary>Версия интервала с указанной нижней границей</summary>
    /// <param name="Value">Новая нижняя граница</param>
    /// <returns>Новый интервал</returns>
    public Interval SetMin(double Value) => new(Value, _MinInclude, _Max, _MaxInclude);
    /// <summary>Версия интервала с указанной нижней границей и её включением</summary>
    /// <param name="Value">Новая нижняя граница</param>
    /// <param name="IncludeMin">Включать ли нижнюю границу</param>
    /// <returns>Новый интервал</returns>
    public Interval SetMin(double Value, bool IncludeMin) => new(Value, IncludeMin, _Max, _MaxInclude);
    /// <summary>Версия интервала с указанной верхней границей</summary>
    /// <param name="Value">Новая верхняя граница</param>
    /// <returns>Новый интервал</returns>
    public Interval SetMax(double Value) => new(_Min, _MinInclude, Value, _MaxInclude);
    /// <summary>Версия интервала с указанной верхней границей и её включением</summary>
    /// <param name="Value">Новая верхняя граница</param>
    /// <param name="IncludeMax">Включать ли верхнюю границу</param>
    /// <returns>Новый интервал</returns>
    public Interval SetMax(double Value, bool IncludeMax) => new(_Min, _MinInclude, Value, IncludeMax);

    /// <summary>Деконструкция интервала на нижнюю и верхнюю границы</summary>
    /// <param name="min">Нижняя граница</param>
    /// <param name="max">Верхняя граница</param>
    public void Deconstruct(out double min, out double max)
    {
        min = _Min;
        max = _Max;
    }

    /// <summary>Деконструкция интервала на границы и их включения</summary>
    /// <param name="min">Нижняя граница</param>
    /// <param name="IncludeMinMin">Включена ли нижняя граница</param>
    /// <param name="max">Верхняя граница</param>
    /// <param name="IncludeMinMax">Включена ли верхняя граница</param>
    public void Deconstruct(out double min, out bool IncludeMinMin, out double max, out bool IncludeMinMax)
    {
        min = _Min;
        max = _Max;
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
        if (Value > _Max) Value = _Max;
        else if (Value < _Min) Value = _Min;
    }

    /// <summary>Проверка вхождения значения в интервал с учётом смещений границ</summary>
    /// <param name="X">Проверяемое значение</param>
    /// <param name="MinOffset">Смещение нижней границы</param>
    /// <param name="MaxOffset">Смещение верхней границы</param>
    /// <returns>Истина, если значение входит в интервал</returns>
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

    /// <summary>Проверка вхождения значения в интервал с учётом смещения границ</summary>
    /// <param name="X">Проверяемое значение</param>
    /// <param name="Offset">Смещение границ (верхняя смещается на -Offset)</param>
    /// <returns>Истина, если значение входит в интервал</returns>
    public bool Check(double X, double Offset) => Check(X, Offset, -Offset);

    /// <summary>Масштабирует нормализованное значение в диапазон интервала</summary>
    /// <param name="Value">Нормализованное значение</param>
    /// <param name="ValueMax">Максимум диапазона источника</param>
    /// <returns>Значение, приведённое к границам интервала</returns>
    public double FitValue(double Value, double ValueMax) => Min + Value * Length / ValueMax;
    /// <summary>Масштабирует значение из диапазона в границы интервала</summary>
    /// <param name="Value">Значение источника</param>
    /// <param name="ValueMin">Минимум диапазона источника</param>
    /// <param name="ValueMax">Максимум диапазона источника</param>
    /// <returns>Значение, приведённое к границам интервала</returns>
    public double FitValue(double Value, double ValueMin, double ValueMax) => Min + (Value - ValueMin) * Length / (ValueMax - ValueMin);
    /// <summary>Масштабирует значение из другого интервала в границы текущего</summary>
    /// <param name="Value">Значение источника</param>
    /// <param name="ValueInterval">Интервал источника</param>
    /// <returns>Значение, приведённое к границам текущего интервала</returns>
    public double FitValue(double Value, Interval ValueInterval) => Min + (Value - ValueInterval.Min) * Length / ValueInterval.Length;

    /// <summary>Проверка, что указанный интервал не входит полностью в текущий</summary>
    /// <param name="I">Проверяемый интервал</param>
    /// <returns>Истина, если интервал не входит полностью</returns>
    public bool IsExclude(Interval I) => !IsInclude(I);

    /// <summary>Проверка, что указанный интервал входит полностью в текущий</summary>
    /// <param name="I">Проверяемый интервал</param>
    /// <returns>Истина, если интервал входит полностью</returns>
    public bool IsInclude(Interval I) =>
        Check(I._MinInclude ? I._Min : I._Min + double.Epsilon) &&
        Check(I._MaxInclude ? I._Max : I._Max - double.Epsilon);

    /// <summary>Проверка пересечения текущего интервала с указанным</summary>
    /// <param name="I">Проверяемый интервал</param>
    /// <returns>Истина, если интервалы пересекаются</returns>
    public bool IsIntersect(Interval I)
    {
        if (Math.Abs(I._Min - _Min) < double.Epsilon && Math.Abs(I._Max - _Max) < double.Epsilon) return true;

        var min_include = Check(I._Min) || Math.Abs(I._Min - _Max) > double.Epsilon;
        var max_include = Check(I._Max) || Math.Abs(I._Max - _Min) > double.Epsilon;

        return min_include || max_include;
    }

    /// <summary>Проверка пересечения текущего интервала с кортежем границ</summary>
    /// <param name="I">Кортеж границ</param>
    /// <returns>Истина, если интервалы пересекаются</returns>
    public bool IsIntersect((double Min, double Max) I)
    {
        if (Math.Abs(I.Min - _Min) < double.Epsilon && Math.Abs(I.Max - _Max) < double.Epsilon) return true;

        var min_include = Check(I.Min) || Math.Abs(I.Min - _Max) > double.Epsilon;
        var max_include = Check(I.Max) || Math.Abs(I.Max - _Min) > double.Epsilon;

        return min_include || max_include;
    }

    /// <summary>Проверка пересечения текущего интервала с кортежем границ (int, double)</summary>
    /// <param name="I">Кортеж границ</param>
    /// <returns>Истина, если интервалы пересекаются</returns>
    public bool IsIntersect((int Min, double Max) I)
    {
        if (Math.Abs(I.Min - _Min) < double.Epsilon && Math.Abs(I.Max - _Max) < double.Epsilon) return true;

        var min_include = Check(I.Min) || Math.Abs(I.Min - _Max) > double.Epsilon;
        var max_include = Check(I.Max) || Math.Abs(I.Max - _Min) > double.Epsilon;

        return min_include || max_include;
    }

    /// <summary>Проверка пересечения текущего интервала с кортежем границ (double, int)</summary>
    /// <param name="I">Кортеж границ</param>
    /// <returns>Истина, если интервалы пересекаются</returns>
    public bool IsIntersect((double Min, int Max) I)
    {
        if (Math.Abs(I.Min - _Min) < double.Epsilon && Math.Abs(I.Max - _Max) < double.Epsilon) return true;

        var min_include = Check(I.Min) || Math.Abs(I.Min - _Max) > double.Epsilon;
        var max_include = Check(I.Max) || Math.Abs(I.Max - _Min) > double.Epsilon;

        return min_include || max_include;
    }

    #endregion

    /// <summary>Сравнение значения с интервалом: 0, если входит; -1, если меньше; 1, если больше</summary>
    /// <param name="x">Сравниваемое значение</param>
    /// <returns>0, если значение входит в интервал; иначе -1 или 1</returns>
    public int CompareTo(double x) =>
        (x > _Min && x < _Max) ||
        (_MinInclude && Math.Abs(Min - x) < double.Epsilon) ||
        (_MaxInclude && Math.Abs(Max - x) < double.Epsilon)
            ? 0
            : x < _Min ? -1 : 1;

    #region Цыклы

    /// <summary>Выполняет действие для указанного количества значений, равномерно распределённых по интервалу</summary>
    /// <param name="samples">Количество значений</param>
    /// <param name="Do">Выполняемое действие</param>
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

    /// <summary>Выполняет действие для указанного количества значений, равномерно распределённых по интервалу, с индексом</summary>
    /// <param name="samples">Количество значений</param>
    /// <param name="Do">Выполняемое действие (получает индекс и значение)</param>
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

    /// <summary>Выполняет действие для значений внутри интервала с заданным шагом</summary>
    /// <param name="step">Шаг перечисления</param>
    /// <param name="Do">Выполняемое действие</param>
    public void WhileInInterval(double step, Action<double> Do)
    {
        var min = Math.Min(_Max, _Min);
        step = _Max < _Min && step > 0 ? -step : step;
        var x = min + (_MinInclude ? 0 : double.Epsilon);
        while (Check(x)) Do(x += step);
    }

    #endregion

    /// <summary>Возвращает указанное количество значений, равномерно распределённых по интервалу</summary>
    /// <param name="Count">Количество значений</param>
    /// <returns>Последовательность значений</returns>
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

    /// <summary>Возвращает инвертированный интервал (границы меняются местами)</summary>
    /// <returns>Инвертированный интервал</returns>
    public Interval GetInvertedInterval() => new(_Max, _MaxInclude, _Min, _MinInclude);

    /// <summary>Возвращает значения внутри интервала с заданным шагом</summary>
    /// <param name="Step">Шаг перечисления</param>
    /// <returns>Последовательность значений</returns>
    public IEnumerable<double> GetValues(double Step)
    {
        var position = _Min;
        do
        {
            yield return position;
            position += Step;
        } while ((_MaxInclude && position <= _Max) || (!_MaxInclude && position < Max));
    }

    /// <summary>Возвращает последовательность подинтервалов, на которые разбивается текущий интервал</summary>
    /// <param name="Count">Количество подинтервалов</param>
    /// <returns>Последовательность подинтервалов</returns>
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

    /// <summary>Форматирует интервал с использованием заданного формата чисел</summary>
    /// <param name="Format">Формат чисел</param>
    /// <returns>Строковое представление интервала</returns>
    public string ToString(string Format) => new StringBuilder()
       .Append(_MinInclude ? '[' : '(')
       .Append(_Min.ToString(Format))
       .Append(", ")
       .Append(_Max.ToString(Format))
       .Append(_MaxInclude ? ']' : ')')
       .ToString();

    /// <summary>Форматирует интервал с использованием заданного поставщика формата</summary>
    /// <param name="FormatProvider">Поставщик формата</param>
    /// <returns>Строковое представление интервала</returns>
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
    public string ToString(string? Format, IFormatProvider? FormatProvider) => new StringBuilder()
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
    public override bool Equals(object? obj) => obj is Interval I && Equals(I);

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Неявное преобразование интервала в кортеж границ</summary>
    public static implicit operator (double Min, double Max)(Interval I) => (I.Min, I.Max);

    /// <summary>Неявное преобразование кортежа границ в интервал</summary>
    public static implicit operator Interval((double, double) V) => new(V.Item1, V.Item2);

    /// <summary>Неявное преобразование кортежа (миним, макс, включение) в интервал</summary>
    public static implicit operator Interval((double, double, bool) V) => new(V.Item1, V.Item2, V.Item3);

    /// <summary>Неявное преобразование кортежа (миним, включение, макс) в интервал</summary>
    public static implicit operator Interval((double, bool, double) V) => new(V.Item1, V.Item2, V.Item3, false);

    /// <summary>Неявное преобразование кортежа (миним, включение, макс, включение) в интервал</summary>
    public static implicit operator Interval((double, bool, double, bool) V) => new(V.Item1, V.Item2, V.Item3, V.Item4);

    /// <summary>Неявное преобразование интервала в его длину</summary>
    public static implicit operator double(Interval I) => I.Length;

    /// <summary>Явное преобразование значения в интервал от нуля до значения</summary>
    public static explicit operator Interval(double V) => new(0, true, V, true);

    /// <summary>Сдвиг интервала на заданную величину</summary>
    public static Interval operator +(Interval I, double x) => new(I._Min + x, I._MinInclude, I._Max + x, I._MaxInclude);

    /// <summary>Сдвиг интервала на заданную величину в отрицательную сторону</summary>
    public static Interval operator -(Interval I, double x) => new(I._Min - x, I._MinInclude, I._Max - x, I._MaxInclude);

    /// <summary>Масштабирование интервала на заданный коэффициент</summary>
    public static Interval operator *(Interval I, double x) => new(I._Min * x, I._MinInclude, I._Max * x, I._MaxInclude);

    /// <summary>Деление границ интервала на заданный коэффициент</summary>
    public static Interval operator /(Interval I, double x) => new(I._Min / x, I._MinInclude, I._Max / x, I._MaxInclude);

    /* ------------------------------------------------------------------------------------------ */
}