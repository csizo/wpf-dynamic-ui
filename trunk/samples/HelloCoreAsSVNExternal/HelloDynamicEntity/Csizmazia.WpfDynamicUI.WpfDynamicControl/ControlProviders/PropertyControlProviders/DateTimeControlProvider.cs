using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using Csizmazia.Discovering;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.PropertyControlProviders
{
    public class DateTimeControlProvider : SimplePropertyControlProvider
    {
        public override Control ProvideControl(PropertyInfo propertyInfo, object viewModel)
        {
            Binding binding = propertyInfo.GetBinding();

            var editableAttribute = propertyInfo.GetAttribute<EditableAttribute>();

            Control control = new DatePicker();
            control.SetBinding(DatePicker.SelectedDateProperty, binding);

            control.IsEnabled = propertyInfo.IsControlEnabled();


            return control;
        }
    }
}