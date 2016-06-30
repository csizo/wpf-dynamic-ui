using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Csizmazia.WpfDynamicUI.SampleBusinessModel;

// ReSharper disable CheckNamespace
namespace DynamicCodeSample
// ReSharper restore CheckNamespace
{
    public class DynamicHelloWorldModel : NavigationModel
    {
        private string _message;
        private string _username;

        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    var before = _username;
                    _username = value;
                    OnPropertyChanged("Username", before, value);

                }
            }
        }

        [Editable(false)]
        public string Message
        {
            get { return _message; }
            private set
            {
                if (_message != value)
                {
                    var before = _message;
                    _message = value;
                    OnPropertyChanged("Message", before, value);
                }
            }
        }

        private bool _canSayHello;

        [Display(AutoGenerateField = false)]
        public bool CanSayHello
        {
            get { return _canSayHello; }
            private set
            {
                if (_canSayHello != value)
                {
                    var before = _canSayHello;
                    _canSayHello = value;
                    OnPropertyChanged("CanSayHello", before, value);
                }
            }
        }


        public void SayHello()
        {
            string message = string.Format("Hello '{0}'", Username);
            Message = message;
        }

        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == "Username")
            {
                CanSayHello = !string.IsNullOrEmpty(Username);
            }
            base.OnPropertyChanged(propertyName, before, after);
        }

        public static void OpenDynamicSayHello(WorkspaceModel workspaceModel)
        {
            BusinessApplication.Instance.OpenModel(() => new DynamicHelloWorldModel());
        }
    }
}