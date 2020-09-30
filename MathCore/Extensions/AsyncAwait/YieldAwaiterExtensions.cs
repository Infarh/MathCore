using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    // ReSharper disable once UnusedType.Global
    public static class YieldAwaiterExtensions
    {
        public static YieldAwaitableThreadPool ConfigureAwait(this YieldAwaitable _, bool LockContext) => new YieldAwaitableThreadPool(LockContext);

        public static TaskSchedulerAwaitable ConfigureAwait(this YieldAwaitable _, TaskScheduler Scheduler) => new TaskSchedulerAwaitable(Scheduler);
    }
}