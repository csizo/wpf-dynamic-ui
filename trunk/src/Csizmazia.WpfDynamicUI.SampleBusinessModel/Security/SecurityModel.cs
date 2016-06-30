using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.BusinessModel;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.Security
{
    public class SecurityModel : NavigationModel
    {
        private string LoggedInUser { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 4)]
        public string Password { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 4)]
        public string Password2 { get; set; }

        public string Email { get; set; }

        public void Register()
        {
            if (ValidateModel())
            {
                BusinessApplication.Instance.ShowPopup(string.Format("User {0} has been registered by {1}...", Username,
                                                                     LoggedInUser));
            }
        }

        [Display(AutoGenerateField = true, Name = "Register new user")]
        public static void RegisterNewUser(WorkspaceModel workspaceModel)
        {
            BusinessApplication.Instance.OpenModel(
                () => new SecurityModel {LoggedInUser = workspaceModel.LoggedInUser});
        }
    }
}