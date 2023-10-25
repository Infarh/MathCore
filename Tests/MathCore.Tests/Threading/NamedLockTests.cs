using System.Collections.Concurrent;

using MathCore.Threading;

namespace MathCore.Tests.Threading;

[TestClass]
public class NamedLockTests
{
    [TestMethod, Timeout(10000)]
    public async Task MultipleAccessTest()
    {
        const string resource_name = "test";

        var resource_lock = new NamedLock();

        var result_list = new ConcurrentList<string>();

        var start_action = new ManualResetEvent(false);

        var access_task1 = Task.Run(async () =>
        {
            start_action.WaitOne();
            await resource_lock.LockAsync(resource_name);

            await Task.Delay(200).ConfigureAwait(false);
            result_list.Add("R-1.0");

            await Task.Delay(200).ConfigureAwait(false);
            result_list.Add("R-1.1");

            await Task.Delay(200).ConfigureAwait(false);
            result_list.Add("R-1.2");

            await resource_lock.UnlockAsync(resource_name);
        });

        var access_task2 = Task.Run(async () =>
        {
            start_action.WaitOne();

            await Task.Delay(100).ConfigureAwait(false);

            await resource_lock.LockAsync(resource_name);

            await Task.Delay(50).ConfigureAwait(false);
            result_list.Add("R-2.0");

            await Task.Delay(50).ConfigureAwait(false);
            result_list.Add("R-2.1");

            await Task.Delay(50).ConfigureAwait(false);
            result_list.Add("R-2.2");

            await resource_lock.UnlockAsync(resource_name);
        });

        var access_task3 = Task.Run(async () =>
        {
            start_action.WaitOne();

            await Task.Delay(50).ConfigureAwait(false);

            await resource_lock.LockAsync(resource_name);

            await Task.Delay(75).ConfigureAwait(false);
            result_list.Add("R-3.0");

            await Task.Delay(75).ConfigureAwait(false);
            result_list.Add("R-3.1");

            await Task.Delay(75).ConfigureAwait(false);
            result_list.Add("R-3.2");

            await resource_lock.UnlockAsync(resource_name);
        });

        start_action.Set();

        await Task.WhenAll(access_task1, access_task2, access_task3).ConfigureAwait(false);

        var result = result_list.JoinStrings("; ");

        result.AssertEquals("R-1.0; R-1.1; R-1.2; R-3.0; R-3.1; R-3.2; R-2.0; R-2.1; R-2.2");
    }
}
