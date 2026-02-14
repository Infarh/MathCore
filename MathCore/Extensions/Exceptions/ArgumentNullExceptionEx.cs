#if !NET8_0_OR_GREATER

using System.Runtime.CompilerServices;

using MathCore.Annotations;
using MathCore.Attributes;

namespace System;

internal static class ArgumentNullExceptionEx
{
    extension(ArgumentNullException)
    {
        public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (argument is null)
                Throw(paramName);
        }
    }

    [DoesNotReturn]
    internal static void Throw(string? paramName) => throw new ArgumentNullException(paramName);
}


#endif