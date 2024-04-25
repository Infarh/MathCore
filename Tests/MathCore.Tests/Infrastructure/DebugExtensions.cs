#nullable enable
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;

using MathCore;

// ReSharper disable once CheckNamespace
namespace System.Diagnostics;

internal static class DebugExtensions
{
    public static T ToDebug<T>(this T value, [CallerArgumentExpression(nameof(value))] string? Prefix = null)
    {
        if (Prefix is { Length: > 0 })
        {
            FormattableString msg = $"{Prefix} = {value}";
            Debug.WriteLine(msg.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            FormattableString msg = $"{value}";
            Debug.WriteLine(msg.ToString(CultureInfo.InvariantCulture));
        }
        return value;
    }

    public static void ToDebugEnum<T>(this IEnumerable<T> items, [CallerArgumentExpression(nameof(items))] string? Name = null)
    {
        string? pad_str = null;
        if (Name is { Length: > 0 })
        {
            Debug.WriteLine("{0}[] {1} =", typeof(T).Name, Name);
            Debug.WriteLine("[");
            pad_str = "    ";
        }
        var i = 0;
        var m = items is ICollection { Count: var items_count }
            ? items_count.Log10Int() + 1
            : 2;
        var culture = CultureInfo.InvariantCulture;
        foreach (var item in items)
        {

            FormattableString msg = $"{pad_str}/*[{i.ToString().PadLeft(m)}]*/ {item},";
            Debug.WriteLine(msg.ToString(culture));
            i++;
        }
        if (pad_str is not null)
            Debug.WriteLine("]");
    }

    public static void ToDebugEnum(this IEnumerable<Complex> items, [CallerArgumentExpression(nameof(items))] string? Name = null)
    {
        string? pad_str = null;
        if (Name is { Length: > 0 })
        {
            Debug.WriteLine("Complex[] {0} =", (object)Name);
            Debug.WriteLine("[");
            pad_str = "    ";
        }
        var i = 0;
        var m = items is ICollection { Count: var items_count }
            ? items_count.Log10Int() + 1
            : 2;
        var culture = CultureInfo.InvariantCulture;
        foreach (var (re, im) in items)
        {
            FormattableString msg = $"{pad_str}/*[{i.ToString().PadLeft(m)}]*/ ({re:F18}, {im:F18}),";
            Debug.WriteLine(msg.ToString(culture));
            i++;
        }
        if (pad_str is not null)
            Debug.WriteLine("}");
    }
}
