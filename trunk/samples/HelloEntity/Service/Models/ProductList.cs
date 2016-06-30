using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Csizmazia.Collections;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Data;
using LinqKit;

namespace Service.Models
{
    public class ProductDelete : EntityDeleteModel<Product>
    {
        [Editable(false)]
        public int ProductId { get; set; }

        [Editable(false)]
        [StringLength(50)]
        public string Name { get; set; }

        [Editable(false)]
        [StringLength(1000)]
        public string Description { get; set; }
    }
    public class ProductNew : EntityNewModel<Product>
    {
        [Required()]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public decimal ListPrice { get; set; }

        TweakedObservableCollection<Category> _CategoryList = new TweakedObservableCollection<Category>();
        public TweakedObservableCollection<Category> CategoryList
        {
            get { return _CategoryList; }
        }
        private void CategoryListInit()
        {
            var query = from c in EntityRiportModel.Context.Set<Category>()
                        orderby c.Name
                        select c;

            _CategoryList.Clear();
            _CategoryList.AddRange(query.ToList());
        }

        [Display(AutoGenerateField = false)]
        [Required]
        public Category Category { get; set; }

        protected override void OnOpened()
        {
            base.OnOpened();

            CategoryListInit();
        }
    }
    public class ProductDetails : EntityDetailsModel<Product>
    {
        [Editable(false)]
        public int ProductId { get; set; }

        [Required()]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [Editable(false)]
        public decimal ListPrice { get; set; }

        TweakedObservableCollection<Category> _CategoryList = new TweakedObservableCollection<Category>();
        public TweakedObservableCollection<Category> CategoryList
        {
            get { return _CategoryList; }
        }
        private void CategoryListInit()
        {
            var query = from c in EntityRiportModel.Context.Set<Category>()
                        orderby c.Name
                        select c;

            _CategoryList.Clear();
            _CategoryList.AddRange(query.ToList());
        }

        [Display(AutoGenerateField = false)]
        [Required]
        public Category Category { get; set; }

        protected override void OnOpened()
        {
            base.OnOpened();

            CategoryListInit();
        }

        public void DecreasePrice()
        {
            ListPrice = ListPrice * 0.9M;
        }

        public void IncreasePrice()
        {
            ListPrice = ListPrice * 1.1M;
        }
    }

    public class ProductList : EntityRiportModel<Product>
    {
        [Display(AutoGenerateFilter = true)]
        public string FilterName { get; set; }

        protected override IOrderedQueryable<Product> ProvideQuery(System.Data.Entity.DbContext dbContext)
        {
            return from c in dbContext.Set<Product>()
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

        private Expression<Func<Product, bool>> BuildFilter()
        {
            Expression<Func<Product, bool>> condition = c => true;

            if (FilterName != null)
                condition = condition.And(c => c.Name.Contains(FilterName));

            return condition;
        }

        public static void Products(WorkspaceModel workspaceModel)
        {
            BusinessApplication.Instance.OpenModel(() => new ProductList());
        }
    }
}