#nullable enable
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

using MathCore.Hash.CRC;

// ReSharper disable once CheckNamespace
namespace System.IO;

/// <summary>Методы расширения для работы с потоками данных</summary>
public static class StreamExtensions
{
    /// <summary>Получить объект чтения текстовых данных</summary>
    /// <param name="stream">Поток данных</param>
    /// <returns>Объект <see cref="StreamReader"/></returns>
    /// <exception cref="InvalidOperationException">Возникает в случае если поток не предоставляет возможности чтения</exception>
    public static StreamReader GetStreamReader(this Stream stream) => stream.CanRead 
        ? new(stream) 
        : throw new InvalidOperationException("Поток не допускает операций чтения");

    /// <summary>Получить объект чтения текстовых данных</summary>
    /// <param name="stream">Поток данных</param>
    /// <param name="encoding">Кодировка текста</param>
    /// <returns>Объект <see cref="StreamReader"/></returns>
    /// <exception cref="InvalidOperationException">Возникает в случае если поток не предоставляет возможности чтения</exception>
    public static StreamReader GetStreamReader(this Stream stream, Encoding encoding) => stream.CanRead 
        ? new(stream, encoding) 
        : throw new InvalidOperationException("Поток не допускает операций чтения");

    /// <summary>Получить объект чтения двоичных данных</summary>
    /// <param name="stream">Поток данных</param>
    /// <returns>Объект <see cref="BinaryReader"/></returns>
    /// <exception cref="InvalidOperationException">Возникает в случае если поток не предоставляет возможности чтения</exception>
    public static BinaryReader GetBinaryReader(this Stream stream) => stream.CanRead 
        ? new(stream) 
        : throw new InvalidOperationException("Поток не допускает операций чтения");

    /// <summary>Получить объект записи текстовых данных</summary>
    /// <param name="stream">Поток данных</param>
    /// <returns>Объект <see cref="StreamWriter"/></returns>
    /// <exception cref="InvalidOperationException">Возникает в случае если поток не предоставляет возможности записи</exception>
    public static StreamWriter GetStreamWriter(this Stream stream) => stream.CanWrite 
        ? new(stream) 
        : throw new InvalidOperationException("Поток не допускает операций записи");

    /// <summary>Получить объект записи двоичных данных</summary>
    /// <param name="stream">Поток данных</param>
    /// <returns>Объект <see cref="BinaryWriter"/></returns>
    /// <exception cref="InvalidOperationException">Возникает в случае если поток не предоставляет возможности записи</exception>
    public static BinaryWriter GetBinaryWriter(this Stream stream) => stream.CanWrite
        ? new(stream) 
        : throw new InvalidOperationException("Поток не допускает операций записи");

    /// <summary>Заполняет буфер данными из потока</summary>
    /// <param name="stream">Поток данных</param>
    /// <param name="buffer">Буфер для заполнения</param>
    /// <returns>Количество прочитанных байтов</returns>
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

    /// <summary>Асинхронно заполняет буфер данными из потока</summary>
    /// <param name="stream">Поток данных</param>
    /// <param name="buffer">Буфер для заполнения</param>
    /// <param name="Cancel">Токен отмены операции</param>
    /// <returns>Количество прочитанных байтов</returns>
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

    /// <summary>Копирует данные из входного потока в выходной</summary>
    /// <param name="input">Входной поток</param>
    /// <param name="output">Выходной поток</param>
    /// <param name="BufferLength">Размер буфера для копирования</param>
    /// <exception cref="ArgumentOutOfRangeException">Длина буфера менее одного байта</exception>
    public static void CopyToStream(this Stream input, Stream output, int BufferLength)
    {
        if (BufferLength < 1) throw new ArgumentOutOfRangeException(nameof(BufferLength), "Длина буфера копирования менее одного байта");

        input.CopyToStream(output, new byte[BufferLength]);
    }

    /// <summary>Копирует данные из входного потока в выходной с использованием указанного буфера</summary>
    /// <param name="input">Входной поток</param>
    /// <param name="output">Выходной поток</param>
    /// <param name="Buffer">Буфер для копирования</param>
    /// <exception cref="ArgumentNullException">Один из потоков или буфер равен null</exception>
    /// <exception cref="ArgumentException">Входной поток недоступен для чтения или выходной поток недоступен для записи</exception>
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

    /// <summary>Асинхронно копирует данные из входного потока в выходной</summary>
    /// <param name="input">Входной поток</param>
    /// <param name="output">Выходной поток</param>
    /// <param name="BufferLength">Размер буфера для копирования (по умолчанию 4096 байт)</param>
    /// <param name="Cancel">Токен отмены операции</param>
    /// <returns>Задача, представляющая асинхронную операцию копирования</returns>
    /// <exception cref="ArgumentOutOfRangeException">Длина буфера менее одного байта</exception>
    public static Task CopyToStreamAsync(this Stream input, Stream output, int BufferLength = 0x1000, CancellationToken Cancel = default) =>
        BufferLength < 1
            ? throw new ArgumentOutOfRangeException(nameof(BufferLength), "Длина буфера копирования менее одного байта")
            : input.CopyToAsync(output, new byte[BufferLength], Cancel);

    /// <summary>Асинхронно копирует данные из входного потока в выходной с использованием указанного буфера</summary>
    /// <param name="input">Входной поток</param>
    /// <param name="output">Выходной поток</param>
    /// <param name="Buffer">Буфер для копирования</param>
    /// <param name="Cancel">Токен отмены операции</param>
    /// <returns>Задача, представляющая асинхронную операцию копирования</returns>
    /// <exception cref="ArgumentNullException">Один из потоков или буфер равен null</exception>
    /// <exception cref="ArgumentException">Входной поток недоступен для чтения или выходной поток недоступен для записи</exception>
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

    /// <summary>Асинхронно копирует указанное количество данных из входного потока в выходной с отчётом о прогрессе</summary>
    /// <param name="input">Входной поток</param>
    /// <param name="output">Выходной поток</param>
    /// <param name="Buffer">Буфер для копирования</param>
    /// <param name="Length">Количество байт для копирования</param>
    /// <param name="Progress">Объект для отчёта о прогрессе (от 0.0 до 1.0)</param>
    /// <param name="Cancel">Токен отмены операции</param>
    /// <returns>Задача, представляющая асинхронную операцию копирования</returns>
    /// <exception cref="ArgumentNullException">Один из потоков или буфер равен null</exception>
    /// <exception cref="ArgumentException">Входной поток недоступен для чтения или выходной поток недоступен для записи</exception>
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

    /// <summary>Вычисляет хеш SHA-256 для данных в потоке</summary>
    /// <param name="stream">Поток данных</param>
    /// <returns>Массив байтов, содержащий вычисленный хеш SHA-256</returns>
    public static byte[] ComputeSHA256(this Stream stream)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(stream);
    }

    /// <summary>Вычисляет хеш MD5 для данных в потоке</summary>
    /// <param name="stream">Поток данных</param>
    /// <returns>Массив байтов, содержащий вычисленный хеш MD5</returns>
    public static byte[] ComputeMD5(this Stream stream)
    {
        using var md5 = MD5.Create();
        return md5.ComputeHash(stream);
    }

#if NET5_0_OR_GREATER
    /// <summary>Вычисляет CRC-8 для данных из потока</summary>
    /// <param name="stream">Поток данных</param>
    /// <param name="Polynomial">Полином для вычисления CRC-8 (по умолчанию 0x07)</param>
    /// <param name="InitialValue">Начальное значение CRC (по умолчанию 0x00)</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC (по умолчанию 0x00)</param>
    /// <param name="RefIn">Отражение входных байтов (по умолчанию false)</param>
    /// <param name="RefOut">Отражение выходного значения (по умолчанию false)</param>
    /// <returns>Вычисленное значение CRC-8</returns>
    public static byte ComputeCRC8(
        this Stream stream,
        byte Polynomial = 0x07,
        byte InitialValue = 0x00,
        byte XOROut = 0x00,
        bool RefIn = false,
        bool RefOut = false) =>
        CRC8.Hash(stream, Polynomial, InitialValue, RefIn, RefOut, XOROut);

    /// <summary>Асинхронно вычисляет CRC-8 для данных из потока</summary>
    /// <param name="stream">Поток данных</param>
    /// <param name="Polynomial">Полином для вычисления CRC-8 (по умолчанию 0x07)</param>
    /// <param name="InitialValue">Начальное значение CRC (по умолчанию 0x00)</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC (по умолчанию 0x00)</param>
    /// <param name="RefIn">Отражение входных байтов (по умолчанию false)</param>
    /// <param name="RefOut">Отражение выходного значения (по умолчанию false)</param>
    /// <param name="Cancel">Токен отмены операции</param>
    /// <returns>Вычисленное значение CRC-8</returns>
    public static Task<byte> ComputeCRC8Async(
        this Stream stream,
        byte Polynomial = 0x07,
        byte InitialValue = 0x00,
        byte XOROut = 0x00,
        bool RefIn = false,
        bool RefOut = false,
        CancellationToken Cancel = default) =>
        CRC8.HashAsync(stream, Polynomial, InitialValue, RefIn, RefOut, XOROut, Cancel);

    /// <summary>Вычисляет CRC-16 для данных из потока</summary>
    /// <param name="stream">Поток данных</param>
    /// <param name="Polynomial">Полином для вычисления CRC-16 (по умолчанию 0x1021 - XMODEM)</param>
    /// <param name="InitialValue">Начальное значение CRC (по умолчанию 0x0000)</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC (по умолчанию 0x0000)</param>
    /// <param name="RefIn">Отражение входных байтов (по умолчанию false)</param>
    /// <param name="RefOut">Отражение выходного значения (по умолчанию false)</param>
    /// <returns>Вычисленное значение CRC-16</returns>
    public static ushort ComputeCRC16(
        this Stream stream,
        ushort Polynomial = 0x1021,
        ushort InitialValue = 0x0000,
        ushort XOROut = 0x0000,
        bool RefIn = false,
        bool RefOut = false) =>
        CRC16.Hash(stream, Polynomial, InitialValue, RefIn, RefOut, XOROut);

    /// <summary>Асинхронно вычисляет CRC-16 для данных из потока</summary>
    /// <param name="stream">Поток данных</param>
    /// <param name="Polynomial">Полином для вычисления CRC-16 (по умолчанию 0x1021 - XMODEM)</param>
    /// <param name="InitialValue">Начальное значение CRC (по умолчанию 0x0000)</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC (по умолчанию 0x0000)</param>
    /// <param name="RefIn">Отражение входных байтов (по умолчанию false)</param>
    /// <param name="RefOut">Отражение выходного значения (по умолчанию false)</param>
    /// <param name="Cancel">Токен отмены операции</param>
    /// <returns>Вычисленное значение CRC-16</returns>
    public static Task<ushort> ComputeCRC16Async(
        this Stream stream,
        ushort Polynomial = 0x1021,
        ushort InitialValue = 0x0000,
        ushort XOROut = 0x0000,
        bool RefIn = false,
        bool RefOut = false,
        CancellationToken Cancel = default) =>
        CRC16.HashAsync(stream, Polynomial, InitialValue, RefIn, RefOut, XOROut, Cancel);

    /// <summary>Вычисляет CRC-32 для данных из потока</summary>
    /// <param name="stream">Поток данных</param>
    /// <param name="Polynomial">Полином для вычисления CRC-32 (по умолчанию 0x04C11DB7 - стандартный)</param>
    /// <param name="InitialValue">Начальное значение CRC (по умолчанию 0xFFFFFFFF)</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC (по умолчанию 0xFFFFFFFF)</param>
    /// <param name="RefIn">Отражение входных байтов (по умолчанию false)</param>
    /// <param name="RefOut">Отражение выходного значения (по умолчанию false)</param>
    /// <returns>Вычисленное значение CRC-32</returns>
    public static uint ComputeCRC32(
        this Stream stream,
        uint Polynomial = 0x04C11DB7,
        uint InitialValue = 0xFFFFFFFF,
        uint XOROut = 0xFFFFFFFF,
        bool RefIn = false,
        bool RefOut = false) =>
        CRC32.Hash(stream, Polynomial, InitialValue, RefIn, RefOut, XOROut);

    /// <summary>Асинхронно вычисляет CRC-32 для данных из потока</summary>
    /// <param name="stream">Поток данных</param>
    /// <param name="Polynomial">Полином для вычисления CRC-32 (по умолчанию 0x04C11DB7 - стандартный)</param>
    /// <param name="InitialValue">Начальное значение CRC (по умолчанию 0xFFFFFFFF)</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC (по умолчанию 0xFFFFFFFF)</param>
    /// <param name="RefIn">Отражение входных байтов (по умолчанию false)</param>
    /// <param name="RefOut">Отражение выходного значения (по умолчанию false)</param>
    /// <param name="Cancel">Токен отмены операции</param>
    /// <returns>Вычисленное значение CRC-32</returns>
    public static Task<uint> ComputeCRC32Async(
        this Stream stream,
        uint Polynomial = 0x04C11DB7,
        uint InitialValue = 0xFFFFFFFF,
        uint XOROut = 0xFFFFFFFF,
        bool RefIn = false,
        bool RefOut = false,
        CancellationToken Cancel = default) =>
        CRC32.HashAsync(stream, Polynomial, InitialValue, RefIn, RefOut, XOROut, Cancel);

    /// <summary>Вычисляет CRC-64 для данных из потока</summary>
    /// <param name="stream">Поток данных</param>
    /// <param name="Polynomial">Полином для вычисления CRC-64 (по умолчанию 0x000000000000001B - ISO3309)</param>
    /// <param name="InitialValue">Начальное значение CRC (по умолчанию 0x0000000000000000)</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC (по умолчанию 0x0000000000000000)</param>
    /// <param name="RefIn">Отражение входных байтов (по умолчанию false)</param>
    /// <param name="RefOut">Отражение выходного значения (по умолчанию false)</param>
    /// <returns>Вычисленное значение CRC-64</returns>
    public static ulong ComputeCRC64(
        this Stream stream,
        ulong Polynomial = 0x000000000000001B,
        ulong InitialValue = 0x0000000000000000,
        ulong XOROut = 0x0000000000000000,
        bool RefIn = false,
        bool RefOut = false) =>
        CRC64.Hash(stream, Polynomial, InitialValue, RefIn, RefOut, XOROut);

    /// <summary>Асинхронно вычисляет CRC-64 для данных из потока</summary>
    /// <param name="stream">Поток данных</param>
    /// <param name="Polynomial">Полином для вычисления CRC-64 (по умолчанию 0x000000000000001B - ISO3309)</param>
    /// <param name="InitialValue">Начальное значение CRC (по умолчанию 0x0000000000000000)</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC (по умолчанию 0x0000000000000000)</param>
    /// <param name="RefIn">Отражение входных байтов (по умолчанию false)</param>
    /// <param name="RefOut">Отражение выходного значения (по умолчанию false)</param>
    /// <param name="Cancel">Токен отмены операции</param>
    /// <returns>Вычисленное значение CRC-64</returns>
    public static Task<ulong> ComputeCRC64Async(
        this Stream stream,
        ulong Polynomial = 0x000000000000001B,
        ulong InitialValue = 0x0000000000000000,
        ulong XOROut = 0x0000000000000000,
        bool RefIn = false,
        bool RefOut = false,
        CancellationToken Cancel = default) =>
        CRC64.HashAsync(stream, Polynomial, InitialValue, RefIn, RefOut, XOROut, Cancel);
#endif

    /// <summary>Создать буферизованный поток данных</summary>
    /// <param name="DataStream">Исходный поток данных</param>
    /// <param name="BufferSize">Размер буфера (по умолчанию 4096 байта)</param>
    /// <returns>Буферизованный поток данных</returns>
    public static BufferedStream GetBufferedStream(this Stream DataStream, int BufferSize = 4096) => new(DataStream, BufferSize);

    /// <summary>Создаёт обёртку над потоком данных</summary>
    /// <param name="BaseStream">Базовый поток данных</param>
    /// <returns>Обёртка над потоком <see cref="StreamWrapper"/></returns>
    public static StreamWrapper GetWrapper(this Stream BaseStream) => new(BaseStream);

    /// <summary>Читает структуру из потока</summary>
    /// <typeparam name="T">Тип читаемой структуры</typeparam>
    /// <param name="stream">Поток данных</param>
    /// <returns>Прочитанная структура</returns>
    /// <exception cref="InvalidOperationException">В потоке недостаточно данных для чтения структуры</exception>
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

    /// <summary>Перечисляет структуры, последовательно читая их из потока</summary>
    /// <typeparam name="T">Тип читаемых структур</typeparam>
    /// <param name="stream">Поток данных</param>
    /// <returns>Последовательность прочитанных структур</returns>
    public static IEnumerable<T> EnumStructures<T>(this Stream stream) where T : struct
    {
        var size = Marshal.SizeOf(typeof(T));
        var data = new byte[size];
        var gch  = GCHandle.Alloc(data, GCHandleType.Pinned);

        try
        {
            var ptr = gch.AddrOfPinnedObject();
            while (stream.Read(data, 0, size) == size)
                yield return (T)Marshal.PtrToStructure(ptr, typeof(T))!;
        }
        finally
        {
            gch.Free();
        }
    }

    /// <summary>Записывает структуру в поток</summary>
    /// <typeparam name="T">Тип записываемой структуры</typeparam>
    /// <param name="stream">Поток данных</param>
    /// <param name="value">Значение структуры для записи</param>
    public static void WriteStructure<T>(this Stream stream, T value) where T : struct
    {
        var size   = Marshal.SizeOf(value);
        var buffer = new byte[size];
        var g_lock = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        try
        {
            var p = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            Marshal.StructureToPtr(value, p, true);
        }
        finally
        {
            g_lock.Free();
        }

        stream.Write(buffer, 0, size);
    }

    /// <summary>Читает все данные из потока в массив байтов</summary>
    /// <param name="stream">Поток данных</param>
    /// <returns>Массив байтов, содержащий все данные потока</returns>
    public static byte[] ToArray(this Stream stream)
    {
#if NET8_0_OR_GREATER
        var array = new byte[stream.Length];
        _ = stream.Read(array);
#else
        var array = new byte[stream.Length];
        _ = stream.Read(array, 0, array.Length);
#endif
        return array;
    }

    /// <summary>Асинхронно читает все данные из потока в массив байтов</summary>
    /// <param name="stream">Поток данных</param>
    /// <param name="Cancel">Токен отмены операции</param>
    /// <returns>Массив байтов, содержащий все данные потока</returns>
    public static async Task<byte[]> ToArrayAsync(this Stream stream, CancellationToken Cancel = default)
    {
#if NET8_0_OR_GREATER
        var array = new byte[stream.Length];
        _ = await stream.ReadAsync(array, Cancel).ConfigureAwait(false);
#else
        var array = new byte[stream.Length];
        _ = await stream.ReadAsync(array, 0, array.Length, Cancel).ConfigureAwait(false);
#endif
        return array;
    }

    /// <summary>Читает все данные из потока как строку</summary>
    /// <param name="stream">Поток данных</param>
    /// <returns>Строка, содержащая все данные потока</returns>
    public static string ReadToEndAsString(this Stream stream) => new StreamReader(stream).ReadToEnd();

    /// <summary>Асинхронно читает все данные из потока как строку</summary>
    /// <param name="stream">Поток данных</param>
    /// <returns>Строка, содержащая все данные потока</returns>
    public static Task<string> ReadToEndAsStringAsync(this Stream stream) => new StreamReader(stream).ReadToEndAsync();

    /// <summary>Асинхронно читает все данные из потока как строку с указанной кодировкой</summary>
    /// <param name="stream">Поток данных</param>
    /// <param name="encoding">Кодировка текста</param>
    /// <returns>Строка, содержащая все данные потока</returns>
    public static Task<string> ReadToEndAsStringAsync(this Stream stream, Encoding encoding) =>
        new StreamReader(stream, encoding).ReadToEndAsync();

    /// <summary>Асинхронно читает все данные из потока как строку с поддержкой отмены</summary>
    /// <param name="stream">Поток данных</param>
    /// <param name="cancel">Токен отмены операции</param>
    /// <returns>Строка, содержащая все данные потока</returns>
    /// <exception cref="InvalidOperationException">Внутренняя ошибка выполнения</exception>
    public static async Task<string> ReadToEndAsStringAsync(this Stream stream, CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        using var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, false);
#if NET8_0_OR_GREATER
        return await reader.ReadToEndAsync(cancel).ConfigureAwait(false);
#else
        using (cancel.Register(r => ((StreamReader)r).Dispose(), reader))
            try
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
            catch (ObjectDisposedException) when (cancel.IsCancellationRequested)
            {
                cancel.ThrowIfCancellationRequested();
            }
#endif

        throw new InvalidOperationException("Что-то пошло не так");
    }

    /// <summary>Получает перечисление строк из потока</summary>
    /// <param name="stream">Поток данных</param>
    /// <returns>Перечисление строк из потока</returns>
    public static IEnumerable<string> GetStringLines(this Stream stream) => new StreamReader(stream).GetStringLines();
}