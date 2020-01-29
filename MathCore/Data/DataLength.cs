using System;
using MathCore.Annotations;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMember.Global

namespace MathCore.Data
{
    public readonly struct DataLength : IFormattable
    {
        //private const double __Base = 1024;
        private const double __Threshold = 0.948964729348844D; // Log(1024 * 0.7, __Base) - 70%
        private static readonly string[] __Units = {string.Empty, "k", "M", "G", "T", "P", "E", "Z", "Y" };

        public static double Value(ulong value, double Base, out string unit)
        {
            if(value == 0)
            {
                unit = __Units[0];
                return 0;
            }

            var v_base = Math.Log(value, Base);
            var u = (int)Math.Truncate(v_base);
            var d = v_base - u;
            if(d > __Threshold)
            {
                u++;
                d--;
            }
            unit = __Units[Math.Min(u, __Units.Length - 1)];
            return Math.Pow(Base, d);
        }

        public static double Value(double value, double Base, out string unit)
        {
            if(value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, "Значение должно быть больше 0");
            if(value.Equals(0d))
            {
                unit = __Units[0];
                return 0;
            }

            var v_base = Math.Log(value, Base);
            var u = (int)Math.Truncate(v_base);
            var d = v_base - u;
            if(d > __Threshold)
            {
                u++;
                d--;
            }
            unit = __Units[Math.Min(u, __Units.Length - 1)];
            return Math.Pow(Base, d);
        }

        private readonly double _Length;
        private readonly string _Unit;
        private readonly double _FormattedLength;
        private readonly double _Base;

        public double Length => _Length;

        public double Base => _Base;

        public double FormattedLength => _FormattedLength;

        public string Unit => _Unit;

        public DataLength(double Length, double Base) => _FormattedLength = Value(_Length = Length, _Base = Base, out _Unit);

        /// <inheritdoc />
        [NotNull]
        public override string ToString() => $"{_FormattedLength}{_Unit}";

        [NotNull] public string ToString(string format) => $"{_FormattedLength.ToString(format)}{_Unit}";

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider FormatProvider) => $"{_FormattedLength.ToString(format, FormatProvider)}{_Unit}";
    }
}