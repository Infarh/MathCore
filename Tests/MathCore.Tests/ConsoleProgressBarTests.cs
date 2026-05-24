using System.Reflection;

namespace MathCore.Tests;

[TestClass]
public class ConsoleProgressBarTests
{
    [TestMethod]
    public void TimerHandler_DoesNotThrow_ForEmptyGradientSet()
    {
        using var progress_bar = new ConsoleProgressBar(10)
        {
            GradientBlockSet = string.Empty
        };

        progress_bar.Report(0.5);
        SetPrivateField(progress_bar, "_CanRender", true);

        InvokeTimerHandler(progress_bar);
    }

    [TestMethod]
    public void TimerHandler_DoesNotThrow_ForNullGradientSet()
    {
        using var progress_bar = new ConsoleProgressBar(10)
        {
            GradientBlockSet = null!
        };

        progress_bar.Report(0.5);
        SetPrivateField(progress_bar, "_CanRender", true);

        InvokeTimerHandler(progress_bar);
    }

    [TestMethod]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var progress_bar = new ConsoleProgressBar();

        progress_bar.Dispose();
        progress_bar.Dispose();
    }

    private static void InvokeTimerHandler(ConsoleProgressBar ProgressBar)
    {
        var timer_handler = typeof(ConsoleProgressBar)
           .GetMethod("TimerHandler", BindingFlags.Instance | BindingFlags.NonPublic);

        if (timer_handler is null)
            Assert.Fail("TimerHandler method not found");

        timer_handler.Invoke(ProgressBar, [null]);
    }

    private static void SetPrivateField(ConsoleProgressBar ProgressBar, string FieldName, object Value)
    {
        var field = typeof(ConsoleProgressBar)
           .GetField(FieldName, BindingFlags.Instance | BindingFlags.NonPublic);

        if (field is null)
            Assert.Fail($"Field '{FieldName}' not found");

        field.SetValue(ProgressBar, Value);
    }
}