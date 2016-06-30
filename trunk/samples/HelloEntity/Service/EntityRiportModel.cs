using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Csizmazia.WpfDynamicUI.Collections;

namespace Service
{
    public abstract class EntityRiportModel<T> : EntityModel
        where T : class
    {
        private PagedQueryable<T> _riport;
        public PagedQueryable<T> Riport { get { return _riport; } }

        [Display(AutoGenerateField = false)]
        public T SelectedRiportItem { get; set; }


        protected abstract IOrderedQueryable<T> ProvideQuery(DbContext dbContext);
        protected override void OnOpened()
        {
            base.OnOpened();

            //check if modes are supported
            IsNewSupported = ExportedModels.Any(m => m is EntityNewModel<T>);
            IsDeleteSupported = ExportedModels.Any(m => m is EntityDeleteModel<T>);
            IsDetailsSupported = ExportedModels.Any(m => m is EntityDetailsModel<T>);
            IsNewEnabled = IsDetailsSupported;

            _riport = new PagedQueryable<T>(ProvideQuery(Context));
        }


        public void New()
        {
            var newModel = base.ExportedModels.OfType<EntityNewModel<T>>().FirstOrDefault();
            if (newModel != null)
            {
                newModel.Entity = Context.Set<T>().Create<T>();
                newModel.EntityRiportModel = this;
                BusinessApplication.Instance.OpenModel(() => newModel);
            }
        }


        public void Details()
        {
            var detailsModel = base.ExportedModels.OfType<EntityDetailsModel<T>>().FirstOrDefault();
            if (detailsModel != null)
            {
                detailsModel.Entity = SelectedRiportItem;
                detailsModel.EntityRiportModel = this;
                BusinessApplication.Instance.OpenModel(() => detailsModel);
            }
        }

        public void Delete()
        {
            var deleteModel = base.ExportedModels.OfType<EntityDeleteModel<T>>().FirstOrDefault();
            if (deleteModel != null)
            {
                deleteModel.Entity = SelectedRiportItem;
                deleteModel.EntityRiportModel = this;
                BusinessApplication.Instance.OpenModel(() => deleteModel);
            }
        }

        [Display(AutoGenerateField = false)]
        public bool IsDetailsEnabled { get; private set; }

        [Display(AutoGenerateField = false)]
        public bool IsDeleteEnabled { get; private set; }

        [Display(AutoGenerateField = false)]
        public bool IsNewEnabled { get; private set; }

        private bool IsNewSupported { get; set; }
        private bool IsDeleteSupported { get; set; }
        private bool IsDetailsSupported { get; set; }


        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            switch (propertyName)
            {
                case "SelectedRiportItem":
                    IsDetailsEnabled = IsDetailsSupported && after != null;
                    IsDeleteEnabled = IsDeleteSupported && after != null;
                    break;
            }
            base.OnPropertyChanged(propertyName, before, after);
        }
    }
}