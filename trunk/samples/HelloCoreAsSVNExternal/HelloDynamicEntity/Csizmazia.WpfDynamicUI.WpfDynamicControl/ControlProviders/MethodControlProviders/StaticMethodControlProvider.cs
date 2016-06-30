using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ReflectiveWpfTools;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.MethodControlProviders
{
    public class StaticMethodControlProvider : MethodControlProvider
    {
        public override Control ProvideControl(MethodInfo methodInfo, object viewModel)
        {
            if (methodInfo == null) throw new ArgumentNullException("methodInfo");

            var button = new Button();


            PropertyInfo conditionProperty = methodInfo.GetStaticPropertyForUserInterfaceActionCondition();

            button.Command = new ReflectiveStaticCommand(methodInfo, conditionProperty);

            TrySetCommandParameterProperty(button, methodInfo, viewModel);

            //set button display name
            //button.Content = methodInfo.GetDisplayName();
            button.SetBinding(ContentControl.ContentProperty, methodInfo.GetDisplayNameBinding());

            //set button tooltip
            //string description = methodInfo.GetDisplayDescription();
            //if (description != null)
            //    button.SetValue(FrameworkElement.ToolTipProperty, description);
            button.SetBinding(FrameworkElement.ToolTipProperty, methodInfo.GetDisplayDescriptionBinding());

            button.SetValue(FrameworkElement.MarginProperty, new Thickness(3));


            button.TrySetButtonStyle(methodInfo);

            return button;
        }
    }
}