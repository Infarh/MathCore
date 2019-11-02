using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MathCore.Annotations;

namespace MathCore.Extensions
{
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

        public static Task<int> WaitAsync([NotNull] this Process process)
        {
            var tcs = new TaskCompletionSource<int>();
            process.EnableRaisingEvents = true;
            process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode);
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

        [DllImport("ntdll.dll", SetLastError = true, ExactSpelling = true)]
        private static extern uint NtQueryInformationProcess(
            IntPtr ProcessHandle,
            uint ProcessInformationClass,
            ref PROCESS_BASIC_INFORMATION ProcessInformation,
            int ProcessInformationLength,
            out int ReturnLength
        );

        public static Process GetMotherProcess(this Process process)
        {
            var info = new PROCESS_BASIC_INFORMATION();
            if (NtQueryInformationProcess(process.Handle, 0, ref info, Marshal.SizeOf(info), out var writed) != 0 ||
                writed == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return Process.GetProcessById(info.InheritedFromUniqueProcessId.ToInt32());
        }
    }
}