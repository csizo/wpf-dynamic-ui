using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Csizmazia.WpfDynamicUI.SampleBusinessModel.Forms;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.Browser
{
    public class BrowserModel : NavigationModel
    {
        private string _webSite = "http://wpfdynamicui.codeplex.com";

        [UIHint(UIHints.DisplayWebBrowser, "Wpf", UIHints.DisplayParameters.Height, 600.0)]
        public string WebSite
        {
            get { return _webSite; }
            internal set { _webSite = value; }
        }

        [Display(AutoGenerateField = true, Name = "Wpf Browser")]
        public static void OpenBrowser(WorkspaceModel workspaceModel)
        {
            BusinessApplication.Instance.OpenModel(() => new BrowserModel());
        }
    }
}
