using System.ComponentModel.DataAnnotations;
using System.Linq;
using Csizmazia.WpfDynamicUI.Properties;

namespace Csizmazia.WpfDynamicUI.BusinessModel.BuiltInModel
{
    public class StartupRequiredModel : Model
    {
        public StartupRequiredModel()
        {
            Message = "No suitable startup model found";
        }


        [Editable(false)]
        [UIHint(UIHints.DisplayMultiLineTextBox)]
        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public string Message { get; private set; }
    }
    public abstract class StartupModelBase : Model
    {
        protected StartupModelBase()
        {
            WelcomeMessage =
                "Welcome to WPF Dynamic User Interface - The Business Application Library!";
        }


        [Editable(false)]
        [UIHint(UIHints.DisplayMultiLineTextBox)]
        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public string WelcomeMessage { get; private set; }




    }
}