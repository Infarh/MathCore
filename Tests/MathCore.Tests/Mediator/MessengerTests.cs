using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathCore.Mediator;

namespace MathCore.Tests.Mediator;

[TestClass]
public class MessengerTests
{
    [TestMethod]
    public void AddHandle_Send_CallsHandler()
    {
        var messenger = new Messenger();
        var called = false;

        void Handler(object s, string m) { called = true; Assert.AreSame(messenger, s); Assert.AreEqual("test", m); }

        messenger.AddHandle<string>(Handler);
        messenger.Send(messenger, "test");

        Assert.IsTrue(called);
    }

    [TestMethod]
    public void RemoveHandler_RemovesHandler()
    {
        var messenger = new Messenger();
        var count = 0;

        void H1(object s, int v) => count += 1;
        void H2(object s, int v) => count += 10;

        messenger.AddHandle<int>(H1!);
        messenger.AddHandle<int>(H2!);

        var removed = messenger.RemoveHandler<int>(H1!);

        messenger.Send(messenger, 1);

        Assert.IsTrue(removed);
        Assert.AreEqual(10, count);
    }

    [TestMethod]
    public async Task SendAsync_CallsHandler()
    {
        var messenger = new Messenger();
        var tcs = new TaskCompletionSource<bool>();

        void Handler(object s, string m) => tcs.TrySetResult(true);

        messenger.AddHandle<string>(Handler!);

        await messenger.SendAsync(messenger, "ok");

        var completed = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(2))) == tcs.Task;
        Assert.IsTrue(completed, "Handler was not invoked within timeout");
    }
}
