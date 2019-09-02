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
        public override object ConvertFrom(ITypeDescriptorContext c, CultureInfo i, object v) =>
            v switch
            {
                string s => Complex.Parse(s),
                double x => new Complex(x),
                int x => new Complex(x),
                _ => base.ConvertFrom(c, i, v)
            };

        /// <inheritdoc />
        public override bool CanConvertTo(ITypeDescriptorContext c, Type t) =>
            base.CanConvertTo(c, t)
            || t == typeof(double);

        /// <inheritdoc />
        public override object ConvertTo(ITypeDescriptorContext c, CultureInfo i, object v, Type t) =>
            t == typeof(string)
                ? ((Complex) v).ToString()
                : t == typeof(double) 
                    ? ((Complex) v).Abs 
                    : base.ConvertTo(c, i, v, t);
    }
}