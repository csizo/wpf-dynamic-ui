using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Csizmazia.WpfDynamicUI.BusinessModel.BuiltInModel;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel
{
    public class StartupModel : StartupModelBase
    {

        [Display(AutoGenerateField = true, ResourceType = typeof(WpfDynamicUI.Properties.Resources))]
        public void Login()
        {
            BusinessApplication.Instance.CloseCurrentModelAndNavigateTo(() => new LogonModel());
        }
    }
}