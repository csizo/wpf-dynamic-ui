using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Csizmazia.Discovering;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders
{
    public abstract class MethodControlProvider
    {
        public abstract Control ProvideControl(MethodInfo methodInfo, object viewModel);


        internal static bool TrySetButtonContent(MethodInfo actionMethod, Control control)
        {
            var displayAttribute = actionMethod.GetAttribute<DisplayAttribute>();

            //set display name
            if (displayAttribute != null)
            {
                string name = displayAttribute.GetName();
                if (!String.IsNullOrEmpty(name))
                {
                    control.SetValue(ContentControl.ContentProperty, name);
                    return true;
                }
            }
            return false;
        }

        internal static void TrySetCommandParameterProperty(ButtonBase button, MethodInfo actionMethod,
                                                            object activeModel)
        {
            ParameterInfo parameter = actionMethod.GetMethodParameters().SingleOrDefault();

            if (parameter == null)
                return;

            if (parameter.ParameterType.IsInstanceOfType(activeModel))
            {
                button.SetValue(ButtonBase.CommandParameterProperty, activeModel);
            }
        }
    }
}