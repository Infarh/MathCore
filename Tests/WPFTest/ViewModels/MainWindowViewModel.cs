using System.Windows;
using System.Windows.Input;

using MathCore.CommandProcessor;
using MathCore.Mediator;
using MathCore.Threading.Tasks.Schedulers;
using MathCore.ViewModels;
using WPFTest.Infrastructure.Commands;
using WPFTest.Infrastructure.Commands.Base;
using WPFTest.Models;

namespace WPFTest.ViewModels;

public class MainWindowViewModel : ViewModel
{
    private readonly IMessenger _Messenger;

    #region Title : string - Заголовок окна

    /// <summary>Заголовок окна</summary>
    private string _Title = "Главное окно программы";

    /// <summary>Заголовок окна</summary>
    public string Title { get => _Title; set => Set(ref _Title, value); }

    #endregion

    #region Command SendMessageCommand : string - Summary

    /// <summary>Summary</summary>
    private ICommand _SendMessageCommand;

    /// <summary>Summary</summary>
    public ICommand SendMessageCommand => _SendMessageCommand
        ??= new LambdaCommand(OnSendMessageCommandExecuted);

    /// <summary>Проверка возможности выполнения - Summary</summary>
    private void OnSendMessageCommandExecuted(object p) => _Messenger.Send(this, "To second model",
        new ServiceMessage(p as string ?? p?.ToString() ?? "Message from Main model"));

    #endregion

    #region Command TestTaskCommand - 

    private LambdaCommand? _TestTaskCommand;

    public ICommand TestTaskCommand => _TestTaskCommand ??= new(OnTestTaskCommandExecuted);
    //.NewBackground(OnTestTaskCommandExecuted, CanTestTaskCommandExecute);

    private static async void OnTestTaskCommandExecuted()
    {
        TaskScheduler gui_scheduler = new CurrentThreadTaskScheduler();
        var           gui_context   = SynchronizationContext.Current;

        var thread_id1 = Environment.CurrentManagedThreadId;
        await Task.Yield().ConfigureAwait(false);

        var thread_id2 = Environment.CurrentManagedThreadId;

        var thread_id3 = 0;
        var thread_id4 = 0;
        var wait_task = Task.Run(
            async () =>
            {
                thread_id3 = Environment.CurrentManagedThreadId;
                await Task.Delay(100).ConfigureAwait(false);
                thread_id4 = Environment.CurrentManagedThreadId;

                return 42;
            });

        var thread_id5 = 0;
        var update_value_task_async = wait_task.OnSuccess(
            x =>
            {
                thread_id5 = Environment.CurrentManagedThreadId;

            });

        var thread_id6 = 0;
        var update_value_task_gui1 = wait_task.OnSuccess(
            x =>
            {
                thread_id6 = Environment.CurrentManagedThreadId;
            }, gui_scheduler);

        var thread_id7 = 0;
        var update_value_task_gui2 = wait_task.OnSuccess(
            x =>
            {
                thread_id7 = Environment.CurrentManagedThreadId;
            }, gui_context);

        await Task.WhenAll(wait_task, update_value_task_async, update_value_task_gui1, update_value_task_gui2).ConfigureAwait(false);
        var thread_id_end = Environment.CurrentManagedThreadId;
    }

    #endregion

    public MainWindowViewModel(IMessenger Messenger)
    {
        _Messenger = Messenger;
    }
}