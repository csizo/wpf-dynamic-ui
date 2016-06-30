using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using Csizmazia.Discovering;
using Microsoft.Windows.Controls;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.PropertyControlProviders
{
    public class IntegerControlProvider : SimplePropertyControlProvider
    {
        public override Control ProvideControl(PropertyInfo propertyInfo, object viewModel)
        {
            Binding binding = propertyInfo.GetBinding();

            var editableAttribute = propertyInfo.GetAttribute<EditableAttribute>();

            Control control = new IntegerUpDown();
            control.SetBinding(IntegerUpDown.ValueProperty, binding);
            control.IsEnabled = propertyInfo.IsControlEnabled();


            return control;
        }
    }
}