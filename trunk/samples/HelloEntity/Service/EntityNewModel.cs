using System;
using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Omu.ValueInjecter;

namespace Service
{
    public abstract class EntityNewModel<T> : NavigationModel
        where T : class
    {
        [Display(AutoGenerateField = false)]
        public T Entity { get; internal set; }

        [Display(AutoGenerateField = false)]
        public EntityRiportModel<T> EntityRiportModel { get; internal set; }

        [Display(AutoGenerateField = false)]
        public bool CanInsert { get; set; }
        public void Insert()
        {
            try
            {
                if (!ValidateModel())
                    return;

                Entity.InjectFrom(this);

                EntityRiportModel.Context.Set<T>().Add(Entity);



                EntityRiportModel.Context.SaveChanges();

                EntityRiportModel.Riport.Reload();
                IsDirty = false;

                BusinessApplication.Instance.ShowPopup("Item saved");
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
                                       CanInsert = true;

                                   };

            EntityRiportModel.PropertyChanged += (o, e) =>
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
                    CanInsert = IsDirty && Error == null;
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