using System.Reflection;
using System.Windows.Controls;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.MethodControlProviders;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl
{
    public class InstanceMethodControlProviderHandler : IControlProviderHandler<MethodInfo>
    {
        private static readonly InstanceMethodControlProvider InstanceMethodControlProvider =
            new InstanceMethodControlProvider();

        #region IControlProviderHandler<MethodInfo> Members

        public bool CanProvideControl(MethodInfo methodInfo)
        {
            return !methodInfo.IsStatic;
        }

        public Control ProvideControl(MethodInfo methodInfo, object viewModel)
        {
            return InstanceMethodControlProvider.ProvideControl(methodInfo, viewModel);
        }

        #endregion
    }
}