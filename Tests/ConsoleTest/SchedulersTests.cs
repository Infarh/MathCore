using MathCore.Threading.Tasks.Schedulers;

namespace ConsoleTest
{
    internal static class SchedulersTests
    {
        public static async void Start()
        {
            var single_thread_scheduler = new QueuedTaskScheduler();

            await Task.Yield().ConfigureAwait(single_thread_scheduler);
        }
    }
}
