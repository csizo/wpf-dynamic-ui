using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Csizmazia.Discovering;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ReflectiveWpfTools;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.PropertyControlProviders
{
    public class ModelControlProvider : InheritedPropertyControlProvider
    {
        public override Control ProvideControl(PropertyInfo propertyInfo, object viewModel)
        {
            Binding binding = propertyInfo.GetBinding();

            //var editableAttribute = propertyInfo.GetAttribute<EditableAttribute>();

            var uiHintAttribute = propertyInfo.GetAttribute<UIHintAttribute>();

            Control control = new DynamicModelControl();
            control.SetValue(DynamicModelControl.DisplayMenuProperty, false);
            control.SetBinding(FrameworkElement.DataContextProperty, binding);

            if (uiHintAttribute != null)
            {
                foreach (var controlParameter in uiHintAttribute.ControlParameters)
                {
                    switch (controlParameter.Key)
                    {
                        case "VisibilityConverter":
                            if (controlParameter.Value.ToString() == "NotNullToVisibilityConverter")
                            {
                                Binding visibilityBinding = propertyInfo.GetBinding();
                                visibilityBinding.Mode = BindingMode.OneWay;
                                visibilityBinding.ConverterParameter = new NotNullToVisibilityConverter();
                                control.SetBinding(UIElement.VisibilityProperty, visibilityBinding);
                            }

                            break;
                    }
                }
            }

            return control;
        }
    }
}