#nullable enable
using System.Globalization;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System;

public static class CharExtensions
{
    [DST] public static bool IsDigit(this char c) => char.IsDigit(c);

    private const int __IndexOf0 = '0';

    [DST]
    public static int ToDigit(this char c) => char.IsDigit(c) 
        ? c - __IndexOf0 
        : throw new InvalidOperationException($"Символ \'{c}\' не является цифрой");

    public static char ToLower(this char c) => char.ToLower(c);
    public static char ToLower(this char c, CultureInfo culture) => char.ToLower(c, culture);
    public static char ToLoverInvariant(this char c) => char.ToLowerInvariant(c);

    public static char ToUpper(this char c) => char.ToUpper(c);
    public static char ToUpper(this char c, CultureInfo culture) => char.ToUpper(c, culture);
    public static char ToUpperInvariant(this char c) => char.ToUpperInvariant(c);
}