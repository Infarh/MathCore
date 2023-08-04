#nullable enable
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

// ReSharper disable once CheckNamespace
namespace System.IO;

public static class StreamExtensions
{
    public static int FillBuffer(this Stream stream, byte[] buffer)
    {
        var length = buffer.Length;
        var readed = stream.Read(buffer, 0, length);
        if (readed == 0)
            return 0;

        while (readed < length)
        {
            var last_readed = stream.Read(buffer, readed, length - readed);
            if (last_readed == 0)
                return readed;

            readed += last_readed;
        }

        return readed;
    }

    public static async Task<int> FillBufferAsync(this Stream stream, byte[] buffer, CancellationToken Cancel = default)
    {
        var length = buffer.Length;
        var readed = await stream.ReadAsync(buffer, 0, length, Cancel).ConfigureAwait(false);
        if (readed == 0)
            return 0;

        while (readed < length)
        {
            var last_readed = await stream.ReadAsync(buffer, readed, length - readed, Cancel).ConfigureAwait(false);
            if (last_readed == 0)
                return readed;

            readed += last_readed;
        }

        return readed;
    }

    public static void CopyToStream(this Stream input, Stream output, int BufferLength)
    {
        if (BufferLength < 1) throw new ArgumentOutOfRangeException(nameof(BufferLength), "Длина буфера копирования менее одного байта");

        input.CopyToStream(output, new byte[BufferLength]);
    }

    public static void CopyToStream(this Stream input, Stream output, byte[] Buffer)
    {
        if (input is null) throw new ArgumentNullException(nameof(input));
        if (!input.CanRead) throw new ArgumentException("Входной поток недоступен для чтения", nameof(input));
        if (output is null) throw new ArgumentNullException(nameof(output));
        if (!output.CanWrite) throw new ArgumentException("Выходной поток недоступен для записи", nameof(output));

        if (Buffer is null) throw new ArgumentNullException(nameof(Buffer));
        if (Buffer.Length == 0) throw new ArgumentException("Размер буфера для копирования равен 0", nameof(Buffer));

        var buffer_length = Buffer.Length;
        int readed;
        do
        {
            readed = input.Read(Buffer, 0, buffer_length);
            if (readed == 0) continue;

            output.Write(Buffer, 0, readed);
        }
        while (readed > 0);
    }

    public static Task CopyToStreamAsync(this Stream input, Stream output, int BufferLength = 0x1000, CancellationToken Cancel = default) =>
        BufferLength < 1
            ? throw new ArgumentOutOfRangeException(nameof(BufferLength), "Длина буфера копирования менее одного байта")
            : input.CopyToAsync(output, new byte[BufferLength], Cancel);

    public static async Task CopyToAsync(
        this Stream input,
        Stream output,
        byte[] Buffer,
        CancellationToken Cancel = default)
    {
        if (input is null) throw new ArgumentNullException(nameof(input));
        if (!input.CanRead) throw new ArgumentException("Входной поток недоступен для чтения", nameof(input));
        if (output is null) throw new ArgumentNullException(nameof(output));
        if (!output.CanWrite) throw new ArgumentException("Выходной поток недоступен для записи", nameof(output));

        if (Buffer is null) throw new ArgumentNullException(nameof(Buffer));
        if (Buffer.Length == 0) throw new ArgumentException("Размер буфера для копирования равен 0", nameof(Buffer));

        var buffer_length = Buffer.Length;
        int readed;
        do
        {
            Cancel.ThrowIfCancellationRequested();
            readed = await input.ReadAsync(Buffer, 0, buffer_length, Cancel).ConfigureAwait(false);
            if (readed == 0) continue;

            Cancel.ThrowIfCancellationRequested();
            await output.WriteAsync(Buffer, 0, readed, Cancel).ConfigureAwait(false);
        }
        while (readed > 0);
    }

    public static async Task CopyToAsync(
        this Stream input,
        Stream output,
        byte[] Buffer,
        long Length,
        IProgress<double>? Progress = null,
        CancellationToken Cancel = default)
    {
        if (input is null) throw new ArgumentNullException(nameof(input));
        if (!input.CanRead) throw new ArgumentException("Входной поток недоступен для чтения", nameof(input));
        if (output is null) throw new ArgumentNullException(nameof(output));
        if (!output.CanWrite) throw new ArgumentException("Выходной поток недоступен для записи", nameof(output));

        if (Buffer is null) throw new ArgumentNullException(nameof(Buffer));
        if (Buffer.Length == 0) throw new ArgumentException("Размер буфера для копирования равен 0", nameof(Buffer));

        var buffer_length = Buffer.Length;
        int readed;
        var total_readed = 0;
        var last_percent = 0d;
        do
        {
            Cancel.ThrowIfCancellationRequested();
            readed = await input
               .ReadAsync(Buffer, 0, (int)Math.Min(buffer_length, Length - total_readed), Cancel)
               .ConfigureAwait(false);
            if (readed == 0) continue;

            total_readed += readed;
            Cancel.ThrowIfCancellationRequested();
            await output.WriteAsync(Buffer, 0, readed, Cancel).ConfigureAwait(false);
            var percent = (double)total_readed / Length;
            if (percent - last_percent >= 0.01)
                Progress?.Report(last_percent = percent);
        }
        while (readed > 0 && total_readed < Length);
    }

    public static byte[] ComputeSHA256(this Stream stream)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(stream);
    }

    public static byte[] ComputeMD5(this Stream stream)
    {
        using var md5 = MD5.Create();
        return md5.ComputeHash(stream);
    }

    /// <summary>Создать буферизованный поток данных</summary>
    /// <param name="DataStream">Исходный поток данных</param>
    /// <param name="BufferSize">Размер буфера (по умолчанию 4096 байта)</param>
    /// <returns>Буферизованный поток данных</returns>
    public static BufferedStream GetBufferedStream(this Stream DataStream, int BufferSize = 4096) => new(DataStream, BufferSize);

    public static StreamWrapper GetWrapper(this Stream BaseStream) => new(BaseStream);

    public static T ReadStructure<T>(this Stream stream)
    {
        var size = Marshal.SizeOf(typeof(T));
        var data = new byte[size];
        if (stream.Read(data, 0, size) != size)
            throw new InvalidOperationException($"В потоке не достаточно данных для чтения структуры {typeof(T)} - требуется байт: {size}");

        var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            var ptr = gch.AddrOfPinnedObject();
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }
        finally
        {
            gch.Free();
        }
    }

    public static IEnumerable<T> EnumStructures<T>(this Stream stream) where T : struct
    {
        var size = Marshal.SizeOf(typeof(T));
        var data = new byte[size];
        var gch  = GCHandle.Alloc(data, GCHandleType.Pinned);

        try
        {
            var ptr = gch.AddrOfPinnedObject();
            while (stream.Read(data, 0, size) == size)
                yield return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }
        finally
        {
            gch.Free();
        }
    }

    public static void WriteStructure<T>(this Stream stream, T value) where T : struct
    {
        var size   = Marshal.SizeOf(value);
        var buffer = new byte[size];                              // создать массив
        var g_lock = GCHandle.Alloc(buffer, GCHandleType.Pinned); // зафиксировать в памяти
        try
        {
            var p = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0); // и взять его адрес
            Marshal.StructureToPtr(value, p, true);                    // копировать в массив
        }
        finally
        {
            g_lock.Free(); // снять фиксацию
        }

        stream.Write(buffer, 0, size);
    }

    public static byte[] ToArray(this Stream stream)
    {
        var array = new byte[stream.Length];
        stream.Read(array, 0, array.Length);
        return array;
    }

    public static async Task<byte[]> ToArrayAsync(this Stream stream, CancellationToken Cancel = default)
    {
        var array = new byte[stream.Length];
        await stream.ReadAsync(array, 0, array.Length, Cancel).ConfigureAwait(false);
        return array;
    }

    public static string ReadToEndAsString(this Stream stream) => new StreamReader(stream).ReadToEnd();

    public static Task<string> ReadToEndAsStringAsync(this Stream stream) => new StreamReader(stream).ReadToEndAsync();

    public static Task<string> ReadToEndAsStringAsync(this Stream stream, Encoding encoding) =>
        new StreamReader(stream, encoding).ReadToEndAsync();

    public static async Task<string> ReadToEndAsStringAsync(this Stream stream, CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        using var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true);
        using (cancel.Register(r => ((StreamReader)r).Dispose(), reader))
            try
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
            catch (ObjectDisposedException) when (cancel.IsCancellationRequested)
            {
                cancel.ThrowIfCancellationRequested();
            }

        throw new InvalidOperationException("Что-то пошло не так");
    }

    public static IEnumerable<string> GetStringLines(this Stream stream) => new StreamReader(stream).GetStringLines();
}