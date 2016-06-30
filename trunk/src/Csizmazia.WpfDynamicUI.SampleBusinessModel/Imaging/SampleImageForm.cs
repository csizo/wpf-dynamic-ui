using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using Csizmazia.WpfDynamicUI.BusinessModel;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.Imaging
{
    public class SampleImageForm : NavigationModel
    {
        [Editable(false)]
        [Display(AutoGenerateField = true, Name = "Image name", Description = "Name of the image to display")]
        public string ImageName { get; private set; }

        [Display(AutoGenerateField = true, Description = "The displayed image")]
        public Image Image { get; set; }

        public void LoadImage()
        {
            string[] names = typeof(SampleImageForm).Assembly.GetManifestResourceNames();
            ImageName = "sample-image.png";

            Stream stream = typeof(SampleImageForm).Assembly.
                GetManifestResourceStream(names.FirstOrDefault(n => n.Contains(ImageName)));

            if (stream != null)
            {
                Image = new Bitmap(stream);
            }

            BusinessApplication.Instance.ShowPopup("Image has been loaded");
        }

        [Display(AutoGenerateField = true, Name = "Display Image")]
        public static void OpenImageForm(WorkspaceModel workspaceModel)
        {
            BusinessApplication.Instance.OpenModel(() => new SampleImageForm());

            BusinessApplication.Instance.ShowPopup("Load the image to display it");
        }
    }
}