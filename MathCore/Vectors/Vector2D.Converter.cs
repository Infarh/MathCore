#nullable enable
using System;
using System.ComponentModel;
using System.Globalization;

namespace MathCore.Vectors;

/// <summary>Конвертер для <see cref="Vector2D"/></summary>
internal class Vector2DConverter : TypeConverter
{
    /// <inheritdoc />
    public override object? ConvertFrom(ITypeDescriptorContext Context, CultureInfo Info, object? value)
    {
        //Аргумент не является строкой, либо строка пуста
        if (value.NotNull() is not string { Length: > 0 } str)
            return base.ConvertFrom(Context, Info, value);

        //Убираем все начальные и конечные скобки, ковычки и апострофы
        while (str is ['{', .. var s, '}']) str   = s;
        while (str is ['[', .. var s, ']']) str   = s;
        while (str is ['(', .. var s, ')']) str   = s;
        while (str is ['"', .. var s, '"']) str   = s;
        while (str is ['\'', .. var s, '\'']) str = s;

        return str
           .Replace(" ", string.Empty)
           .Split(';', ':', '|')
           .ConvertTo(double.Parse) 
            is [var x, var y]
            ? new Vector2D(x, y) 
            : throw new ArgumentNullException(nameof(value));
    }
}