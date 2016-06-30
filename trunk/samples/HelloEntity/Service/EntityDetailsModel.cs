using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Omu.ValueInjecter;

namespace Service
{
    public abstract class EntityDetailsModel<T> : NavigationModel
        where T : class
    {

        [Display(AutoGenerateField = false)]
        public T Entity { get; internal set; }

        [Display(AutoGenerateField = false)]
        public EntityRiportModel<T> EntityRiportModel { get; internal set; }

        [Display(AutoGenerateField = false)]
        public bool CanUpdate { get; set; }
        public virtual void Update()
        {
            try
            {
                if (!ValidateModel())
                    return;

                //var localEntity = Context.Set<T>().Attach(Entity);

                Entity.InjectFrom(this);

                EntityRiportModel.Context.SaveChanges();

                EntityRiportModel.Riport.Reload();

                BusinessApplication.Instance.CloseCurrentModel();
            }
            catch (Exception ex)
            {
                BusinessApplication.Instance.ShowPopup("There was an error saving the changes\r\n" + ex.Message);
            }

        }

        protected override void OnOpened()
        {
            base.OnOpened();
            PropertyChanged += (o, e) =>
            {
                IsDirty = true;
            };

            EntityRiportModel.Context.SavingChanges += (o, e) =>
            {
                IsDirty = false;
            };
        }
        [Display(AutoGenerateField = false)]
        public bool IsDirty { get; protected set; }

        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            switch (propertyName)
            {
                case "Error":
                case "IsDirty":
                    CanUpdate = IsDirty && string.IsNullOrEmpty(Error);
                    break;
                case "Entity":
                    if (after != null)
                        this.InjectFrom(after);
                    break;
            }
            base.OnPropertyChanged(propertyName, before, after);
        }


    }
}