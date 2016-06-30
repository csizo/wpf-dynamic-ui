using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Csizmazia.Collections;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Csizmazia.WpfDynamicUI.Collections;
using LinqKit;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.Paging
{
    public class PagedQueryRiport : NavigationModel
    {
        private PagedQueryable<RiportDto> _riport;

        #region filters

        [Display(AutoGenerateField = true, AutoGenerateFilter = true, Name = "Id", Prompt = "Enter Id to filter")]
        public int FilterId { get; set; }

        [Display(AutoGenerateField = true, AutoGenerateFilter = true, Name = "Name", Prompt = "Enter Name to filter")]
        public string FilterName { get; set; }

        [Display(AutoGenerateField = true, AutoGenerateFilter = true, Name = "Description",
            Prompt = "Enter description to filter")]
        public string FilterDescription { get; set; }

        #endregion

        public PagedQueryable<RiportDto> Riport
        {
            get { return _riport; }
        }


        public RiportDto SelectedRiportItem { get; set; }

        protected override void OnOpened()
        {
            var items = new List<RiportDto>();
            for (int i = 1; i < 100; i++)
            {
                items.Add(new RiportDto {Id = i, Name = "Name" + i, Description = "Description" + i});
            }

            //create new riport query with page size of 10
            _riport = new PagedQueryable<RiportDto>(items.AsQueryable().OrderBy(d => d.Id), 10);
        }


        private Expression<Func<RiportDto, bool>> BuildFilter()
        {
            Expression<Func<RiportDto, bool>> condition = c => true;

            if (FilterId > 0)
                condition = condition.And(c => c.Id == FilterId);

            if (!string.IsNullOrEmpty(FilterName))
                condition = condition.And(c => c.Name.Contains(FilterName));

            if (!string.IsNullOrEmpty(FilterDescription))
                condition = condition.And(c => c.Description.Contains(FilterDescription));

            return condition;
        }

        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName.StartsWith("Filter"))
                Riport.Filter = BuildFilter();

            base.OnPropertyChanged(propertyName, before, after);
        }


        [Display(AutoGenerateField = true, Name = "IQueryable riport")]
        public static void LoadPagedRiport(WorkspaceModel workspace)
        {
            BusinessApplication.Instance.OpenModel(() => new PagedQueryRiport());
        }


        [Display(AutoGenerateField = true, Name = "Save all", Description = "Save all items")]
        public void SaveAllItems()
        {
            foreach (RiportDto item in Riport.Items.Where(dto => dto.CanUpdate))
            {
                item.Update();
            }
        }
    }
}