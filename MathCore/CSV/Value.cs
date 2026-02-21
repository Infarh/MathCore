using System.Globalization;

namespace MathCore.CSV;

/// <summary>
/// Обёртка для строкового значения из CSV с методами преобразования в различные типы данных
/// </summary>
/// <remarks>
/// Структура предоставляет удобный способ преобразования строковых значений из CSV-данных
/// в типизированные значения (целые числа, вещественные числа, логические значения, перечисления и т.д.).
/// Поддерживает как обязательные преобразования (выбрасывают исключение при ошибке),
/// так и опциональные (возвращают null при ошибке).
/// 
/// Примеры использования:
/// - Прямое преобразование: Value val = new("42", CultureInfo.CurrentCulture); int i = val.Int32Value;
/// - Через неявное преобразование: int i = val; // использует встроенные операторы
/// - С обработкой ошибок: int? i = val.AsInt32OrNull();
/// - С значением по умолчанию: int i = val.AsInt32OrDefault(0);
/// </remarks>
public readonly ref struct Value(string value, IFormatProvider Culture)
{
    /// <summary>
    /// Получить строковое значение с удалёнными пробелами и кавычками
    /// </summary>
    /// <value>Обрезанное строковое значение</value>
    public string TrimmedStringValue => value.Trim(' ', '\'', '"');

    /// <summary>
    /// Получить исходное строковое значение без обработки
    /// </summary>
    /// <value>Исходное строковое значение</value>
    public string StringValue => value;

    /// <summary>Получить значение как беззнаковое 8-битное целое число</summary>
    /// <value>Преобразованное значение типа byte</value>
    public byte Int8Value => byte.Parse(TrimmedStringValue, Culture);
    /// <summary>Получить значение как знаковое 8-битное целое число</summary>
    /// <value>Преобразованное значение типа sbyte</value>
    public sbyte SInt8Value => sbyte.Parse(TrimmedStringValue, Culture);
    /// <summary>Получить значение как знаковое 16-битное целое число</summary>
    /// <value>Преобразованное значение типа short</value>
    public short Int16Value => short.Parse(TrimmedStringValue, Culture);
    /// <summary>Получить значение как беззнаковое 16-битное целое число</summary>
    /// <value>Преобразованное значение типа ushort</value>
    public ushort UInt16Value => ushort.Parse(TrimmedStringValue, Culture);
    /// <summary>Получить значение как знаковое 32-битное целое число</summary>
    /// <value>Преобразованное значение типа int</value>
    public int Int32Value => int.Parse(TrimmedStringValue, Culture);
    /// <summary>Получить значение как беззнаковое 32-битное целое число</summary>
    /// <value>Преобразованное значение типа uint</value>
    public uint UInt32Value => uint.Parse(TrimmedStringValue, Culture);
    /// <summary>Получить значение как знаковое 64-битное целое число</summary>
    /// <value>Преобразованное значение типа long</value>
    public long Int64Value => long.Parse(TrimmedStringValue, Culture);
    /// <summary>Получить значение как беззнаковое 64-битное целое число</summary>
    /// <value>Преобразованное значение типа ulong</value>
    public ulong UInt64Value => ulong.Parse(TrimmedStringValue, Culture);

    /// <summary>Получить значение как число одинарной точности (float), возвращает NaN для пустых строк</summary>
    /// <value>Преобразованное значение типа float</value>
    public float FloatValue => TrimmedStringValue is { Length: > 0 } ? float.Parse(TrimmedStringValue, Culture) : float.NaN;
    /// <summary>Получить значение как число двойной точности (double), возвращает NaN для пустых строк</summary>
    /// <value>Преобразованное значение типа double</value>
    public double DoubleValue => TrimmedStringValue is { Length: > 0 } ? double.Parse(TrimmedStringValue, Culture) : double.NaN;
    /// <summary>Получить значение как десятичное число высокой точности</summary>
    /// <value>Преобразованное значение типа decimal</value>
    public decimal DecimalValue => decimal.Parse(TrimmedStringValue, Culture);

    /// <summary>Получить значение как логическое значение</summary>
    /// <value>Преобразованное значение типа bool</value>
    public bool BoolValue => bool.Parse(TrimmedStringValue);

    /* ------------------------------------------------------------------------------------------------------------- */

    /// <summary>Получить значение как nullable беззнаковое 8-битное целое число</summary>
    /// <value>Преобразованное значение типа byte? или null если преобразование не удалось</value>
    public byte? Int8NullValue => byte.TryParse(TrimmedStringValue, NumberStyles.Any, Culture, out var v) ? v : null;
    /// <summary>Получить значение как nullable знаковое 8-битное целое число</summary>
    /// <value>Преобразованное значение типа sbyte? или null если преобразование не удалось</value>
    public sbyte? SInt8NullValue => sbyte.TryParse(TrimmedStringValue, NumberStyles.Any, Culture, out var v) ? v : null;
    /// <summary>Получить значение как nullable знаковое 16-битное целое число</summary>
    /// <value>Преобразованное значение типа short? или null если преобразование не удалось</value>
    public short? Int16NullValue => short.TryParse(TrimmedStringValue, NumberStyles.Any, Culture, out var v) ? v : null;
    /// <summary>Получить значение как nullable беззнаковое 16-битное целое число</summary>
    /// <value>Преобразованное значение типа ushort? или null если преобразование не удалось</value>
    public ushort? UInt16NullValue => ushort.TryParse(TrimmedStringValue, NumberStyles.Any, Culture, out var v) ? v : null;
    /// <summary>Получить значение как nullable знаковое 32-битное целое число</summary>
    /// <value>Преобразованное значение типа int? или null если преобразование не удалось</value>
    public int? Int32NullValue => int.TryParse(TrimmedStringValue, NumberStyles.Any, Culture, out var v) ? v : null;
    /// <summary>Получить значение как nullable беззнаковое 32-битное целое число</summary>
    /// <value>Преобразованное значение типа uint? или null если преобразование не удалось</value>
    public uint? UInt32NullValue => uint.TryParse(TrimmedStringValue, NumberStyles.Any, Culture, out var v) ? v : null;
    /// <summary>Получить значение как nullable знаковое 64-битное целое число</summary>
    /// <value>Преобразованное значение типа long? или null если преобразование не удалось</value>
    public long? Int64NullValue => long.TryParse(TrimmedStringValue, NumberStyles.Any, Culture, out var v) ? v : null;
    /// <summary>Получить значение как nullable беззнаковое 64-битное целое число</summary>
    /// <value>Преобразованное значение типа ulong? или null если преобразование не удалось</value>
    public ulong? UInt64NullValue => ulong.TryParse(TrimmedStringValue, NumberStyles.Any, Culture, out var v) ? v : null;

    /// <summary>Получить значение как nullable число одинарной точности</summary>
    /// <value>Преобразованное значение типа float? или null если преобразование не удалось</value>
    public float? FloatNullValue => float.TryParse(TrimmedStringValue, NumberStyles.Any, Culture, out var v) ? v : null;
    /// <summary>Получить значение как nullable число двойной точности</summary>
    /// <value>Преобразованное значение типа double? или null если преобразование не удалось</value>
    public double? DoubleNullValue => double.TryParse(TrimmedStringValue, NumberStyles.Any, Culture, out var v) ? v : null;
    /// <summary>Получить значение как nullable десятичное число</summary>
    /// <value>Преобразованное значение типа decimal? или null если преобразование не удалось</value>
    public decimal? DecimalNullValue => decimal.TryParse(TrimmedStringValue, NumberStyles.Any, Culture, out var v) ? v : null;

    /// <summary>Получить значение как nullable логическое значение</summary>
    /// <value>Преобразованное значение типа bool? или null если преобразование не удалось</value>
    public bool? BoolNullValue => bool.TryParse(TrimmedStringValue, out var v) ? v : null;

    /* ------------------------------------------------------------------------------------------------------------- */

    /// <summary>Преобразовать в беззнаковое 8-битное целое число с указанным форматом</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public byte AsInt8(IFormatProvider provider) => byte.Parse(TrimmedStringValue, provider);
    /// <summary>Преобразовать в беззнаковое 8-битное целое число с указанным стилем и форматом</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public byte AsInt8(NumberStyles Style, IFormatProvider provider) => byte.Parse(TrimmedStringValue, Style, provider);
    /// <summary>Преобразовать в знаковое 8-битное целое число</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public sbyte AsSInt8(IFormatProvider provider) => sbyte.Parse(TrimmedStringValue, provider);
    /// <summary>Преобразовать в знаковое 8-битное целое число с указанным стилем</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public sbyte AsSInt8(NumberStyles Style, IFormatProvider provider) => sbyte.Parse(TrimmedStringValue, Style, provider);
    /// <summary>Преобразовать в знаковое 16-битное целое число</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public short AsInt16(IFormatProvider provider) => short.Parse(TrimmedStringValue, provider);
    /// <summary>Преобразовать в знаковое 16-битное целое число с указанным стилем</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public short AsInt16(NumberStyles Style, IFormatProvider provider) => short.Parse(TrimmedStringValue, Style, provider);
    /// <summary>Преобразовать в беззнаковое 16-битное целое число</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public ushort AsUInt16(IFormatProvider provider) => ushort.Parse(TrimmedStringValue, provider);
    /// <summary>Преобразовать в беззнаковое 16-битное целое число с указанным стилем</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public ushort AsUInt16(NumberStyles Style, IFormatProvider provider) => ushort.Parse(TrimmedStringValue, Style, provider);
    /// <summary>Преобразовать в знаковое 32-битное целое число</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public int AsInt32(IFormatProvider provider) => int.Parse(TrimmedStringValue, provider);
    /// <summary>Преобразовать в знаковое 32-битное целое число с указанным стилем</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public int AsInt32(NumberStyles Style, IFormatProvider provider) => int.Parse(TrimmedStringValue, Style, provider);
    /// <summary>Преобразовать в беззнаковое 32-битное целое число</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public uint AsUInt32(IFormatProvider provider) => uint.Parse(TrimmedStringValue, provider);
    /// <summary>Преобразовать в беззнаковое 32-битное целое число с указанным стилем</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public uint AsUInt32(NumberStyles Style, IFormatProvider provider) => uint.Parse(TrimmedStringValue, Style, provider);
    /// <summary>Преобразовать в знаковое 64-битное целое число</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public long AsInt64(IFormatProvider provider) => long.Parse(TrimmedStringValue, provider);
    /// <summary>Преобразовать в знаковое 64-битное целое число с указанным стилем</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public long AsInt64(NumberStyles Style, IFormatProvider provider) => long.Parse(TrimmedStringValue, Style, provider);
    /// <summary>Преобразовать в беззнаковое 64-битное целое число с указанным стилем</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public ulong AsUInt64(NumberStyles Style, IFormatProvider provider) => ulong.Parse(TrimmedStringValue, Style, provider);

    /// <summary>Преобразовать в число одинарной точности</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public float AsFloat(IFormatProvider provider) => float.Parse(TrimmedStringValue, provider);
    /// <summary>Преобразовать в число одинарной точности с указанным стилем</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public float AsFloat(NumberStyles Style, IFormatProvider provider) => float.Parse(TrimmedStringValue, Style, provider);
    /// <summary>Преобразовать в число двойной точности</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public double AsDouble(IFormatProvider provider) => double.Parse(TrimmedStringValue, provider);
    /// <summary>Преобразовать в число двойной точности с указанным стилем</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public double AsDouble(NumberStyles Style, IFormatProvider provider) => double.Parse(TrimmedStringValue, Style, provider);
    /// <summary>Преобразовать в десятичное число</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public decimal AsDecimal(IFormatProvider provider) => decimal.Parse(TrimmedStringValue, provider);
    /// <summary>Преобразовать в десятичное число с указанным стилем</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public decimal AsDecimal(NumberStyles Style, IFormatProvider provider) => decimal.Parse(TrimmedStringValue, Style, provider);

    /* ------------------------------------------------------------------------------------------------------------- */

    /// <summary>Попытать преобразовать в беззнаковое 8-битное целое число, возвращая null при ошибке</summary>
    /// <returns>Преобразованное значение или null</returns>
    public byte? AsInt8OrNull() => byte.TryParse(TrimmedStringValue, out var v) ? v : null;
    /// <summary>Попытать преобразовать в беззнаковое 8-битное целое число с указанным форматом</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public byte? AsInt8OrNull(IFormatProvider provider) => byte.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : null;
    /// <summary>Попытать преобразовать в беззнаковое 8-битное целое число с указанным стилем и форматом</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public byte? AsInt8OrNull(NumberStyles Style, IFormatProvider provider) => byte.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : null;
    /// <summary>Преобразовать в беззнаковое 8-битное целое число, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public byte AsInt8OrDefault(byte Default) => byte.TryParse(TrimmedStringValue, out var v) ? v : Default;
    /// <summary>Преобразовать в беззнаковое 8-битное целое число с указанным форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public byte AsInt8OrDefault(IFormatProvider provider, byte Default) => byte.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : Default;
    /// <summary>Преобразовать в беззнаковое 8-битное целое число с указанным стилем и форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public byte AsInt8OrDefault(NumberStyles Style, IFormatProvider provider, byte Default) => byte.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : Default;

    /// <summary>Попытать преобразовать в знаковое 8-битное целое число, возвращая null при ошибке</summary>
    /// <returns>Преобразованное значение или null</returns>
    public sbyte? AsSInt8OrNull() => sbyte.TryParse(TrimmedStringValue, out var v) ? v : null;
    /// <summary>Попытать преобразовать в знаковое 8-битное целое число с указанным форматом</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public sbyte? AsSInt8OrNull(IFormatProvider provider) => sbyte.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : null;
    /// <summary>Попытать преобразовать в знаковое 8-битное целое число с указанным стилем и форматом</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public sbyte? AsSInt8OrNull(NumberStyles Style, IFormatProvider provider) => sbyte.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : null;
    /// <summary>Преобразовать в знаковое 8-битное целое число, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public sbyte AsSInt8OrDefault(sbyte Default) => sbyte.TryParse(TrimmedStringValue, out var v) ? v : Default;
    /// <summary>Преобразовать в знаковое 8-битное целое число с указанным форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public sbyte AsSInt8OrDefault(IFormatProvider provider, sbyte Default) => sbyte.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : Default;
    /// <summary>Преобразовать в знаковое 8-битное целое число с указанным стилем и форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public sbyte AsSInt8OrDefault(NumberStyles Style, IFormatProvider provider, sbyte Default) => sbyte.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : Default;

    /// <summary>Попытать преобразовать в знаковое 16-битное целое число, возвращая null при ошибке</summary>
    /// <returns>Преобразованное значение или null</returns>
    public short? AsInt16OrNull() => short.TryParse(TrimmedStringValue, out var v) ? v : null;
    /// <summary>Попытать преобразовать в знаковое 16-битное целое число с указанным форматом</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public short? AsInt16OrNull(IFormatProvider provider) => short.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : null;
    /// <summary>Попытать преобразовать в знаковое 16-битное целое число с указанным стилем и форматом</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public short? AsInt16OrNull(NumberStyles Style, IFormatProvider provider) => short.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : null;
    /// <summary>Преобразовать в знаковое 16-битное целое число, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public short AsInt16OrDefault(short Default) => short.TryParse(TrimmedStringValue, out var v) ? v : Default;
    /// <summary>Преобразовать в знаковое 16-битное целое число с указанным форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public short AsInt16OrDefault(IFormatProvider provider, short Default) => short.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : Default;
    /// <summary>Преобразовать в знаковое 16-битное целое число с указанным стилем и форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public short AsInt16OrDefault(NumberStyles Style, IFormatProvider provider, short Default) => short.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : Default;

    /// <summary>Попытать преобразовать в беззнаковое 16-битное целое число, возвращая null при ошибке</summary>
    /// <returns>Преобразованное значение или null</returns>
    public ushort? AsUInt16OrNull() => ushort.TryParse(TrimmedStringValue, out var v) ? v : null;
    /// <summary>Попытать преобразовать в беззнаковое 16-битное целое число с указанным форматом</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public ushort? AsUInt16OrNull(IFormatProvider provider) => ushort.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : null;
    /// <summary>Попытать преобразовать в беззнаковое 16-битное целое число с указанным стилем и форматом</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public ushort? AsUInt16OrNull(NumberStyles Style, IFormatProvider provider) => ushort.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : null;
    /// <summary>Преобразовать в беззнаковое 16-битное целое число, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public ushort AsUInt16OrDefault(ushort Default) => ushort.TryParse(TrimmedStringValue, out var v) ? v : Default;
    /// <summary>Преобразовать в беззнаковое 16-битное целое число с указанным форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public ushort AsUInt16OrDefault(IFormatProvider provider, ushort Default) => ushort.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : Default;
    /// <summary>Преобразовать в беззнаковое 16-битное целое число с указанным стилем и форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public ushort AsUInt16OrDefault(NumberStyles Style, IFormatProvider provider, ushort Default) => ushort.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : Default;

    /// <summary>Попытать преобразовать в знаковое 32-битное целое число, возвращая null при ошибке</summary>
    /// <returns>Преобразованное значение или null</returns>
    public int? AsInt32OrNull() => int.TryParse(TrimmedStringValue, out var v) ? v : null;
    /// <summary>Попытать преобразовать в знаковое 32-битное целое число с указанным форматом</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public int? AsInt32OrNull(IFormatProvider provider) => int.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : null;
    /// <summary>Попытать преобразовать в знаковое 32-битное целое число с указанным стилем и форматом</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public int? AsInt32OrNull(NumberStyles Style, IFormatProvider provider) => int.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : null;
    /// <summary>Преобразовать в знаковое 32-битное целое число, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public int AsInt32OrDefault(int Default) => int.TryParse(TrimmedStringValue, out var v) ? v : Default;
    /// <summary>Преобразовать в знаковое 32-битное целое число с указанным форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public int AsInt32OrDefault(IFormatProvider provider, int Default) => int.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : Default;
    /// <summary>Преобразовать в знаковое 32-битное целое число с указанным стилем и форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public int AsInt32OrDefault(NumberStyles Style, IFormatProvider provider, int Default) => int.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : Default;

    /// <summary>Попытать преобразовать в беззнаковое 32-битное целое число, возвращая null при ошибке</summary>
    /// <returns>Преобразованное значение или null</returns>
    public uint? AsUInt32OrNull() => uint.TryParse(TrimmedStringValue, out var v) ? v : null;
    /// <summary>Попытать преобразовать в беззнаковое 32-битное целое число с указанным форматом</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public uint? AsUInt32OrNull(IFormatProvider provider) => uint.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : null;
    /// <summary>Попытать преобразовать в беззнаковое 32-битное целое число с указанным стилем и форматом</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public uint? AsUInt32OrNull(NumberStyles Style, IFormatProvider provider) => uint.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : null;
    /// <summary>Преобразовать в беззнаковое 32-битное целое число, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public uint AsUInt32OrDefault(uint Default) => uint.TryParse(TrimmedStringValue, out var v) ? v : Default;
    /// <summary>Преобразовать в беззнаковое 32-битное целое число с указанным форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public uint AsUInt32OrDefault(IFormatProvider provider, uint Default) => uint.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : Default;
    /// <summary>Преобразовать в беззнаковое 32-битное целое число с указанным стилем и форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public uint AsUInt32OrDefault(NumberStyles Style, IFormatProvider provider, uint Default) => uint.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : Default;

    /// <summary>Попытать преобразовать в знаковое 64-битное целое число, возвращая null при ошибке</summary>
    /// <returns>Преобразованное значение или null</returns>
    public long? AsInt64OrNull() => long.TryParse(TrimmedStringValue, out var v) ? v : null;
    /// <summary>Попытать преобразовать в знаковое 64-битное целое число с указанным форматом</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public long? AsInt64OrNull(IFormatProvider provider) => long.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : null;
    /// <summary>Попытать преобразовать в знаковое 64-битное целое число с указанным стилем и форматом</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public long? AsInt64OrNull(NumberStyles Style, IFormatProvider provider) => long.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : null;
    /// <summary>Преобразовать в знаковое 64-битное целое число, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public long AsInt64OrDefault(long Default) => long.TryParse(TrimmedStringValue, out var v) ? v : Default;
    /// <summary>Преобразовать в знаковое 64-битное целое число с указанным форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public long AsInt64OrDefault(IFormatProvider provider, long Default) => long.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : Default;
    /// <summary>Преобразовать в знаковое 64-битное целое число с указанным стилем и форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public long AsInt64OrDefault(NumberStyles Style, IFormatProvider provider, long Default) => long.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : Default;

    /// <summary>Попытать преобразовать в беззнаковое 64-битное целое число, возвращая null при ошибке</summary>
    /// <returns>Преобразованное значение или null</returns>
    public ulong? AsUInt64OrNull() => ulong.TryParse(TrimmedStringValue, out var v) ? v : null;
    /// <summary>Попытать преобразовать в беззнаковое 64-битное целое число с указанным форматом</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public ulong? AsUInt64OrNull(IFormatProvider provider) => ulong.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : null;
    /// <summary>Попытать преобразовать в беззнаковое 64-битное целое число с указанным стилем и форматом</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public ulong? AsUInt64OrNull(NumberStyles Style, IFormatProvider provider) => ulong.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : null;
    /// <summary>Преобразовать в беззнаковое 64-битное целое число, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public ulong AsUInt64OrDefault(ulong Default) => ulong.TryParse(TrimmedStringValue, out var v) ? v : Default;
    /// <summary>Преобразовать в беззнаковое 64-битное целое число с указанным форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public ulong AsUInt64OrDefault(IFormatProvider provider, ulong Default) => ulong.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : Default;
    /// <summary>Преобразовать в беззнаковое 64-битное целое число с указанным стилем и форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public ulong AsUInt64OrDefault(NumberStyles Style, IFormatProvider provider, ulong Default) => ulong.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : Default;

    /// <summary>Попытать преобразовать в число одинарной точности, возвращая null при ошибке</summary>
    /// <returns>Преобразованное значение или null</returns>
    public float? AsFloatOrNull() => float.TryParse(TrimmedStringValue, out var v) ? v : null;
    /// <summary>Попытать преобразовать в число одинарной точности с указанным форматом</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public float? AsFloatOrNull(IFormatProvider provider) => float.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : null;
    /// <summary>Попытать преобразовать в число одинарной точности с указанным стилем и форматом</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public float? AsFloatOrNull(NumberStyles Style, IFormatProvider provider) => float.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : null;
    /// <summary>Преобразовать в число одинарной точности, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public float AsFloatOrDefault(float Default) => float.TryParse(TrimmedStringValue, out var v) ? v : Default;
    /// <summary>Преобразовать в число одинарной точности с указанным форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public float AsFloatOrDefault(IFormatProvider provider, float Default) => float.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : Default;
    /// <summary>Преобразовать в число одинарной точности с указанным стилем и форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public float AsFloatOrDefault(NumberStyles Style, IFormatProvider provider, float Default) => float.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : Default;

    /// <summary>Попытать преобразовать в число двойной точности, возвращая null при ошибке</summary>
    /// <returns>Преобразованное значение или null</returns>
    public double? AsDoubleOrNull() => double.TryParse(TrimmedStringValue, out var v) ? v : null;
    /// <summary>Попытать преобразовать в число двойной точности с указанным форматом</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public double? AsDoubleOrNull(IFormatProvider provider) => double.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : null;
    /// <summary>Попытать преобразовать в число двойной точности с указанным стилем и форматом</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public double? AsDoubleOrNull(NumberStyles Style, IFormatProvider provider) => double.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : null;
    /// <summary>Преобразовать в число двойной точности, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public double AsDoubleOrDefault(double Default) => double.TryParse(TrimmedStringValue, out var v) ? v : Default;
    /// <summary>Преобразовать в число двойной точности с указанным форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public double AsDoubleOrDefault(IFormatProvider provider, double Default) => double.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : Default;
    /// <summary>Преобразовать в число двойной точности с указанным стилем и форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public double AsDoubleOrDefault(NumberStyles Style, IFormatProvider provider, double Default) => double.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : Default;

    /// <summary>Попытать преобразовать в десятичное число, возвращая null при ошибке</summary>
    /// <returns>Преобразованное значение или null</returns>
    public decimal? AsDecimalOrNull() => decimal.TryParse(TrimmedStringValue, out var v) ? v : null;
    /// <summary>Попытать преобразовать в десятичное число с указанным форматом</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public decimal? AsDecimalOrNull(IFormatProvider provider) => decimal.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : null;
    /// <summary>Попытать преобразовать в десятичное число с указанным стилем и форматом</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <returns>Преобразованное значение или null</returns>
    public decimal? AsDecimalOrNull(NumberStyles Style, IFormatProvider provider) => decimal.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : null;
    /// <summary>Преобразовать в десятичное число, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public decimal AsDecimalOrDefault(decimal Default) => decimal.TryParse(TrimmedStringValue, out var v) ? v : Default;
    /// <summary>Преобразовать в десятичное число с указанным форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public decimal AsDecimalOrDefault(IFormatProvider provider, decimal Default) => decimal.TryParse(TrimmedStringValue, NumberStyles.Any, provider, out var v) ? v : Default;
    /// <summary>Преобразовать в десятичное число с указанным стилем и форматом, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Style">Стиль числового формата</param>
    /// <param name="provider">Поставщик формата для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public decimal AsDecimalOrDefault(NumberStyles Style, IFormatProvider provider, decimal Default) => decimal.TryParse(TrimmedStringValue, Style, provider, out var v) ? v : Default;

    /// <summary>Попытать преобразовать в логическое значение, возвращая null при ошибке</summary>
    /// <returns>Преобразованное значение или null</returns>
    public bool? AsBoolOrNull() => bool.TryParse(TrimmedStringValue, out var v) ? v : null;
    /// <summary>Преобразовать в логическое значение, возвращая значение по умолчанию при ошибке</summary>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или значение по умолчанию</returns>
    public bool AsBoolOrDefault(bool Default) => bool.TryParse(TrimmedStringValue, out var v) ? v : Default;

    /// <summary>Преобразовать в значение перечисления (Enum)</summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    /// <returns>Преобразованное значение</returns>
    public T AsEnum<T>() where T : Enum => (T)Enum.Parse(typeof(T), TrimmedStringValue);

#if NET8_0_OR_GREATER
    /// <summary>Преобразовать в значение перечисления с опциональным игнорированием регистра</summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    /// <param name="IgnoreCase">Игнорировать регистр букв при сравнении</param>
    /// <returns>Преобразованное значение</returns>
    public T AsEnum<T>(bool IgnoreCase) where T : struct, Enum => Enum.Parse<T>(TrimmedStringValue, IgnoreCase);
#else
    /// <summary>Преобразовать в значение перечисления с опциональным игнорированием регистра</summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    /// <param name="IgnoreCase">Игнорировать регистр букв при сравнении</param>
    /// <returns>Преобразованное значение</returns>
    public T AsEnum<T>(bool IgnoreCase) where T : struct, Enum => (T)Enum.Parse(typeof(T), TrimmedStringValue, IgnoreCase);
#endif

    /// <summary>Попытаться преобразовать в значение перечисления, возвращая null при ошибке</summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    /// <returns>Преобразованное значение или null</returns>
    public T? AsEnumOrNull<T>() where T : struct, Enum => Enum.TryParse(TrimmedStringValue, out T v) ? v : null;
    /// <summary>Попытать преобразовать в значение перечисления с опциональным игнорированием регистра, возвращая null при ошибке</summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    /// <param name="IgnoreCase">Игнорировать регистр букв при сравнении</param>
    /// <returns>Преобразованное значение или null</returns>
    public T? AsEnumOrNull<T>(bool IgnoreCase) where T : struct, Enum => Enum.TryParse(TrimmedStringValue, IgnoreCase, out T v) ? v : null;

    /// <summary>Преобразовать в произвольный тип с использованием Convert.ChangeType</summary>
    /// <typeparam name="T">Целевой тип преобразования</typeparam>
    /// <returns>Преобразованное значение</returns>
    public T As<T>() => (T)Convert.ChangeType(TrimmedStringValue, typeof(T));

    /* ------------------------------------------------------------------------------------------------------------- */

    /// <summary>Неявное преобразование в строку</summary>
    public static implicit operator string(in Value value) => value.StringValue;
    /// <summary>Неявное преобразование в логическое значение</summary>
    public static implicit operator bool(in Value value) => value.BoolValue;
    /// <summary>Неявное преобразование в nullable логическое значение</summary>
    public static implicit operator bool?(in Value value) => value.AsBoolOrNull();

    /// <summary>Неявное преобразование в беззнаковое 8-битное целое число</summary>
    public static implicit operator byte(in Value value) => value.Int8Value;
    /// <summary>Неявное преобразование в знаковое 8-битное целое число</summary>
    public static implicit operator sbyte(in Value value) => value.SInt8Value;
    /// <summary>Неявное преобразование в знаковое 16-битное целое число</summary>
    public static implicit operator short(in Value value) => value.Int16Value;
    /// <summary>Неявное преобразование в беззнаковое 16-битное целое число</summary>
    public static implicit operator ushort(in Value value) => value.UInt16Value;
    /// <summary>Неявное преобразование в знаковое 32-битное целое число</summary>
    public static implicit operator int(in Value value) => value.Int32Value;
    /// <summary>Неявное преобразование в беззнаковое 32-битное целое число</summary>
    public static implicit operator uint(in Value value) => value.UInt32Value;
    /// <summary>Неявное преобразование в знаковое 64-битное целое число</summary>
    public static implicit operator long(in Value value) => value.Int64Value;
    /// <summary>Неявное преобразование в беззнаковое 64-битное целое число</summary>
    public static implicit operator ulong(in Value value) => value.UInt64Value;

    /// <summary>Неявное преобразование в число одинарной точности</summary>
    public static implicit operator float(in Value value) => value.FloatValue;
    /// <summary>Неявное преобразование в число двойной точности</summary>
    public static implicit operator double(in Value value) => value.DoubleValue;
    /// <summary>Неявное преобразование в десятичное число</summary>
    public static implicit operator decimal(in Value value) => value.DecimalValue;

    /// <summary>Неявное преобразование в nullable беззнаковое 8-битное целое число</summary>
    public static implicit operator byte?(in Value value) => value.Int8NullValue;
    /// <summary>Неявное преобразование в nullable знаковое 8-битное целое число</summary>
    public static implicit operator sbyte?(in Value value) => value.SInt8NullValue;
    /// <summary>Неявное преобразование в nullable знаковое 16-битное целое число</summary>
    public static implicit operator short?(in Value value) => value.Int16NullValue;
    /// <summary>Неявное преобразование в nullable беззнаковое 16-битное целое число</summary>
    public static implicit operator ushort?(in Value value) => value.UInt16NullValue;
    /// <summary>Неявное преобразование в nullable знаковое 32-битное целое число</summary>
    public static implicit operator int?(in Value value) => value.Int32NullValue;
    /// <summary>Неявное преобразование в nullable беззнаковое 32-битное целое число</summary>
    public static implicit operator uint?(in Value value) => value.UInt32NullValue;
    /// <summary>Неявное преобразование в nullable знаковое 64-битное целое число</summary>
    public static implicit operator long?(in Value value) => value.Int64NullValue;
    /// <summary>Неявное преобразование в nullable беззнаковое 64-битное целое число</summary>
    public static implicit operator ulong?(in Value value) => value.UInt64NullValue;

    /// <summary>Неявное преобразование в nullable число одинарной точности</summary>
    public static implicit operator float?(in Value value) => value.FloatNullValue;
    /// <summary>Неявное преобразование в nullable число двойной точности</summary>
    public static implicit operator double?(in Value value) => value.DoubleNullValue;
    /// <summary>Неявное преобразование в nullable десятичное число</summary>
    public static implicit operator decimal?(in Value value) => value.DecimalNullValue;
}