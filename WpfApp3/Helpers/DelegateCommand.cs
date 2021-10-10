using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp3.Helpers
{
    internal class DelegateCommand : ICommand

    {
        private Action<object> execute;
        private Predicate<object>? canExecute;
     

        public event EventHandler? CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value; 
            }
        }


        public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;
    

        public void Execute(object? parameter)
        {
            this.execute(parameter);
        }

        public DelegateCommand(Action<object> execute) : this(execute, null) { }
        public DelegateCommand(Action<object> execute, Predicate<object>? canExecute)
        {
        
            this.execute = execute;
            this.canExecute = canExecute;
        }
    }
}
