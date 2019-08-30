using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace System.IO
{
    /// <summary>Класс методов расширений для объектов класса System.IO.FileInfo</summary>
    public static class FileInfoExtensions
    {
        public static byte[] ComputeSHA256([NotNull] this FileInfo file)
        {
            using var stream = file.OpenRead();
            using var md5 = new Security.Cryptography.SHA256Managed();
            return md5.ComputeHash(stream);
        }

        public static byte[] ComputeMD5([NotNull] this FileInfo file)
        {
            using var stream = file.OpenRead();
            using var md5 = new Security.Cryptography.MD5CryptoServiceProvider();
            return md5.ComputeHash(stream);
        }

        public static IEnumerable<byte> ReadByteses([NotNull] this FileInfo file)
        {
            using var stream = file.OpenRead();
            using var reader = new BinaryReader(stream);
            while(!reader.IsEOF())
                yield return reader.ReadByte();
        }

        public static IEnumerable<byte[]> ReadByteses([NotNull] this FileInfo file, int Length)
        {
            using var stream = file.OpenRead();
            using var reader = new BinaryReader(stream);
            while(!reader.IsEOF())
                yield return reader.ReadBytes(Length);
        }

        public static IEnumerable<string> ReadLines([NotNull] this FileInfo file)
        {
            using var reader = file.OpenText();
            while(!reader.EndOfStream)
                yield return reader.ReadLine();
        }

        public static IEnumerable<string> ReadLines([NotNull] this FileInfo file, [CanBeNull] Action<StreamReader> initializer)
        {
            using var reader = new StreamReader(new BufferedStream(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), 1024 * 1024 * 3));
            initializer?.Invoke(reader);
            while(!reader.EndOfStream)
                yield return reader.ReadLine();
        }

        public static string ReadAllText([NotNull] this FileInfo file, bool ThrowNotExist = true)
        {
            if(!file.Exists && !ThrowNotExist) return null;
            using var reader = file.OpenText();
            return reader.ReadToEnd();
        }

        /// <summary>Скопировать файл в дирректорию</summary>
        /// <param name="SourceFile">Файл источник</param>
        /// <param name="DestinationDirectory">Дирректория назначения</param>
        /// <returns>Файл копия</returns>
        [DST]
        public static FileInfo CopyTo([NotNull] this FileInfo SourceFile, [NotNull] DirectoryInfo DestinationDirectory)
        {
            Contract.Requires(SourceFile != null);
            Contract.Requires(DestinationDirectory != null);

            return SourceFile.CopyTo($@"{DestinationDirectory.FullName}\{Path.GetFileName(SourceFile.Name)}");
        }

        /// <summary>Скопировать файл в дирректорию</summary>
        /// <param name="SourceFile">Файл источник</param>
        /// <param name="DestinationDirectory">Дирректория назначения</param>
        /// <param name="Owerride">Перезаписать в случае наличия файла</param>
        /// <returns>Файл копия</returns>
        [DST]
        public static FileInfo CopyTo([NotNull] this FileInfo SourceFile, [NotNull] DirectoryInfo DestinationDirectory, bool Owerride)
        {
            Contract.Requires(SourceFile != null);
            Contract.Requires(DestinationDirectory != null);

            var new_file = $@"{DestinationDirectory.FullName}\{Path.GetFileName(SourceFile.Name)}";
            if(!Owerride && File.Exists(new_file)) return new FileInfo(new_file);
            return SourceFile.CopyTo(new_file, true);
        }

        /// <summary>Скопировать файл</summary>
        /// <param name="SourceFile">Файл источник</param>
        /// <param name="DestinationFile">Файл копия</param>
        [DST]
        public static void CopyTo([NotNull] this FileInfo SourceFile, [NotNull] FileInfo DestinationFile)
        {
            Contract.Requires(SourceFile != null);
            Contract.Requires(DestinationFile != null);

            SourceFile.CopyTo(DestinationFile.FullName);
        }

        /// <summary>Скопировать файл</summary>
        /// <param name="SourceFile">Файл источник</param>
        /// <param name="DestinationFile">Файл копия</param>
        /// <param name="Owerride">Перезаписать в случае наличия файла</param>
        [DST]
        public static void CopyTo(this FileInfo SourceFile, FileInfo DestinationFile, bool Owerride)
        {
            Contract.Requires(SourceFile != null);
            Contract.Requires(DestinationFile != null);

            SourceFile.CopyTo(DestinationFile.FullName, Owerride);
        }

        /// <summary>Получить имя файла без расширения</summary>
        /// <param name="file">Файл</param>
        /// <returns>Имя файла без расширения</returns>
        [DST]
        public static string GetFileNameWithoutExtension(this FileInfo file)
        {
            Contract.Requires(file != null);
            Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

            return Path.GetFileNameWithoutExtension(file.Name);
        }

        /// <summary>Получить имя файла без расширения</summary>
        /// <param name="file">Файл</param>
        /// <returns>Имя файла без расширения</returns>
        [DST]
        public static string GetFullFileNameWithoutExtension(this FileInfo file)
        {
            Contract.Requires(file != null);
            Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

            return $"{Path.GetDirectoryName(file.FullName)}\\{Path.GetFileNameWithoutExtension(file.Name)}";
        }

        /// <summary>Получить имя файла c новым расширением</summary>
        /// <param name="file">Файл</param>
        /// <param name="NewExt">Новое расширение файла в формате ".exe"</param>
        /// <returns>Имя файла без расширения</returns>
        [DST]
        public static string GetFullFileNameWithNewExtension(this FileInfo file, string NewExt)
        {
            Contract.Requires(file != null);
            Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
            return Path.Combine(Path.GetDirectoryName(file.FullName), $"{Path.GetFileNameWithoutExtension(file.Name)}{NewExt}");
        }

        /// <summary>Записать массив байт в файл</summary>
        /// <param name="file">Файл данных</param>
        /// <param name="Data">Данные</param>
        /// <param name="Append">Если истина, то данные будут добавлены в конец файла</param>
        [DST]
        public static void WriteAllBytes(this FileInfo file, byte[] Data, bool Append = false)
        {
            Contract.Requires(file != null);
            Contract.Requires(Data != null);

            using var stream = new FileStream(file.FullName, Append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read);
            stream.Write(Data, 0, Data.Length);
        }

        /// <summary>Записать все данные из потока в файл</summary>
        /// <param name="file">Файл данных</param>
        /// <param name="DataStream">Поток - источник данных</param>
        /// <param name="BufferSize">Размер буфера чтения по умолчанию 1024 байта</param>
        /// <param name="Append">Флаг добавления данных в конец файла</param>
        /// <param name="CompliteHandler">
        /// Обработчик текущего положения коретки чтения данных из потока. 
        /// Вызывается после чтения данных в буфер и до помещения их в файл.
        /// Должен вернуть истину, что бы данные были переданы в файл и процесс был продолжен.
        /// </param>
        /// <param name="OnComplite">Обработчик события завершения процесса записи данных</param>
        [DST]
        public static void WriteAllBytes(
            [NotNull] this FileInfo file, 
            [NotNull] Stream DataStream,
            int BufferSize = 1024,
            bool Append = false, 
            [CanBeNull] Func<long, byte[], bool> CompliteHandler = null,
            [CanBeNull] EventHandler<EventArgs<FileInfo, Stream>> OnComplite = null)
        {
            Contract.Requires(file != null);
            Contract.Requires(DataStream != null);
            Contract.Requires(DataStream.CanRead);

            var buffer = new byte[BufferSize];
            var write = true;
            using(var data = new FileStream(file.FullName, Append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read))
                do
                {
                    var read_count = DataStream.Read(buffer, 0, BufferSize);
                    if(CompliteHandler != null) write = CompliteHandler(DataStream.Position, buffer);
                    if(write && read_count != 0) data.Write(buffer, 0, read_count);
                    else write = false;
                } while(write);
            OnComplite.Start(file, new EventArgs<FileInfo, Stream>(file, DataStream));
        }

        /// <summary>Получить объект наблюдения за файлом</summary>
        /// <param name="file">Наблюдаемый файл</param>
        /// <returns>Объект наблюдатель</returns>
        [DST, CanBeNull]
        public static FileSystemWatcher GetWatcher([CanBeNull] this FileInfo file) => file == null ? null : new FileSystemWatcher(file.Directory.FullName, file.Name);

        [NotNull]
        public static Process Execute([NotNull] string File, [NotNull] string Args = "", bool UseShellExecute = true) => Process.Start(new ProcessStartInfo(File, Args) { UseShellExecute = UseShellExecute });

        [NotNull]
        public static Process Execute([NotNull] this FileInfo File, string Args = "", bool UseShellExecute = true) => Process.Start(new ProcessStartInfo(UseShellExecute ? File.ToString() : File.FullName, Args) { UseShellExecute = UseShellExecute });


        public static Process ShowInExplorer([NotNull] this FileSystemInfo File) => Process.Start("explorer", $"/select,\"{File.FullName}\"");

        public static IEnumerable<string> GetStringLines([NotNull] this FileInfo File)
        {
            using var reader = File.OpenText();
            while(!reader.EndOfStream)
                yield return reader.ReadLine();
        }

        public static IEnumerable<string> GetStringLines([NotNull] this FileInfo File, Encoding encoding)
        {
            using var reader = new StreamReader(File.FullName, encoding);
            while(!reader.EndOfStream)
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
    }
}