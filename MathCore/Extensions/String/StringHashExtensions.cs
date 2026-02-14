using System.Runtime.CompilerServices;
using System.Text;

using MathCore.Hash;

namespace MathCore.Extensions.String;

/// <summary>Предоставляет методы расширения для получения хэша строки</summary>
public static class StringHashExtensions
{
    /// <summary>Вычисляет MD5-хэш строки с использованием кодировки UTF8</summary>
    /// <param name="str">Строка, для которой вычисляется хэш</param>
    /// <returns>Массив байт, представляющий MD5-хэш</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetHashCodeMD5(this string str) => str.GetHashCodeMD5(Encoding.UTF8);

    /// <summary>Вычисляет SHA256-хэш строки с использованием кодировки UTF8</summary>
    /// <param name="str">Строка, для которой вычисляется хэш</param>
    /// <returns>Массив байт, представляющий SHA256-хэш</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetHashCode256(this string str) => str.GetHashCode256(Encoding.UTF8);

    /// <summary>Вычисляет SHA512-хэш строки с использованием кодировки UTF8</summary>
    /// <param name="str">Строка, для которой вычисляется хэш</param>
    /// <returns>Массив байт, представляющий SHA512-хэш</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetHashCode512(this string str) => str.GetHashCode512(Encoding.UTF8);


    /// <summary>Вычисляет MD5-хэш строки с использованием указанной кодировки</summary>
    /// <param name="str">Строка, для которой вычисляется хэш</param>
    /// <param name="encoding">Кодировка, используемая для преобразования строки</param>
    /// <returns>Массив байт, представляющий MD5-хэш</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetHashCodeMD5(this string str, Encoding? encoding) => MD5.Compute(str, encoding);

    /// <summary>Вычисляет SHA256-хэш строки с использованием указанной кодировки</summary>
    /// <param name="str">Строка, для которой вычисляется хэш</param>
    /// <param name="encoding">Кодировка, используемая для преобразования строки</param>
    /// <returns>Массив байт, представляющий SHA256-хэш</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetHashCode256(this string str, Encoding? encoding) => SHA256.Compute(str, encoding);

    /// <summary>Вычисляет SHA512-хэш строки с использованием указанной кодировки</summary>
    /// <param name="str">Строка, для которой вычисляется хэш</param>
    /// <param name="encoding">Кодировка, используемая для преобразования строки</param>
    /// <returns>Массив байт, представляющий SHA512-хэш</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetHashCode512(this string str, Encoding? encoding) => SHA512.Compute(str, encoding);
}
