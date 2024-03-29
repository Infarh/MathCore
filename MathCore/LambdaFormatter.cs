﻿#nullable enable
using System.Globalization;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore;

public class LambdaFormatter<T>(Func<string, object, IFormatProvider, string> Formatter) : IFormatProvider, ICustomFormatter
{
    /// <inheritdoc />
    public object? GetFormat(Type FormatType) => FormatType == typeof(ICustomFormatter) ? this : null;

    /// <inheritdoc />
    public string Format(string format, object arg, IFormatProvider FormatProvider)
    {
        if (arg.GetType() == typeof(T)) return Formatter(format, arg, FormatProvider);
        try
        {
            return HandleOtherFormats(format, arg);
        }
        catch (FormatException e)
        {
            throw new FormatException($"The format of '{format}' is invalid.", e);
        }
    }

    private static string HandleOtherFormats(string format, object? arg) => (arg as IFormattable)?
       .ToString(format, CultureInfo.CurrentCulture) 
        ?? arg?.ToString() 
        ?? string.Empty;
}