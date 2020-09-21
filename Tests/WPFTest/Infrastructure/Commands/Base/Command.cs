using System;
using System.Windows.Input;

namespace WPFTest.Infrastructure.Commands.Base
{
    public abstract class Command : ICommand
    {
        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested += value;
        }

        bool ICommand.CanExecute(object parameter) => _Executable && CanExecute(parameter);

        void ICommand.Execute(object parameter)
        {
            if(CanExecute(parameter))
                Execute(parameter);
        }

        private bool _Executable = true;

        public bool Executable
        {
            get => _Executable;
            set
            {
                if (_Executable == value) return;
                _Executable = value;
                ExecutableChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler ExecutableChanged;

        protected virtual bool CanExecute(object parameter) =>  true;

        protected abstract void Execute(object parameter);


    }
}
