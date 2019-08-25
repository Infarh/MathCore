using System.Diagnostics;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using MathCore.Annotations;

// ReSharper disable UnusedMember.Global

namespace System.Threading.Tasks
{
    public static class ProcessExtentions
    {
        //public static TaskAwaiter<int> GetAwaiter([NotNull] this Process process)
        //{
        //    Contract.Requires(process != null);

        //    var tcs = new TaskCompletionSource<int>();
        //    process.EnableRaisingEvents = true;
        //    process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode);
        //    if(process.HasExited)
        //        tcs.TrySetResult(process.ExitCode);
        //    return tcs.Task.GetAwaiter();
        //}
        public static Task<int> WaitForExitAsync([NotNull] this Process process)
        {
            if (process.HasExited) return Task.FromResult(process.ExitCode);
            var tcs = new TaskCompletionSource<int>();
            process.EnableRaisingEvents = true;
            process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode);
            if (process.HasExited)
                tcs.TrySetResult(process.ExitCode);
            return tcs.Task;
        }
    }
}
