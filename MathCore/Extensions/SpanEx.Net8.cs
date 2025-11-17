#if NET8_0_OR_GREATER

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MathCore.Extensions;

/// <summary>Класс расширений для преобразования между Span и ReadOnlySpan различных типов</summary>
public static class SpanEx
{
    /// <summary>Преобразует Span&lt;byte&gt; в Span&lt;T&gt;</summary>
    /// <typeparam name="T">Тип целевого элемента</typeparam>
    /// <param name="span">Исходный Span&lt;byte&gt;</param>
    /// <returns>Span&lt;T&gt;</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Cast<T>(this Span<byte> span) where T : struct => MemoryMarshal.Cast<byte, T>(span);

    /// <summary>Преобразует ReadOnlySpan&lt;byte&gt; в ReadOnlySpan&lt;T&gt;</summary>
    /// <typeparam name="T">Тип целевого элемента</typeparam>
    /// <param name="span">Исходный ReadOnlySpan&lt;byte&gt;</param>
    /// <returns>ReadOnlySpan&lt;T&gt;</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> Cast<T>(this ReadOnlySpan<byte> span) where T : struct => MemoryMarshal.Cast<byte, T>(span);

    /// <summary>Преобразует Span&lt;T&gt; в Span&lt;byte&gt;</summary>
    /// <typeparam name="T">Тип исходного элемента</typeparam>
    /// <param name="span">Исходный Span&lt;T&gt;</param>
    /// <returns>Span&lt;byte&gt;</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> CastToByte<T>(this Span<T> span) where T : struct => MemoryMarshal.Cast<T, byte>(span);

    /// <summary>Преобразует ReadOnlySpan&lt;T&gt; в ReadOnlySpan&lt;byte&gt;</summary>
    /// <typeparam name="T">Тип исходного элемента</typeparam>
    /// <param name="span">Исходный ReadOnlySpan&lt;T&gt;</param>
    /// <returns>ReadOnlySpan&lt;byte&gt;</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> CastToByte<T>(this ReadOnlySpan<T> span) where T : struct => MemoryMarshal.Cast<T, byte>(span);
}

#endif