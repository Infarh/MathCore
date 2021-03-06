﻿#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MathCore;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.IO
{
    /// <summary>Класс методов расширений для объектов класса System.IO.FileInfo</summary>
    public static class FileInfoExtensions
    {
        /// <summary>Создать двоичный файл</summary>
        /// <param name="File">Информация о создаваемом файле</param>
        /// <returns>Объект для записи данных в двоичный файл</returns>
        public static BinaryWriter CreateBinary(this FileInfo File) => new(File.Create());

        /// <summary>Создать двоичный файл</summary>
        /// <param name="File">Информация о создаваемом файле</param>
        /// <param name="BufferLength">Размер буфера записи</param>
        /// <returns>Объект для записи данных в двоичный файл</returns>
        public static BinaryWriter CreateBinary(this FileInfo File, int BufferLength) =>
            new(new FileStream(File.FullName, FileMode.Create, FileAccess.Write, FileShare.None, BufferLength));

        /// <summary>Создать двоичный файл</summary>
        /// <param name="File">Информация о создаваемом файле</param>
        /// <param name="Encoding">Кодировка</param>
        /// <returns>Объект для записи данных в двоичный файл</returns>
        public static BinaryWriter CreateBinary(this FileInfo File, Encoding Encoding) =>
            new(File.Create(), Encoding);

        /// <summary>Создать двоичный файл</summary>
        /// <param name="File">Информация о создаваемом файле</param>
        /// <param name="BufferLength">Размер буфера записи</param>
        /// <param name="Encoding">Кодировка</param>
        /// <returns>Объект для записи данных в двоичный файл</returns>
        public static BinaryWriter CreateBinary(this FileInfo File, int BufferLength, Encoding Encoding) =>
            new(new FileStream(File.FullName, FileMode.Create, FileAccess.Write, FileShare.None, BufferLength), Encoding);

        /// <summary>Открыть двоичный файл для чтения</summary>
        /// <param name="File">Информация о создаваемом файле</param>
        /// <returns>Объект для чтения данных из двоичного файла</returns>
        public static BinaryReader OpenBinary(this FileInfo File) => new(File.OpenRead());

        /// <summary>Открыть двоичный файл для чтения</summary>
        /// <param name="File">Информация о создаваемом файле</param>
        /// <param name="BufferLength">Размер буфера чтения</param>
        /// <returns>Объект для чтения данных из двоичного файла</returns>
        public static BinaryReader OpenBinary(this FileInfo File, int BufferLength) =>
            new(new FileStream(File.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, BufferLength));

        /// <summary>Открыть двоичный файл для чтения</summary>
        /// <param name="File">Информация о создаваемом файле</param>
        /// <param name="Encoding">Кодировка</param>
        /// <returns>Объект для чтения данных из двоичного файла</returns>
        public static BinaryReader OpenBinary(this FileInfo File, Encoding Encoding) =>
            new(File.OpenRead(), Encoding);

        /// <summary>Открыть двоичный файл для чтения</summary>
        /// <param name="File">Информация о создаваемом файле</param>
        /// <param name="BufferLength">Размер буфера чтения</param>
        /// <param name="Encoding">Кодировка</param>
        /// <returns>Объект для чтения данных из двоичного файла</returns>
        public static BinaryReader OpenBinary(this FileInfo File, int BufferLength, Encoding Encoding) =>
            new(new FileStream(File.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, BufferLength), Encoding);

        /// <summary>Выполнить файл с правами администратора</summary>
        /// <param name="File">Исполняемый файл</param>
        /// <param name="Args">Аргументы командной строки</param>
        /// <param name="UseShellExecute">Использовать интерпретацию файла операционной системой</param>
        /// <returns>Созданный процесс</returns>
        public static Process? ExecuteAsAdmin(this FileInfo File, string Args = "", bool UseShellExecute = true) =>
            // ReSharper disable once StringLiteralTypo
            File.Execute(Args, UseShellExecute, "runas");

        /// <summary>Выполнить файл</summary>
        /// <param name="File">Исполняемый файл</param>
        /// <param name="Args">Аргументы командной строки</param>
        /// <param name="UseShellExecute">Использовать интерпретацию файла операционной системой</param>
        /// <returns>Созданный процесс</returns>
        public static Process? Execute(this FileInfo File, string Args, bool UseShellExecute, string Verb) =>
            Process.Start(new ProcessStartInfo(File.FullName, Args)
            {
                UseShellExecute = UseShellExecute,
                Verb = Verb
            });

        /// <summary>Проверка на существование файла. Если файл не существует, то генерируется исключение</summary>
        /// <param name="file">Проверяемый файл</param>
        /// <param name="Message">Сообщение, добавляемое в исключение</param>
        /// <returns>Информация о файле</returns>
        /// <exception cref="FileNotFoundException">если файл не существует</exception>
        public static FileInfo ThrowIfNotFound(this FileInfo file, string? Message = null)
        {
            file.Refresh();
            return file.Exists ? file : throw new FileNotFoundException(Message ?? $"Файл {file} не найден");
        }

        /// <summary>Вычислить хеш-сумму SHA256</summary>
        /// <param name="file">Файл, контрольную сумму которого надо вычислить</param>
        /// <returns>Массив байт контрольной суммы</returns>
        public static byte[] ComputeSHA256(this FileInfo file)
        {
            if (file is null) throw new ArgumentNullException(nameof(file));
            using var stream = file.ThrowIfNotFound().OpenRead();
            using var md5 = new Security.Cryptography.SHA256Managed();
            return md5.ComputeHash(stream);
        }

        public static byte[] ComputeMD5(this FileInfo file)
        {
            if (file is null) throw new ArgumentNullException(nameof(file));
            using var stream = file.ThrowIfNotFound().OpenRead();
            using var md5 = new Security.Cryptography.MD5CryptoServiceProvider();
            return md5.ComputeHash(stream);
        }
        public static IEnumerable<byte> ReadBytes(this FileInfo file)
        {
            using var stream = file.ThrowIfNotFound().OpenRead();
            using var reader = new BinaryReader(stream);
            while (!reader.IsEOF())
                yield return reader.ReadByte();
        }

        public static IEnumerable<byte[]>? ReadBytes(this FileInfo file, int Length)
        {
            using var stream = file.ThrowIfNotFound().OpenRead();
            using var reader = new BinaryReader(stream);
            while (!reader.IsEOF())
                yield return reader.ReadBytes(Length);
        }

        public static IEnumerable<string?> ReadLines(this FileInfo file)
        {
            using var reader = file.ThrowIfNotFound().OpenText();
            while (!reader.EndOfStream)
                yield return reader.ReadLine();
        }

        public static IEnumerable<string?> ReadLines(
            this FileInfo file, 
            Action<StreamReader>? initializer, 
            int BufferSize = 3 * Consts.DataLength.Bytes.MB)
        {
            using var reader = new StreamReader(
                new BufferedStream(file.ThrowIfNotFound().Open(
                    FileMode.Open, 
                    FileAccess.Read,
                    FileShare.Read), 
                    BufferSize));
            initializer?.Invoke(reader);
            while (!reader.EndOfStream)
                yield return reader.ReadLine();
        }

        public static string? ReadAllText(this FileInfo file, bool ThrowNotExist = true)
        {
            if (!file.Exists && !ThrowNotExist) return null;
            using var reader = file.OpenText();
            return reader.ReadToEnd();
        }

        /// <summary>Скопировать файл в директорию</summary>
        /// <param name="SourceFile">Файл источник</param>
        /// <param name="DestinationDirectory">Директория назначения</param>
        /// <returns>Файл копия</returns>
        public static FileInfo CopyTo(this FileInfo SourceFile, DirectoryInfo DestinationDirectory) => 
            SourceFile.CopyTo(Path.Combine(DestinationDirectory.FullName, Path.GetFileName(SourceFile.Name)));

        /// <summary>Скопировать файл в директорию</summary>
        /// <param name="SourceFile">Файл источник</param>
        /// <param name="DestinationDirectory">Директория назначения</param>
        /// <param name="Overwrite">Перезаписать в случае наличия файла</param>
        /// <returns>Файл копия</returns>
        public static FileInfo CopyTo(this FileInfo SourceFile, DirectoryInfo DestinationDirectory, bool Overwrite)
        {
            var new_file = Path.Combine(DestinationDirectory.FullName, Path.GetFileName(SourceFile.Name));
            return !Overwrite && File.Exists(new_file) ? new FileInfo(new_file) : SourceFile.CopyTo(new_file, true);
        }

        /// <summary>Скопировать файл</summary>
        /// <param name="SourceFile">Файл источник</param>
        /// <param name="DestinationFile">Файл копия</param>
        public static void CopyTo(this FileInfo SourceFile, FileInfo DestinationFile) =>
            SourceFile.CopyTo(DestinationFile.FullName);

        /// <summary>Скопировать файл</summary>
        /// <param name="SourceFile">Файл источник</param>
        /// <param name="DestinationFile">Файл копия</param>
        /// <param name="Overwrite">Перезаписать в случае наличия файла</param>
        public static void CopyTo(this FileInfo SourceFile, FileInfo DestinationFile, bool Overwrite) =>
            SourceFile.CopyTo(DestinationFile.FullName, Overwrite);

        /// <summary>Получить имя файла без расширения</summary>
        /// <param name="file">Файл</param>
        /// <returns>Имя файла без расширения</returns>
        public static string GetFileNameWithoutExtension(this FileInfo file) => Path.GetFileNameWithoutExtension(file.Name);

        /// <summary>Получить имя файла без расширения</summary>
        /// <param name="file">Файл</param>
        /// <returns>Имя файла без расширения</returns>
        public static string GetFullFileNameWithoutExtension(this FileInfo file) =>
            Path.Combine(file.Directory!.FullName, file.GetFileNameWithoutExtension());

        /// <summary>Получить имя файла c новым расширением</summary>
        /// <param name="file">Файл</param>
        /// <param name="NewExt">Новое расширение файла в формате ".exe"</param>
        /// <returns>Имя файла без расширения</returns>
        public static string GetFullFileNameWithNewExtension(this FileInfo file, string NewExt) =>
            Path.ChangeExtension(file.FullName, NewExt);

        /// <summary>Записать массив байт в файл</summary>
        /// <param name="file">Файл данных</param>
        /// <param name="Data">Данные</param>
        /// <param name="Append">Если истина, то данные будут добавлены в конец файла</param>
        public static void WriteAllBytes(this FileInfo file, byte[] Data, bool Append = false)
        {
            using var stream = new FileStream(file.FullName, Append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read);
            stream.Write(Data, 0, Data.Length);
        }

        /// <summary>Записать все данные из потока в файл</summary>
        /// <param name="file">Файл данных</param>
        /// <param name="DataStream">Поток - источник данных</param>
        /// <param name="BufferSize">Размер буфера чтения по умолчанию 1024 байта</param>
        /// <param name="Append">Флаг добавления данных в конец файла</param>
        /// <param name="CompleteHandler">
        /// Обработчик текущего положения каретки чтения данных из потока. 
        /// Вызывается после чтения данных в буфер и до помещения их в файл.
        /// Должен вернуть истину, что бы данные были переданы в файл и процесс был продолжен.
        /// </param>
        /// <param name="OnComplete">Обработчик события завершения процесса записи данных</param>
        public static void WriteAllBytes(
             this FileInfo file,
             Stream DataStream,
            int BufferSize = 1024,
            bool Append = false,
            Func<long, byte[], bool>? CompleteHandler = null,
            EventHandler<EventArgs<FileInfo, Stream>>? OnComplete = null)
        {
            var buffer = new byte[BufferSize];
            var write = true;
            using (var data = new FileStream(file.FullName, Append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read))
                do
                {
                    var read_count = DataStream.Read(buffer, 0, BufferSize);
                    if (CompleteHandler != null) write = CompleteHandler(DataStream.Position, buffer);
                    if (write && read_count != 0) data.Write(buffer, 0, read_count);
                    else write = false;
                } while (write);
            OnComplete.Start(file, new EventArgs<FileInfo, Stream>(file, DataStream));
        }

        /// <summary>Получить объект наблюдения за файлом</summary>
        /// <param name="file">Наблюдаемый файл</param>
        /// <returns>Объект наблюдатель</returns>
        public static FileSystemWatcher GetWatcher(this FileInfo file) => new(file.Directory.NotNull().FullName, file.Name);

        public static Process Execute(string File, string Args = "", bool UseShellExecute = true) => Process.Start(new ProcessStartInfo(File, Args) { UseShellExecute = UseShellExecute }).NotNull();

        public static Process Execute(this FileInfo File, string Args = "", bool UseShellExecute = true) => Process.Start(new ProcessStartInfo(UseShellExecute ? File.ToString() : File.FullName, Args) { UseShellExecute = UseShellExecute }).NotNull();


        public static Process? ShowInExplorer(this FileSystemInfo File) => Process.Start("explorer", $"/select,\"{File.FullName}\"");

        public static IEnumerable<string?> GetStringLines(this FileInfo File)
        {
            using var reader = File.OpenText();
            while (!reader.EndOfStream)
                yield return reader.ReadLine();
        }

        public static IEnumerable<string?> GetStringLines(this FileInfo File, Encoding encoding)
        {
            using var reader = new StreamReader(File.FullName, encoding);
            while (!reader.EndOfStream)
                yield return reader.ReadLine();
        }

        public static void Append(this FileInfo file, string Text)
        {
            using var writer = new StreamWriter(file.Open(FileMode.Append, FileAccess.Write, FileShare.Read));
            writer.Write(Text);
        }

        public static void AppendLine(this FileInfo file, string Text)
        {
            using var writer = new StreamWriter(file.Open(FileMode.Append, FileAccess.Write, FileShare.Read));
            writer.WriteLine(Text);
        }

        public static void Append(this FileInfo file, byte[] buffer)
        {
            using var writer = new StreamWriter(file.Open(FileMode.Append, FileAccess.Write, FileShare.Read));
            writer.Write(buffer);
        }

        public static FileStream Append(this FileInfo File) => File.Open(FileMode.Append, FileAccess.Write);

        public static async Task CheckFileAccessAsync(this FileInfo File, int Timeout = 1000, int IterationCount = 100, CancellationToken Cancel = default)
        {
            if (File is null) throw new ArgumentNullException(nameof(File));
            File.ThrowIfNotFound();
            for (var i = 0; i < IterationCount; i++)
                try
                {
                    Cancel.ThrowIfCancellationRequested();
                    using var stream = File.Open(FileMode.Open, FileAccess.Read);
                    if (stream.Length > 0) return;
                }
                catch (IOException)
                {
                    await Task.Delay(Timeout, Cancel).ConfigureAwait(false);
                }
            Cancel.ThrowIfCancellationRequested();
            throw new InvalidOperationException($"Файл {File.FullName} заблокирован другим процессом");
        }

        public static FileInfo ChangeExtension(this FileInfo File, string? NewExtension) => new(Path.ChangeExtension(File.ParamNotNull(nameof(File)).FullName, NewExtension));

        public static FileInfo Zip(this FileInfo File, string? ArchiveFileName = null, bool Override = true)
        {
            if (File is null) throw new ArgumentNullException(nameof(File));
            File.ThrowIfNotFound();

            if (ArchiveFileName is null) ArchiveFileName = $"{File.FullName}.zip";
            else if (!Path.IsPathRooted(ArchiveFileName))
                ArchiveFileName = (File.Directory ?? throw new InvalidOperationException($"Не удалось получить директорию файла {File}")).CreateFileInfo(ArchiveFileName).FullName;
            using var zip_stream = IO.File.Open(ArchiveFileName, FileMode.OpenOrCreate, FileAccess.Write);
            using var zip = new ZipArchive(zip_stream);
            var file_entry = zip.GetEntry(File.Name);
            if (file_entry != null)
            {
                if (!Override) return new FileInfo(ArchiveFileName);
                file_entry.Delete();
            }

            using var file_entry_stream = zip.CreateEntry(File.Name).Open();
            using var file_stream = File.OpenRead();
            file_stream.CopyTo(file_entry_stream);

            return new FileInfo(ArchiveFileName);
        }

        public static async Task<FileInfo> ZipAsync(
            this FileInfo File, 
            byte[] Buffer, 
            string? ArchiveFileName = null, 
            bool Override = true,
            IProgress<double>? Progress = null,
            CancellationToken Cancel = default)
        {
            if (File is null) throw new ArgumentNullException(nameof(File));
            File.ThrowIfNotFound();
            Cancel.ThrowIfCancellationRequested();

            if (ArchiveFileName is null)
                ArchiveFileName = $"{File.FullName}.zip";
            else if (!Path.IsPathRooted(ArchiveFileName))
                ArchiveFileName = (File.Directory ?? throw new InvalidOperationException($"Не удалось получить директорию файла {File}")).CreateFileInfo(ArchiveFileName).FullName;
            using var zip_stream = IO.File.Open(ArchiveFileName, FileMode.OpenOrCreate, FileAccess.Write);
            using var zip = new ZipArchive(zip_stream);
            var file_entry = zip.GetEntry(File.Name);
            if (file_entry != null)
            {
                if (!Override) return new FileInfo(ArchiveFileName);
                file_entry.Delete();
            }

            using var file_entry_stream = zip.CreateEntry(File.Name).Open();
            using var file_stream = File.OpenRead();
            if (Progress is null)
                await file_stream.CopyToAsync(file_entry_stream, Buffer, Cancel).ConfigureAwait(false);
            else
                await file_stream.CopyToAsync(file_entry_stream, Buffer, file_stream.Length, Progress, Cancel).ConfigureAwait(false);

            return new FileInfo(ArchiveFileName);
        }
    }
}