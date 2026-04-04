using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace MathCore.IO.Transactions;

/// <summary>Объектно-ориентированная транзакция операций с NTFS</summary>
[SupportedOSPlatform("windows")]
public sealed class NtfsTransaction : IDisposable
{
    private static readonly IntPtr __InvalidHandle = new(-1);

    private IntPtr _Handle;
    private NtfsTransactionState _State;

    /// <summary>Создаёт новую транзакцию NTFS</summary>
    /// <param name="Options">Параметры транзакции</param>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается при отрицательном таймауте</exception>
    /// <exception cref="PlatformNotSupportedException">Выбрасывается при запуске вне Windows</exception>
    /// <exception cref="NtfsTransactionException">Выбрасывается при ошибке открытия транзакции</exception>
    public NtfsTransaction(NtfsTransactionOptions? Options = null)
    {
        EnsureWindows();

        var options = Options ?? new NtfsTransactionOptions();

        if (options.TimeoutMilliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(Options), options.TimeoutMilliseconds, "Значение таймаута не может быть отрицательным");

        _Handle = NativeMethods.CreateTransaction(IntPtr.Zero, IntPtr.Zero, 0, 0, 0, (uint)options.TimeoutMilliseconds, options.Description);
        if (_Handle == IntPtr.Zero || _Handle == __InvalidHandle)
            ThrowLastWin32Error("Не удалось открыть транзакцию NTFS");

        _State = NtfsTransactionState.Active;
    }

    /// <summary>Текущее состояние транзакции</summary>
    public NtfsTransactionState State => _State;

    /// <summary>Фиксирует транзакцию</summary>
    /// <exception cref="ObjectDisposedException">Выбрасывается при использовании освобождённого объекта</exception>
    /// <exception cref="InvalidOperationException">Выбрасывается при неверном состоянии транзакции</exception>
    /// <exception cref="NtfsTransactionException">Выбрасывается при ошибке фиксации транзакции</exception>
    public void Commit()
    {
        EnsureCanUseTransaction();

        if (!NativeMethods.CommitTransaction(_Handle))
            ThrowLastWin32Error("Не удалось зафиксировать транзакцию NTFS");

        _State = NtfsTransactionState.Committed;
        CloseHandle();
    }

    /// <summary>Откатывает транзакцию</summary>
    /// <exception cref="ObjectDisposedException">Выбрасывается при использовании освобождённого объекта</exception>
    /// <exception cref="InvalidOperationException">Выбрасывается при неверном состоянии транзакции</exception>
    /// <exception cref="NtfsTransactionException">Выбрасывается при ошибке отката транзакции</exception>
    public void Rollback()
    {
        EnsureCanUseTransaction();

        if (!NativeMethods.RollbackTransaction(_Handle))
            ThrowLastWin32Error("Не удалось откатить транзакцию NTFS");

        _State = NtfsTransactionState.RolledBack;
        CloseHandle();
    }

    /// <summary>Создаёт каталог внутри транзакции</summary>
    /// <param name="Path">Путь создаваемого каталога</param>
    /// <exception cref="ArgumentException">Выбрасывается при пустом пути</exception>
    /// <exception cref="ObjectDisposedException">Выбрасывается при использовании освобождённого объекта</exception>
    /// <exception cref="InvalidOperationException">Выбрасывается при неверном состоянии транзакции</exception>
    /// <exception cref="NtfsTransactionException">Выбрасывается при ошибке создания каталога</exception>
    public void CreateDirectory(string Path)
    {
        EnsureCanUseTransaction();

        var full_path = NormalizePath(Path, nameof(Path));
        if (!NativeMethods.CreateDirectoryTransacted(null, full_path, IntPtr.Zero, _Handle))
            ThrowLastWin32Error($"Не удалось создать каталог '{full_path}' в транзакции");
    }

    /// <summary>Удаляет каталог внутри транзакции</summary>
    /// <param name="Path">Путь удаляемого каталога</param>
    /// <exception cref="ArgumentException">Выбрасывается при пустом пути</exception>
    /// <exception cref="ObjectDisposedException">Выбрасывается при использовании освобождённого объекта</exception>
    /// <exception cref="InvalidOperationException">Выбрасывается при неверном состоянии транзакции</exception>
    /// <exception cref="NtfsTransactionException">Выбрасывается при ошибке удаления каталога</exception>
    public void DeleteDirectory(string Path)
    {
        EnsureCanUseTransaction();

        var full_path = NormalizePath(Path, nameof(Path));
        if (!NativeMethods.RemoveDirectoryTransacted(full_path, _Handle))
            ThrowLastWin32Error($"Не удалось удалить каталог '{full_path}' в транзакции");
    }

    /// <summary>Копирует файл внутри транзакции</summary>
    /// <param name="SourcePath">Путь к исходному файлу</param>
    /// <param name="DestinationPath">Путь к файлу назначения</param>
    /// <param name="FailIfExists">Признак ошибки при существующем целевом файле</param>
    /// <exception cref="ArgumentException">Выбрасывается при пустом пути</exception>
    /// <exception cref="ObjectDisposedException">Выбрасывается при использовании освобождённого объекта</exception>
    /// <exception cref="InvalidOperationException">Выбрасывается при неверном состоянии транзакции</exception>
    /// <exception cref="NtfsTransactionException">Выбрасывается при ошибке копирования файла</exception>
    public void CopyFile(string SourcePath, string DestinationPath, bool FailIfExists = false)
    {
        EnsureCanUseTransaction();

        var source_path = NormalizePath(SourcePath, nameof(SourcePath));
        var destination_path = NormalizePath(DestinationPath, nameof(DestinationPath));
        var flags = FailIfExists ? NativeMethods.CopyFileFailIfExists : 0u;

        if (!NativeMethods.CopyFileTransacted(source_path, destination_path, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, flags, _Handle))
            ThrowLastWin32Error($"Не удалось скопировать файл '{source_path}' в '{destination_path}' в транзакции");
    }

    /// <summary>Перемещает файл внутри транзакции</summary>
    /// <param name="SourcePath">Путь к исходному файлу</param>
    /// <param name="DestinationPath">Путь к файлу назначения</param>
    /// <param name="ReplaceExisting">Признак замены существующего целевого файла</param>
    /// <exception cref="ArgumentException">Выбрасывается при пустом пути</exception>
    /// <exception cref="ObjectDisposedException">Выбрасывается при использовании освобождённого объекта</exception>
    /// <exception cref="InvalidOperationException">Выбрасывается при неверном состоянии транзакции</exception>
    /// <exception cref="NtfsTransactionException">Выбрасывается при ошибке перемещения файла</exception>
    public void MoveFile(string SourcePath, string DestinationPath, bool ReplaceExisting = false)
    {
        EnsureCanUseTransaction();

        var source_path = NormalizePath(SourcePath, nameof(SourcePath));
        var destination_path = NormalizePath(DestinationPath, nameof(DestinationPath));
        var flags = ReplaceExisting ? NativeMethods.MoveFileReplaceExisting : 0u;

        if (!NativeMethods.MoveFileTransacted(source_path, destination_path, IntPtr.Zero, IntPtr.Zero, flags, _Handle))
            ThrowLastWin32Error($"Не удалось переместить файл '{source_path}' в '{destination_path}' в транзакции");
    }

    /// <summary>Удаляет файл внутри транзакции</summary>
    /// <param name="Path">Путь удаляемого файла</param>
    /// <exception cref="ArgumentException">Выбрасывается при пустом пути</exception>
    /// <exception cref="ObjectDisposedException">Выбрасывается при использовании освобождённого объекта</exception>
    /// <exception cref="InvalidOperationException">Выбрасывается при неверном состоянии транзакции</exception>
    /// <exception cref="NtfsTransactionException">Выбрасывается при ошибке удаления файла</exception>
    public void DeleteFile(string Path)
    {
        EnsureCanUseTransaction();

        var full_path = NormalizePath(Path, nameof(Path));
        if (!NativeMethods.DeleteFileTransacted(full_path, _Handle))
            ThrowLastWin32Error($"Не удалось удалить файл '{full_path}' в транзакции");
    }

    /// <summary>Освобождает ресурсы транзакции</summary>
    public void Dispose()
    {
        if (_State is NtfsTransactionState.Disposed)
            return;

        if (_State is NtfsTransactionState.Active)
            Rollback();

        CloseHandle();
        _State = NtfsTransactionState.Disposed;
        GC.SuppressFinalize(this);
    }

    private static void EnsureWindows()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Транзакции NTFS поддерживаются только на Windows");
    }

    private void EnsureCanUseTransaction()
    {
        if (_State is NtfsTransactionState.Disposed)
            throw new ObjectDisposedException(nameof(NtfsTransaction));

        if (_State is not NtfsTransactionState.Active)
            throw new InvalidOperationException($"Транзакция находится в состоянии '{_State}'");
    }

    private static string NormalizePath(string Path, string ParamName)
    {
        if (string.IsNullOrWhiteSpace(Path))
            throw new ArgumentException("Ожидается непустой путь", ParamName);

        return System.IO.Path.GetFullPath(Path);
    }

    private void CloseHandle()
    {
        if (_Handle == IntPtr.Zero || _Handle == __InvalidHandle)
            return;

        _ = NativeMethods.CloseHandle(_Handle);
        _Handle = IntPtr.Zero;
    }

    private static void ThrowLastWin32Error(string Message)
    {
        var error = Marshal.GetLastWin32Error();
        throw new NtfsTransactionException(Message, new Win32Exception(error));
    }

    private static class NativeMethods
    {
        public const uint CopyFileFailIfExists = 0x00000001;
        public const uint MoveFileReplaceExisting = 0x00000001;

        [DllImport("ktmw32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateTransaction(
            IntPtr lpTransactionAttributes,
            IntPtr UOW,
            uint CreateOptions,
            uint IsolationLevel,
            uint IsolationFlags,
            uint Timeout,
            string? Description);

        [DllImport("ktmw32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CommitTransaction(IntPtr TransactionHandle);

        [DllImport("ktmw32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RollbackTransaction(IntPtr TransactionHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr Handle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateDirectoryTransacted(
            string? TemplateDirectory,
            string NewDirectory,
            IntPtr SecurityAttributes,
            IntPtr TransactionHandle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RemoveDirectoryTransacted(string DirectoryName, IntPtr TransactionHandle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteFileTransacted(string FileName, IntPtr TransactionHandle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CopyFileTransacted(
            string ExistingFileName,
            string NewFileName,
            IntPtr ProgressRoutine,
            IntPtr Data,
            IntPtr Cancel,
            uint CopyFlags,
            IntPtr TransactionHandle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MoveFileTransacted(
            string ExistingFileName,
            string NewFileName,
            IntPtr ProgressRoutine,
            IntPtr Data,
            uint Flags,
            IntPtr TransactionHandle);
    }
}
