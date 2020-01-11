using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.IO
{
    /// <summary>Класс методов расширений для объектов класса System.IO.FileInfo</summary>
    public static class FileInfoExtensions
    {
        [CanBeNull]
        public static Process ExecuteAsAdmin([NotNull] this FileInfo File, [NotNull] string Args = "", bool UseShellExecute = true) =>
            File.Execute(Args, UseShellExecute, "runas");

        [CanBeNull]
        public static Process Execute([NotNull] this FileInfo File, [NotNull] string Args, bool UseShellExecute, string Verb) =>
            Process.Start(new ProcessStartInfo(File.FullName, Args)
            {
                UseShellExecute = UseShellExecute,
                Verb = Verb
            });

        [NotNull]
        public static FileInfo ThrowIfNotFound([NotNull] this FileInfo file, [CanBeNull] string Message = null)
        {
            if (!file.Exists) throw new FileNotFoundException(Message ?? $"Файл {file} не найден");
            return file;
        }

        [NotNull]
        public static byte[] ComputeSHA256([NotNull] this FileInfo file)
        {
            if (file is null) throw new ArgumentNullException(nameof(file));
            using var stream = file.ThrowIfNotFound().OpenRead();
            using var md5 = new Security.Cryptography.SHA256Managed();
            return md5.ComputeHash(stream);
        }

        [NotNull]
        public static byte[] ComputeMD5([NotNull] this FileInfo file)
        {
            if (file is null) throw new ArgumentNullException(nameof(file));
            using var stream = file.ThrowIfNotFound().OpenRead();
            using var md5 = new Security.Cryptography.MD5CryptoServiceProvider();
            return md5.ComputeHash(stream);
        }

        public static IEnumerable<byte> ReadByteses([NotNull] this FileInfo file)
        {
            using var stream = file.ThrowIfNotFound().OpenRead();
            using var reader = new BinaryReader(stream);
            while (!reader.IsEOF())
                yield return reader.ReadByte();
        }

        [ItemNotNull]
        public static IEnumerable<byte[]> ReadByteses([NotNull] this FileInfo file, int Length)
        {
            using var stream = file.ThrowIfNotFound().OpenRead();
            using var reader = new BinaryReader(stream);
            while (!reader.IsEOF())
                yield return reader.ReadBytes(Length);
        }

        [ItemCanBeNull]
        public static IEnumerable<string> ReadLines([NotNull] this FileInfo file)
        {
            using var reader = file.ThrowIfNotFound().OpenText();
            while (!reader.EndOfStream)
                yield return reader.ReadLine();
        }

        [ItemCanBeNull]
        public static IEnumerable<string> ReadLines([NotNull] this FileInfo file, [CanBeNull] Action<StreamReader> initializer)
        {
            using var reader = new StreamReader(new BufferedStream(file.ThrowIfNotFound().Open(FileMode.Open, FileAccess.Read, FileShare.Read), 1024 * 1024 * 3));
            initializer?.Invoke(reader);
            while (!reader.EndOfStream)
                yield return reader.ReadLine();
        }

        [CanBeNull]
        public static string ReadAllText([NotNull] this FileInfo file, bool ThrowNotExist = true)
        {
            if (!file.Exists && !ThrowNotExist) return null;
            using var reader = file.OpenText();
            return reader.ReadToEnd();
        }

        /// <summary>Скопировать файл в дирректорию</summary>
        /// <param name="SourceFile">Файл источник</param>
        /// <param name="DestinationDirectory">Дирректория назначения</param>
        /// <returns>Файл копия</returns>
        [DST]
        [NotNull]
        public static FileInfo CopyTo([NotNull] this FileInfo SourceFile, [NotNull] DirectoryInfo DestinationDirectory) => SourceFile.CopyTo($@"{DestinationDirectory.FullName}\{Path.GetFileName(SourceFile.Name)}");

        /// <summary>Скопировать файл в дирректорию</summary>
        /// <param name="SourceFile">Файл источник</param>
        /// <param name="DestinationDirectory">Дирректория назначения</param>
        /// <param name="Owerride">Перезаписать в случае наличия файла</param>
        /// <returns>Файл копия</returns>
        [DST]
        [NotNull]
        public static FileInfo CopyTo([NotNull] this FileInfo SourceFile, [NotNull] DirectoryInfo DestinationDirectory, bool Owerride)
        {
            var new_file = $@"{DestinationDirectory.FullName}\{Path.GetFileName(SourceFile.Name)}";
            return !Owerride && File.Exists(new_file) ? new FileInfo(new_file) : SourceFile.CopyTo(new_file, true);
        }

        /// <summary>Скопировать файл</summary>
        /// <param name="SourceFile">Файл источник</param>
        /// <param name="DestinationFile">Файл копия</param>
        [DST]
        public static void CopyTo([NotNull] this FileInfo SourceFile, [NotNull] FileInfo DestinationFile) => SourceFile.CopyTo(DestinationFile.FullName);

        /// <summary>Скопировать файл</summary>
        /// <param name="SourceFile">Файл источник</param>
        /// <param name="DestinationFile">Файл копия</param>
        /// <param name="Owerride">Перезаписать в случае наличия файла</param>
        [DST]
        public static void CopyTo([NotNull] this FileInfo SourceFile, [NotNull] FileInfo DestinationFile, bool Owerride) => SourceFile.CopyTo(DestinationFile.FullName, Owerride);

        /// <summary>Получить имя файла без расширения</summary>
        /// <param name="file">Файл</param>
        /// <returns>Имя файла без расширения</returns>
        [DST]
        [NotNull]
        public static string GetFileNameWithoutExtension([NotNull] this FileInfo file) => Path.GetFileNameWithoutExtension(file.Name);

        /// <summary>Получить имя файла без расширения</summary>
        /// <param name="file">Файл</param>
        /// <returns>Имя файла без расширения</returns>
        [DST]
        [NotNull]
        public static string GetFullFileNameWithoutExtension([NotNull] this FileInfo file) => $"{Path.GetDirectoryName(file.FullName)}\\{Path.GetFileNameWithoutExtension(file.Name)}";

        /// <summary>Получить имя файла c новым расширением</summary>
        /// <param name="file">Файл</param>
        /// <param name="NewExt">Новое расширение файла в формате ".exe"</param>
        /// <returns>Имя файла без расширения</returns>
        [DST]
        [NotNull]
        public static string GetFullFileNameWithNewExtension([NotNull] this FileInfo file, string NewExt) => Path.Combine(Path.GetDirectoryName(file.FullName).NotNull(), $"{Path.GetFileNameWithoutExtension(file.Name)}{NewExt}");

        /// <summary>Записать массив байт в файл</summary>
        /// <param name="file">Файл данных</param>
        /// <param name="Data">Данные</param>
        /// <param name="Append">Если истина, то данные будут добавлены в конец файла</param>
        [DST]
        public static void WriteAllBytes([NotNull] this FileInfo file, [NotNull] byte[] Data, bool Append = false)
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
        /// Обработчик текущего положения коретки чтения данных из потока. 
        /// Вызывается после чтения данных в буфер и до помещения их в файл.
        /// Должен вернуть истину, что бы данные были переданы в файл и процесс был продолжен.
        /// </param>
        /// <param name="OnComplete">Обработчик события завершения процесса записи данных</param>
        [DST]
        public static void WriteAllBytes(
            [NotNull] this FileInfo file,
            [NotNull] Stream DataStream,
            int BufferSize = 1024,
            bool Append = false,
            [CanBeNull] Func<long, byte[], bool> CompleteHandler = null,
            [CanBeNull] EventHandler<EventArgs<FileInfo, Stream>> OnComplete = null)
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
        [DST, CanBeNull]
        public static FileSystemWatcher GetWatcher([CanBeNull] this FileInfo file) => file is null ? null : new FileSystemWatcher(file.Directory.NotNull().FullName, file.Name);

        [NotNull]
        public static Process Execute([NotNull] string File, [NotNull] string Args = "", bool UseShellExecute = true) => Process.Start(new ProcessStartInfo(File, Args) { UseShellExecute = UseShellExecute }).NotNull();

        [NotNull]
        public static Process Execute([NotNull] this FileInfo File, string Args = "", bool UseShellExecute = true) => Process.Start(new ProcessStartInfo(UseShellExecute ? File.ToString() : File.FullName, Args) { UseShellExecute = UseShellExecute }).NotNull();


        [CanBeNull]
        public static Process ShowInExplorer([NotNull] this FileSystemInfo File) => Process.Start("explorer", $"/select,\"{File.FullName}\"");

        [ItemCanBeNull]
        public static IEnumerable<string> GetStringLines([NotNull] this FileInfo File)
        {
            using var reader = File.OpenText();
            while (!reader.EndOfStream)
                yield return reader.ReadLine();
        }

        [ItemCanBeNull]
        public static IEnumerable<string> GetStringLines([NotNull] this FileInfo File, [NotNull] Encoding encoding)
        {
            using var reader = new StreamReader(File.FullName, encoding);
            while (!reader.EndOfStream)
                yield return reader.ReadLine();
        }

        public static void Append([NotNull] this FileInfo file, string Text)
        {
            using var writer = new StreamWriter(file.Open(FileMode.Append, FileAccess.Write, FileShare.Read));
            writer.Write(Text);
        }

        public static void AppendLine([NotNull] this FileInfo file, string Text)
        {
            using var writer = new StreamWriter(file.Open(FileMode.Append, FileAccess.Write, FileShare.Read));
            writer.WriteLine(Text);
        }

        public static void Append([NotNull] this FileInfo file, byte[] buffer)
        {
            using var writer = new StreamWriter(file.Open(FileMode.Append, FileAccess.Write, FileShare.Read));
            writer.Write(buffer);
        }

        [NotNull] public static FileStream Append([NotNull] this FileInfo File) => File.Open(FileMode.Append, FileAccess.Write);

        public static async Task CheckFileAccessAsync([NotNull] this FileInfo File, int Timeout = 1000, int IterationCount = 100, CancellationToken Cancel = default)
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

        [NotNull] public static FileInfo ChangeExtension([NotNull] this FileInfo File, string NewExtension) => new FileInfo(Path.ChangeExtension(File.ParamNotNull(nameof(File)).FullName, NewExtension));

        [NotNull]
        public static FileInfo Zip([NotNull] this FileInfo File, string ArchiveFileName = null, bool Override = true)
        {
            if (File is null) throw new ArgumentNullException(nameof(File));
            File.ThrowIfNotFound();

            if (ArchiveFileName is null) ArchiveFileName = File.FullName + ".zip";
            else if (!Path.IsPathRooted(ArchiveFileName))
                ArchiveFileName = File.Directory.CreateFileInfo(ArchiveFileName).FullName;
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
    }
}