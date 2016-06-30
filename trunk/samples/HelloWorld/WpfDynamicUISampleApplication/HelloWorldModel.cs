using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.BusinessModel;

namespace WpfDynamicUISampleApplication
{
    public class HelloWorldModel : INotifyPropertyChanged
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
                    _username = value;
                    OnPropertyChanged("UserName");
                    OnPropertyChanged("CanSayHello");
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
                    _message = value;
                    OnPropertyChanged("Message");
                }
            }
        }

        [Display(AutoGenerateField = false)]
        public bool CanSayHello
        {
            get { return !string.IsNullOrEmpty(_username); }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                                new PropertyChangedEventArgs(propertyName));
        }

        public void SayHello()
        {
            string message = string.Format("Hello '{0}'", Username);
            Message = message;
        }
    }
}