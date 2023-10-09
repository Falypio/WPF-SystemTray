using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WpfApp1.Common
{
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }
        public void Execute(object parameter)
        {
            CommandAction();
        }
        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
