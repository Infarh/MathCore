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

        public static async Task<Process> WaitAsync(this Process process, CancellationToken Cancel = default)
        {
            if (process.HasExited) return process;
            Cancel.ThrowIfCancellationRequested();

            var result = new TaskCompletionSource<Process>(TaskCreationOptions.RunContinuationsAsynchronously);

            using var registraction_cancellation = Cancel.IsCancellationRequested
                ? Cancel.Register(o => ((TaskCompletionSource<Process>)o).TrySetCanceled(), result)
                : (IDisposable?)null;

            process.EnableRaisingEvents = true;
            void Handler(object _, EventArgs __) => result.TrySetResult(process);
            process.Exited += Handler;

            try
            {
                return await result.Task.ConfigureAwait(false);
            }
            finally
            {
                process.Exited -= Handler;
            }
        }

        public static Task<Process> StartAsync(this Process process)
        {
            if (process.HasExited) return Task.FromResult(process);
            if (!process.Start()) return Task.FromResult(process);

            var result = new TaskCompletionSource<Process>();
            process.EnableRaisingEvents = true;
            process.Exited += (_, _) => result.TrySetResult(process);


            if (process.HasExited)
                result.TrySetResult(process);
            
            return result.Task;
        }
    }
}