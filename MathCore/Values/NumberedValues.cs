#nullable enable
using System.Collections;

// ReSharper disable UnusedMember.Global

namespace MathCore.Values;

/// <summary>Массив значений в привязке их к сетке времени</summary>
/// <typeparam name="TValue">Тип значений</typeparam>
/// <param name="Dt">Шаг сетки времени (дискретизацции)</param>
/// <param name="Values">Массив значений</param>
/// <param name="T0">Начальное смещение сетки времени</param>
public class NumberedValues<TValue>(double Dt, TValue[] Values, double T0 = 0)
    : IEnumerable<KeyValuePair<double, TValue>>
{
    public double t0 => T0;
    public double dt => Dt;
    public int Count => Values.Length;
    public double T => Count * dt;

    public ref TValue this[int i] => ref Values[i];

    public ref TValue this[double t] => ref this[IndexOf(t)];

    public NumberedValues(double dt, int N, double t0 = 0) : this(dt, new TValue[N], t0) { }

    public NumberedValues(double dt, IEnumerable<TValue> Values, double t0 = 0) : this(dt, Values.ToArray(), t0) { }

    /// <summary>Возвращает индекс значения, соответствующего времени <paramref name="t"/></summary>
    /// <param name="t">Время</param>
    /// <returns>Индекс значения</returns>
    /// <remarks>
    ///     Функция возвращает ближайший индекс, не превышающий указанное время.
    ///     Если <paramref name="t"/> меньше <see cref="T0"/>, то возвращается 0.
    ///     Если <paramref name="t"/> больше или равно <see cref="T"/>, то возвращается <see cref="Count"/> - 1.
    /// </remarks>
    public int IndexOf(double t)
    {
        var i = (int)Math.Round((t - T0) / dt);
        return i < 0 ? 0 : (i >= Count ? Count - 1 : i);
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<double, TValue>> GetEnumerator() => Values.Cast<TValue>()
       .Select((v, i) => new KeyValuePair<double, TValue>(i * Dt + T0, v))
       .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<TValue> GetValues() => Values;

    public IEnumerable<double> GetTimes() => Enumerable.Range(0, Count).Select(i => i * Dt + T0);
}