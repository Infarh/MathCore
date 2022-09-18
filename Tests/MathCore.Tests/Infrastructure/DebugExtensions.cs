#nullable enable
using System.Globalization;
using System.Runtime.CompilerServices;

using MathCore;

// ReSharper disable once CheckNamespace
namespace System.Diagnostics;

internal static class DebugExtensions
{
    public static T ToDebug<T>(this T value, [CallerArgumentExpression("value")] string? Prefix = null)
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

    public static void ToDebugEnum<T>(this IEnumerable<T> items, [CallerArgumentExpression("items")] string? Name = null)
    {
        if (Name is { Length: > 0 })
            Debug.WriteLine("{0}[] {1} = {{", typeof(T).Name, Name);
        var i = 0;
        var culture = CultureInfo.InvariantCulture;
        foreach (var item in items)
        {
            FormattableString msg = $"/*[{i,2}]*/ {item},";
            Debug.WriteLine(msg.ToString(culture));
            i++;
        }
        Debug.WriteLine("}");
    }

    public static void ToDebugEnum(this IEnumerable<Complex> items, [CallerArgumentExpression("items")] string? Name = null)
    {
        if (Name is { Length: > 0 })
            Debug.WriteLine("Complex[] {0} = {{", (object)Name);
        var i = 0;
        var culture = CultureInfo.InvariantCulture;
        foreach (var (re, im) in items)
        {
            FormattableString msg = $"/*[{i,2}]*/ ({re:F18}, {im:F18}),";
            Debug.WriteLine(msg.ToString(culture));
            i++;
        }
        Debug.WriteLine("}");
    }
}
