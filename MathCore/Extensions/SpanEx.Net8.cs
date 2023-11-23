#if NET8_0_OR_GREATER

using System.Runtime.InteropServices;

namespace MathCore.Extensions;

public static class SpanEx
{
    [DST]
    public static Span<T> Cast<T>(this Span<byte> span) where T : unmanaged => MemoryMarshal.Cast<byte, T>(span);
    [DST]
    public static ReadOnlySpan<T> Cast<T>(this ReadOnlySpan<byte> span) where T : unmanaged => MemoryMarshal.Cast<byte, T>(span);

    [DST]
    public static Span<byte> CastToByte<T>(this Span<T> span) where T : unmanaged => MemoryMarshal.Cast<T, byte>(span);
    [DST]
    public static ReadOnlySpan<byte> CastToByte<T>(this ReadOnlySpan<T> span) where T : unmanaged => MemoryMarshal.Cast<T, byte>(span);
}


#endif