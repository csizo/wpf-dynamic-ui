using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Csizmazia.WpfDynamicUI.BusinessModel.BuiltInModel;
using Data;

namespace Service.Models
{
    public class StartupModel : StartupModelBase
    {

        private Lazy<DbEntities> _DbContext = new Lazy<DbEntities>(() => new DbEntities());
        protected DbEntities DbContext
        {
            get { return _DbContext.Value; }
        }

        protected override void OnClosed()
        {
            DbContext.Dispose();
            base.OnClosed();
        }

        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        public void Logon()
        {
            //check user
            if (DbContext.Users.Any(u => u.Username == Username && u.Password == Password))
                BusinessApplication.Instance.CloseCurrentModelAndNavigateTo(() => new WorkspaceModel());
            else
            {
                BusinessApplication.Instance.ShowPopup("Invalid username or password");
            }
        }

        public void RegisterUser()
        {
            var user = new User()
                           {
                               Username = Username,
                               Password = Password
                           };

            DbContext.Users.Add(user);
            DbContext.SaveChanges();

            BusinessApplication.Instance.ShowPopup("User saved");
        }
    }
}