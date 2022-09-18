using DST = System.Diagnostics.DebuggerStepThroughAttribute;
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
}