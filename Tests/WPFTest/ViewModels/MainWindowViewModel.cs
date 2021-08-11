using System.Windows.Input;
using MathCore.Mediator;
using MathCore.ViewModels;
using WPFTest.Infrastructure.Commands;
using WPFTest.Models;

namespace WPFTest.ViewModels
{
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

        public MainWindowViewModel(IMessenger Messenger)
        {
            _Messenger = Messenger;
        }
    }
}
