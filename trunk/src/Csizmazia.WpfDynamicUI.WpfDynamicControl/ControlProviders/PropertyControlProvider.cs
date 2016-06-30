using System.Reflection;
using System.Windows.Controls;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders
{
    public abstract class PropertyControlProvider
    {
        public abstract Control ProvideControl(PropertyInfo propertyInfo, object viewModel);
    }
}