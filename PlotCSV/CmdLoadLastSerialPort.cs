using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PlotCSV
{
    public class RelayCommand : ICommand
    {
        private readonly Predicate<object> _m_CanExecute;
        private readonly Action<object> _m_Execute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute) : this(execute, null)
        {

        }

        public RelayCommand( Action<object> execute, Predicate<object> canExecute)
        {
            _m_Execute = execute;
            _m_CanExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return (null == _m_CanExecute) || _m_CanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _m_Execute(parameter);
        }

        public void CheckExecute()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
