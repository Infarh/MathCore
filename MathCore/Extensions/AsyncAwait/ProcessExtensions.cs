#nullable enable
using System.Diagnostics;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

// ReSharper disable once UnusedType.Global
public static class ProcessExtensions
{
    //public static TaskAwaiter<int> GetAwaiter(this Process process)
    //{
    //    var result = new TaskCompletionSource<int>();
    //    process.EnableRaisingEvents = true;
    //    process.Exited += (s, e) => result.TrySetResult(process.ExitCode);
    //    if(process.HasExited)
    //        result.TrySetResult(process.ExitCode);
    //    return result.Task.GetAwaiter();
    //}

    public static async Task<Process> WaitAsync(this Process process, CancellationToken Cancel = default, bool KillIfCancel = false, int KillTimeout = 1000)
    {
        if (process is null) throw new ArgumentNullException(nameof(process));

        process.Refresh();
        try
        {
            process = Process.GetProcessById(process.Id);
        }
        catch (InvalidOperationException)
        {
            return process;
        }
        catch (ArgumentException)
        {
            return process;
        }

        if (process.HasExited) return process;
        Cancel.ThrowIfCancellationRequested();

        var result = new TaskCompletionSource<Process>(TaskCreationOptions.RunContinuationsAsynchronously);

        using var registration_cancellation = Cancel.IsCancellationRequested
            ? Cancel.Register(o => ((TaskCompletionSource<Process>)o).TrySetCanceled(), result)
            : (IDisposable?)null;

        process.EnableRaisingEvents = true;
        void Handler(object? _, EventArgs __) => result.TrySetResult(process);
        process.Exited += Handler;

        try
        {
            return await result.Task.ConfigureAwait(false);
        }
        catch (OperationCanceledException e) when (KillIfCancel && e.CancellationToken == Cancel)
        {
            process.CloseMainWindow();
            if (KillTimeout > 0)
                await Task.Delay(KillTimeout, CancellationToken.None).ConfigureAwait(false);
            if (KillTimeout >= 0)
                process.Kill();
            throw;
        }
        finally
        {
            process.Exited -= Handler;
        }
    }

    public static async Task<Process> StartAsync(this Process process, CancellationToken Cancel = default, bool KillIfCancel = false, int KillTimeout = 1000)
    {
        if (process is null) throw new ArgumentNullException(nameof(process));

        process.Refresh();
        if (process.HasExited) return process;
        try
        {
            process = Process.GetProcessById(process.Id);
            return process;
        }
        catch (InvalidOperationException)
        {
            // данного процесса ещё не существует
        }
        catch (ArgumentException)
        {
            // данного процесса ещё не существует
        }

        if (!process.Start()) return process;

        var result = new TaskCompletionSource<Process>();

        using var cancel_cts_registration = Cancel.CanBeCanceled
            ? Cancel.Register(o => ((TaskCompletionSource<Process>)o).TrySetCanceled(), result)
            : (IDisposable?)null;

        process.EnableRaisingEvents = true;
        void Handler(object? _, EventArgs __) => result.TrySetResult(process);
        process.Exited += Handler;

        try
        {
            return await result.Task.ConfigureAwait(false);
        }
        catch (OperationCanceledException e) when (KillIfCancel && e.CancellationToken == Cancel)
        {
            process.CloseMainWindow();
            if (KillTimeout > 0)
                await Task.Delay(KillTimeout, CancellationToken.None).ConfigureAwait(false);
            if (KillTimeout >= 0)
                process.Kill();
            throw;
        }
        finally
        {
            process.Exited -= Handler;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESSENTRY32
    {
        public uint dwSize;
        public uint cntUsage;
        public uint th32ProcessID;
        public IntPtr th32DefaultHeapID;
        public uint th32ModuleID;
        public uint cntThreads;
        public uint th32ParentProcessID;
        public int pcPriClassBase;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szExeFile;
    };

    private const uint TH32CS_SNAPPROCESS = 2;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

    [DllImport("kernel32.dll")]
    private static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

    [DllImport("kernel32.dll")]
    private static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

    [Copyright("CIsSharp", url = "https://spradip.wordpress.com/2008/10/24/getting-parent-process-id-from-child-without-passing-any-arguments-2/")]
    public static Process? GetParentProcess(this Process process)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Поддерживается только на платформе Windows на WinAPI");

        var process_pid = process.Id;

        var handle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

        if (handle == IntPtr.Zero)
            return null;

        var proc_info = new PROCESSENTRY32 { dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32)) };

        if (!Process32First(handle, ref proc_info))
            return null;

        var parent_pid = 0;
        do
        {
            if (process_pid == proc_info.th32ProcessID)
                parent_pid = (int)proc_info.th32ParentProcessID;
        }
        while (parent_pid == 0 && Process32Next(handle, ref proc_info));

        return parent_pid > 0 ? Process.GetProcessById(parent_pid) : null;
    }

    public static IEnumerable<Process> GetChildProcesses(this Process process)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Поддерживается только на платформе Windows на WinAPI");

        var pid = process.Id;

        var handle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

        if (handle == IntPtr.Zero)
            yield break;

        var proc_info = new PROCESSENTRY32 { dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32)) };

        if (!Process32First(handle, ref proc_info))
            yield break;

        do
        {
            var child_parent_pid = (int)proc_info.th32ParentProcessID;
            if (child_parent_pid == pid)
                yield return Process.GetProcessById((int)proc_info.th32ProcessID);
        }
        while (Process32Next(handle, ref proc_info));
    }
}

