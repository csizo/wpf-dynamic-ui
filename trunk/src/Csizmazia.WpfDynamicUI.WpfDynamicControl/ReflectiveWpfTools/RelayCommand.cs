using System;
using System.Windows.Input;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ReflectiveWpfTools
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly Func<object, bool> _condition;

        public RelayCommand(Action<object> action)
        {
            _action = action;
        }

        public RelayCommand(Action<object> action, Func<object, bool> condition) : this(action)
        {
            _condition = condition;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _condition == null || _condition(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        #endregion

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}