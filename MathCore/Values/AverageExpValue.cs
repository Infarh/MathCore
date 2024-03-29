﻿#nullable enable
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace MathCore.Values;

/// <summary>Экспоненциальное скользящее среднее y = (a * x) + (1-a) * y</summary>
[Serializable]
public class AverageExpValue : ISerializable, IValue<double>, IResettable
{
    private static double CheckFactorRange(double Factor, string? ParameterName = null) => Factor switch
    {
        double.NaN   => throw new InvalidOperationException("Фактор сглаживания не является числом"),
        <= 0 or >= 1 => throw new ArgumentOutOfRangeException(ParameterName ?? nameof(Factor), "Фактор сглаживания должен быть больше 0 и меньше 1"),
        _            => Factor
    };

    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Номер итерации усреднения</summary>
    private int _N;
    /// <summary>Текущее значение усредняемой величины</summary>
    private double _Value;

    private double _Value2;

    /// <summary>Начальное значение</summary>
    private readonly double _StartValue;

    /// <summary>Сглаживающий фактор</summary>
    private double _Factor;

    public double _Min = double.PositiveInfinity;

    public double _Max = double.NegativeInfinity;

    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Начальное значение</summary>
    public double StartValue => _StartValue;

    /// <summary>Сглаживающий фактор</summary>
    public double Factor { get => _Factor; set => _Factor = CheckFactorRange(value, nameof(value)); }

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
    /// <param name="Factor">Сглаживающий фактор</param>
    public AverageExpValue(double Factor)
    {
        _Factor     = CheckFactorRange(Factor);
        _N          = 0;
        _StartValue = StartValue;
    }

    /// <summary>Инициализация нового скользящего среднего</summary>
    public AverageExpValue(double Factor, double StartValue)
    {
        _Factor     = CheckFactorRange(Factor);
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
            _Value  = (1 - _Factor) * _Value + _Factor * value;
            _Value2 = (1 - _Factor) * _Value2 + _Factor * (value * value - _Value2);
            _N++;

            if(value > _Max)
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

    /// <summary>Оператор неявного приведения к типу вещественного числа</summary>
    /// <param name="Value">Усредняемое значение</param>
    public static implicit operator double(AverageExpValue Value) => Value.Value;

    /// <summary>Оператор неявного приведения вещественного числа к скользящему среднему</summary>
    /// <param name="Data">Вещественное число</param>
    public static implicit operator AverageExpValue(double Data) => new(Data);

    public static implicit operator Interval(AverageExpValue Value) => Value.Interval;

    /* --------------------------------------------------------------------------------------------- */

    #region ISerializable Members

    /// <summary>Новая усредняемая величина</summary>
    /// <param name="info">Сериализационная информация</param>
    /// <param name="context">Контекст сериализации</param>
    protected AverageExpValue(SerializationInfo info, StreamingContext context)
    {
        if (info is null) throw new ArgumentNullException(nameof(info));
        _Value = info.GetDouble("Value");
        _N     = info.GetInt32("N");
        Factor = info.GetInt32("Length");
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
        info.AddValue("Length", Factor);
    }

    #endregion

    /* --------------------------------------------------------------------------------------------- */
}