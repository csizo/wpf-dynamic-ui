using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using Csizmazia.Discovering;
using Microsoft.Windows.Controls;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.PropertyControlProviders
{
    public class DoubleControlProvider : SimplePropertyControlProvider
    {
        public override Control ProvideControl(PropertyInfo propertyInfo, object viewModel)
        {
            Binding binding = propertyInfo.GetBinding();

            var editableAttribute = propertyInfo.GetAttribute<EditableAttribute>();

            Control control = new DoubleUpDown();
            control.SetBinding(DoubleUpDown.ValueProperty, binding);

            control.IsEnabled = editableAttribute == null || editableAttribute.AllowEdit ||
                                binding.Mode == BindingMode.TwoWay;


            return control;
        }
    }
}