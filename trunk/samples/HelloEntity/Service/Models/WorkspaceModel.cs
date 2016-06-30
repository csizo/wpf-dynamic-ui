using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI;
using Csizmazia.WpfDynamicUI.BusinessModel;

namespace Service.Models
{
    public class WorkspaceModel : Model
    {
        private string _aboutTheApplication = "http://wpfdynamicui.codeplex.com";

        [UIHint(UIHints.DisplayWebBrowser, "Wpf", UIHints.DisplayParameters.Height, 600.0)]
        public string AboutTheApplication
        {
            get { return _aboutTheApplication; }
            set { _aboutTheApplication = value; }
        }

        public void RefreshWebPage()
        {
            AboutTheApplication = "about:tabs";
            AboutTheApplication = "http://wpfdynamicui.codeplex.com";
        }
    }
}