using System.ComponentModel;
using System.Globalization;

namespace MathCore;

internal class IntervalConverter : ExpandableObjectConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type t) => t == typeof(string) || base.CanConvertFrom(context, t);

    public override object ConvertFrom(ITypeDescriptorContext Context, CultureInfo Info, object Value)
    {
        if (Value is not string { Length: > 0 } str) 
            return base.ConvertFrom(Context, Info, Value);

        var min_include = false;
        var max_include = false;
        var str_ptr     = str.AsStringPtr();
        switch (str_ptr[0])
        {
            case '[':
                min_include = true;
                str_ptr     = str_ptr.Substring(1);
                break;
            case '(':
                str_ptr     = str_ptr.Substring(1);
                break;
        }

        switch (str_ptr[^1])
        {
            case ']':
                max_include = true;
                str_ptr     = str_ptr.Substring(-1);
                break;
            case ')':
                str_ptr     = str_ptr.Substring(-1);
                break;
        }

        var values = str_ptr.Split(';').GetEnumerator();

        var min = values.ParseNextDoubleOrThrow(Info);
        var max = values.ParseNextDoubleOrThrow(Info);

        return new Interval(min, min_include, max, max_include);
    }

    public override object ConvertTo(
        ITypeDescriptorContext Context,
        CultureInfo Culture,
        object Value,
        Type DestType) =>
        DestType == typeof(string) && Value is Interval
            ? Value.ToString()
            : base.ConvertTo(Context, Culture, Value, DestType);
}