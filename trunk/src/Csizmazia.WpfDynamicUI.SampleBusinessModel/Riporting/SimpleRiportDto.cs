using System;
using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.BusinessModel;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.Riporting
{
    public class SimpleRiportDto : NotifyPropertyChanged
    {
        [Editable(false)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTill { get; set; }
        public bool IsEnabled { get; set; }

        [Display(AutoGenerateField = true, Name = "Update")]
        public void Update()
        {
            //update item in database
            BusinessApplication.Instance.ShowPopup("The SimpleRiportDto item has been updated in the database");
        }

        [Display(AutoGenerateField = true, Name = "Reload")]
        public void Reload()
        {
            BusinessApplication.Instance.ShowPopup("The SimpleRiportDto item has been reloaded from the database");
        }
    }
}