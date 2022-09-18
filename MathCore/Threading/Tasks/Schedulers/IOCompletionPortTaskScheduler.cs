#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Win32.SafeHandles;
// ReSharper disable UnusedType.Global

namespace MathCore.Threading.Tasks.Schedulers;

/// <summary>Provides a TaskScheduler that uses an I/O completion port for concurrency control.</summary>
// ReSharper disable once InconsistentNaming
public sealed class IOCompletionPortTaskScheduler : TaskScheduler, IDisposable
{
    /// <summary>The queue of tasks to be scheduled.</summary>
    private readonly ConcurrentQueue<Task> _Tasks;
    /// <summary>The I/O completion port to use for concurrency control.</summary>
    private readonly IOCompletionPort _IOcp;
    /// <summary>Whether the current thread is a scheduler thread.</summary>
    private readonly ThreadLocal<bool> _SchedulerThread;
    /// <summary>Event used to wait for all threads to shutdown.</summary>
    private readonly CountdownEvent _RemainingThreadsToShutdown;

    /// <summary>Initializes the IOCompletionPortTaskScheduler.</summary>
    /// <param name="MaxConcurrencyLevel">The maximum number of threads in the scheduler to be executing concurrently.</param>
    /// <param name="NumAvailableThreads">The number of threads to have available in the scheduler for executing tasks.</param>
    public IOCompletionPortTaskScheduler(int MaxConcurrencyLevel, int NumAvailableThreads)
    {
        // Validate arguments
        if (MaxConcurrencyLevel < 1) throw new ArgumentNullException(nameof(MaxConcurrencyLevel));
        if (NumAvailableThreads < 1) throw new ArgumentNullException(nameof(NumAvailableThreads));

        _Tasks                      = new ConcurrentQueue<Task>();
        _IOcp                       = new IOCompletionPort(MaxConcurrencyLevel);
        _SchedulerThread            = new ThreadLocal<bool>();
        _RemainingThreadsToShutdown = new CountdownEvent(NumAvailableThreads);

        // Create and start the threads
        for (var i = 0; i < NumAvailableThreads; i++)
            new Thread(() =>
                {
                    try
                    {
                        // Note that this is a scheduler thread.  Used for inlining checks.
                        _SchedulerThread.Value = true;

                        // Continually wait on the I/O completion port until 
                        // there's a work item, then process it.
                        while (_IOcp.WaitOne())
                            if (_Tasks.TryDequeue(out var next)) 
                                TryExecuteTask(next);
                    }
                    finally { _RemainingThreadsToShutdown.Signal(); }
                })
                { IsBackground = true }.Start();
    }

    /// <summary>Dispose of the scheduler.</summary>
    public void Dispose()
    {
        // Close the I/O completion port.  This will cause any threads blocked
        // waiting for items to wake up.
        _IOcp.Dispose();

        // Wait for all threads to shutdown.  This could cause deadlock
        // if the current thread is calling Dispose or is part of such a cycle.
        _RemainingThreadsToShutdown.Wait();
        _RemainingThreadsToShutdown.Dispose();

        // Clean up remaining state
        _SchedulerThread.Dispose();
    }

    /// <summary>Gets a list of all tasks scheduled to this scheduler.</summary>
    /// <returns>An enumerable of all scheduled tasks.</returns>
    protected override IEnumerable<Task> GetScheduledTasks() => _Tasks.ToArray();

    /// <summary>Queues a task to this scheduler for execution.</summary>
    /// <param name="task">The task to be executed.</param>
    protected override void QueueTask(Task task)
    {
        // Store the task and let the I/O completion port know that more work has arrived.
        _Tasks.Enqueue(task);
        _IOcp.NotifyOne();
    }

    /// <summary>Try to execute a task on the current thread.</summary>
    /// <param name="task">The task to execute.</param>
    /// <param name="TaskWasPreviouslyQueued">Whether the task was previously queued to this scheduler.</param>
    /// <returns>Whether the task was executed.</returns>
    protected override bool TryExecuteTaskInline(Task task, bool TaskWasPreviouslyQueued) =>
        // Only inline from scheduler threads.  This is to ensure concurrency control 
        // is able to handle inlining as well.
        _SchedulerThread.Value && TryExecuteTask(task);

    /// <summary>Provides a simple managed wrapper for an I/O completion port.</summary>
    // ReSharper disable once InconsistentNaming
    private sealed class IOCompletionPort : IDisposable
    {
        /// <summary>Infinite timeout value to use for GetQueuedCompletedStatus.</summary>
        private const uint __InfiniteTimeout = unchecked((uint) Timeout.Infinite);

        /// <summary>An invalid file handle value.</summary>
        private readonly IntPtr _InvalidFileHandle = unchecked((IntPtr)(-1));
        /// <summary>An invalid I/O completion port handle value.</summary>
        private readonly IntPtr _InvalidIocpHandle = IntPtr.Zero;

        /// <summary>The I/O completion porth handle.</summary>
        private readonly SafeFileHandle _Handle;

        /// <summary>Initializes the I/O completion port.</summary>
        /// <param name="MaxConcurrencyLevel">The maximum concurrency level allowed by the I/O completion port.</param>
        public IOCompletionPort(int MaxConcurrencyLevel)
        {
            // Validate the argument and create the port.
            if (MaxConcurrencyLevel < 1) throw new ArgumentOutOfRangeException(nameof(MaxConcurrencyLevel));
            _Handle = CreateIoCompletionPort(_InvalidFileHandle, _InvalidIocpHandle, UIntPtr.Zero, (uint)MaxConcurrencyLevel);
        }

        /// <summary>Clean up.</summary>
        public void Dispose() => _Handle.Dispose();

        /// <summary>Notify that I/O completion port that new work is available.</summary>
        public void NotifyOne()
        {
            if (PostQueuedCompletionStatus(_Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero)) return;
            throw new Win32Exception();
        }

        /// <summary>Waits for an item on the I/O completion port.</summary>
        /// <returns>true if an item was available; false if the completion port closed before an item could be retrieved.</returns>
        public bool WaitOne()
        {
            // Wait for an item to be posted.
            // DangerousGetHandle is used so that the safe handle can be closed even while blocked in the call to GetQueuedCompletionStatus.
            if (GetQueuedCompletionStatus(_Handle.DangerousGetHandle(), out _, out _, out _, __InfiniteTimeout)) return true;
            var error_code = Marshal.GetLastWin32Error();
            if (error_code is 735 or 6 /*ERROR_ABANDONED_WAIT_0*/ /*INVALID_HANDLE*/)
                return false;
            throw new Win32Exception(error_code);
        }

        /// <summary>
        /// Creates an input/output (I/O) completion port and associates it with a specified file handle, 
        /// or creates an I/O completion port that is not yet associated with a file handle, allowing association at a later time.
        /// </summary>
        /// <param name="FileHandle">An open file handle or INVALID_HANDLE_VALUE.</param>
        /// <param name="ExistingCompletionPort">A handle to an existing I/O completion port or NULL.</param>
        /// <param name="CompletionKey">The per-handle user-defined completion key that is included in every I/O completion packet for the specified file handle.</param>
        /// <param name="NumberOfConcurrentThreads">The maximum number of threads that the operating system can allow to concurrently process I/O completion packets for the I/O completion port.</param>
        /// <returns>If the function succeeds, the return value is the handle to an I/O completion port.  If the function fails, the return value is NULL.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern SafeFileHandle CreateIoCompletionPort(IntPtr FileHandle, IntPtr ExistingCompletionPort, UIntPtr CompletionKey, uint NumberOfConcurrentThreads);

        /// <summary>Attempts to dequeue an I/O completion packet from the specified I/O completion port.</summary>
        /// <param name="CompletionPort">A handle to the completion port.</param>
        /// <param name="LpNumberOfBytes">A pointer to a variable that receives the number of bytes transferred during an I/O operation that has completed.</param>
        /// <param name="LpCompletionKey">A pointer to a variable that receives the completion key value associated with the file handle whose I/O operation has completed.</param>
        /// <param name="LpOverlapped">A pointer to a variable that receives the address of the OVERLAPPED structure that was specified when the completed I/O operation was started.</param>
        /// <param name="DwMilliseconds">The number of milliseconds that the caller is willing to wait for a completion packet to appear at the completion port. </param>
        /// <returns>Returns nonzero (TRUE) if successful or zero (FALSE) otherwise.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetQueuedCompletionStatus(IntPtr CompletionPort, out uint LpNumberOfBytes, out IntPtr LpCompletionKey, out IntPtr LpOverlapped, uint DwMilliseconds);

        /// <summary>Posts an I/O completion packet to an I/O completion port.</summary>
        /// <param name="CompletionPort">A handle to the completion port.</param>
        /// <param name="DwNumberOfBytesTransferred">The value to be returned through the lpNumberOfBytesTransferred parameter of the GetQueuedCompletionStatus function.</param>
        /// <param name="DwCompletionKey">The value to be returned through the lpCompletionKey parameter of the GetQueuedCompletionStatus function.</param>
        /// <param name="LpOverlapped">The value to be returned through the lpOverlapped parameter of the GetQueuedCompletionStatus function.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool PostQueuedCompletionStatus(SafeFileHandle CompletionPort, IntPtr DwNumberOfBytesTransferred, IntPtr DwCompletionKey, IntPtr LpOverlapped);
    }
}