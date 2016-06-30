using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Csizmazia.Discovering;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.CustomControls;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.PropertyControlProviders
{
    public class PagedQueryableControlProvider : InheritedPropertyControlProvider
    {
        public override Control ProvideControl(PropertyInfo propertyInfo, object viewModel)
        {
            var control = new DynamicPagerControl();

            Binding binding = propertyInfo.GetBinding();
            control.SetBinding(FrameworkElement.DataContextProperty, binding);

            PropertyInfo itemsProperty =
                propertyInfo.PropertyType.GetInstanceProperties().FirstOrDefault(pi => pi.Name == "Items");


            //check for selected item
            if (itemsProperty.HasPropertyForSelectedItem(propertyInfo.Name, propertyInfo.DeclaringType))
            {
                //create a selectedItem binding
                Binding selectedItemBinding =
                    itemsProperty.GetPropertyForSelectedItem(propertyInfo.Name, propertyInfo.DeclaringType).GetBinding();

                //set binding source to viewModel
                selectedItemBinding.Source = viewModel;

                //pass the SelectedItemBinding to DynamicPagerControl
                control.SelectedItemBinding = selectedItemBinding;
            }

            control.Unloaded += (o, e) =>
                                    {
                                        var ctl = o as DynamicPagerControl;
                                        if (ctl == null)
                                            return;

                                        BindingOperations.ClearAllBindings(ctl);
                                    };

            return control;
        }
    }
}