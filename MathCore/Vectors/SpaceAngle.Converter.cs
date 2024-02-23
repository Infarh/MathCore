#nullable enable
using System.ComponentModel;
using System.Globalization;

namespace MathCore.Vectors;

internal class SpaceAngleConverter : TypeConverter<string, SpaceAngle>
{
    public override object? ConvertFrom(ITypeDescriptorContext Context, CultureInfo Info, object? value)
    {
        //Аргумент не является строкой, либо строка пуста
        //var ss = value.NotNull() as string;
        if (value.NotNull() is not string { Length: > 0 } ss)
            return base.ConvertFrom(Context, Info, value);

        //Убираем все начальные и конечные скобки, ковычки и апострофы
        while (ss is ['{', .. var s, '}']) ss   = s;
        while (ss is ['[', .. var s, ']']) ss   = s;
        while (ss is ['(', .. var s, ')']) ss   = s;
        while (ss is ['"', .. var s, '"']) ss   = s;
        while (ss is ['\'', .. var s, '\'']) ss = s;

        var values = ss.Replace(" ", string.Empty).Split(',', ';');

        var theta = 0d;
        var phi = 0d;
        bool th_set = false, ph_set = false;
        var type = AngleType.Deg;
        foreach (var str_value in values.Where(s => s is { Length: > 0 }))
            if (str_value.Length > 2 && str_value.Split('=') is [ [var name0, ..] name, { Length: > 0 } value_str])
                if (name.Equals("type", StringComparison.InvariantCultureIgnoreCase))
                    type = value_str.Equals("rad", StringComparison.InvariantCultureIgnoreCase) 
                        ? AngleType.Rad 
                        : AngleType.Deg;
                else switch (name0)
                {
                    case 'T':
                    case 't':
                        double.TryParse(value_str, out theta);
                        th_set = true;
                        break;
                    case 'P':
                    case 'p':
                        double.TryParse(value_str, out phi);
                        ph_set = true;
                        break;
                }
            else if (str_value.AsDouble() is { } vv)
                if (!th_set)
                {
                    theta = vv;
                    th_set = true;
                }
                else if (!ph_set)
                {
                    phi = vv;
                    ph_set = true;
                }

        return type == AngleType.Deg
            ? new(theta, phi, AngleType.Deg)
            : new SpaceAngle(theta, phi, AngleType.Deg).InRad;
    }
}