using System;
using System.ComponentModel;
using System.Globalization;

namespace MathCore
{
    /// <summary>Конвертер комплексных чисел</summary>
    internal class ComplexConverter : TypeConverter
    {
        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext c, Type t) => 
            base.CanConvertFrom(c, t) 
            || t == typeof(double) 
            || t == typeof(int);

        /// <inheritdoc />
        public override object ConvertFrom(ITypeDescriptorContext c, CultureInfo i, object v)
        {
            switch (v)
            {
                case string s: return Complex.Parse(s);
                case double x: return new Complex(x);
                case int x: return new Complex(x);
                default: return base.ConvertFrom(c, i, v);
            } 
        }

        /// <inheritdoc />
        public override bool CanConvertTo(ITypeDescriptorContext c, Type t) =>
            base.CanConvertTo(c, t)
            || t == typeof(double);

        /// <inheritdoc />
        public override object ConvertTo(ITypeDescriptorContext c, CultureInfo i, object v, Type t)
        {
            if (t == typeof(string)) return ((Complex) v).ToString();
            if (t == typeof(double)) return ((Complex) v).Abs;
            return base.ConvertTo(c, i, v, t);
        }
    }
}
