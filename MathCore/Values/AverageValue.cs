﻿#nullable enable
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

namespace MathCore.Values;

/// <summary>Скользящее среднее</summary>
[Serializable]
public class AverageValue : ISerializable, IValue<double>, IResettable
{
    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Номер итерации усреднения</summary>
    private int _N;

    /// <summary>Текущее значение усредняемой величины</summary>
    private double _Value;

    private double _Value2;

    /// <summary>Начальное значение</summary>
    private readonly double _StartValue;

    /// <summary>Размер окна усреднения</summary>
    private int _Length;

    public double _Min = double.PositiveInfinity;

    public double _Max = double.NegativeInfinity;

    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Начальное значение</summary>
    public double StartValue => _StartValue;

    /// <summary>Размер окна усреднения</summary>
    public int Length { get => _Length; set => _Length = value; }

    /// <summary>Текущее значение усредняемой величины</summary>
    public double Value { get => _Value; set => AddValue(value); }

    /// <summary>Дисперсия значений</summary>
    public double Dispersion => _Value * _Value - _Value2;

    /// <summary>Количество точек усреднения</summary>
    public int ValuesCount => _N;

    public double Min => double.IsPositiveInfinity(_Min) ? double.NaN : _Min;

    public double Max => double.IsNegativeInfinity(_Max) ? double.NaN : _Max;

    public Interval Interval => double.IsPositiveInfinity(_Min) || double.IsNegativeInfinity(_Max)
        ? new()
        : new(_Min, _Max, true);

    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Инициализация нового скользящего среднего</summary>
    /// <param name="Length">Размер окна усреднения</param>
    public AverageValue(int Length = -1)
    {
        _Length     = Length;
        _N          = 0;
        _StartValue = double.NaN;
    }

    /// <summary>Инициализация нового скользящего среднего</summary>
    /// <param name="StartValue">Начальное значение для усреднения</param>
    public AverageValue(double StartValue)
    {
        _Length     = -1;
        _N          = 1;
        _StartValue = StartValue;
        _Value      = _StartValue;
    }

    /// <summary>Инициализация нового скользящего среднего</summary>
    /// <param name="StartValue">Начальное значение</param>
    /// <param name="Length">Размер окна усреднения</param>
    public AverageValue(double StartValue, int Length)
    {
        _Length     = Length;
        _N          = 1;
        _StartValue = StartValue;
        _Value      = _StartValue;
    }


    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Добавить значение к усреднению</summary>
    /// <param name="value">Добавляемое значение</param>
    public double AddValue(double value)
    {
        if (_N >= 1)
        {
            // Если указано количество итераций усреднения
            _Value  += (value - _Value) / (_N + 1);
            _Value2 += (value * value - _Value2) / (_N + 1);
            if (_Length < 0 || _N < _Length) _N++;

            if (value > _Max)
                _Max = value;

            if (value < _Min)
                _Min = value;
        }
        else
        {
            // Если количество итераций усреднения не указано
            _Value = value;
            _N++;

            if (value > _Max)
                _Max = value;

            if (value < _Min)
                _Min = value;
        }

        return _Value;
    }

    /// <summary>Сбросить состояние</summary>
    public void Reset()
    {
        _Value2 = 0;
        if (double.IsNaN(_StartValue))
        {
            _N     = 0;
            _Value = 0;
        }
        else
        {
            _N     = 1;
            _Value = _StartValue;
        }

        _Min = double.PositiveInfinity;
        _Max = double.NegativeInfinity;
    }

    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Преобразование в строку</summary>
    /// <returns>Текстовое представление</returns>
    public override string ToString() => _Value.ToString(CultureInfo.CurrentCulture);

    /// <summary>Преобразование в строку с форматированием</summary>
    /// <param name="Format">Формат</param>
    /// <returns>Текстовое представление</returns>
    public string ToString(string Format) => _Value.ToString(Format);

    /* --------------------------------------------------------------------------------------------- */

    public void Deconstruct(out double Mean, out double Variance)
    {
        Mean     = _Value;
        Variance = _Value2;
    }

    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Оператор неявного приведения к типу вещественного числа</summary>
    /// <param name="Value">Усредняемое значение</param>
    public static implicit operator double(AverageValue Value) => Value.Value;

    /// <summary>Оператор неявного приведения вещественного числа к скользящему среднему</summary>
    /// <param name="Data">Вещественное число</param>
    public static implicit operator AverageValue(double Data) => new(Data);

    public static implicit operator Interval(AverageValue Value) => Value.Interval;

    /* --------------------------------------------------------------------------------------------- */

    #region ISerializable Members

    /// <summary>Новая усредняемая величина</summary>
    /// <param name="info">Сериализационная информация</param>
    /// <param name="context">Контекст сериализации</param>
    protected AverageValue(SerializationInfo info, StreamingContext context)
    {
        if (info is null) throw new ArgumentNullException(nameof(info));
        
        _Value = info.GetDouble("Value");
        _N     = info.GetInt32("N");
        Length = info.GetInt32("Length");
    }

    /// <inheritdoc />
#if !NET8_0_OR_GREATER
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)] 
#endif
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info is null) throw new ArgumentNullException(nameof(info));
        GetObjectData(info, context);
    }

    /// <summary>Получить состояние объекта</summary>
    /// <param name="info">Объект сериализации</param>
    /// <param name="context">Контекст операции сериализации</param>
    /// <exception cref="ArgumentNullException">Если <paramref name="info"/> is null</exception>
#if !NET8_0_OR_GREATER
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)] 
#endif
    // ReSharper disable once UnusedParameter.Global
    protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info is null) throw new ArgumentNullException(nameof(info));

        info.AddValue("Value", _Value);
        info.AddValue("N", _N);
        info.AddValue("Length", Length);
    }

    #endregion

    /* --------------------------------------------------------------------------------------------- */
}