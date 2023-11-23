#if NET8_0_OR_GREATER

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MathCore.Extensions;

public static class MemoryEx
{
    public static Memory<T> Cast<T>(this Memory<byte> memory) where T : unmanaged
    {
        var t_memory = Unsafe.BitCast<Memory<byte>, Memory<T>>(memory);
        var size = Marshal.SizeOf(typeof(T));
        return t_memory[..(memory.Length / size)];
    }
}

#endif