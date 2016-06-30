using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.Properties;

namespace Csizmazia.WpfDynamicUI.BusinessModel
{
    public abstract class NavigationModel : Model
    {
        [Display(AutoGenerateField = true, ResourceType = typeof (Resources))]
        public void Close()
        {
            BusinessApplication.Instance.CloseCurrentModel();
        }
    }
}