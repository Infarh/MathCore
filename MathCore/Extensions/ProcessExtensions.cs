using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using MathCore.Annotations;

namespace MathCore.Extensions;

/// <summary>Методы-расширения для процессов</summary>
public static class ProcessExtensions
{
    //public static TaskAwaiter<int> GetAwaiter(this Process process)
    //{
    //    var tcs = new TaskCompletionSource<int>();
    //    process.EnableRaisingEvents = true;
    //    process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode);
    //    if(process.HasExited) tcs.TrySetResult(process.ExitCode);
    //    return tcs.Task.GetAwaiter();
    //}

    /// <summary>Ожидание завершения процесса</summary>
    /// <param name="process">Процесс, завершение которого требуется дождаться</param>
    /// <returns>Задача, результат которой является кодом завершения процесса</returns>
    public static Task<int> WaitAsync([Annotations.NotNull] this Process process)
    {
        var tcs = new TaskCompletionSource<int>();
        process.EnableRaisingEvents =  true;
        process.Exited              += (_, _) => tcs.TrySetResult(process.ExitCode);
        if (process.HasExited) tcs.TrySetResult(process.ExitCode);
        return tcs.Task;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESS_BASIC_INFORMATION
    {
        public uint ExitStatus;
        public IntPtr PebBaseAddress; // Zero if 32 bit process try get info about 64 bit process 
        public IntPtr AffinityMask;
        public int BasePriority;
        public IntPtr UniqueProcessId;
        public IntPtr InheritedFromUniqueProcessId;
    }

    // ReSharper disable once StringLiteralTypo
    [DllImport("ntdll.dll", SetLastError = true, ExactSpelling = true)]
    private static extern uint NtQueryInformationProcess(
        IntPtr ProcessHandle,
        uint ProcessInformationClass,
        ref PROCESS_BASIC_INFORMATION ProcessInformation,
        int ProcessInformationLength,
        out int ReturnLength
    );

    /// <summary>Получить родительский процесс</summary>
    /// <param name="process">Дочерний процесс</param>
    /// <returns>Родительский процесс</returns>
    [Annotations.NotNull]
    public static Process GetMotherProcess([Annotations.NotNull] this Process process)
    {
        var info = new PROCESS_BASIC_INFORMATION();
        if (NtQueryInformationProcess(process.Handle, 0, ref info, Marshal.SizeOf(info), out var written) != 0 
            || written == 0)
            throw new Win32Exception(Marshal.GetLastWin32Error());
        return Process.GetProcessById(info.InheritedFromUniqueProcessId.ToInt32());
    }
}