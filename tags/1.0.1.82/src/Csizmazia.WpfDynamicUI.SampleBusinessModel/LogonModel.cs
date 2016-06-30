using System.ComponentModel.DataAnnotations;
using System.Linq;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Csizmazia.WpfDynamicUI.SampleBusinessModel.Properties;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel
{
    public class LogonModel : Model
    {
        public LogonModel()
        {
            HelpText = Resources.LogonModelHelpText;
        }

        [StringLength(15)]
        [Required]
        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        [UIHint(UIHints.DisplayDelayedBindingTextBox)]
        public string Username { get; set; }

        [StringLength(15)]
        [Required]
        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        [UIHint(UIHints.DisplayDelayedBindingTextBox)]
        public string Password { get; set; }


        [Display(AutoGenerateField = false)]
        public bool CanLogin { get; private set; }

        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public void Login()
        {
            //check username and password... perhaps from a database.

            if (Username == "admin" && Password == "admin")
            {
                BusinessApplication.Instance.CloseCurrentModelAndNavigateTo(
                    () => new WorkspaceModel { LoggedInUser = Username });
            }
            else
            {
                BusinessApplication.Instance.ShowPopup(string.Format("Login failed for user {0}", Username));
            }
        }

        protected override void OnActivated()
        {
            BusinessApplication.Instance.ShowPopup("Use Username: admin and password: admin to login");
        }

        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public void CancelLogin()
        {
            BusinessApplication.Instance.CloseCurrentModelAndNavigateTo(() => new StartupModel());
        }

        //#if DEBUG
        //        public void DebugExit()
        //        {
        //            BusinessApplication.Instance.CloseCurrentModel();
        //        }
        //#endif

        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            CanLogin = !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);

            base.OnPropertyChanged(propertyName, before, after);
        }
    }
}