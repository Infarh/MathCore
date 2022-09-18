#nullable enable
using System.Collections.Generic;
using System.Diagnostics;

using MathCore.IO;

// ReSharper disable once CheckNamespace
namespace System.IO;

public static class FileSystemInfoExtensions
{
    public static IReadOnlyCollection<Process> GetLockingProcesses(this FileSystemInfo Info) => Info switch
    {
        null => throw new ArgumentNullException(nameof(Info)),
        DirectoryInfo { Exists: false, FullName: var path } => throw new FileNotFoundException("Указанная директория не существует", path),
        FileInfo { Exists: false, FullName: var path } => throw new FileNotFoundException("Указанный файл не существует", path),
        _ => Win32Processes.GetLockingProcesses(Info.FullName)
    };
}
