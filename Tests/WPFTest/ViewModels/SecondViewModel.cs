#nullable enable
using System.Windows;
using MathCore.Mediator;
using MathCore.ViewModels;
using WPFTest.Models;

namespace WPFTest.ViewModels
{
    public class SecondViewModel : ViewModel
    {
        private readonly IMessenger _Messenger;

        public SecondViewModel(IMessenger Messenger)
        {
            _Messenger = Messenger;
            _Messenger.AddHandle<ServiceMessage>("To second model", OnMessageToSecondModel);
        }

        private void OnMessageToSecondModel(object? Sender, ServiceMessage E) => MessageBox.Show(E.Message);
    }
}
