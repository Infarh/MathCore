#nullable enable
using System.Collections.Generic;
using System.Diagnostics;

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
        void Handler(object _, EventArgs __) => result.TrySetResult(process);
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
}