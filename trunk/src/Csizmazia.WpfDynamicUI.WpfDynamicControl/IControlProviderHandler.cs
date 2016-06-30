using System.Windows.Controls;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl
{
    public interface IControlProviderHandler<in TParameter>
    {
        bool CanProvideControl(TParameter parameter);
        Control ProvideControl(TParameter parameter, object viewModel);
    }
}