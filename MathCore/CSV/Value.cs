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
        public float FloatValue => float.Parse(_Value, _Culture);

        public double DoubleValue => double.Parse(_Value, _Culture);
        public decimal DecimalValue => decimal.Parse(_Value, _Culture);

        public bool BoolValue => bool.Parse(_Value);

        public Value(string value, CultureInfo Culture)
        {
            _Value = value;
            _Culture = Culture;
        }

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

        public static implicit operator string(in Value value) => value.StringValue;

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
    }
}
