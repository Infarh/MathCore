using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

using MathCore;

using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using MathCore.Annotations;
using MathCore.Trees;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.IO
{
    /// <summary>Класс методов-расширений для объектов класса System.IO.DirectoryInfo</summary>
    public static class DirectoryInfoExtensions
    {
        public static FileInfo Zip(this DirectoryInfo Directory) => Directory.Zip(Directory.FullName + ".zip");

        public static async Task<FileInfo> ZipAsync(this DirectoryInfo Directory, CancellationToken Cancel = default) => await Directory.ZipAsync(Directory.FullName + ".zip", Cancel);

        public static FileInfo Zip(this DirectoryInfo Directory, string ArchiveFilePath) => Directory.Zip(new FileInfo(ArchiveFilePath));

        public static async Task<FileInfo> ZipAsync(this DirectoryInfo Directory, string ArchiveFilePath, CancellationToken Cancel = default) => await Directory.ZipAsync(new FileInfo(ArchiveFilePath), Cancel);

        [NotNull]
        public static FileInfo Zip([NotNull] this DirectoryInfo Directory, [NotNull] FileInfo ArchiveFile)
        {
            Directory.ThrowIfNotFound("Архивируемая директория не найдена");

            static ZipArchive GetArchive(FileInfo file) => file.Exists
                ? new ZipArchive(file.OpenWrite(), ZipArchiveMode.Update)
                : new ZipArchive(file.Create(), ZipArchiveMode.Create, false);

            const int buffer_size = Consts.DataLength.Bytes.MB;
            var buffer = new byte[buffer_size];
            using (var zip = GetArchive(ArchiveFile))
                foreach (var file in Directory.EnumerateFiles("*.*", SearchOption.AllDirectories))
                {
                    var path = Path.Combine(file.Directory!.GetRelativePosition(Directory)!, file.Name);
                    var entry = zip.CreateEntry(path, CompressionLevel.Optimal);
                    using var zip_stream = entry.Open();
                    using var file_stream = file.Open(FileMode.Open);
                    file_stream.CopyTo(zip_stream, buffer);
                }

            ArchiveFile.Refresh();
            return ArchiveFile;
        }

        [NotNull]
        public static async Task<FileInfo> ZipAsync([NotNull] this DirectoryInfo Directory, [NotNull] FileInfo ArchiveFile, CancellationToken Cancel = default)
        {
            Directory.ThrowIfNotFound("Архивируемая директория не найдена");

            var is_new = !ArchiveFile.Exists;

            static ZipArchive GetArchive(FileInfo file) => file.Exists
                ? new ZipArchive(file.OpenWrite(), ZipArchiveMode.Update)
                : new ZipArchive(file.Create(), ZipArchiveMode.Create, false);

            const int buffer_size = Consts.DataLength.Bytes.MB;
            var buffer = new byte[buffer_size];
            try
            {
                using var zip = GetArchive(ArchiveFile);
                foreach (var file in Directory.EnumerateFiles("*.*", SearchOption.AllDirectories))
                {
                    var path = Path.Combine(file.Directory!.GetRelativePosition(Directory)!, file.Directory.Name, file.Name);
                    var entry = zip.CreateEntry(path, CompressionLevel.Optimal);
                    using var zip_stream = entry.Open();
                    using var file_stream = file.Open(FileMode.Open);
                    await file_stream.CopyToAsync(zip_stream, buffer, Cancel).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (is_new)
            {
                ArchiveFile.Delete();
            }

            ArchiveFile.Refresh();
            return ArchiveFile;
        }

        /// <summary>Проверка, что директория существует</summary>
        /// <param name="Dir">Проверяемая директория</param>
        /// <param name="Message">Сообщение, добавляемое в исключение, если директория не найдена</param>
        /// <returns>Директория, гарантированно существующая</returns>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">В случае если <paramref name="Dir"/> не существует.</exception>
        [NotNull]
        public static DirectoryInfo ThrowIfNotFound([CanBeNull] this DirectoryInfo Dir, [CanBeNull] string Message = null)
        {
            var dir = Dir.NotNull("Отсутствует ссылка на директории");
            return !dir.Exists ? throw new DirectoryNotFoundException(Message ?? $"Директория {dir.FullName} не найдена") : dir;
        }

        /// <summary>Представить директорию в виде узла дерева</summary>
        /// <param name="dir">Преобразуемая директория</param>
        /// <returns>Узел дерева каталогов</returns>
        [NotNull] public static TreeNode<DirectoryInfo> AsTreeNode([NotNull] this DirectoryInfo dir) => dir.AsTreeNode(d => d.EnumerateDirectories(), d => d.Parent);

        /// <summary>Представить директорию в виде узла дерева</summary>
        /// <param name="dir">Преобразуемая директория</param>
        /// <param name="OnError">Метод обработки ошибок доступа</param>
        /// <returns>Узел дерева каталогов</returns>
        [NotNull] public static TreeNode<DirectoryInfo> AsTreeNode([NotNull] this DirectoryInfo dir, [CanBeNull] Action<DirectoryInfo, Exception> OnError) => dir.AsTreeNode(d => d.Try(v => v.EnumerateDirectories(), OnError), d => d.Parent);

        [NotNull]
        public static Process ShowInExplorer([NotNull] this FileSystemInfo dir) => Process.Start("explorer", $"/select,\"{dir.FullName}\"") ?? throw new InvalidOperationException();

        [NotNull]
        public static Process OpenInFileExplorer([NotNull] this DirectoryInfo dir) => Process.Start("explorer", dir.FullName) ?? throw new InvalidOperationException();

        [CanBeNull]
        public static string GetRelativePosition([NotNull] this DirectoryInfo current, [NotNull] DirectoryInfo other)
        {
            if (current is null) throw new ArgumentNullException(nameof(current));
            if (other is null) throw new ArgumentNullException(nameof(other));
            return GetRelativePosition(current.FullName, other.FullName);
        }

        [CanBeNull]
        public static string GetRelativePosition([NotNull] string current, [NotNull] string other)
        {
            if (current is null) throw new ArgumentNullException(nameof(current));
            if (other is null) throw new ArgumentNullException(nameof(other));

            const StringComparison str_cmp = StringComparison.InvariantCultureIgnoreCase;
            return !string.Equals(Path.GetPathRoot(current), Path.GetPathRoot(other), str_cmp)
                ? null
                : current.StartsWith(other, str_cmp)
                    ? current.Remove(0, other.Length)
                    : other.StartsWith(current, str_cmp)
                        ? other.Remove(0, current.Length)
                        : null;
        }

        /// <summary>Является ли одна директория поддиректорией другой?</summary>
        /// <param name="target">Дочерняя директория</param>
        /// <param name="parent">Родительская директория</param>
        /// <returns>Истина, если путь к директории, заявленной как дочерняя является вложенным в путь к директории, заявленной как родительская</returns>
        public static bool IsSubDirectoryOf([CanBeNull] this DirectoryInfo target, [CanBeNull] DirectoryInfo parent)
        {
            if (target?.FullName is not { } target_path) return false;
            if (parent?.FullName is not { } parent_path) return false;
            const StringComparison comp = StringComparison.InvariantCultureIgnoreCase;
            return target_path.Length > parent_path.Length && target_path.StartsWith(parent_path, comp);
        }

        /// <summary>Создать объект с информацией о вложенном файле</summary>
        /// <param name="directory">Родительская директория</param>
        /// <param name="FileRelativePath">Относительный путь к файлу внутри директории</param>
        /// <returns>Фал по указанному пути внутри директории</returns>
        [NotNull]
        public static FileInfo CreateFileInfo([NotNull] this DirectoryInfo directory, string FileRelativePath) => 
            new(Path.Combine(directory.FullName, FileRelativePath.Replace(':', '.')));

        /// <summary>Создать объект с информацией о поддиректории</summary>
        /// <param name="directory">Родительская директория</param>
        /// <param name="DirectoryRelativePath">Относительный путь к дочерней директории</param>
        /// <returns>Дочерняя директория</returns>
        [NotNull]
        public static DirectoryInfo CreateDirectoryInfo([NotNull] this DirectoryInfo directory, string DirectoryRelativePath) => 
            new(Path.Combine(directory.FullName, DirectoryRelativePath.Replace(':', '.')));

        /// <summary>Определить число всех вложенных файлов</summary>
        /// <param name="Directory">Исследуемая директория</param>
        /// <returns>Число файлов во всех вложенных поддиректориях</returns>
        [DST]
        public static long GetFilesCount(this DirectoryInfo Directory) => Directory.EnumerateFiles().Count();

        /// <summary>Определить число всех вложенных файлов</summary>
        /// <param name="Directory">Исследуемая директория</param>
        /// <param name="Pattern">Маска файлов</param>
        /// <returns>Число файлов во всех вложенных поддиректориях</returns>
        [DST]
        public static long GetFilesCount(this DirectoryInfo Directory, string Pattern) => Directory.EnumerateFiles(Pattern).Count();

        /// <summary>Определить объём всех вложенных файлов включая поддиректории</summary>
        /// <param name="Directory">Исследуемая директория</param>
        /// <returns>Число байт всех вложенных файлов</returns>
        [DST]
        public static long GetSize(this DirectoryInfo Directory)
        {
            var result = 0L;
            var queue = new Queue<DirectoryInfo>();
            queue.Enqueue(Directory);
            do
            {
                var info = queue.Dequeue();
                result += info.GetFiles().Sum(f => f.Length);
                info.GetDirectories().Foreach(queue.Enqueue);
            } while(queue.Count != 0);
            return result;
        }

        /// <summary>Определить число поддиректорий</summary>
        /// <param name="Directory">Исследуемая директория</param>
        /// <returns>Число элементов в дереве поддиректорий</returns>
        [DST]
        public static long GetSubdirectoriesCount(this DirectoryInfo Directory)
        {
            var num = 0L;
            var queue = new Queue<DirectoryInfo>();
            queue.Enqueue(Directory);
            do
            {
                var directories = queue.Dequeue().GetDirectories();
                num += directories.Length;
                directories.Foreach(queue.Enqueue);
            } while(queue.Count != 0);
            return num;
        }

        /// <summary>Проверить - является ли директория пустой</summary>
        /// <param name="Directory">Проверяемая директория</param>
        /// <returns>Истина, если директория пуста</returns>
        [DST]
        public static bool IsEmpty([NotNull] this DirectoryInfo Directory) => Directory.GetDirectories().Length == 0 && (Directory.GetFiles().Length == 0);

        /// <summary>Получить объект наблюдения за директорией</summary>
        /// <param name="directory">Наблюдаемая директория</param>
        /// <param name="filter">Фильтр файлов</param>
        /// <returns>Объект наблюдатель</returns>
        [DST]
        [NotNull]
        public static FileSystemWatcher GetWatcher([NotNull] this DirectoryInfo directory, [CanBeNull] string filter = null) =>
            string.IsNullOrEmpty(filter)
                ? new FileSystemWatcher(directory.FullName)
                : new FileSystemWatcher(directory.FullName, filter);

        [NotNull]
        public static FileSystemWatcher GetWatcher([NotNull] this DirectoryInfo directory, [CanBeNull] string filter, [CanBeNull] Action<FileSystemWatcher> initializer)
        {
            var watcher = string.IsNullOrWhiteSpace(filter)
                        ? new FileSystemWatcher(directory.FullName)
                        : new FileSystemWatcher(directory.FullName, filter);
            initializer?.Invoke(watcher);
            return watcher;
        }

        [NotNull]
        public static FileSystemWatcher GetWatcher([NotNull] this DirectoryInfo directory, [CanBeNull] Action<FileSystemWatcher> initializer = null) => directory.GetWatcher(null, initializer);

        public static bool ContainsFile([NotNull] this DirectoryInfo directory, [NotNull] string file) => File.Exists(Path.Combine(directory.FullName, file));

        public static bool ContainsFileMask([NotNull] this DirectoryInfo directory, [NotNull] string mask) => directory.EnumerateFiles(mask).Any();
        
        public static bool IsParentOf([NotNull] this DirectoryInfo parent, [NotNull] DirectoryInfo directory) => directory.IsSubDirectoryOf(parent);

        [NotNull, ItemNotNull] public static IEnumerable<FileInfo> FindFiles([NotNull] this DirectoryInfo dir, [NotNull] string mask) =>
            dir.EnumerateFiles(mask, SearchOption.AllDirectories);

        /// <summary>Получение поддиректории по заданному пути. Если поддиректория отсутствует, то создать новую</summary>
        /// <param name="ParentDirectory">Родительская директория</param>
        /// <param name="SubDirectoryPath">Относительный путь к поддиректории</param>
        /// <returns>Поддиректория</returns>
        [NotNull]
        public static DirectoryInfo SubDirectoryOrCreate([NotNull] this DirectoryInfo ParentDirectory, [NotNull] string SubDirectoryPath)
        {
            if (ParentDirectory is null) throw new ArgumentNullException(nameof(ParentDirectory));
            if (SubDirectoryPath is null) throw new ArgumentNullException(nameof(SubDirectoryPath));
            if (string.IsNullOrWhiteSpace(SubDirectoryPath)) throw new ArgumentException("Не указан путь дочернего каталога", nameof(SubDirectoryPath));

            var sub_dir_path = Path.Combine(ParentDirectory.FullName, SubDirectoryPath);
            var sub_dir = new DirectoryInfo(sub_dir_path);
            if (sub_dir.Exists) return sub_dir;
            sub_dir.Create();
            sub_dir.Refresh();
            return sub_dir;
        }

        /// <summary>Формирование информации о поддиректории, заданной своим именем, либо относительным путём</summary>
        /// <param name="Directory">Корневая директория</param><param name="SubDirectoryPath">Путь к поддиректории</param>
        /// <exception cref="ArgumentNullException">Если указана пустая ссылка на <paramref name="Directory"/></exception>
        /// <exception cref="ArgumentNullException">Если указана пустая ссылка на <paramref name="SubDirectoryPath"/></exception>
        /// <returns>Информация о поддиректории</returns>
        [NotNull]
        public static DirectoryInfo SubDirectory([NotNull] this DirectoryInfo Directory, [NotNull] string SubDirectoryPath)
        {
            if (Directory is null) throw new ArgumentNullException(nameof(Directory));
            if (SubDirectoryPath is null) throw new ArgumentNullException(nameof(SubDirectoryPath));
            return string.IsNullOrEmpty(SubDirectoryPath) ? Directory : new DirectoryInfo(Path.Combine(Directory.FullName, SubDirectoryPath));
        }
    }
}