using System;
using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.BusinessModel;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.Forms
{
    public class SimpleWorkflowForm : NavigationModel
    {
        [Required]
        [StringLength(80)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTill { get; set; }
        public bool IsEnabled { get; set; }

        [Display(AutoGenerateField = false)]
        public bool CanSave { get; set; }

        public void Save()
        {
            CanSave = false;
        }

        public void CancelForm()
        {
            Name = null;
            Description = null;
            ValidFrom = null;
            ValidTill = null;

            CanSave = false;
        }


        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName != "CanSave")
                CanSave = true;


            base.OnPropertyChanged(propertyName, before, after);
        }


        [Display(AutoGenerateField = true, Name = "Simple workflow")]
        public static void LoadSimpleWorkflowForm(WorkspaceModel workspaceModel)
        {
            BusinessApplication.Instance.OpenModel(() => new SimpleWorkflowForm());
        }

        protected override void OnOpened()
        {
            BusinessApplication.Instance.ShowPopup("Opened Workflow Form");
        }
    }
}