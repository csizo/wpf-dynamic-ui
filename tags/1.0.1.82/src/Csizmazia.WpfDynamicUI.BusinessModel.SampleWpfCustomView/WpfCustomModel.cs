using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.SampleBusinessModel;

namespace Csizmazia.WpfDynamicUI.BusinessModel.SampleWpfCustomView
{
    public class WpfCustomModel : NavigationModel
    {
        public UserControl1 CustomWpfControl { get; set; }

        protected override void OnOpened()
        {
            CustomWpfControl = new UserControl1();
        }


        [Display(AutoGenerateField = true, Name = "Custom Wpf control")]
        public static void LoadCustomWpfForm(WorkspaceModel workspaceModel)
        {
            BusinessApplication.Instance.OpenModel(() => new WpfCustomModel());

            BusinessApplication.Instance.ShowPopup("Displays CustomWpfControl UserControl property");
        }
    }
}