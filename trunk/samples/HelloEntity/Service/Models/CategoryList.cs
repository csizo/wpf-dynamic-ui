using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Data;
using LinqKit;

namespace Service.Models
{
    public class CategoryDelete : EntityDeleteModel<Category>
    {
        [Editable(false)]
        public int CategoryId { get; set; }

        [Editable(false)]
        [StringLength(50)]
        public string Name { get; set; }

        [Editable(false)]
        [StringLength(1000)]
        public string Description { get; set; }
    }
    public class CategoryNew : EntityNewModel<Category>
    {
        [Required()]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }
    }
    public class CategoryDetails : EntityDetailsModel<Category>
    {
        [Editable(false)]
        public int CategoryId { get; set; }

        [Required()]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }
    }
    public class CategoryList : EntityRiportModel<Category>
    {
        [Display(AutoGenerateFilter = true)]
        public string FilterName { get; set; }

        protected override IOrderedQueryable<Category> ProvideQuery(System.Data.Entity.DbContext dbContext)
        {
            return from c in dbContext.Set<Category>()
                   orderby c.Name
                   select c;
        }

        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName.StartsWith("Filter"))
            {
                Riport.Filter = BuildFilter();
            }
            base.OnPropertyChanged(propertyName, before, after);
        }

        private Expression<Func<Category, bool>> BuildFilter()
        {
            Expression<Func<Category, bool>> condition = c => true;

            if (FilterName != null)
                condition = condition.And(c => c.Name.Contains(FilterName));

            return condition;
        }

        public static void Categories(WorkspaceModel workspaceModel)
        {
            BusinessApplication.Instance.OpenModel(() => new CategoryList());
        }
    }
}
