#nullable enable
using System.Diagnostics;

// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
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

        public static Task<int> WaitForExitAsync(this Process process)
        {
            if (process.HasExited) return Task.FromResult(process.ExitCode);
            var result = new TaskCompletionSource<int>();
            process.EnableRaisingEvents = true;
            process.Exited += (s, e) => result.TrySetResult(process.ExitCode);
            if (process.HasExited)
                result.TrySetResult(process.ExitCode);
            return result.Task;
        }
    }
}