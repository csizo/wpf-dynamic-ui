using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Csizmazia.Collections;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Csizmazia.WpfDynamicUI.BusinessModel.BuiltInModel;
using Csizmazia.WpfDynamicUI.Collections;
using LinqKit;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.AboutModels
{
    public class RiportingModelsModel : NavigationModel
    {
        private PagedQueryable<ModelDto> _modelList;

        public PagedQueryable<ModelDto> ModelList
        {
            get { return _modelList; }
        }

        [Display(AutoGenerateFilter = true, Name = "Type name", Description = "Type name filter",
            Prompt = "Enter type name filter")]
        public string FilterTypeName { get; set; }

        [Display(AutoGenerateFilter = true, Name = "Title", Description = "Title filter", Prompt = "Enter title filter")
        ]
        public string FilterTitle { get; set; }

        private Expression<Func<ModelDto, bool>> BuildFilter()
        {
            Expression<Func<ModelDto, bool>> condition = c => true;

            if (!string.IsNullOrEmpty(FilterTypeName))
                condition = condition.And(c => c.ModelTypeName.Contains(FilterTypeName));

            if (!string.IsNullOrEmpty(FilterTitle))
                condition = condition.And(c => c.ModelTitle.Contains(FilterTitle));

            return condition;
        }

        protected override void OnOpened()
        {
            IOrderedQueryable<ModelDto> query = from m in ExportedModels.AsQueryable()
                                                select
                                                    new ModelDto
                                                        {ModelTitle = m.ModelTitle, ModelTypeName = m.GetType().Name}
                                                into dto
                                                orderby dto.ModelTypeName
                                                select dto;

            _modelList = new PagedQueryable<ModelDto>(query);

            base.OnOpened();
        }

        [Display(AutoGenerateField = true, Name = "Model riport", Description = "List all models handled by the system")
        ]
        public static void LoadRiport(AboutModel aboutModel)
        {
            BusinessApplication.Instance.OpenModel(() => new RiportingModelsModel());
        }

        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName.StartsWith("Filter"))
                ModelList.Filter = BuildFilter();

            base.OnPropertyChanged(propertyName, before, after);
        }

        #region Nested type: ModelDto

        public class ModelDto
        {
            public string ModelTypeName { get; set; }
            public string ModelTitle { get; set; }
        }

        #endregion
    }
}