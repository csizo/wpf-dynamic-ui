using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ReflectiveWpfTools;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.MethodControlProviders
{
    public class InstanceMethodControlProvider : MethodControlProvider
    {
        public override Control ProvideControl(MethodInfo methodInfo, object viewModel)
        {
            if (methodInfo == null) throw new ArgumentNullException("methodInfo");

            var button = new Button();

            button.TrySetButtonStyle(methodInfo);


            PropertyInfo conditionProperty = methodInfo.GetPropertyForUserInterfaceActionCondition();

            button.Command = new ReflectiveCommand(viewModel, methodInfo, conditionProperty);


            //set display name
            button.SetBinding(ContentControl.ContentProperty, methodInfo.GetDisplayNameBinding());


            button.SetValue(FrameworkElement.MarginProperty, new Thickness(3));



            button.SetBinding(FrameworkElement.ToolTipProperty, methodInfo.GetDisplayDescriptionBinding());

            button.Unloaded += (o, e) =>
                                   {
                                       var control = o as Button;
                                       if (control == null)
                                           return;

                                       BindingOperations.ClearAllBindings(control);
                                   };
            return button;
        }
    }
}