using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using Csizmazia.WpfDynamicUI.WeakEvents;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ReflectiveWpfTools
{
    public class ReflectiveCommand : ICommand
    {
        private readonly PropertyInfo _executeConditionProperty;
        private readonly MethodInfo _executeMethod;
        private readonly WeakReference<object> _model;


        public ReflectiveCommand(object model, MethodInfo executeMethod, PropertyInfo executeConditionProperty)
        {
            _model = new WeakReference<object>(model);
            _executeMethod = executeMethod;
            _executeConditionProperty = executeConditionProperty;

            if (_executeConditionProperty != null)
            {
                var propertyChanged = model as INotifyPropertyChanged;

                if (propertyChanged != null)
                {
                    propertyChanged.PropertyChanged +=
                        new PropertyChangedEventHandler(OnModelPropertyChanged).MakeWeak(eh =>
                                                                                         propertyChanged.PropertyChanged
                                                                                         -= eh);
                }
            }
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            object model = _model.Target;

            if (_executeConditionProperty != null && model != null)
                return (bool) _executeConditionProperty.GetValue(model, null);
            return true;
        }

        public void Execute(object parameter)
        {
            object model = _model.Target;
            if (model != null)
                _executeMethod.Invoke(model, null);
        }

        #endregion

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _executeConditionProperty.Name)
            {
                OnCanExecuteChanged();
            }
        }

        protected virtual void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}