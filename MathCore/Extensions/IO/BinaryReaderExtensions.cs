#nullable enable
#if NET8_0_OR_GREATER
using System.Buffers;
#endif
using System.Runtime.InteropServices;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.IO;

public static class BinaryReaderExtensions
{
    /// <summary>Признак конца потока</summary>
    /// <param name="reader">Объект чтения потока</param>
    /// <returns>Истина, если поток закончен</returns>
    [DST]
    public static bool IsEOF(this BinaryReader reader) => reader.BaseStream.Position == reader.BaseStream.Length;

    /// <summary>Получить перечисление, содержащее массивы байт заданной длины из потока</summary>
    /// <param name="reader">Объект чтения потока данных</param>
    /// <param name="BufferSize">Размер буфера</param>
    /// <returns>Последовательность массивов байт указанной длины (последний массив будет меньше)</returns>
    public static IEnumerable<byte[]> GetByteBuffer(this BinaryReader reader, int BufferSize)
    {
        int readed;
        do
        {
            var buffer = new byte[BufferSize];
            readed = reader.Read(buffer, 0, BufferSize);
            if (readed != BufferSize)
                Array.Resize(ref buffer, readed);
            yield return buffer;
        }
        while (readed == BufferSize);
    }

    public readonly struct ReadedBuffer(byte[] Buffer, int Readed)
    {
        public byte[] Buffer { get; } = Buffer;

        public int Readed { get; } = Readed;

        public byte[] GetReaded()
        {
            var buffer = Buffer;
            if (Readed < buffer.Length)
                Array.Resize(ref buffer, Readed);
            return buffer;
        }

        public static implicit operator byte[](ReadedBuffer buffer) => buffer.GetReaded();

        public void Deconstruct(out byte[] buffer, out int readed) => (buffer, readed) = (Buffer, Readed);
    }

    /// <summary>Получить перечисление, содержащее массивы байт заданной длины из потока</summary>
    /// <param name="reader">Объект чтения потока данных</param>
    /// <param name="Buffer">Буфер чтения</param>
    /// <returns>Перечислитель</returns>
    public static IEnumerable<ReadedBuffer> GetByteBuffer(this BinaryReader reader, byte[] Buffer)
    {
        if (Buffer is null) throw new ArgumentNullException(nameof(Buffer));
        if (Buffer.Length == 0) throw new ArgumentException("Размер буфера должен быть больше 0", nameof(Buffer));

        var buffer_size = Buffer.Length;
        int readed;
        do
        {
            var buffer = new byte[buffer_size];
            readed = reader.Read(buffer, 0, buffer_size);
            yield return new(buffer, readed);
        }
        while (readed == buffer_size);
    }

    public static IEnumerable<char[]> GetCharBuffer(this BinaryReader reader, int BufferSize)
    {
        int readed;
        do
        {
            var buffer = new char[BufferSize];
            readed = reader.Read(buffer, 0, BufferSize);
            if (readed != BufferSize)
                Array.Resize(ref buffer, readed);
            yield return buffer;
        }
        while (readed == BufferSize);
    }

    public static T ReadStructure<T>(this BinaryReader reader) where T : struct
    {
        var data = reader.ReadBytes(Marshal.SizeOf(typeof(T)));
        var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            var ptr = gch.AddrOfPinnedObject();
            return (T)Marshal.PtrToStructure(ptr, typeof(T))!;
        }
        finally
        {
            gch.Free();
        }
    }

    public static void WriteStructure<T>(this BinaryWriter writer, T value) where T : struct
    {
        var buffer = new byte[Marshal.SizeOf(value)];             // создать массив
        var g_lock = GCHandle.Alloc(buffer, GCHandleType.Pinned); // зафиксировать в памяти
        try
        {
            var p = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0); // и взять его адрес
            Marshal.StructureToPtr(value, p, true);                    // копировать в массив
            writer.Write(buffer);
        }
        finally
        {
            g_lock.Free(); // снять фиксацию
        }
    }

    public static async Task<double> ReadDoubleAsync(this BinaryReader Reader, CancellationToken Cancel = default)
    {
        const int size = 8;

#if NET8_0_OR_GREATER
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            var readed = await Reader.BaseStream.ReadAsync(buffer.AsMemory(0, size), Cancel).ConfigureAwait(false);
            return readed < size
                ? throw new InvalidOperationException($"Не удалось прочитать из потока 8 байт. Было прочитано {readed} байт")
                : BitConverter.ToDouble(buffer, 0);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
#else
        var buffer = new byte[size];
        var readed = await Reader.BaseStream.ReadAsync(buffer, 0, size, Cancel).ConfigureAwait(false); 

        return readed < size
            ? throw new InvalidOperationException($"Не удалось прочитать из потока 8 байт. Было прочитано {readed} байт")
            : BitConverter.ToDouble(buffer, 0);
#endif
    }

    public static async Task<float> ReadSingleAsync(this BinaryReader Reader, CancellationToken Cancel = default)
    {
        const int size = 4;

#if NET8_0_OR_GREATER
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            var readed = await Reader.BaseStream.ReadAsync(buffer.AsMemory(0, size), Cancel).ConfigureAwait(false);
            return readed < size
                ? throw new InvalidOperationException($"Не удалось прочитать из потока 4 байт. Было прочитано {readed} байт")
                : BitConverter.ToSingle(buffer, 0);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
#else
        var buffer = new byte[size];
        var readed = await Reader.BaseStream.ReadAsync(buffer, 0, size, Cancel).ConfigureAwait(false); 

        return readed < size
            ? throw new InvalidOperationException($"Не удалось прочитать из потока 4 байт. Было прочитано {readed} байт")
            : BitConverter.ToSingle(buffer, 0);
#endif
    }

    public static async Task<long> ReadInt64Async(this BinaryReader Reader, CancellationToken Cancel = default)
    {
        const int size = 8;

#if NET8_0_OR_GREATER
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            var readed = await Reader.BaseStream.ReadAsync(buffer.AsMemory(0, size), Cancel).ConfigureAwait(false);
            return readed < size
                ? throw new InvalidOperationException($"Не удалось прочитать из потока 8 байт. Было прочитано {readed} байт")
                : BitConverter.ToInt64(buffer, 0);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
#else
        var buffer = new byte[size];
        var readed = await Reader.BaseStream.ReadAsync(buffer, 0, size, Cancel).ConfigureAwait(false); 

        return readed < size
            ? throw new InvalidOperationException($"Не удалось прочитать из потока 8 байт. Было прочитано {readed} байт")
            : BitConverter.ToInt64(buffer, 0);
#endif
    }

    public static async Task<int> ReadInt32Async(this BinaryReader Reader, CancellationToken Cancel = default)
    {
        const int size = 4;

#if NET8_0_OR_GREATER
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            var readed = await Reader.BaseStream.ReadAsync(buffer.AsMemory(0, size), Cancel).ConfigureAwait(false);
            return readed < size
                ? throw new InvalidOperationException($"Не удалось прочитать из потока 4 байт. Было прочитано {readed} байт")
                : BitConverter.ToInt32(buffer, 0);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
#else
        var buffer = new byte[size];
        var readed = await Reader.BaseStream.ReadAsync(buffer, 0, size, Cancel).ConfigureAwait(false); 

        return readed < size
            ? throw new InvalidOperationException($"Не удалось прочитать из потока 4 байт. Было прочитано {readed} байт")
            : BitConverter.ToInt32(buffer, 0);
#endif
    }

    public static async Task<bool> ReadBooleanAsync(this BinaryReader Reader, CancellationToken Cancel = default)
    {
        const int size = 1;

#if NET8_0_OR_GREATER
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            var readed = await Reader.BaseStream.ReadAsync(buffer.AsMemory(0, size), Cancel).ConfigureAwait(false);
            return readed < size
                ? throw new InvalidOperationException($"Не удалось прочитать из потока 1 байт. Было прочитано {readed} байт")
                : BitConverter.ToBoolean(buffer, 0);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
#else
        var buffer = new byte[size];
        var readed = await Reader.BaseStream.ReadAsync(buffer, 0, size, Cancel).ConfigureAwait(false); 

        return readed < size
            ? throw new InvalidOperationException($"Не удалось прочитать из потока 1 байт. Было прочитано {readed} байт")
            : BitConverter.ToBoolean(buffer, 0);
#endif
    }
}