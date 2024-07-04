#nullable enable
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

using MathCore.Annotations;

#if NET8_0_OR_GREATER
using System.Buffers;
#endif

// ReSharper disable UnusedMember.Local

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

// ReSharper disable ArgumentsStyleOther

namespace MathCore.IO.Compression.ZipCompression;

/// <summary>Класс zip-архива для работы с файлами zip-архивов (упаковка/распаковка)</summary>
[Copyright("Jaime Olivares, v2.35 (March 14, 2010)", url = "zipstorer.codeplex.com")]
[PublicAPI]
public sealed partial class Zip : IEnumerable<Zip.Entry>, IDisposable
{
    //#region Встроенные типы данных

    ///// <summary>Метод сжатия</summary>
    //public enum Compression : ushort
    //{
    //    /// <summary>Без сжатия</summary> 
    //    Store = 0,
    //    /// <summary>Со сжатием</summary>
    //    Deflate = 8
    //}

    //#endregion

    #region Поля

    /// <summary>Список элементов архива</summary>
    private KeyedCollection<string, Entry>? _ArchiveEntries;

    /// <summary>Имя файла архива</summary>
    private readonly string _FileName;

    /// <summary>Поток данных файла архива</summary>
    private Stream? _ArchiveStream;

    /// <summary>Комментарий архива</summary>
    private string _Comment;

    /// <summary>Метод доступа к архиву</summary>
    private readonly FileAccess _Access;

    /// <summary>Статическая таблица контрольной суммы CRC32</summary>
    private static readonly uint[] __CrcTable;

    /// <summary>Строковый кодировщик по умолчанию</summary>
    private static readonly Encoding __DefaultEncoding = Encoding.Default;

    private bool _HasChanges;

    #endregion

    #region Свойства

    public bool HasChanges => _HasChanges;

    /// <summary>Истина, если используется кодировка UTF8 в имени файла и комментарии; ложь, если кодировка по-умолчанию(CP 437)</summary>
    public bool EncodeUTF8 { get; set; } = false;

    /// <summary>Принудительно использовать сжатие, даже если алгоритм увеличивает размер файла</summary>
    public bool ForceCompress { get; set; } = false;

    /// <summary>Комментарий архива</summary>
    public string Comment
    {
        get => _Comment;
        set
        {
            if (string.Equals(_Comment, value)) return;
            _Comment    = value;
            _HasChanges = true;
        }
    }

    public int EntriesCount => _ArchiveEntries.Count;

    public Entry this[int index] => _ArchiveEntries[index];

    #endregion

    #region Интейрфейс

    #region Статические методы

    /// <summary>Инициализация типа</summary>
    /// <remarks>Создаёт таблицу CRC32</remarks>
    static Zip()
    {
        // Создание таблицы CRC32
        __CrcTable = new uint[256];
        for (uint i = 0; i < __CrcTable.Length; i++)
        {
            var c = i;
            for (var j = 0; j < 8; j++) 
                c = (c & 1) == 0 
                    ? c >> 1 
                    : 0xEDB88320 ^ (c >> 1);
            __CrcTable[i] = c;
        }
    }

    /// <summary>Создание нового архива в указанном файле</summary>
    /// <param name="FileName">Полный путь к файлу архива</param>
    /// <param name="Comment">Комментарий</param>
    /// <returns>Архив</returns>
    public static Zip Create(string FileName, string? Comment = "") =>
        Create(new FileStream(FileName.NotNull(), FileMode.Create, FileAccess.ReadWrite), Comment ?? "");

    /// <summary>Создать новый архиватор в потоке данных</summary>
    /// <param name="Stream">Поток с данными архива</param>
    /// <param name="Comment">Комментарий</param>
    /// <returns>Архив</returns>
    public static Zip Create(Stream Stream, string? Comment = "") =>
        new(Stream.NotNull(), Comment ?? "");

    /// <summary>Открытие файла архива из файла</summary>
    /// <param name="FileName">Полный путь к файлу</param>
    /// <param name="Access">Режим доступа к файлу</param>
    /// <returns>Архив</returns>
    public static Zip Open(string FileName, FileAccess Access = FileAccess.ReadWrite) =>
        Open(new FileStream(FileName, FileMode.Open, Access == FileAccess.Read ? FileAccess.Read : FileAccess.ReadWrite), Access);

    /// <summary>Открытие файла архива из файла</summary>
    /// <param name="FileName">Полный путь к файлу</param>
    /// <param name="Access">Режим доступа к файлу</param>
    /// <returns>Архив</returns>
    public static Task<Zip> OpenAsync(string FileName, FileAccess Access = FileAccess.ReadWrite) =>
        OpenAsync(new FileStream(FileName, FileMode.Open, Access == FileAccess.Read ? FileAccess.Read : FileAccess.ReadWrite), Access);

    /// <summary>Открытие архива из потока</summary>
    /// <param name="Stream">Открытый поток с содержимым zip-архива и возможностью навигации</param>
    /// <param name="Access">Режим доступа к потоку</param>
    /// <returns>Архив</returns>
    public static Zip Open(Stream Stream, FileAccess Access = FileAccess.ReadWrite)
    {
        if (!Stream.NotNull().CanSeek && Access != FileAccess.Read)
            throw new InvalidOperationException("Поток не предоставляет возможности перемещения в нём");

        var zip = new Zip(Stream, "", Access);
        zip.Initialize();
        return zip;
    }

    /// <summary>Открытие архива из потока</summary>
    /// <param name="Stream">Открытый поток с содержимым zip-архива и возможностью навигации</param>
    /// <param name="Access">Режим доступа к потоку</param>
    /// <returns>Архив</returns>
    public static async Task<Zip> OpenAsync(Stream Stream, FileAccess Access = FileAccess.ReadWrite, CancellationToken Cancel = default)
    {
        if (!Stream.NotNull().CanSeek && Access != FileAccess.Read)
            throw new InvalidOperationException("Поток не предоставляет возможности перемещения в нём");

        var zip = new Zip(Stream, "", Access);
        await zip.InitializeAsync(Cancel).ConfigureAwait(false);
        return zip;
    }

    #endregion

    /// <summary>Запись архива по указанному имени</summary><param name="Name">Имя записи архива</param>
    /// <returns>Запись архива, если она присутствует в архиве, и null в противном случае</returns>
    public Entry? this[string Name] => _ArchiveEntries is null
        ? null : (_ArchiveEntries.Contains(NormalizedFileName(Name)) ? _ArchiveEntries[NormalizedFileName(Name)] : null);

    /// <summary>Создать на основе потока</summary>
    private Zip(Stream Stream, string? Comment = null, FileAccess Access = FileAccess.Write)
    {
        _ArchiveStream = Stream.NotNull();
        _Comment       = Comment ?? "";
        _Access        = Access;

        if (Stream is FileStream file_stream)
            _FileName = file_stream.Name;
    }

    /// <summary>Проверка существования файла в архиве</summary><param name="FileName">Имя проверяемого файла</param>
    /// <returns>Истина, если файл существует в архиве</returns>
    public bool FileExists(string FileName) => _ArchiveEntries.Contains(FileName);

    /// <summary>Добавить файл в архив</summary>
    /// <param name="SourceFileName">Полный путь к добавляемому файлу</param>
    /// <param name="FileNameInZip">Имя и путь к файлу в архиве</param>
    /// <param name="EntryComment">Комментарий</param>
    /// <param name="Compressed">Добавлять сжатым</param>
    public Entry Add(string SourceFileName, string? FileNameInZip = null, string? EntryComment = "", bool Compressed = true)
    {
        if (!File.Exists(SourceFileName.NotNull())) 
            throw new FileNotFoundException("Добавляемый в архив файл отсутствует", SourceFileName);

        FileNameInZip ??= Path.GetFileName(SourceFileName);
        EntryComment ??= "";

        if ((_Access & FileAccess.Write) != FileAccess.Write)
            throw new InvalidOperationException("Архив в режиме только для чтения");

        using var stream = new FileStream(SourceFileName, FileMode.Open, FileAccess.Read);
        return Add(stream, FileNameInZip, File.GetLastWriteTime(SourceFileName), EntryComment, Compressed);
    }

    /// <summary>Добавить потоковое содержимое в архив</summary>
    /// <param name="Source">Поток-источник данных файла в архиве</param>
    /// <param name="FileNameInZip">Путь к файлу внутри архива</param>
    /// <param name="ModificationTime">Время модификации файла в архиве</param>
    /// <param name="EntryComment">Комментарий к файлу</param>
    /// <param name="Compressed">Добавлять сжатым</param>
    public Entry Add(Stream Source, string FileNameInZip, DateTime ModificationTime, string? EntryComment = "", bool Compressed = true)
    {
        FileNameInZip.NotNull();
        Source.NotNull();

        if ((_Access & FileAccess.Write) != FileAccess.Write)
            throw new InvalidOperationException("Архив в режиме только для чтения");

        var entry = new Entry(this)
        {
            Compressed    = Compressed,
            EncodeUTF8    = EncodeUTF8,
            FileNameInZip = NormalizedFileName(FileNameInZip),
            Comment       = EntryComment ?? "",
            Crc32         = 0,
            HeaderOffset  = (uint)_ArchiveStream.Position,
            ModifyTime    = ModificationTime
        };

        // Заголовок придётся переписать после определения сжатого размера и контрольной суммы.

        // Запись заголовка
        WriteLocalHeader(entry);
        _HasChanges = true;

        // Запись элемента в архив
        Store(entry, Source);

        // Обновление контрольной суммы и размера
        UpdateCrcAndSizes(entry);
        _ArchiveEntries ??= new LambdaKeyedCollection<string, Entry>(e => e.FileNameInZip);
        _ArchiveEntries.Add(entry);
        return entry;
    }

    /// <summary>Добавить потоковое содержимое в архив</summary>
    /// <param name="Source">Поток-источник данных файла в архиве</param>
    /// <param name="FileNameInZip">Путь к файлу внутри архива</param>
    /// <param name="ModificationTime">Время модификации файла в архиве</param>
    /// <param name="Compressed">Метод сжатия</param>
    /// <param name="EntryComment">Комментарий к файлу</param>
    public async Task<Entry> AddAsync(
        Stream Source, 
        string FileNameInZip, 
        DateTime ModificationTime, 
        bool Compressed, 
        string? EntryComment = "",
        CancellationToken Cancel = default)
    {
        FileNameInZip.NotNull();
        Source.NotNull();

        if ((_Access & FileAccess.Write) != FileAccess.Write)
            throw new InvalidOperationException("Архив в режиме только для чтения");

        var entry = new Entry(this)
        {
            Compressed    = Compressed,
            EncodeUTF8    = EncodeUTF8,
            FileNameInZip = NormalizedFileName(FileNameInZip),
            Comment       = EntryComment ?? "",
            Crc32         = 0,
            HeaderOffset  = (uint)_ArchiveStream.Position,
            ModifyTime    = ModificationTime
        };

        // Заголовок придётся переписать после определения сжатого размера и контрольной суммы.

        // Запись заголовка
        await WriteLocalHeaderAsync(entry, Cancel).ConfigureAwait(false);
        _HasChanges      = true;
        entry.FileOffset = (uint)_ArchiveStream.Position;

        // Запись элемента в архив
        await StoreAsync(entry, Source, Cancel).ConfigureAwait(false);
        Source.Close();

        // Обновление контрольной суммы и размера
        await UpdateCrcAndSizesAsync(entry, Cancel).ConfigureAwait(false);

        _ArchiveEntries.Add(entry);
        return entry;
    }

    public bool Remove(string FileName) => FileExists(FileName) && Remove(_ArchiveEntries[FileName]);

    public bool Remove(Entry entry)
    {
        if (!_ArchiveEntries.Contains(entry)) return false;
        var entries_to_move = _ArchiveEntries
           .Where(e => e.HeaderOffset > entry.HeaderOffset)
           .OrderBy(e => e.HeaderOffset)
           .ToList();

        if (entries_to_move.Count == 0)
        {
            _ArchiveStream.Seek(entry.HeaderOffset, SeekOrigin.Begin);
            _ArchiveEntries.Remove(entry);
            return _HasChanges = true;
        }

        const int buffer_length = 0x400;

        var buffer     = new byte[buffer_length];
        var entry_size = entries_to_move[0].HeaderOffset - entry.HeaderOffset;
        var last_pos   = _ArchiveStream.Position;

        int bytes_readed;

        _ArchiveStream.Seek(entry.HeaderOffset + entry_size, SeekOrigin.Begin);
        _ArchiveStream.Seek(entries_to_move[0].HeaderOffset, SeekOrigin.Begin);
        do
        {
            bytes_readed = _ArchiveStream.Read(buffer, 0, buffer_length);
            if (bytes_readed == 0) break;

            _ArchiveStream.Seek(-entry_size - bytes_readed, SeekOrigin.Current);
            _ArchiveStream.Write(buffer, 0, bytes_readed);
            _ArchiveStream.Seek(entry_size, SeekOrigin.Current);

        } 
        while (bytes_readed > 0);

        foreach (var e in entries_to_move)
            e.HeaderOffset -= entry_size;

        _ArchiveEntries.Remove(entry);

        _ArchiveStream.Seek(last_pos - entry_size, SeekOrigin.Begin);
        return _HasChanges = true;
    }

    /// <summary>При необходимости обновляет главный каталог архива и закрывает архив</summary>
    /// <remarks>Этот шег необходим, если отсутствует явное управление ресурсами через интерфейс <see cref="IDisposable"/></remarks>
    public void Close()
    {
        try
        {
            if (!_HasChanges || (_Access & FileAccess.Write) != FileAccess.Write) return;
            var central_offset = (uint)_ArchiveStream.Position;
            var central_size   = 0u;

            if (_ArchiveEntries != null)
                foreach (var entry in _ArchiveEntries)
                {
                    var pos = _ArchiveStream.Position;
                    WriteCentralDirRecord(entry);
                    central_size += (uint)(_ArchiveStream.Position - pos);
                }

            WriteEndRecord(central_size, central_offset);
        }
        finally
        {
            _ArchiveStream.Dispose();
            _ArchiveStream = null;
        }
    }

    /// <summary>При необходимости обновляет главный каталог архива и закрывает архив</summary>
    /// <remarks>Этот шег необходим, если отсутствует явное управление ресурсами через интерфейс <see cref="IDisposable"/></remarks>
    public async Task CloseAsync(CancellationToken Cancel = default)
    {
        if ((_Access & FileAccess.Write) == FileAccess.Write)
        {
            var central_offset = (uint)_ArchiveStream.Position;
            var central_size   = 0u;

            foreach (var entry in _ArchiveEntries)
            {
                var pos = _ArchiveStream.Position;
                await WriteCentralDirRecordAsync(entry, Cancel).ConfigureAwait(false);
                central_size += (uint)(_ArchiveStream.Position - pos);
            }

            await WriteEndRecordAsync(central_size, central_offset, Cancel).ConfigureAwait(false);
        }

        if (_ArchiveStream is null) return;

#if NET8_0_OR_GREATER
        await
#endif
        using(_ArchiveStream)
            await _ArchiveStream.FlushAsync(Cancel).ConfigureAwait(false);

        _ArchiveStream = null;
    }

    ///// <summary>Читает все записи архива из главного каталога</summary>
    ///// <returns>Перечисление элементов архива</returns>

    private IEnumerable<Entry> EnumerateEntries(byte[] CentralDirImage)
    {
        if (CentralDirImage is null)
            throw new ArgumentNullException(nameof(CentralDirImage), "Индекс архива не существует");

        ushort filename_size;
        ushort extra_size;
        ushort comment_size;
        for (var ptr = 0; ptr < CentralDirImage.Length; ptr += filename_size + extra_size + comment_size + 46)
        {
            var signature = BitConverter.ToUInt32(CentralDirImage, ptr);
            if (signature != 0x02014b50)
                throw new FormatException($"Ошибка в формате индекса архива: в записи со смещением {ptr} отсутствует сигнатура 0x02014b50");

            var encode_utf8     = (BitConverter.ToUInt16(CentralDirImage, ptr + 8) & 0x0800) != 0;
            var method          = BitConverter.ToUInt16(CentralDirImage, ptr + 10);
            var modify_time     = BitConverter.ToUInt32(CentralDirImage, ptr + 12);
            var crc32           = BitConverter.ToUInt32(CentralDirImage, ptr + 16);
            var compressed_size = BitConverter.ToUInt32(CentralDirImage, ptr + 20);
            var file_size       = BitConverter.ToUInt32(CentralDirImage, ptr + 24);
            filename_size       = BitConverter.ToUInt16(CentralDirImage, ptr + 28);
            extra_size          = BitConverter.ToUInt16(CentralDirImage, ptr + 30);
            comment_size        = BitConverter.ToUInt16(CentralDirImage, ptr + 32);
            var header_offset   = BitConverter.ToUInt32(CentralDirImage, ptr + 42);
            var header_size     = (uint)(46 + filename_size + extra_size + comment_size);

            var encoder = encode_utf8 ? Encoding.UTF8 : __DefaultEncoding;

            var entry = new Entry(this)
            {
                Compressed     = method > 0,
                FileNameInZip  = encoder.GetString(CentralDirImage, ptr + 46, filename_size),
                FileOffset     = GetFileOffset(header_offset),
                FileSize       = file_size,
                CompressedSize = compressed_size,
                HeaderOffset   = header_offset,
                HeaderSize     = header_size,
                Crc32          = crc32,
                ModifyTime     = DosTimeToDateTime(modify_time),
                Comment        = comment_size > 0 ? encoder.GetString(CentralDirImage, ptr + 46 + filename_size + extra_size, comment_size) : ""
            };
            if (comment_size > 0)
                entry.Comment = encoder.GetString(CentralDirImage, ptr + 46 + filename_size + extra_size, comment_size);

            yield return entry;
        }
    }

    #endregion

    #region Скрытые методы

    /// <summary>Определение смещения файла по информации из заголовка</summary>
    /// <param name="HeaderOffset">Смещение заголовка</param>
    /// <returns>Смещение файла по логическому заголовку</returns>
#if !NET8_0_OR_GREATER
    private uint GetFileOffset(uint HeaderOffset)
    {
        var buffer = new byte[2];

        _ArchiveStream.Seek(HeaderOffset + 26, SeekOrigin.Begin);
        _ = _ArchiveStream.Read(buffer, 0, 2);
        var file_name_size = BitConverter.ToUInt16(buffer, 0);
        _ = _ArchiveStream.Read(buffer, 0, 2);
        var extra_size = BitConverter.ToUInt16(buffer, 0);

        return (uint)(30 + file_name_size + extra_size + HeaderOffset);
    }
#else
    private uint GetFileOffset(uint HeaderOffset)
    {
        _ArchiveStream.Seek(HeaderOffset + 26, SeekOrigin.Begin);

        Span<ushort> buffer_ushort = stackalloc ushort[2];
        _ = _ArchiveStream.Read(buffer_ushort.CastToByte());
        var file_name_size = buffer_ushort[0];
        var extra_size = buffer_ushort[1];

        return (uint)(30 + file_name_size + extra_size + HeaderOffset);
    }
#endif

    /// <summary>Определение смещения файла по информации из заголовка</summary>
    /// <param name="HeaderOffset">Смещение заголовка</param>
    /// <returns>Смещение файла по логическому заголовку</returns>
#if !NET8_0_OR_GREATER
    private async Task<uint> GetFileOffsetAsync(uint HeaderOffset)
    {
        var buffer = new byte[2];

        _ArchiveStream.Seek(HeaderOffset + 26, SeekOrigin.Begin);
        _ = await _ArchiveStream.ReadAsync(buffer, 0, 2).ConfigureAwait(false);
        var file_name_size = BitConverter.ToUInt16(buffer, 0);
        _ = await _ArchiveStream.ReadAsync(buffer, 0, 2).ConfigureAwait(false);
        var extra_size = BitConverter.ToUInt16(buffer, 0);

        return (uint)(30 + file_name_size + extra_size + HeaderOffset);
    }
#else
    private async Task<uint> GetFileOffsetAsync(uint HeaderOffset)
    {
        _ArchiveStream.Seek(HeaderOffset + 26, SeekOrigin.Begin);

        var buffer_byte_array = ArrayPool<byte>.Shared.Rent(4);
        try
        {
            var buffer = buffer_byte_array.AsMemory(0, 4);
            _ = await _ArchiveStream.ReadAsync(buffer).ConfigureAwait(false);
            var file_name_size = buffer.Span.Cast<uint>()[0];
            var extra_size = buffer.Span.Cast<uint>()[1];

            return 30 + file_name_size + extra_size + HeaderOffset;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer_byte_array);
        }
    }
#endif

    //[Flags]
    //public enum FileHeaderFlags : ushort
    //{
    //    NotSet = 0b0,
    //    Encrypted = 0b1,
    //    Compression0 = 0b10,
    //    Compression1 = 0b100,
    //    DataDescriptor = 0b1000,
    //    EnhancedDeflation = 0b10000,
    //    CompressedPatchedData = 0b100000,
    //    StrongEncryption = 0b1000000,
    //    Unused7 = 0b10000000,
    //    Unused8 = 0b100000000,
    //    Unused9 = 0b1000000000,
    //    Unused10 = 0b10000000000,
    //    LanguageEncoding = 0b100000000000,
    //    Reserved12 = 0b1000000000000,
    //    MaskHeaderValues = 0b10000000000000,
    //    Reserved14 = 0b100000000000000,
    //    Reserved15 = 0b1000000000000000
    //}

    /// <summary>Запись логического заголовка</summary>
    /// <param name="entry">Элемент архива</param>
    /// <remarks>
    /// Заголовок файла:
    ///    Сигнатура логического заголовка         4 байта == 0x04034b50<br/>
    ///    версия содержимого                      2 байта<br/>
    ///    Бит флага общего назначения             2 байта<br/>
    ///    Метод сжатия                            2 байта<br/>
    ///    Время последнего изменения файла        2 байта<br/>
    ///    Дата последнего изменения файла         2 байта<br/>
    ///    32-битная контрольная сумма             4 байта<br/>
    ///    Сжатый размер                           4 байта<br/>
    ///    Исходный размер                         4 байта<br/>
    ///    Длина имени файла                       2 байта<br/>
    ///    Длина поля с дополнительной информацией 2 байта
    /// </remarks>
#if !NET8_0_OR_GREATER
    private void WriteLocalHeader(Entry entry)
    {
        var encoding = entry.NotNull().EncodeUTF8 ? Encoding.UTF8 : __DefaultEncoding;
        var encoded_file_name = encoding.GetBytes(entry.FileNameInZip);

        var buffer = new byte[30 + encoded_file_name.Length];
        entry.HeaderSize = (uint)buffer.Length;
        entry.FileOffset = (uint)(_ArchiveStream.Position + buffer.Length);

        Array.Copy(new byte[] { 0x50, 0x4B, 0x03, 0x04 }, 0, buffer, 0, 4); // Сигнатура
        Array.Copy(new byte[] { 0x14, 0x00 }, 0, buffer, 4, 2); // Версия для распаковки
        Array.Copy(BitConverter.GetBytes((ushort)(entry.EncodeUTF8 ? 0b0000_1000_0000_0000 : 0)), 0, buffer, 6, 2); // Кодировка 
        Array.Copy(BitConverter.GetBytes((ushort)(entry.Compressed ? 8 : 0)), 0, buffer, 8, 2); // Метод сжатия
        Array.Copy(BitConverter.GetBytes(DateTimeToDosTime(entry.ModifyTime)), 0, buffer, 10, 4); // Время изменения файла
        var null_byte12_array = new byte[12];
        Array.Copy(null_byte12_array, 0, buffer, 14, 12); // Контрольная сумма и размеры файла в сжатом и несжатом виде. Обновляются в последствии
        Array.Copy(BitConverter.GetBytes((ushort)encoded_file_name.Length), 0, buffer, 26, 2); // Длина имени файла
        Array.Copy(null_byte12_array, 0, buffer, 28, 2); // Длина дополнительных данных
        Array.Copy(encoded_file_name, 0, buffer, 30, encoded_file_name.Length);

        _ArchiveStream.Write(buffer, 0, buffer.Length);
    } 
#else
    private void WriteLocalHeader(Entry entry)
    {
        var encoding = entry.NotNull().EncodeUTF8 ? Encoding.UTF8 : __DefaultEncoding;
        var encoded_file_name_byte_length = (ushort)encoding.GetByteCount(entry.FileNameInZip);

        entry.HeaderSize = (uint)(30 + encoded_file_name_byte_length);
        entry.FileOffset = (uint)(_ArchiveStream.Position + 30 + encoded_file_name_byte_length);

        // 0x50, 0x4B, 0x03, 0x04
        // 0x14, 0x00
        Span<byte> bytes = stackalloc byte[30]
        {
            0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        };

        FillEntryLocalHeader(bytes, entry);

        _ArchiveStream.Write(bytes);

        using var writer = new StreamWriter(_ArchiveStream, encoding, leaveOpen: true);
        writer.Write(entry.FileNameInZip);
    }

    private static void FillEntryLocalHeader(Span<byte> bytes, Entry entry)
    {
        if (bytes.Length < 30)
            throw new ArgumentException($"Недостаточно объёма буфера для записи данных. Объём буфера должен быть не менее 30 байт, а получен буфер объёмом {bytes.Length} байт");

        bytes.CastTo<uint>() = 0x04034b50;

        var ushort_buffer = bytes.Cast<ushort>();

        ushort_buffer[2] = 0x0014;
        ushort_buffer[3] = entry.EncodeUTF8 ? (ushort)0x0800 : (ushort)0x0000;
        ushort_buffer[4] = entry.Compressed ? (ushort)0x0008 : (ushort)0x0000;
        bytes[10..].CastTo<uint>() = DateTimeToDosTime(entry.ModifyTime);

        var encoding = entry.EncodeUTF8 ? Encoding.UTF8 : __DefaultEncoding;
        var encoded_file_name_byte_length = (ushort)encoding.GetByteCount(entry.FileNameInZip);

        bytes[14..30].Clear();
        bytes[26..].CastTo<ushort>() = encoded_file_name_byte_length;
    }
#endif

    /// <summary>Запись логического заголовка</summary>
    /// <param name="entry">Элемент архива</param>
    /// <remarks>
    /// Заголовок файла:<br/>
    ///    Сигнатура логического заголовка         4 байта == 0x04034b50<br/>
    ///    версия содержимого                      2 байта<br/>
    ///    Бит флага общего назначения             2 байта<br/>
    ///    Метод сжатия                            2 байта<br/>
    ///    Время последнего изменения файла        2 байта<br/>
    ///    ДАта последнего изменения файла         2 байта<br/>
    ///    32-битная контрольная сумма             4 байта<br/>
    ///    Сжатый размер                           4 байта<br/>
    ///    Исходный размер                         4 байта<br/>
    ///    Длина имени файла                       2 байта<br/>
    ///    Длина поля с дополнительной информацией 2 байта
    /// </remarks>
#if !NET8_0_OR_GREATER
    private async Task WriteLocalHeaderAsync(Entry entry, CancellationToken Cancel)
    {
        entry.NotNull();

        var position         = _ArchiveStream.Position;
        var encoder          = entry.EncodeUTF8 ? Encoding.UTF8 : __DefaultEncoding;
        var encoded_filename = encoder.GetBytes(entry.FileNameInZip);

        var buffer = new byte[30 + encoded_filename.Length];
        Array.Copy(new byte[] { 80, 75, 3, 4, 20, 0 }, 0, buffer, 0, 6);
        Array.Copy(BitConverter.GetBytes((ushort)(entry.EncodeUTF8 ? 0x0800 : 0)), 0, buffer, 6, 2);
        Array.Copy(BitConverter.GetBytes((ushort)(entry.Compressed ? 8 : 0)), 0, buffer, 8, 2);
        Array.Copy(BitConverter.GetBytes(DateTimeToDosTime(entry.ModifyTime)), 0, buffer, 10, 4);
        byte[] null_byte12_array = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
        Array.Copy(null_byte12_array, 0, buffer, 14, 12);
        Array.Copy(BitConverter.GetBytes((ushort)encoded_filename.Length), 0, buffer, 26, 2);
        Array.Copy(null_byte12_array, 0, buffer, 28, 2);
        Array.Copy(encoded_filename, 0, buffer, 30, encoded_filename.Length);

        await _ArchiveStream.WriteAsync(buffer, 0, buffer.Length, Cancel).ConfigureAwait(false);

        entry.HeaderSize = (uint)(_ArchiveStream.Position - position);
    }
#else
    private async Task WriteLocalHeaderAsync(Entry entry, CancellationToken Cancel)
    {
        entry.NotNull();

        var position = _ArchiveStream.Position;

        var byte_buffer = ArrayPool<byte>.Shared.Rent(30);
        try
        {
            var buffer = byte_buffer.AsMemory(0, 30);
            FillEntryLocalHeader(buffer.Span, entry);

            await _ArchiveStream.WriteAsync(buffer, Cancel).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(byte_buffer);
        }

        await using var writer = new StreamWriter(_ArchiveStream, entry.EncodeUTF8 ? Encoding.UTF8 : __DefaultEncoding, leaveOpen: true);
        await writer.WriteAsync(entry.FileNameInZip.AsMemory(), Cancel).ConfigureAwait(false);

        entry.HeaderSize = (uint)(_ArchiveStream.Position - position);
    }
#endif

    /// <summary>Запись индекса архива</summary>
    /// <param name="entry">Элемент архива</param>
    /// <remarks>
    /// Заголовок индекса архива:<br/>
    ///     Сигнатура заголовка(индекса) архива             4 байта == 0x02014b50<br/>
    ///     Версия                                          2 байта<br/>
    ///     Версия для извлечения                           2 байта<br/>
    ///     Битовый флаг общего назначения                  2 байта<br/>
    ///     Метод сжатия                                    2 байта<br/>
    ///     Время последнего изменения файла                2 байта<br/>
    ///     Дата последнего изменения файла                 2 байта<br/>
    ///     32-битная контрольная сумма                     4 байта<br/>
    ///     Сжатый размер                                   4 байта<br/>
    ///     Несжатый размер                                 4 байта<br/>
    ///     Длина имени файла                               2 байта<br/>
    ///     Длина поля доп. данных                          2 байта<br/>
    ///     Длина комментария файла                         2 байта<br/>
    ///     Номер тома                                      2 байта<br/>
    ///     Внутренние файловые атрибуты                    2 байта<br/>
    ///     Внешние файловые атрибуты                       4 байта<br/>
    ///     Относительное смещение от логического заголовка 4 байта<br/>
    ///     <br/>
    ///     Имя файла указанная длина<br/>
    ///     Поле с доп. информацией указанная длина<br/>
    ///     Комментарий                                     указанная длина
    /// </remarks>
#if !NET8_0_OR_GREATER
    private void WriteCentralDirRecord(Entry entry)
    {
        var encoder          = entry.EncodeUTF8 ? Encoding.UTF8 : __DefaultEncoding;
        var encoded_filename = encoder.GetBytes(entry.FileNameInZip);
        var encoded_comment  = encoder.GetBytes(entry.Comment);

        _ArchiveStream.Write([0x50, 0x4B, 0x01, 0x02], 0, 4);                                               // Сигнатура заголовка(индекса) архива
        _ArchiveStream.Write([0x17, 0x0B], 0, 2);                                                           // Версия
        _ArchiveStream.Write([0x14, 0x00], 0, 2);                                                           // Версия для извлечения
        _ArchiveStream.Write(BitConverter.GetBytes((ushort)(entry.EncodeUTF8 ? 0b100000000000 : 0)), 0, 2); // Кодировка имени файла и комментария 
        _ArchiveStream.Write(BitConverter.GetBytes((ushort)(entry.Compressed ? 8 : 0)), 0, 2);              // Метод сжатия
        _ArchiveStream.Write(BitConverter.GetBytes(DateTimeToDosTime(entry.ModifyTime)), 0, 4);             // Время изменения файла
        _ArchiveStream.Write(BitConverter.GetBytes(entry.Crc32), 0, 4);                                     // Контрольная сумма
        _ArchiveStream.Write(BitConverter.GetBytes(entry.CompressedSize), 0, 4);                            // Сжатый размер
        _ArchiveStream.Write(BitConverter.GetBytes(entry.FileSize), 0, 4);                                  // Несжатый размер
        _ArchiveStream.Write(BitConverter.GetBytes((ushort)encoded_filename.Length), 0, 2);                 // Имя файла в архиве
        _ArchiveStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);                                       // Длина поля доп. данных
        _ArchiveStream.Write(BitConverter.GetBytes((ushort)encoded_comment.Length), 0, 2);

        _ArchiveStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);                                       // Том-0
        _ArchiveStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);                                       // Тип файла: двоичный
        _ArchiveStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);                                       //Внутренние файловые атрибуты
        _ArchiveStream.Write(BitConverter.GetBytes((ushort)0x8100), 0, 2);                                  // Внешние файловые атрибуты (нормальный/только для чтения)
        _ArchiveStream.Write(BitConverter.GetBytes(entry.HeaderOffset), 0, 4);                              // Смещение заголовка

        _ArchiveStream.Write(encoded_filename, 0, encoded_filename.Length);
        _ArchiveStream.Write(encoded_comment, 0, encoded_comment.Length);
    }
#else
    private void WriteCentralDirRecord(Entry entry)
    {
        Span<byte> buffer = stackalloc byte[46]
        {
            0x50, 0x4B, 0x01, 0x02, //  0... 4(4) 0x02014b50
            0x17, 0x0B,             //  4... 6(2) 0x0b17
            0x14, 0x00,             //  6... 8(2) 0x0014
            0x00, 0x00,             //  8...10(2) EncodeUTF8 ? 0x0800 : 0
            0x00, 0x00,             // 10...12(2) Compressed ? 0x0008 : 0
            0x00, 0x00, 0x00, 0x00, // 12...16(4) ModifyTime
            0x00, 0x00, 0x00, 0x00, // 16...20(4) Crc32
            0x00, 0x00, 0x00, 0x00, // 20...24(4) CompressedSize
            0x00, 0x00, 0x00, 0x00, // 24...28(4) FileSize
            0x00, 0x00,             // 28...30(2) encoded_filename.Length
            0x00, 0x00,             // 30...32(2) Доп. Данные == 0
            0x00, 0x00,             // 32...34(2) encoded_comment.Length
            0x00, 0x00,             // 34...36(2) == 0 Том-0
            0x00, 0x00,             // 36...38(2) == 0 Тип файла: двоичный
            0x00, 0x00,             // 38...40(2) Внутренние файловые атрибут
            0x00, 0x81,             // 40...42(2) 0x8100 Внешние файловые атрибуты (нормальный/только для чтения)
            0x00, 0x00, 0x00, 0x00  // 42...46(4) Смещение заголовка
        };

        FillCentralDirRecord(buffer, entry);

        _ArchiveStream.Write(buffer);

        using var writer = new StreamWriter(_ArchiveStream, entry.EncodeUTF8 ? Encoding.UTF8 : __DefaultEncoding, leaveOpen: true);
        writer.Write(entry.FileNameInZip);
        writer.Write(entry.Comment);
    }

    private static void FillCentralDirRecord(Span<byte> buffer, Entry entry)
    {
        var buffer_ushort = buffer.Cast<ushort>();

        buffer.CastTo<uint>() = 0x02014b50;
        buffer_ushort[2] = 0x0b17;
        buffer_ushort[3] = 0x0014;
        buffer_ushort[4] = entry.EncodeUTF8 ? (ushort)0x0800 : (ushort)0;
        buffer_ushort[5] = entry.Compressed ? (ushort)0x0008 : (ushort)0;

        var buffer_uint_20 = buffer[20..].Cast<uint>();

        buffer_uint_20[0] = DateTimeToDosTime(entry.ModifyTime);
        buffer_uint_20[1] = entry.Crc32;
        buffer_uint_20[2] = entry.CompressedSize;
        buffer_uint_20[3] = entry.FileSize;

        var encoding = entry.EncodeUTF8 ? Encoding.UTF8 : __DefaultEncoding;
        var encoded_filename_length = (ushort)encoding.GetByteCount(entry.FileNameInZip);
        var encoded_comment_length = (ushort)encoding.GetByteCount(entry.Comment);

        buffer_ushort[14] = encoded_filename_length;
        buffer_ushort[15] = 0;
        buffer_ushort[16] = encoded_comment_length;
        buffer_ushort[17] = 0;
        buffer_ushort[18] = 0;
        buffer_ushort[19] = 0;
        buffer_ushort[20] = 0x8100;

        buffer[42..].CastTo<uint>() = entry.HeaderOffset;
    }
#endif

    /// <summary>Запись индекса архива</summary>
    /// <param name="entry">Элемент архива</param>
    /// <remarks>
    /// Заголовок индекса архива:<br/>
    ///     Сигнатура заголовка(индекса) архива            4 байта == 0x02014b50<br/>
    ///     Версия                                          2 байта<br/>
    ///     Версия для извлечения                           2 байта<br/>
    ///     Битовый флаг общего назначения                  2 байта<br/>
    ///     Метод сжатия                                    2 байта<br/>
    ///     Время последнего изменения файла                2 байта<br/>
    ///     Дата последнего изменения файла                 2 байта<br/>
    ///     32-битная контрольная сумма                     4 байта<br/>
    ///     Сжатый размер                                   4 байта<br/>
    ///     Несжатый размер                                 4 байта<br/>
    ///     Длина имени файла                               2 байта<br/>
    ///     Длина поля доп. данных                          2 байта<br/>
    ///     Длина комментария файла                         2 байта<br/>
    ///     Номер тома                                      2 байта<br/>
    ///     Внутренние файловые атрибуты                    2 байта<br/>
    ///     Внешние файловые атрибуты                       4 байта<br/>
    ///     Относительное смещение от логического заголовка 4 байта<br/>
    ///     <br/>
    ///     Имя файла указанная длина<br/>
    ///     Поле с доп. информацией указанная длина<br/>
    ///     Комментарий                                     указанная длина
    /// </remarks>
#if !NET8_0_OR_GREATER
    private async Task WriteCentralDirRecordAsync(Entry entry, CancellationToken Cancel)
    {
        var encoder          = entry.NotNull().EncodeUTF8 ? Encoding.UTF8 : __DefaultEncoding;
        var encoded_filename = encoder.GetBytes(entry.FileNameInZip);
        var encoded_comment  = encoder.GetBytes(entry.Comment);

        var buffer = new byte[46 + encoded_filename.Length + encoded_comment.Length];
        Array.Copy(new byte[] { 0x50, 0x4B, 0x01, 0x02 }, buffer, 4);
        Array.Copy(new byte[] { 0x17, 0x0B }, 0, buffer, 4, 2);
        Array.Copy(new byte[] { 0x14, 0x00 }, 0, buffer, 6, 2);
        Array.Copy(BitConverter.GetBytes((ushort)(entry.EncodeUTF8 ? 0x0800 : 0)), 0, buffer, 8, 2);
        Array.Copy(BitConverter.GetBytes((ushort)(entry.Compressed ? 8 : 0)), 0, buffer, 10, 2);
        Array.Copy(BitConverter.GetBytes(DateTimeToDosTime(entry.ModifyTime)), 0, buffer, 12, 4);
        Array.Copy(BitConverter.GetBytes(entry.Crc32), 0, buffer, 16, 4);
        Array.Copy(BitConverter.GetBytes(entry.CompressedSize), 0, buffer, 20, 4);
        Array.Copy(BitConverter.GetBytes(entry.FileSize), 0, buffer, 24, 4);
        Array.Copy(BitConverter.GetBytes((ushort)encoded_filename.Length), 0, buffer, 28, 2);
        var null_byte2_array = BitConverter.GetBytes((ushort)0);
        Array.Copy(null_byte2_array, 0, buffer, 30, 2);
        Array.Copy(BitConverter.GetBytes((ushort)encoded_comment.Length), 0, buffer, 32, 2);
        Array.Copy(null_byte2_array, 0, buffer, 34, 2);
        Array.Copy(null_byte2_array, 0, buffer, 36, 2);
        Array.Copy(null_byte2_array, 0, buffer, 38, 2);
        Array.Copy(BitConverter.GetBytes((ushort)0x8100), 0, buffer, 40, 2);
        Array.Copy(BitConverter.GetBytes(entry.HeaderOffset), 0, buffer, 42, 4);

        Array.Copy(encoded_filename, 0, buffer, 46, encoded_filename.Length);
        Array.Copy(encoded_comment, 0, buffer, 46 + encoded_filename.Length, encoded_comment.Length);
        await _ArchiveStream.WriteAsync(buffer, 0, buffer.Length, Cancel).ConfigureAwait(false);
    }
#else
    private async Task WriteCentralDirRecordAsync(Entry entry, CancellationToken Cancel)
    {
        var byte_buffer = ArrayPool<byte>.Shared.Rent(48);
        try
        {
            var buffer = byte_buffer.AsMemory(0, 48);

            FillCentralDirRecord(buffer.Span, entry);

            await _ArchiveStream.WriteAsync(buffer, Cancel).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(byte_buffer);
        }

        await using var writer = new StreamWriter(_ArchiveStream, entry.EncodeUTF8 ? Encoding.UTF8 : __DefaultEncoding, leaveOpen: true);
        await writer.WriteAsync(entry.FileNameInZip.AsMemory(), Cancel).ConfigureAwait(false);
        await writer.WriteAsync(entry.Comment.AsMemory(), Cancel).ConfigureAwait(false);
    }
#endif

    /// <summary>Запись окончания файла</summary>
    /// <param name="Size">Размер</param>
    /// <param name="Offset">Смещение</param>
    /// <remarks>
    /// Завершение индекса архива:<br/>
    ///  Сигнатура завершения архива                     4 байт  (0x06054b50)<br/>
    ///  Номер тома                                      2 байт<br/>
    ///  Номер тома с началом индекса архива             2 байт<br/>
    ///  Общее число записей в индексе в этом томе       2 байт<br/>
    ///  Общее число записей в индексе                   2 байт<br/>
    ///  Размер индекса                                  4 байт<br/>
    ///  Смещение начала индекса с учётом номера тома    4 байт<br/>
    ///  Длина комментария архива                        2 байт<br/>
    ///  Комментарий архива                              Указанная длина
    /// </remarks>
#if !NET8_0_OR_GREATER
    private void WriteEndRecord(uint Size, uint Offset)
    {
        var encoder         = EncodeUTF8 ? Encoding.UTF8 : __DefaultEncoding;
        var encoded_comment = encoder.GetBytes(_Comment);

        var buffer = new byte[22 + encoded_comment.Length];
        Array.Copy(new byte[] { 0x50, 0x4B, 0x05, 0x06 }, 0, buffer, 0, 4); // Сигнатура заголовка 0x06.05.4b.50
        Array.Copy(new byte[] { 0x00, 0x00 }, 0, buffer, 4, 2);             // Номер тома
        Array.Copy(new byte[] { 0x00, 0x00 }, 0, buffer, 6, 2);             // Номер тома с началом индекса архива
        var entry_count = BitConverter.GetBytes((ushort)_ArchiveEntries.Count);
        Array.Copy(entry_count, 0, buffer, 8, 2);                    // Общее число записей в индексе в этом томе
        Array.Copy(entry_count, 0, buffer, 10, 2);                   // Общее число записей в индексе 
        Array.Copy(BitConverter.GetBytes(Size), 0, buffer, 12, 4);   // Размер индекса
        Array.Copy(BitConverter.GetBytes(Offset), 0, buffer, 16, 4); // Смещение начала индекса с учётом номера тома
        Array.Copy(BitConverter.GetBytes((ushort)encoded_comment.Length), 0, buffer, 20, 2);
        Array.Copy(encoded_comment, 0, buffer, 22, encoded_comment.Length);

        _ArchiveStream.Write(buffer, 0, buffer.Length);
        _ArchiveStream.SetLength(_ArchiveStream.Position);
    }
#else
    private void WriteEndRecord(uint Size, uint Offset)
    {
        Span<byte> bytes = stackalloc byte[22]
        {
            0x50, 0x4B, 0x05, 0x06, //  0... 4(4) 0x06054b50
            0x00, 0x00,             //  4... 6(2) 0x0000
            0x00, 0x00,             //  6... 8(2) 0x0000

            0x00, 0x00,             //  8...10(2) tom entries count
            0x00, 0x00,             // 10...12(2) total entries count
            0x00, 0x00, 0x00, 0x00, // 12...16(4) Size
            0x00, 0x00, 0x00, 0x00, // 16...20(4) Offset
            0x00, 0x00              // 20...22(2) encoded_comment_length
        };

        FillEndRecord(bytes, Size, Offset);

        _ArchiveStream.Write(bytes);

        using(var writer = new StreamWriter(_ArchiveStream, EncodeUTF8 ? Encoding.UTF8 : __DefaultEncoding, leaveOpen: true))
            writer.Write(_Comment);

        _ArchiveStream.SetLength(_ArchiveStream.Position);
    }

    private void FillEndRecord(Span<byte> bytes, uint Size, uint Offset)
    {
        var buffer_ushort = bytes.Cast<ushort>();
        var buffer_uint = bytes.Cast<uint>();

        buffer_uint[0] = 0x06054b50;
        buffer_ushort[2] = 0x0000;
        buffer_ushort[3] = 0x0000;

        buffer_ushort[4] = (ushort)_ArchiveEntries.Count;
        buffer_ushort[5] = (ushort)_ArchiveEntries.Count;

        buffer_uint[3] = Size;
        buffer_uint[4] = Offset;

        var encoding = EncodeUTF8 ? Encoding.UTF8 : __DefaultEncoding;
        var encoded_comment_length = (ushort)encoding.GetByteCount(_Comment);

        buffer_ushort[10] = encoded_comment_length;
    }
#endif

    /// <summary>Запись окончания файла</summary>
    /// <param name="Size">Размер</param>
    /// <param name="Offset">Смещение</param>
    /// <remarks>
    /// Завершение индекса архива:<br/>
    ///  Сигнатура завершения архива                     4 байт  (0x06054b50)<br/>
    ///  Номер тома                                      2 байт<br/>
    ///  Номер тома с началом индекса архива             2 байт<br/>
    ///  Общее число записей в индексе в этом томе       2 байт<br/>
    ///  Общее число записей в индексе                   2 байт<br/>
    ///  Размер индекса                                  4 байт<br/>
    ///  Смещение начала индекса с учётом номера тома    4 байт<br/>
    ///  Длина комментария архива                        2 байт<br/>
    ///  Комментарий архива                              Указанная длина
    /// </remarks>
#if !NET8_0_OR_GREATER
    private async Task WriteEndRecordAsync(uint Size, uint Offset, CancellationToken Cancel)
    {
        var encoder         = EncodeUTF8 ? Encoding.UTF8 : __DefaultEncoding;
        var encoded_comment = encoder.GetBytes(_Comment);
        var buffer          = new byte[22 + encoded_comment.Length];
        Array.Copy(new byte[] { 80, 75, 5, 6, 0, 0, 0, 0 }, 0, buffer, 0, 8);
        var entry_count_bytes = BitConverter.GetBytes((ushort)_ArchiveEntries.Count);
        Array.Copy(entry_count_bytes, 0, buffer, 8, 2);
        Array.Copy(entry_count_bytes, 0, buffer, 10, 2);
        Array.Copy(BitConverter.GetBytes(Size), 0, buffer, 12, 4);
        Array.Copy(BitConverter.GetBytes(Offset), 0, buffer, 16, 4);
        Array.Copy(BitConverter.GetBytes((ushort)encoded_comment.Length), 0, buffer, 20, 2);
        Array.Copy(encoded_comment, 0, buffer, 22, encoded_comment.Length);
        await _ArchiveStream.WriteAsync(buffer, 0, buffer.Length, Cancel).ConfigureAwait(false);
    }
#else
    private async Task WriteEndRecordAsync(uint Size, uint Offset, CancellationToken Cancel)
    {
        var bytes_buffer = ArrayPool<byte>.Shared.Rent(22);
        try
        {
            var bytes = bytes_buffer.AsMemory(0, 22);

            FillEndRecord(bytes.Span, Size, Offset);

            await _ArchiveStream.WriteAsync(bytes, Cancel).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(bytes_buffer);
        }

        await using (var writer = new StreamWriter(_ArchiveStream, EncodeUTF8 ? Encoding.UTF8 : __DefaultEncoding, leaveOpen: true))
            await writer.WriteAsync(_Comment.AsMemory(), Cancel).ConfigureAwait(false);

        _ArchiveStream.SetLength(_ArchiveStream.Position);
    }
#endif

    /// <summary>Копирование данных из файла-источника в элемент архива</summary>
    /// <param name="entry">Элемент архива</param>
    /// <param name="Source">Источник</param>
#if !NET8_0_OR_GREATER
    private void Store(Entry entry, Stream Source)
    {
        while (true)
        {
            var  buffer = new byte[0x4000];
            int  bytes_read;
            uint total_read = 0;

            var pos_start    = _ArchiveStream.Position;
            var source_start = Source.Position;

            var out_stream = entry.Compressed ? new DeflateStream(_ArchiveStream, CompressionMode.Compress, true) : _ArchiveStream;

            entry.Crc32 = 0 ^ 0xffffffff;

            do
            {
                bytes_read =  Source.Read(buffer, 0, buffer.Length);
                total_read += (uint)bytes_read;
                if (bytes_read <= 0) continue;
                out_stream.Write(buffer, 0, bytes_read);

                for (uint i = 0; i < bytes_read; i++)
                    entry.Crc32 = __CrcTable[(entry.Crc32 ^ buffer[i]) & 0xFF] ^ (entry.Crc32 >> 8);

            } while (bytes_read == buffer.Length);
            out_stream.Flush();

            if (entry.Compressed)
                out_stream.Dispose();

            entry.Crc32          ^= 0xffffffff;
            entry.FileSize       =  total_read;
            entry.CompressedSize =  (uint)(_ArchiveStream.Position - pos_start);

            // Проверка реального уровня сжатия (если без сжатия, либо не включено принудительное сжатие, либо сжатый размер меньше исходного, то выход)
            if (!entry.Compressed || ForceCompress || !Source.CanSeek || entry.CompressedSize <= entry.FileSize) return;

            // Start operation again with Store algorithm
            entry.Compressed = false;
            _ArchiveStream.Seek(pos_start, SeekOrigin.Begin);
            _ArchiveStream.SetLength(pos_start);
            Source.Position = source_start;
            Source.CopyTo(_ArchiveStream);
            entry.CompressedSize = (uint)(_ArchiveStream.Position - source_start);
        }
    }
#else
    private void Store(Entry entry, Stream Source)
    {
        Span<byte> buffer = stackalloc byte[0x4000];
        while (true)
        {
            int bytes_read;
            uint total_read = 0;

            var pos_start = _ArchiveStream.Position;
            var source_start = Source.Position;

            var out_stream = entry.Compressed ? new DeflateStream(_ArchiveStream, CompressionMode.Compress, true) : _ArchiveStream;

            entry.Crc32 = 0 ^ 0xffffffff;

            do
            {
                bytes_read = Source.Read(buffer);
                total_read += (uint)bytes_read;
                if (bytes_read <= 0) continue;
                out_stream.Write(buffer[..bytes_read]);

                for (var i = 0; i < bytes_read; i++)
                    entry.Crc32 = __CrcTable[(entry.Crc32 ^ buffer[i]) & 0xFF] ^ (entry.Crc32 >> 8);

            } while (bytes_read == buffer.Length);
            out_stream.Flush();

            if (entry.Compressed)
                out_stream.Dispose();

            entry.Crc32 ^= 0xffffffff;
            entry.FileSize = total_read;
            entry.CompressedSize = (uint)(_ArchiveStream.Position - pos_start);

            // Проверка реального уровня сжатия (если без сжатия, либо не включено принудительное сжатие, либо сжатый размер меньше исходного, то выход)
            if (!entry.Compressed || ForceCompress || !Source.CanSeek || entry.CompressedSize <= entry.FileSize) return;

            // Start operation again with Store algorithm
            entry.Compressed = false;
            _ArchiveStream.Seek(pos_start, SeekOrigin.Begin);
            _ArchiveStream.SetLength(pos_start);
            Source.Position = source_start;
            Source.CopyTo(_ArchiveStream);
            entry.CompressedSize = (uint)(_ArchiveStream.Position - source_start);
        }
    }
#endif

    /// <summary>Копирование данных из файла-источника в элемент архива</summary>
    /// <param name="entry">Элемент архива</param>
    /// <param name="Source">Источник</param>
    private async Task StoreAsync(Entry entry, Stream Source, CancellationToken Cancel)
    {
        entry.NotNull();
        while (true)
        {
            var  buffer = new byte[16384];
            int  bytes_read;
            uint total_read = 0;

            var pos_start    = _ArchiveStream.Position;
            var source_start = Source.Position;

            var out_stream = entry.Compressed ? new DeflateStream(_ArchiveStream, CompressionMode.Compress, true) : _ArchiveStream;

            entry.Crc32 = 0 ^ 0xffffffff;

            do
            {
                bytes_read =  await Source.ReadAsync(buffer, 0, buffer.Length, Cancel).ConfigureAwait(false);
                total_read += (uint)bytes_read;
                if (bytes_read <= 0) continue;
                await out_stream.WriteAsync(buffer, 0, bytes_read, Cancel).ConfigureAwait(false);

                for (uint i = 0; i < bytes_read; i++)
                    entry.Crc32 = __CrcTable[(entry.Crc32 ^ buffer[i]) & 0xFF] ^ (entry.Crc32 >> 8);
            } while (bytes_read == buffer.Length);
            await out_stream.FlushAsync(Cancel).ConfigureAwait(false);

            if (entry.Compressed)
                out_stream.Dispose();

            entry.Crc32          ^= 0xffffffff;
            entry.FileSize       =  total_read;
            entry.CompressedSize =  (uint)(_ArchiveStream.Position - pos_start);

            // Проверка реального уровня сжатия (если без сжатия, либо не включено принудительное сжатие, либо сжатый размер меньше исходного, то выход)
            if (!entry.Compressed || ForceCompress || !Source.CanSeek || entry.CompressedSize <= entry.FileSize) return;
            // Start operation again with Store algorithm
            entry.Compressed        = false;
            _ArchiveStream.Position = pos_start;
            _ArchiveStream.SetLength(pos_start);
            Source.Position = source_start;
        }
    }

    /// <summary>Преобразование даты и времени в MS-DOS-формат</summary>
    /// <param name="time">Упаковываемое время</param>
    /// <returns>4 байта структуры времени</returns>
    /// <remarks>
    /// DOS-формат времени и даты:
    ///   MS-DOS-формат даты. ДАта упаковывается в следующий формат. Описание битов 
    ///       0-4 День месяца (1–31) 
    ///       5-8 Месяц (1 = Январь, 2 = февраль, и.т.д.) 
    ///       9-15 Год. Отсчитывается от 1980 (прибавить к значению 1980 для получения корректного результата) 
    ///   MS-DOS-формат времени. Время упаковывается в следующий формат. Описание битов 
    ///       0-4 Секунды, делённые на 2 
    ///       5-10 Минуты (0–59) 
    ///       11-15 Часы (0–23 - в 24-часовом формате) 
    /// </remarks>
    private static uint DateTimeToDosTime(DateTime time) =>
        (uint)(
            (time.Second / 2) | (time.Minute << 5) | (time.Hour << 11) |
            (time.Day << 16) | (time.Month << 21) | ((time.Year - 1980) << 25));

    /// <summary>Преобразование времени из формата MS-DOS</summary>
    /// <param name="dos_time">Структура времени в формате MS-DOS</param>
    /// <returns>Восстановленное время</returns>
    private static DateTime DosTimeToDateTime(uint dos_time) =>
        new(
            year: (int)(dos_time >> 25) + 1980,
            month: (int)(dos_time >> 21) & 15,
            day: (int)(dos_time >> 16) & 31,
            hour: (int)(dos_time >> 11) & 31,
            minute: (int)(dos_time >> 5) & 63,
            second: (int)(dos_time & 31) * 2);

    /// <summary>Обновление контрольной суммы и длин</summary>
    /// <param name="entry">Элемент архива</param>
    /// <remarks>
    /// CRC32 алгоритм<br/>
    /// "Магическое число" алгоритма CRC - 0xdebb20e3.
    /// The proper CRC pre and post conditioning
    /// is used, meaning that the CRC register is
    /// pre-conditioned with all ones (начальное значение
    /// 0xffffffff) and the value is post-conditioned by
    /// taking the one's complement of the CRC residual.
    /// Если третий бит флага общего назначения установлен, это
    /// полу равно нулю в локальном заголовке и корректное значение
    /// расположено в дескрипторе данных и в главном индексе
    /// </remarks>
    private void UpdateCrcAndSizes(Entry entry)
    {
        var last_pos = _ArchiveStream.Position; // Запоминаем положение в потоке данных

        _ArchiveStream.Seek(entry.HeaderOffset + 8, SeekOrigin.Begin);
        _ArchiveStream.Write(BitConverter.GetBytes((ushort)(entry.Compressed ? 8 : 0)), 0, 2); // метод сжатия

        _ArchiveStream.Seek(4, SeekOrigin.Current);
        _ArchiveStream.Write(BitConverter.GetBytes(entry.Crc32), 0, 4);          // Обновление CRC
        _ArchiveStream.Write(BitConverter.GetBytes(entry.CompressedSize), 0, 4); // Размер после сжатия
        _ArchiveStream.Write(BitConverter.GetBytes(entry.FileSize), 0, 4);       // Размер до сжатия

        _ArchiveStream.Seek(last_pos, SeekOrigin.Begin);
    }

    /// <summary>Обновление контрольной суммы и длин</summary>
    /// <param name="entry">Элемент архива</param>
    /// <remarks>
    /// CRC32 алгоритм
    /// "Магическое число" алгоритма CRC - 0xdebb20e3.  
    /// The proper CRC pre and post conditioning
    /// is used, meaning that the CRC register is
    /// pre-conditioned with all ones (начальное значение
    /// 0xffffffff) and the value is post-conditioned by
    /// taking the one's complement of the CRC residual.
    /// Если третий бит флага общего назначения установлен, это
    /// полу равно нулю в локальном заголовке и корректное значение
    /// расположено в дескрипторе данных и в главном индексе
    /// </remarks>
    private async Task UpdateCrcAndSizesAsync(Entry entry, CancellationToken Cancel)
    {
        if (entry is null) throw new ArgumentNullException(nameof(entry));
        var last_pos = _ArchiveStream.Position; // Запоминаем положение в потоке данных

        _ArchiveStream.Position = entry.HeaderOffset + 8;
        await _ArchiveStream.WriteAsync(BitConverter.GetBytes((ushort)(entry.Compressed ? 8 : 0)), 0, 2, Cancel).ConfigureAwait(false); // метод сжатия

        _ArchiveStream.Position = entry.HeaderOffset + 14;
        var buffer = new byte[12];
        Array.Copy(BitConverter.GetBytes(entry.Crc32), buffer, 4);
        Array.Copy(BitConverter.GetBytes(entry.CompressedSize), 0, buffer, 4, 4);
        Array.Copy(BitConverter.GetBytes(entry.FileSize), 0, buffer, 8, 4);
        await _ArchiveStream.WriteAsync(buffer, 0, buffer.Length, Cancel).ConfigureAwait(false);

        _ArchiveStream.Position = last_pos; // Восстанавливаем положение в потоке
    }

    /// <summary>Нормализация имени файл для размещения его в архиве</summary>
    /// <param name="FileName">Имя файла для нормализации</param>
    /// <returns>Имя файла, в котором проведена замена '\' -> '/', удалена буква диска, удалены начальная и конечная '/'</returns>
    private string NormalizedFileName(string FileName)
    {
        var file_name = FileName.Replace('\\', '/');

        var pos                 = file_name.IndexOf(':');
        if (pos >= 0) file_name = file_name.Remove(0, pos + 1);

        return file_name.Trim('/');
    }

    /// <summary>Чтение окончания индекса файла</summary>
    /// <returns>Истина, если чтение прошло успешно</returns>
    private void Initialize()
    {
        if (_ArchiveStream.Length < 22) 
            throw new FormatException("Размер файла меньше 22 байт - минимального размера пустого архива");

        try
        {
            _ArchiveStream.Seek(-22, SeekOrigin.End);
            var reader = new BinaryReader(_ArchiveStream);
            do
            {
                if (reader.ReadUInt32() != 0x06_05_4b_50) 
                    continue;

                var buffer             = new byte[22];
                var readed_bytes_count = _ArchiveStream.Read(buffer, 4, buffer.Length - 4);
                if (readed_bytes_count != buffer.Length - 4) 
                    throw new FormatException("Сигнатура 0x06054b50 индекса архива не найдена");

                //var book_no = BitConverter.ToInt16(buffer, 4);
                //var book_no_start = BitConverter.ToInt16(buffer, 6);
                //var entries = BitConverter.ToInt16(buffer, 8);
                //var entries_total = BitConverter.ToInt16(buffer, 10);
                var central_size       = BitConverter.ToInt32(buffer, 12);
                var central_dir_offset = BitConverter.ToInt32(buffer, 16);
                var comment_size       = BitConverter.ToInt16(buffer, 20);

                // Чтение поля комментария - самого последнего в файле
                if (_ArchiveStream.Length - _ArchiveStream.Position != comment_size)
                    throw new FormatException("Ошибка формата файла zip-архива: размер раздела комментария файла больше заявленного в индексе");

                if (comment_size > 0)
                {
                    var comment_bytes = new byte[comment_size];
                    _ = _ArchiveStream.Read(comment_bytes, 0, comment_size);
                    _Comment = Encoding.Default.GetString(comment_bytes);
                }

                // Копирование индекса в память
                //_ArchiveStream.Seek(central_dir_offset, SeekOrigin.Begin);
                _ArchiveStream.Seek(-22 - central_size, SeekOrigin.End);
                var central_dir_image = new byte[central_size];
                _ = _ArchiveStream.Read(central_dir_image, 0, central_size);
                _ArchiveEntries = new LambdaKeyedCollection<string, Entry>(e => e.FileNameInZip, EnumerateEntries(central_dir_image));
                // Оставляем указатель на начале индекса для добавления новых файлов
                _ArchiveStream.Seek(-22 - central_size, SeekOrigin.End);
                //_ArchiveStream.Seek(central_dir_offset, SeekOrigin.Begin);
                return;
            } 
            while (_ArchiveStream.Position > 0);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            throw;
        }
        throw new FormatException("Сигнатура 0x06054b50 индекса архива не найдена");
    }

    /// <summary>Чтение окончания индекса файла</summary>
    /// <returns>Истина, если чтение прошло успешно</returns>
    private async Task InitializeAsync(CancellationToken Cancel)
    {
        if (_ArchiveStream.Length < 22) throw new FormatException("Размер файла меньше 22 байт - минимального размера пустого архива");

        try
        {
            _ArchiveStream.Seek(-17, SeekOrigin.End);
            do
            {
                _ArchiveStream.Seek(-5, SeekOrigin.Current);
                var buffer = new byte[12];
                if (await _ArchiveStream.ReadAsync(buffer, 0, 4, Cancel).ConfigureAwait(false) != 4) 
                    throw new FormatException("Сигнатура 0x06054b50 индекса архива не найдена");

                var sig = BitConverter.ToUInt32(buffer, 0);
                if (sig != 0x06054b50) continue;
                _ArchiveStream.Seek(6, SeekOrigin.Current);

                if (await _ArchiveStream.ReadAsync(buffer, 0, buffer.Length, Cancel).ConfigureAwait(false) != buffer.Length)
                    throw new FormatException("Не удалось прочитать индекс архива");

                var central_size       = BitConverter.ToUInt16(buffer, 2);
                var central_dir_offset = BitConverter.ToUInt16(buffer, 6);
                var comment_size       = BitConverter.ToUInt16(buffer, 10);

                // Чтение поля комментария - самого последнего в файле
                if (_ArchiveStream.Position + comment_size != _ArchiveStream.Length)
                    throw new FormatException("Ошибка формата файла zip-архива: размер раздела комментария файла больше заявленного в индексе");

                // Копирование индекса в память
                _ArchiveStream.Seek(central_dir_offset, SeekOrigin.Begin);
                var central_dir_image = new byte[central_size];
                if (await _ArchiveStream.ReadAsync(central_dir_image, 0, central_size, Cancel).ConfigureAwait(false) != central_size)
                    throw new FormatException("Не удалось считать структуру индекса архива");

                _ArchiveEntries = new LambdaKeyedCollection<string, Entry>(e => e.FileNameInZip, EnumerateEntries(central_dir_image));

                // Оставляем указатель на начале индекса для добавления новых файлов
                _ArchiveStream.Seek(central_dir_offset, SeekOrigin.Begin);

                return;
            } 
            while (_ArchiveStream.Position > 0);
        }
        // ReSharper disable once CatchAllClause
        catch (Exception e)
        {
            Debug.WriteLine(e);
            throw;
        }

        throw new FormatException("Сигнатура 0x06054b50 индекса архива не найдена");
    }

#endregion

    #region IDisposable

    /// <inheritdoc />
    void IDisposable.Dispose() => Close();

    #endregion

    #region IEnumerable<Entry>

    /// <inheritdoc />
    IEnumerator<Entry> IEnumerable<Entry>.GetEnumerator() => _ArchiveEntries.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => _ArchiveEntries.GetEnumerator();

    #endregion
}