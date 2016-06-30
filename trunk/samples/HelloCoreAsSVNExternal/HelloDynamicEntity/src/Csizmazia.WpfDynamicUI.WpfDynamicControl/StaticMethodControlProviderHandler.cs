using System.Reflection;
using System.Windows.Controls;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.MethodControlProviders;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl
{
    public class StaticMethodControlProviderHandler : IControlProviderHandler<MethodInfo>
    {
        private static readonly StaticMethodControlProvider StaticMethodControlProvider =
            new StaticMethodControlProvider();

        #region IControlProviderHandler<MethodInfo> Members

        public bool CanProvideControl(MethodInfo methodInfo)
        {
            return methodInfo.IsStatic;
        }

        public Control ProvideControl(MethodInfo methodInfo, object viewModel)
        {
            return StaticMethodControlProvider.ProvideControl(methodInfo, viewModel);
        }

        #endregion
    }
}