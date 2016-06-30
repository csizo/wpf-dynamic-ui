using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.PropertyControlProviders
{
    public class WpfControlControlProvider : InheritedPropertyControlProvider
    {
        public override Control ProvideControl(PropertyInfo propertyInfo, object viewModel)
        {
            Binding binding = propertyInfo.GetBinding();
            binding.Mode = BindingMode.OneWay;

            Control control = new ContentControl();


            control.SetBinding(ContentControl.ContentProperty, binding);


            return control;
        }
    }
}