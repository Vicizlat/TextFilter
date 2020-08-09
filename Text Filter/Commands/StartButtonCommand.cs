using TextFilter.ViewModels;
using System;
using System.Windows.Input;

namespace TextFilter.Commands
{
    internal class StartButtonCommand : ICommand
    {
        private MainViewModel viewModel;
        public StartButtonCommand(MainViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public bool CanExecute(object parameter)
        {
            return viewModel.CanUpdateStartButton;
        }
        public void Execute(object parameter)
        {
            viewModel.StartButtonPressed();
        }

        #endregion
    }
}