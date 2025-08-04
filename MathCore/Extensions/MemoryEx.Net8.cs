#if NET8_0_OR_GREATER

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MathCore.Extensions;

/// <summary>Статический класс с расширениями для работы с Memory</summary>
public static class MemoryEx
{
    /// <summary>Преобразует память байтов в память указанного структурного типа</summary>
    /// <typeparam name="T">Структурный тип, в память которого требуется преобразовать байты</typeparam>
    /// <param name="memory">Исходная память байтов</param>
    /// <returns>Память, содержащая данные в формате указанного структурного типа</returns>
    public static Memory<T> Cast<T>(this Memory<byte> memory) where T : struct
    {
        var t_memory = Unsafe.BitCast<Memory<byte>, Memory<T>>(memory);
        var size = Marshal.SizeOf(typeof(T));
        return t_memory[..(memory.Length / size)];
    }
}

#endif