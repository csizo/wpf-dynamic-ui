using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.BusinessModel;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel
{
    public class WorkspaceModel : Model
    {
        [Editable(false)]
        public string LoggedInUser { get; internal set; }

        [Editable(false)]
        [UIHint(UIHints.DisplayMultiLineTextBox)]
        public string Information { get; internal set; }

        protected override void OnActivated()
        {
            Information =
                "Application is now authenticated\r\nSee the module menu on the left to start exploring the WpfSynamicUI possibilities.\r\nMeanwhile please do not forget; WpfDynamicUI is a runtime View engine...\r\nFor any questions or feedback navigate to http:\\wpfdynamicui.codeplex.com";
        }

        public void Logoff()
        {
            BusinessApplication.Instance.CloseCurrentModelAndNavigateTo(() => new LogonModel());
        }

        protected override void OnOpened()
        {
            BusinessApplication.Instance.ShowPopup("Now logged into the system!");
        }
    }
}