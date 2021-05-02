#nullable enable
using System;
using System.Globalization;

namespace MathCore.CSV
{
    public readonly ref struct Value
    {
        private readonly string _Value;
        private readonly CultureInfo _Culture;

        public string StringValue => _Value;

        public byte Int8Value => byte.Parse(_Value, _Culture);
        public sbyte SInt8Value => sbyte.Parse(_Value, _Culture);
        public short Int16Value => short.Parse(_Value, _Culture);
        public ushort UInt16Value => ushort.Parse(_Value, _Culture);
        public int Int32Value => int.Parse(_Value, _Culture);
        public uint UInt32Value => uint.Parse(_Value, _Culture);
        public long Int64Value => long.Parse(_Value, _Culture);
        public ulong UInt64Value => ulong.Parse(_Value, _Culture);

        public float FloatValue => _Value is { Length: > 0 } ? float.Parse(_Value, _Culture) : float.NaN;
        public double DoubleValue => _Value is { Length: > 0 } ? double.Parse(_Value, _Culture) : double.NaN;
        public decimal DecimalValue => decimal.Parse(_Value, _Culture);

        public bool BoolValue => bool.Parse(_Value);

        /* ------------------------------------------------------------------------------------------------------------- */

        public byte? Int8NullValue => byte.TryParse(_Value, NumberStyles.Any, _Culture, out var v) ? v : null;
        public sbyte? SInt8NullValue => sbyte.TryParse(_Value, NumberStyles.Any, _Culture, out var v) ? v : null;
        public short? Int16NullValue => short.TryParse(_Value, NumberStyles.Any, _Culture, out var v) ? v : null;
        public ushort? UInt16NullValue => ushort.TryParse(_Value, NumberStyles.Any, _Culture, out var v) ? v : null;
        public int? Int32NullValue => int.TryParse(_Value, NumberStyles.Any, _Culture, out var v) ? v : null;
        public uint? UInt32NullValue => uint.TryParse(_Value, NumberStyles.Any, _Culture, out var v) ? v : null;
        public long? Int64NullValue => long.TryParse(_Value, NumberStyles.Any, _Culture, out var v) ? v : null;
        public ulong? UInt64NullValue => ulong.TryParse(_Value, NumberStyles.Any, _Culture, out var v) ? v : null;

        public float? FloatNullValue => float.TryParse(_Value, NumberStyles.Any, _Culture, out var v) ? v : null;
        public double? DoubleNullValue => double.TryParse(_Value, NumberStyles.Any, _Culture, out var v) ? v : null;
        public decimal? DecimalNullValue => decimal.TryParse(_Value, NumberStyles.Any, _Culture, out var v) ? v : null;

        public bool? BoolNullValue => bool.TryParse(_Value, out var v) ? v : null;

        /* ------------------------------------------------------------------------------------------------------------- */

        public Value(string value, CultureInfo Culture)
        {
            _Value = value;
            _Culture = Culture;
        }

        /* ------------------------------------------------------------------------------------------------------------- */

        public byte AsInt8(IFormatProvider provider) => byte.Parse(_Value, provider);
        public byte AsInt8(NumberStyles Style, IFormatProvider provider) => byte.Parse(_Value, Style, provider);
        public sbyte AsSInt8(IFormatProvider provider) => sbyte.Parse(_Value, provider);
        public sbyte AsSInt8(NumberStyles Style, IFormatProvider provider) => sbyte.Parse(_Value, Style, provider);
        public short AsInt16(IFormatProvider provider) => short.Parse(_Value, provider);
        public short AsInt16(NumberStyles Style, IFormatProvider provider) => short.Parse(_Value, Style, provider);
        public ushort AsUInt16(IFormatProvider provider) => ushort.Parse(_Value, provider);
        public ushort AsUInt16(NumberStyles Style, IFormatProvider provider) => ushort.Parse(_Value, Style, provider);
        public int AsInt32(IFormatProvider provider) => int.Parse(_Value, provider);
        public int AsInt32(NumberStyles Style, IFormatProvider provider) => int.Parse(_Value, Style, provider);
        public uint AsUInt32(IFormatProvider provider) => uint.Parse(_Value, provider);
        public uint AsUInt32(NumberStyles Style, IFormatProvider provider) => uint.Parse(_Value, Style, provider);
        public long AsInt64(IFormatProvider provider) => long.Parse(_Value, provider);
        public long AsInt64(NumberStyles Style, IFormatProvider provider) => long.Parse(_Value, Style, provider);
        public ulong AsUInt64(NumberStyles Style, IFormatProvider provider) => ulong.Parse(_Value, Style, provider);

        public float AsFloat(IFormatProvider provider) => float.Parse(_Value, provider);
        public float AsFloat(NumberStyles Style, IFormatProvider provider) => float.Parse(_Value, Style, provider);
        public double AsDouble(IFormatProvider provider) => double.Parse(_Value, provider);
        public double AsDouble(NumberStyles Style, IFormatProvider provider) => double.Parse(_Value, Style, provider);
        public decimal AsDecimal(IFormatProvider provider) => decimal.Parse(_Value, provider);
        public decimal AsDecimal(NumberStyles Style, IFormatProvider provider) => decimal.Parse(_Value, Style, provider);

        /* ------------------------------------------------------------------------------------------------------------- */

        public byte? AsInt8OrNull() => byte.TryParse(_Value, out var v) ? v : null;
        public byte? AsInt8OrNull(IFormatProvider provider) => byte.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : null;
        public byte? AsInt8OrNull(NumberStyles Style, IFormatProvider provider) => byte.TryParse(_Value, Style, provider, out var v) ? v : null;
        public byte AsInt8OrDefault(byte Default) => byte.TryParse(_Value, out var v) ? v : Default;
        public byte AsInt8OrDefault(IFormatProvider provider, byte Default) => byte.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : Default;
        public byte AsInt8OrDefault(NumberStyles Style, IFormatProvider provider, byte Default) => byte.TryParse(_Value, Style, provider, out var v) ? v : Default;

        public sbyte? AsSInt8OrNull() => sbyte.TryParse(_Value, out var v) ? v : null;
        public sbyte? AsSInt8OrNull(IFormatProvider provider) => sbyte.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : null;
        public sbyte? AsSInt8OrNull(NumberStyles Style, IFormatProvider provider) => sbyte.TryParse(_Value, Style, provider, out var v) ? v : null;
        public sbyte AsSInt8OrDefault(sbyte Default) => sbyte.TryParse(_Value, out var v) ? v : Default;
        public sbyte AsSInt8OrDefault(IFormatProvider provider, sbyte Default) => sbyte.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : Default;
        public sbyte AsSInt8OrDefault(NumberStyles Style, IFormatProvider provider, sbyte Default) => sbyte.TryParse(_Value, Style, provider, out var v) ? v : Default;

        public short? AsInt16OrNull() => short.TryParse(_Value, out var v) ? v : null;
        public short? AsInt16OrNull(IFormatProvider provider) => short.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : null;
        public short? AsInt16OrNull(NumberStyles Style, IFormatProvider provider) => short.TryParse(_Value, Style, provider, out var v) ? v : null;
        public short AsInt16OrDefault(short Default) => short.TryParse(_Value, out var v) ? v : Default;
        public short AsInt16OrDefault(IFormatProvider provider, short Default) => short.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : Default;
        public short AsInt16OrDefault(NumberStyles Style, IFormatProvider provider, short Default) => short.TryParse(_Value, Style, provider, out var v) ? v : Default;

        public ushort? AsUInt16OrNull() => ushort.TryParse(_Value, out var v) ? v : null;
        public ushort? AsUInt16OrNull(IFormatProvider provider) => ushort.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : null;
        public ushort? AsUInt16OrNull(NumberStyles Style, IFormatProvider provider) => ushort.TryParse(_Value, Style, provider, out var v) ? v : null;
        public ushort AsUInt16OrDefault(ushort Default) => ushort.TryParse(_Value, out var v) ? v : Default;
        public ushort AsUInt16OrDefault(IFormatProvider provider, ushort Default) => ushort.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : Default;
        public ushort AsUInt16OrDefault(NumberStyles Style, IFormatProvider provider, ushort Default) => ushort.TryParse(_Value, Style, provider, out var v) ? v : Default;

        public int? AsInt32OrNull() => int.TryParse(_Value, out var v) ? v : null;
        public int? AsInt32OrNull(IFormatProvider provider) => int.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : null;
        public int? AsInt32OrNull(NumberStyles Style, IFormatProvider provider) => int.TryParse(_Value, Style, provider, out var v) ? v : null;
        public int AsInt32OrDefault(int Default) => int.TryParse(_Value, out var v) ? v : Default;
        public int AsInt32OrDefault(IFormatProvider provider, int Default) => int.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : Default;
        public int AsInt32OrDefault(NumberStyles Style, IFormatProvider provider, int Default) => int.TryParse(_Value, Style, provider, out var v) ? v : Default;

        public uint? AsUInt32OrNull() => uint.TryParse(_Value, out var v) ? v : null;
        public uint? AsUInt32OrNull(IFormatProvider provider) => uint.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : null;
        public uint? AsUInt32OrNull(NumberStyles Style, IFormatProvider provider) => uint.TryParse(_Value, Style, provider, out var v) ? v : null;
        public uint AsUInt32OrDefault(uint Default) => uint.TryParse(_Value, out var v) ? v : Default;
        public uint AsUInt32OrDefault(IFormatProvider provider, uint Default) => uint.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : Default;
        public uint AsUInt32OrDefault(NumberStyles Style, IFormatProvider provider, uint Default) => uint.TryParse(_Value, Style, provider, out var v) ? v : Default;

        public long? AsInt64OrNull() => long.TryParse(_Value, out var v) ? v : null;
        public long? AsInt64OrNull(IFormatProvider provider) => long.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : null;
        public long? AsInt64OrNull(NumberStyles Style, IFormatProvider provider) => long.TryParse(_Value, Style, provider, out var v) ? v : null;
        public long AsInt64OrDefault(long Default) => long.TryParse(_Value, out var v) ? v : Default;
        public long AsInt64OrDefault(IFormatProvider provider, long Default) => long.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : Default;
        public long AsInt64OrDefault(NumberStyles Style, IFormatProvider provider, long Default) => long.TryParse(_Value, Style, provider, out var v) ? v : Default;

        public ulong? AsUInt64OrNull() => ulong.TryParse(_Value, out var v) ? v : null;
        public ulong? AsUInt64OrNull(IFormatProvider provider) => ulong.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : null;
        public ulong? AsUInt64OrNull(NumberStyles Style, IFormatProvider provider) => ulong.TryParse(_Value, Style, provider, out var v) ? v : null;
        public ulong AsUInt64OrDefault(ulong Default) => ulong.TryParse(_Value, out var v) ? v : Default;
        public ulong AsUInt64OrDefault(IFormatProvider provider, ulong Default) => ulong.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : Default;
        public ulong AsUInt64OrDefault(NumberStyles Style, IFormatProvider provider, ulong Default) => ulong.TryParse(_Value, Style, provider, out var v) ? v : Default;

        public float? AsFloatOrNull() => float.TryParse(_Value, out var v) ? v : null;
        public float? AsFloatOrNull(IFormatProvider provider) => float.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : null;
        public float? AsFloatOrNull(NumberStyles Style, IFormatProvider provider) => float.TryParse(_Value, Style, provider, out var v) ? v : null;
        public float AsFloatOrDefault(float Default) => float.TryParse(_Value, out var v) ? v : Default;
        public float AsFloatOrDefault(IFormatProvider provider, float Default) => float.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : Default;
        public float AsFloatOrDefault(NumberStyles Style, IFormatProvider provider, float Default) => float.TryParse(_Value, Style, provider, out var v) ? v : Default;

        public double? AsDoubleOrNull() => double.TryParse(_Value, out var v) ? v : null;
        public double? AsDoubleOrNull(IFormatProvider provider) => double.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : null;
        public double? AsDoubleOrNull(NumberStyles Style, IFormatProvider provider) => double.TryParse(_Value, Style, provider, out var v) ? v : null;
        public double AsDoubleOrDefault(double Default) => double.TryParse(_Value, out var v) ? v : Default;
        public double AsDoubleOrDefault(IFormatProvider provider, double Default) => double.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : Default;
        public double AsDoubleOrDefault(NumberStyles Style, IFormatProvider provider, double Default) => double.TryParse(_Value, Style, provider, out var v) ? v : Default;

        public decimal? AsDecimalOrNull() => decimal.TryParse(_Value, out var v) ? v : null;
        public decimal? AsDecimalOrNull(IFormatProvider provider) => decimal.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : null;
        public decimal? AsDecimalOrNull(NumberStyles Style, IFormatProvider provider) => decimal.TryParse(_Value, Style, provider, out var v) ? v : null;
        public decimal AsDecimalOrDefault(decimal Default) => decimal.TryParse(_Value, out var v) ? v : Default;
        public decimal AsDecimalOrDefault(IFormatProvider provider, decimal Default) => decimal.TryParse(_Value, NumberStyles.Any, provider, out var v) ? v : Default;
        public decimal AsDecimalOrDefault(NumberStyles Style, IFormatProvider provider, decimal Default) => decimal.TryParse(_Value, Style, provider, out var v) ? v : Default;

        public bool? AsBoolOrNull() => bool.TryParse(_Value, out var v) ? v : null;
        public bool AsBoolOrDefault(bool Default) => bool.TryParse(_Value, out var v) ? v : Default;

        public T AsEnum<T>() where T : Enum => (T)Enum.Parse(typeof(T), _Value);
        public T AsEnum<T>(bool IgnoreCase) where T : struct, Enum => (T)Enum.Parse(typeof(T), _Value, IgnoreCase);

        public T? AsEnumOrNull<T>() where T : struct, Enum => Enum.TryParse(_Value, out T v) ? v : null;
        public T? AsEnumOrNull<T>(bool IgnoreCase) where T : struct, Enum => Enum.TryParse(_Value, IgnoreCase, out T v) ? v : null;

        public T As<T>() => (T)Convert.ChangeType(_Value, typeof(T));

        /* ------------------------------------------------------------------------------------------------------------- */

        public static implicit operator string(in Value value) => value.StringValue;
        public static implicit operator bool(in Value value) => value.BoolValue;
        public static implicit operator bool?(in Value value) => value.AsBoolOrNull();

        public static implicit operator byte(in Value value) => value.Int8Value;
        public static implicit operator sbyte(in Value value) => value.SInt8Value;
        public static implicit operator short(in Value value) => value.Int16Value;
        public static implicit operator ushort(in Value value) => value.UInt16Value;
        public static implicit operator int(in Value value) => value.Int32Value;
        public static implicit operator uint(in Value value) => value.UInt32Value;
        public static implicit operator long(in Value value) => value.Int64Value;
        public static implicit operator ulong(in Value value) => value.UInt64Value;

        public static implicit operator float(in Value value) => value.FloatValue;
        public static implicit operator double(in Value value) => value.DoubleValue;
        public static implicit operator decimal(in Value value) => value.DecimalValue;

        public static implicit operator byte?(in Value value) => value.Int8NullValue;
        public static implicit operator sbyte?(in Value value) => value.SInt8NullValue;
        public static implicit operator short?(in Value value) => value.Int16NullValue;
        public static implicit operator ushort?(in Value value) => value.UInt16NullValue;
        public static implicit operator int?(in Value value) => value.Int32NullValue;
        public static implicit operator uint?(in Value value) => value.UInt32NullValue;
        public static implicit operator long?(in Value value) => value.Int64NullValue;
        public static implicit operator ulong?(in Value value) => value.UInt64NullValue;

        public static implicit operator float?(in Value value) => value.FloatNullValue;
        public static implicit operator double?(in Value value) => value.DoubleNullValue;
        public static implicit operator decimal?(in Value value) => value.DecimalNullValue;
    }
}
