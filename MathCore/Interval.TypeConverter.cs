using System;
using System.ComponentModel;
using System.Globalization;

namespace MathCore
{
    internal class IntervalConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type t) => t == typeof(string) || base.CanConvertFrom(context, t);

        public override object ConvertFrom(ITypeDescriptorContext Context, CultureInfo Info, object Value)
        {
            var str = Value as string;
            if (string.IsNullOrEmpty(str)) return base.ConvertFrom(Context, Info, Value);

            var min_include = str[0] == '[';
            var max_include = str[str.Length - 1] == ']';

            str = str.Substring(1, str.Length - 2);
            var values = str.Split(';');
            var min = double.Parse(values[0]);
            var max = double.Parse(values[1]);

            return new Interval(min, min_include, max, max_include);
        }

        public override object ConvertTo(
                 ITypeDescriptorContext Context,
                 CultureInfo Culture,
                 object Value,
                 Type DestType)
        {
            return DestType == typeof(string) && Value is Interval
                       ? Value.ToString()
                       : base.ConvertTo(Context, Culture, Value, DestType);
        }
    }
}