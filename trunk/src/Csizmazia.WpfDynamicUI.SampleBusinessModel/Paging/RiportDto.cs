using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.BusinessModel;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.Paging
{
    public class RiportDto : NotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Display(AutoGenerateField = false)]
        public bool CanUpdate { get; private set; }

        public void Update()
        {
            BusinessApplication.Instance.ShowPopup("Item is updated");
            CanUpdate = false;
        }

        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName != "CanUpdate")
                CanUpdate = true;

            base.OnPropertyChanged(propertyName, before, after);
        }
    }
}