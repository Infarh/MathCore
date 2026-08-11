using MathCore.CommandProcessor;

using CommandHandler = System.Action<MathCore.CommandProcessor.ProcessorCommand, int, System.Collections.Generic.IReadOnlyList<MathCore.CommandProcessor.ProcessorCommand>>;

namespace MathCore.Tests.CommandProcessor;

[TestClass]
public class CommandLineProcessorTests
{
    [TestMethod]
    public void Test_RegisterAndGetCommand()
    {
        var processor = new CommandLineProcessor();
        var invoked = false;
        processor["testcmd"] += () => invoked = true;
        var cmd = processor["testcmd"];
        Assert.IsNotNull(cmd);
        Assert.AreEqual(1, cmd.Count);
    }

    [TestMethod]
    public void Test_CommandEventInvoked()
    {
        var processor = new CommandLineProcessor();
        var eventInvoked = false;
        processor.CommandProcess += (_, e) => eventInvoked = (e.Command.Name == "mycommand");
        processor.Process("mycommand");
        Assert.IsTrue(eventInvoked);
    }

    [TestMethod]
    public void Test_CommandWithArguments()
    {
        var processor = new CommandLineProcessor();
        string? capturedValue = null;
        processor["argcmd"] += new CommandHandler((c, _, _) =>
        {
            var arg = c.Argument.FirstOrDefault(a => a.Name == "val");
            if (arg.Name == "val") capturedValue = arg.Value;
        });
        // Формат: команда val=testvalue
        processor.Process("argcmd val=testvalue");
        Assert.AreEqual("testvalue", capturedValue);
    }

    [TestMethod]
    public void Test_UnhandledCommandEvent()
    {
        var processor = new CommandLineProcessor();
        string? unhandledName = null;
        processor.UnhandledCommand += (_, e) => unhandledName = e.Command.Name;
        processor.Process("nonexistent");
        Assert.AreEqual("nonexistent", unhandledName);
    }

    [TestMethod]
    public void Test_SetHandled_StopsUnhandledCommand()
    {
        var processor = new CommandLineProcessor();
        var unhandledCalled = false;
        processor.UnhandledCommand += (_, _) => unhandledCalled = true;
        processor["handledcmd"] += () => { };
        processor.CommandProcess += (_, e) =>
        {
            if (e.Command.Name == "handledcmd") e.Handled = true;
        };
        processor.Process("handledcmd");
        Assert.IsFalse(unhandledCalled);
    }

    [TestMethod]
    public void Test_MultipleCommandsInSequence()
    {
        var processor = new CommandLineProcessor();
        var callOrder = new List<string>();
        processor["cmd1"] += () => callOrder.Add("cmd1");
        processor["cmd2"] += () => callOrder.Add("cmd2");
        processor["cmd3"] += () => callOrder.Add("cmd3");
        processor.Process("cmd1");
        processor.Process("cmd2");
        processor.Process("cmd3");
        Assert.AreEqual(3, callOrder.Count);
        Assert.IsTrue(callOrder[0] == "cmd1");
        Assert.IsTrue(callOrder[1] == "cmd2");
        Assert.IsTrue(callOrder[2] == "cmd3");
    }

    [TestMethod]
    public void Test_CommandWithParameter()
    {
        var processor = new CommandLineProcessor();
        string? param = null;
        processor["paramcmd"] += c => param = c.Parameter;
        processor.Process("paramcmd:myparam");
        Assert.AreEqual("myparam", param);
    }

    [TestMethod]
    public void Test_ProcessMultipleCommandsInLine()
    {
        var processor = new CommandLineProcessor();
        var callOrder = new List<string>();
        processor["a"] += () => callOrder.Add("a");
        processor["b"] += () => callOrder.Add("b");
        foreach (var cmd in processor.Process("a;b"))
            Assert.IsNotNull(cmd);
        Assert.IsTrue(callOrder.Contains("a"), "Команда 'a' должна быть вызвана");
        Assert.IsTrue(callOrder.Contains("b"), "Команда 'b' должна быть вызвана");
    }
}
