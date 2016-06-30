using System;
using System.Reflection;
using System.Windows.Input;
using Csizmazia.Discovering;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ReflectiveWpfTools
{
    public class ReflectiveStaticCommand : ICommand
    {
        private readonly PropertyInfo _executeConditionProperty;
        private readonly MethodInfo _executeMethod;
        private readonly ParameterInfo[] _executeMethodParameters;

        public ReflectiveStaticCommand(MethodInfo executeMethod, PropertyInfo executeConditionProperty)
        {
            _executeMethodParameters = executeMethod.GetMethodParameters();
            if (_executeMethodParameters.Length > 1)
                throw new ArgumentException("Execute method can have only 0 or 1 parameter");

            _executeMethod = executeMethod;
            _executeConditionProperty = executeConditionProperty;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged = delegate { };

        public bool CanExecute(object parameter)
        {
            if (_executeConditionProperty != null)
                return (bool) _executeConditionProperty.GetValue(null, null);
            return true;
        }

        public void Execute(object parameter)
        {
            _executeMethod.Invoke(null, _executeMethodParameters.Length == 1 ? new[] {parameter} : null);
        }

        #endregion
    }
}