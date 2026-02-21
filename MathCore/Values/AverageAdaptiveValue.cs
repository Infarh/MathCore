using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace MathCore.Values;

/// <summary>Адаптивное скользящее среднее с обнаружением скачков статистики</summary>
/// <remarks>
/// Алгоритм автоматически обнаруживает резкие изменения в потоке данных и ускоряет адаптацию.
/// В стационарном режиме обеспечивает плавное усреднение шумов и флуктуаций.
/// </remarks>
/// <example>
/// Пример использования для усреднения скорости чтения файла с сетевого диска:
/// <code><![CDATA[
/// var speed_average = new AverageAdaptiveValue(
///     BaseFactor: 0.15,              // Плавное усреднение в стабильном режиме
///     FastFactor: 0.5,               // Быстрая адаптация при изменении скорости сети
///     JumpThreshold: 3.0,            // Порог обнаружения скачка (3σ)
///     MinSamplesForDetection: 10     // Начать обнаружение после 10 измерений
/// );
/// 
/// while (reading)
/// {
///     var bytes_read = stream.Read(buffer, 0, 4096);
///     var speed = bytes_read / elapsed_time.TotalSeconds;
///     var avg_speed = speed_average.AddValue(speed);
///     Console.WriteLine($"Текущая скорость: {avg_speed:F2} Б/с");
/// }
/// ]]></code>
/// 
/// Пример использования для индикатора вертикальной скорости самолёта:
/// <code><![CDATA[
/// var vspeed_indicator = new AverageAdaptiveValue(
///     BaseFactor: 0.05,              // Плавное усреднение колебаний стрелки
///     FastFactor: 0.6,               // Быстрая реакция на смену режима полёта
///     JumpThreshold: 2.5,            // Чувствительность к резким изменениям
///     MinSamplesForDetection: 5      // Быстрое обнаружение смены режима
/// );
/// 
/// void UpdateVerticalSpeed(double current_vspeed)
/// {
///     var smoothed_vspeed = vspeed_indicator.AddValue(current_vspeed);
///     indicator.SetValue(smoothed_vspeed);
/// }
/// ]]></code>
/// </example>
[Serializable]
public class AverageAdaptiveValue : ISerializable, IValue<double>, IResettable
{
    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Номер итерации усреднения</summary>
    private int _N;

    /// <summary>Текущее значение среднего</summary>
    private double _Value;

    /// <summary>Текущее значение дисперсии</summary>
    private double _Value2;

    /// <summary>Начальное значение</summary>
    private readonly double _StartValue;

    /// <summary>Базовый фактор сглаживания для стационарного режима</summary>
    private double _BaseFactor;

    /// <summary>Фактор сглаживания при обнаружении скачка</summary>
    private double _FastFactor;

    /// <summary>Порог обнаружения скачка (в количестве стандартных отклонений)</summary>
    private double _JumpThreshold;

    /// <summary>Минимальное количество точек для начала обнаружения скачков</summary>
    private int _MinSamplesForDetection;

    public double _Min = double.PositiveInfinity;

    public double _Max = double.NegativeInfinity;

    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Начальное значение</summary>
    public double StartValue => _StartValue;

    /// <summary>Базовый фактор сглаживания (для стационарного режима, 0-1)</summary>
    public double BaseFactor
    {
        get => _BaseFactor;
        set => _BaseFactor = value is > 0 and < 1
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "Фактор должен быть в диапазоне (0, 1)");
    }

    /// <summary>Фактор сглаживания при обнаружении скачка (0-1, должен быть больше BaseFactor)</summary>
    public double FastFactor
    {
        get => _FastFactor;
        set => _FastFactor = value is > 0 and < 1
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "Фактор должен быть в диапазоне (0, 1)");
    }

    /// <summary>Порог обнаружения скачка (в количестве стандартных отклонений)</summary>
    public double JumpThreshold
    {
        get => _JumpThreshold;
        set => _JumpThreshold = value > 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "Порог должен быть положительным");
    }

    /// <summary>Минимальное количество точек для начала обнаружения скачков</summary>
    public int MinSamplesForDetection
    {
        get => _MinSamplesForDetection;
        set => _MinSamplesForDetection = value > 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "Должно быть положительным");
    }

    /// <summary>Текущее значение среднего</summary>
    public double Value { get => _Value; set => AddValue(value); }

    /// <summary>Дисперсия значений</summary>
    public double Dispersion => _Value2 - _Value * _Value;

    /// <summary>Стандартное отклонение</summary>
    public double StandardDeviation => Math.Sqrt(Math.Max(0, Dispersion));

    /// <summary>Количество точек усреднения</summary>
    public int ValuesCount => _N;

    /// <summary>Минимальное значение</summary>
    public double Min => double.IsPositiveInfinity(_Min) ? double.NaN : _Min;

    /// <summary>Максимальное значение</summary>
    public double Max => double.IsNegativeInfinity(_Max) ? double.NaN : _Max;

    /// <summary>Интервал значений [Min, Max]</summary>
    public Interval Interval => double.IsPositiveInfinity(_Min) || double.IsNegativeInfinity(_Max)
        ? new()
        : new(_Min, _Max, true);

    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Инициализация адаптивного скользящего среднего</summary>
    /// <param name="BaseFactor">Базовый фактор сглаживания (по умолчанию 0.1)</param>
    /// <param name="FastFactor">Быстрый фактор при скачке (по умолчанию 0.5)</param>
    /// <param name="JumpThreshold">Порог скачка в σ (по умолчанию 3.0)</param>
    /// <param name="MinSamplesForDetection">Минимум точек для обнаружения (по умолчанию 5)</param>
    public AverageAdaptiveValue(
        double BaseFactor = 0.1,
        double FastFactor = 0.5,
        double JumpThreshold = 3.0,
        int MinSamplesForDetection = 5)
    {
        _BaseFactor = BaseFactor is > 0 and < 1
            ? BaseFactor
            : throw new ArgumentOutOfRangeException(nameof(BaseFactor), "Должен быть в диапазоне (0, 1)");

        _FastFactor = FastFactor is > 0 and < 1
            ? FastFactor
            : throw new ArgumentOutOfRangeException(nameof(FastFactor), "Должен быть в диапазоне (0, 1)");

        _JumpThreshold = JumpThreshold > 0
            ? JumpThreshold
            : throw new ArgumentOutOfRangeException(nameof(JumpThreshold), "Должен быть положительным");

        _MinSamplesForDetection = MinSamplesForDetection > 0
            ? MinSamplesForDetection
            : throw new ArgumentOutOfRangeException(nameof(MinSamplesForDetection), "Должно быть положительным");

        _N = 0;
        _StartValue = double.NaN;
    }

    /// <summary>Инициализация адаптивного скользящего среднего с начальным значением</summary>
    /// <param name="StartValue">Начальное значение</param>
    /// <param name="BaseFactor">Базовый фактор сглаживания (по умолчанию 0.1)</param>
    /// <param name="FastFactor">Быстрый фактор при скачке (по умолчанию 0.5)</param>
    /// <param name="JumpThreshold">Порог скачка в σ (по умолчанию 3.0)</param>
    /// <param name="MinSamplesForDetection">Минимум точек для обнаружения (по умолчанию 5)</param>
    public AverageAdaptiveValue(
        double StartValue,
        double BaseFactor = 0.1,
        double FastFactor = 0.5,
        double JumpThreshold = 3.0,
        int MinSamplesForDetection = 5)
    {
        _BaseFactor = BaseFactor is > 0 and < 1
            ? BaseFactor
            : throw new ArgumentOutOfRangeException(nameof(BaseFactor), "Должен быть в диапазоне (0, 1)");

        _FastFactor = FastFactor is > 0 and < 1
            ? FastFactor
            : throw new ArgumentOutOfRangeException(nameof(FastFactor), "Должен быть в диапазоне (0, 1)");

        _JumpThreshold = JumpThreshold > 0
            ? JumpThreshold
            : throw new ArgumentOutOfRangeException(nameof(JumpThreshold), "Должен быть положительным");

        _MinSamplesForDetection = MinSamplesForDetection > 0
            ? MinSamplesForDetection
            : throw new ArgumentOutOfRangeException(nameof(MinSamplesForDetection), "Должно быть положительным");

        _N = 1;
        _StartValue = StartValue;
        _Value = StartValue;
        _Value2 = StartValue * StartValue;
        _Min = StartValue;
        _Max = StartValue;
    }

    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Добавить значение к усреднению с адаптивной реакцией на скачки</summary>
    /// <param name="Value">Добавляемое значение</param>
    /// <returns>Новое значение среднего</returns>
    public double AddValue(double Value)
    {
        // Обновление min/max
        if (Value > _Max)
            _Max = Value;

        if (Value < _Min)
            _Min = Value;

        if (_N < 1)
        {
            // Первое значение - инициализация
            _Value = Value;
            _Value2 = Value * Value;
            _N = 1;
            return _Value;
        }

        // Определение фактора сглаживания
        var factor = _BaseFactor;

        // Обнаружение скачка, если накоплено достаточно данных
        if (_N >= _MinSamplesForDetection)
        {
            var std_dev = StandardDeviation;

            if (std_dev > 1e-10) // Проверка, что дисперсия не нулевая
            {
                var deviation = Math.Abs(Value - _Value);
                var normalized_deviation = deviation / std_dev;

                // Если отклонение превышает порог - используем быстрый фактор
                if (normalized_deviation > _JumpThreshold)
                    factor = _FastFactor;
            }
        }

        // Обновление среднего и дисперсии по формуле экспоненциального сглаживания
        _Value = (1 - factor) * _Value + factor * Value;
        _Value2 = (1 - factor) * _Value2 + factor * Value * Value;
        _N++;

        return _Value;
    }

    /// <summary>Добавить значение к усреднению</summary>
    /// <param name="Value">Добавляемое значение</param>
    public void Add(double Value) => AddValue(Value);

    /// <summary>Сбросить состояние</summary>
    public void Reset()
    {
        _Value2 = 0;

        if (double.IsNaN(_StartValue))
        {
            _N = 0;
            _Value = 0;
        }
        else
        {
            _N = 1;
            _Value = _StartValue;
            _Value2 = _StartValue * _StartValue;
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

    /// <summary>Деконструкция на среднее и дисперсию</summary>
    /// <param name="Mean">Среднее значение</param>
    /// <param name="Variance">Дисперсия</param>
    public void Deconstruct(out double Mean, out double Variance)
    {
        Mean = _Value;
        Variance = Dispersion;
    }

    /* --------------------------------------------------------------------------------------------- */

    /// <summary>Оператор неявного приведения к типу вещественного числа</summary>
    /// <param name="Value">Усредняемое значение</param>
    public static implicit operator double(AverageAdaptiveValue Value) => Value.Value;

    /// <summary>Оператор неявного приведения вещественного числа к адаптивному среднему</summary>
    /// <param name="Data">Вещественное число</param>
    public static implicit operator AverageAdaptiveValue(double Data) => new(StartValue: Data);

    /// <summary>Оператор неявного приведения к интервалу</summary>
    /// <param name="Value">Усредняемое значение</param>
    public static implicit operator Interval(AverageAdaptiveValue Value) => Value.Interval;

    /* --------------------------------------------------------------------------------------------- */

    #region ISerializable Members

    /// <summary>Новая адаптивная усредняемая величина (десериализация)</summary>
    /// <param name="Info">Сериализационная информация</param>
    /// <param name="Context">Контекст сериализации</param>
    protected AverageAdaptiveValue(SerializationInfo Info, StreamingContext Context)
    {
        ArgumentNullException.ThrowIfNull(Info);

        _Value = Info.GetDouble("Value");
        _Value2 = Info.GetDouble("Value2");
        _N = Info.GetInt32("N");
        _StartValue = Info.GetDouble("StartValue");
        _BaseFactor = Info.GetDouble("BaseFactor");
        _FastFactor = Info.GetDouble("FastFactor");
        _JumpThreshold = Info.GetDouble("JumpThreshold");
        _MinSamplesForDetection = Info.GetInt32("MinSamplesForDetection");
        _Min = Info.GetDouble("Min");
        _Max = Info.GetDouble("Max");
    }

    /// <inheritdoc />
#if !NET8_0_OR_GREATER
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
#endif
    void ISerializable.GetObjectData(SerializationInfo Info, StreamingContext Context)
    {
        ArgumentNullException.ThrowIfNull(Info);
        GetObjectData(Info, Context);
    }

    /// <summary>Получить состояние объекта</summary>
    /// <param name="Info">Объект сериализации</param>
    /// <param name="Context">Контекст операции сериализации</param>
    /// <exception cref="ArgumentNullException">Если <paramref name="Info"/> is null</exception>
#if !NET8_0_OR_GREATER
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
#endif
    // ReSharper disable once UnusedParameter.Global
    protected virtual void GetObjectData(SerializationInfo Info, StreamingContext Context)
    {
        ArgumentNullException.ThrowIfNull(Info);

        Info.AddValue("Value", _Value);
        Info.AddValue("Value2", _Value2);
        Info.AddValue("N", _N);
        Info.AddValue("StartValue", _StartValue);
        Info.AddValue("BaseFactor", _BaseFactor);
        Info.AddValue("FastFactor", _FastFactor);
        Info.AddValue("JumpThreshold", _JumpThreshold);
        Info.AddValue("MinSamplesForDetection", _MinSamplesForDetection);
        Info.AddValue("Min", _Min);
        Info.AddValue("Max", _Max);
    }

    #endregion

    /* --------------------------------------------------------------------------------------------- */
}
