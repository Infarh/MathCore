using System;
using System.ComponentModel;
using System.Globalization;

namespace MathCore
{
    public abstract class TypeConverter<TSource, TDestination> : ExpandableObjectConverter
    {
        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext c, Type t) => 
            t == typeof(TSource) 
            || base.CanConvertFrom(c, t);

        public override object ConvertTo(ITypeDescriptorContext c, CultureInfo i, object v, Type t) => 
            t == typeof(TSource) && v is TDestination
            ? v.ToString()
            : base.ConvertTo(c, i, v, t);
    }
}