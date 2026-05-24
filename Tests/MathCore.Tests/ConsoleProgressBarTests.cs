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

    [TestMethod]
    public void UpdateText_DoesNotThrowAndUpdatesState_WhenRenderingDisabled()
    {
        using var progress_bar = new ConsoleProgressBar();

        SetPrivateField(progress_bar, "_CanRender", false);

        var update_text = typeof(ConsoleProgressBar)
           .GetMethod("UpdateText", BindingFlags.Instance | BindingFlags.NonPublic);

        if (update_text is null)
            Assert.Fail("UpdateText method not found");

        update_text.Invoke(progress_bar, ["state-only-update"]);

        var current_text = GetPrivateField<string>(progress_bar, "_CurrentText");
        current_text.AssertEquals("state-only-update");
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

    private static T GetPrivateField<T>(ConsoleProgressBar ProgressBar, string FieldName)
    {
        var field = typeof(ConsoleProgressBar)
           .GetField(FieldName, BindingFlags.Instance | BindingFlags.NonPublic);

        if (field is null)
            Assert.Fail($"Field '{FieldName}' not found");

        var value = field.GetValue(ProgressBar);

        return value is T typed_value
            ? typed_value
            : throw new AssertFailedException($"Field '{FieldName}' has unexpected type");
    }
}