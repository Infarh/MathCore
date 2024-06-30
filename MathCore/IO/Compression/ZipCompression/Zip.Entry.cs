#nullable enable
using System.IO.Compression;

namespace MathCore.IO.Compression.ZipCompression;

public sealed partial class Zip
{
    /// <summary>Элемент архива</summary>
    public sealed class Entry(Zip zip)
    {
        /// <summary>Метод сжатия</summary>
        public bool Compressed { get; set; }
        /// <summary>Полный путь к файлу в архиве</summary>
        public string FileNameInZip { get; set; }
        /// <summary>Исходный размер</summary>
        public uint FileSize { get; set; }
        /// <summary>Размер после сжатия</summary>
        public uint CompressedSize { get; set; }
        /// <summary>Смещение заголовка в файле архива</summary>
        public uint HeaderOffset { get; set; }
        /// <summary>Смещение файла внутри файла архива</summary>
        public uint FileOffset { get; set; }
        /// <summary>Размер заголовка</summary>
        // ReSharper disable once NotAccessedField.Global
        public uint HeaderSize { get; set; }
        /// <summary>32-битная контрольная сумма файла</summary>
        public uint Crc32 { get; set; }
        /// <summary>Время последнего изменения файла</summary>
        public DateTime ModifyTime { get; set; }
        /// <summary>Пользовательский комментарий к файлу</summary>
        public string Comment { get; set; }

        /// <summary>
        /// Истина, если используется кодировка UTF8 в имени файла и комментарии<br/>
        /// ложь, если кодировка по-умолчанию(CP 437)
        /// </summary>
        public bool EncodeUTF8 { get; set; }

        /// <summary>Получить поток данных архивной записи</summary>
        /// <returns>Поток данных архивной записи</returns>
        public Stream GetStream()
        {
            if (File.Exists(zip._FileName))
            {
                Stream stream = new FileStream(zip._FileName, FileMode.Open, FileAccess.Read);
                stream.Seek(HeaderOffset, SeekOrigin.Begin);
                var sign = new byte[4];
                stream.Read(sign, 0, 4);
                if (BitConverter.ToUInt32(sign, 0) != 0x04034b50)
                {
                    stream.Dispose();
                    throw new FormatException("Сигнатура файла в архиве неверна");
                }
                stream.Seek(FileOffset, SeekOrigin.Begin);
                return Compressed
                    ? new(new DeflateStream(stream, CompressionMode.Decompress, false), FileSize)
                    : new ArchiveStream(stream, FileSize);
            }

            if (FileSize <= 0x4000000)
            {
                var result = new MemoryStream();
                ExtractTo(result);
                result.Seek(0, SeekOrigin.Begin);
                return result;
            }

            var temp_file = Path.GetTempFileName();
            ExtractTo(temp_file);
            var file_stream    = File.Open(temp_file, FileMode.Open, FileAccess.Read);
            var archive_stream = new ArchiveStream(file_stream, FileSize);
            archive_stream.Disposed += (_,_) =>
            {
                file_stream.Dispose();
                File.Delete(temp_file);
            };
            return archive_stream;
        }

        /// <summary>Извлечение содержимого архива в файл</summary>
        /// <param name="FileName">Имя файла на диске для извлечения</param>
        /// <param name="Rewrite">Перезаписывать существующие файлы</param>
        /// <returns>Истина, если файл распакован успешно</returns>
        /// <remarks>Unique compression methods are Store and Deflate</remarks>
        public bool ExtractTo(string FileName, bool Rewrite = false)
        {
            if (FileName is null) throw new ArgumentNullException(nameof(FileName));
            // Определение имени директории
            var path = Path.GetDirectoryName(FileName) ?? throw new ArgumentException("Неправильный формат пути файла", nameof(FileName));

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // Если файл существует и не требуется его перезаписывать, то успешный выход
            if (Directory.Exists(FileName) && Rewrite)
                return true;

            bool result;
            using (var output = new FileStream(FileName, FileMode.Create, FileAccess.Write))
                result = ExtractTo(output);

            //TODO: разделить время на время последнего доступа, время создания и т.п.
            File.SetCreationTime(FileName, ModifyTime);
            File.SetLastWriteTime(FileName, ModifyTime);

            return result;
        }

        /// <summary>Извлечение содержимого архива в файл</summary>
        /// <param name="FileName">Имя файла на диске для извлечения</param>
        /// <param name="Rewrite">Перезаписывать существующие файлы</param>
        /// <returns>Истина, если файл распакован успешно</returns>
        /// <remarks>Unique compression methods are Store and Deflate</remarks>
        public async Task<bool> ExtractToAsync(string FileName, bool Rewrite = false)
        {
            if (FileName is null) throw new ArgumentNullException(nameof(FileName));
            // Определение имени директории
            var path = Path.GetDirectoryName(FileName) ?? throw new ArgumentException("Неправильный формат пути файла", nameof(FileName));

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // Если файл существует и не требуется его перезаписывать, то успешный выход
            if (Directory.Exists(FileName) && Rewrite)
                return true;

            bool result;
            using (var output = new FileStream(FileName, FileMode.Create, FileAccess.Write))
                result = await ExtractToAsync(output).ConfigureAwait(false);

            //TODO: разделить время на время последнего доступа, время создания и т.п.
            File.SetCreationTime(FileName, ModifyTime);
            File.SetLastWriteTime(FileName, ModifyTime);

            return result;
        }

        /// <summary>Извлечение элемента архива в поток</summary>
        /// <param name="Destination">Поток для извлечения</param>
        /// <returns>Истина, если извлечение прошло успешно</returns>
        public bool ExtractTo(Stream Destination)
        {
            if (Destination is null) throw new ArgumentNullException(nameof(Destination));
            if (!Destination.CanWrite)
                throw new InvalidOperationException("Поток данных в режиме только для чтения");

            // Проверяем сигнатуру
            var signature          = new byte[4]; // 4 первых байта должны быть равны 0x04034b50
            var zip_archive_stream = zip._ArchiveStream.NotNull();
            var last_pos           = zip_archive_stream.Position;
            zip_archive_stream.Seek(HeaderOffset, SeekOrigin.Begin);
            zip_archive_stream.Read(signature, 0, 4);
            if (BitConverter.ToUInt32(signature, 0) != 0x04034b50)
                return false; // Сигнатура неверна

            // Select input stream for inflating or just reading

            var in_stream = Compressed ? new DeflateStream(zip_archive_stream, CompressionMode.Decompress, true) : zip_archive_stream;

            // Копирование с буферизацией
            var buffer = new byte[0x4000];
            zip_archive_stream.Seek(FileOffset, SeekOrigin.Begin);
            var bytes_pending = FileSize;
            while (bytes_pending > 0)
            {
                var bytes_read = in_stream.Read(buffer, 0, (int)Math.Min(bytes_pending, buffer.Length));
                Destination.Write(buffer, 0, bytes_read);
                bytes_pending -= (uint)bytes_read;
            }
            Destination.Flush();
            zip_archive_stream.Seek(last_pos, SeekOrigin.Begin);
            return true;
        }

        /// <summary>Извлечение элемента архива в поток</summary>
        /// <param name="Destination">Поток для извлечения</param>
        /// <returns>Истина, если извлечение прошло успешно</returns>
        /// <remarks>Unique compression methods are Store and Deflate</remarks>
        public async Task<bool> ExtractToAsync(Stream Destination, CancellationToken Cancel = default)
        {
            if (Destination is null) throw new ArgumentNullException(nameof(Destination));
            if (!Destination.CanWrite)
                throw new InvalidOperationException("Поток данных в режиме только для чтения");

            // Проверяем сигнатуру
            var signature          = new byte[4]; // 4 первых байта должны быть равны 0x04034b50
            var zip_archive_stream = zip._ArchiveStream.NotNull();
            var last_pos           = zip_archive_stream.Position;
            zip_archive_stream.Seek(HeaderOffset, SeekOrigin.Begin);
            _ = await zip_archive_stream.ReadAsync(signature, 0, 4, Cancel).ConfigureAwait(false);
            if (BitConverter.ToUInt32(signature, 0) != 0x04034b50)
                return false; // Сигнатура неверна

            // Select input stream for inflating or just reading
            Stream? in_stream = null;
            try
            {
                in_stream = Compressed
                    ? zip_archive_stream
                    : new DeflateStream(zip_archive_stream, CompressionMode.Decompress, true);

                // Копирование с буферизацией
                var buffer = new byte[0x4000];
                zip_archive_stream.Seek(FileOffset, SeekOrigin.Begin);
                var bytes_pending = FileSize;
                while (bytes_pending > 0)
                {
                    var bytes_read = await in_stream.ReadAsync(buffer, 0, (int)Math.Min(bytes_pending, buffer.Length), Cancel).ConfigureAwait(false);
                    await Destination.WriteAsync(buffer, 0, bytes_read, Cancel).ConfigureAwait(false);
                    bytes_pending -= (uint)bytes_read;
                }
                await Destination.FlushAsync(Cancel).ConfigureAwait(false);
            }
            finally
            {
                if (Compressed) in_stream?.Dispose();
            }
            zip_archive_stream.Seek(last_pos, SeekOrigin.Begin);
            return true;
        }

        /// <returns>Имя файла в архиве</returns>
        public override string ToString() =>
            $"{FileNameInZip}:{FileSize}b({CompressedSize}b:{(double)CompressedSize / FileSize:p2})[offset:{FileOffset}b] crc32:{Crc32:X8} {Comment}";

        public static implicit operator Stream(Entry? entry) => entry?.GetStream();
    }
}