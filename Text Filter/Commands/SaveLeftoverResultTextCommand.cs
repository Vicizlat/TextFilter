using TextFilter.ViewModels;
using System;
using System.Windows.Input;

namespace TextFilter.Commands
{
    internal class SaveLeftoverResultTextCommand : ICommand
    {
        public SaveLeftoverResultTextCommand(MainViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        private MainViewModel viewModel;

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public bool CanExecute(object parameter)
        {
            return viewModel.CanUpdateLeftover;
        }
        public void Execute(object parameter)
        {
            viewModel.SaveLeftoverResultText();
        }

        #endregion
    }
}