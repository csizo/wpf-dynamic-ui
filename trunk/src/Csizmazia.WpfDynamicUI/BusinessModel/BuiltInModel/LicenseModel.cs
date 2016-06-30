using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.Properties;

namespace Csizmazia.WpfDynamicUI.BusinessModel.BuiltInModel
{
    public class LicenseModel : NotifyPropertyChanged
    {
        [Editable(false)]
        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public string Name { get; set; }

        [Editable(false)]
        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        [UIHint(UIHints.DisplayMultiLineTextBox)]
        public string Description { get; set; }

        [Editable(false)]
        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        [UIHint(UIHints.DisplayHyperlink)]
        public string Url { get; set; }

        [Editable(false)]
        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        [UIHint(UIHints.DisplayMultiLineTextBox)]
        public string License { get; set; }
    }
}