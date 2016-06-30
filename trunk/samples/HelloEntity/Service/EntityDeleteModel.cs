using System;
using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Omu.ValueInjecter;

namespace Service
{
    public abstract class EntityDeleteModel<T> : NavigationModel
        where T : class
    {
        [Display(AutoGenerateField = false)]
        public T Entity { get; internal set; }

        [Display(AutoGenerateField = false)]
        public EntityRiportModel<T> EntityRiportModel { get; internal set; }

        [Display(AutoGenerateField = true)]
        public bool CanDelete { get; set; }
        public void Delete()
        {
            try
            {
                if (!ValidateModel())
                    return;


                //var set = Context.Set<T>();
                //var localEntity = set.Attach(Entity);
                EntityRiportModel.Context.Set<T>().Remove(Entity);
                EntityRiportModel.Context.SaveChanges();

                EntityRiportModel.Riport.Reload();

                BusinessApplication.Instance.CloseCurrentModel();
            }
            catch (Exception ex)
            {
                BusinessApplication.Instance.ShowPopup("There was an error saving the changes\r\n" + ex.Message);
            }

        }


        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            switch (propertyName)
            {
                case "Entity":
                    if (after != null)
                        this.InjectFrom(after);
                    break;
            }

            base.OnPropertyChanged(propertyName, before, after);
        }


    }
}