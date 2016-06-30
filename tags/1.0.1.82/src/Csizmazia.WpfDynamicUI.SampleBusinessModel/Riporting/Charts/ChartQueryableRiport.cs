using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Csizmazia.Collections;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Csizmazia.WpfDynamicUI.Collections;
using LinqKit;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.Riporting.Charts
{
    public class ChartQueryableRiport : NavigationModel
    {
        #region utilities

        private readonly Random _rnd = new Random();
        private List<DataDto> _salesListSource;

        #endregion

        #region Queryable

        private PagedQueryable<DataDto> _salesList;

        [Display(AutoGenerateFilter = true)]
        [UIHint(UIHints.DisplayDelayedBindingTextBox)]
        public string FilterCategory { get; set; }

        [Display(AutoGenerateFilter = true)]
        public double? MinAmount { get; set; }

        [Display(AutoGenerateFilter = true)]
        public double? MaxAmount { get; set; }

        public PagedQueryable<DataDto> SalesList
        {
            get { return _salesList; }
        }

        private Expression<Func<DataDto, bool>> BuildFilter()
        {
            Expression<Func<DataDto, bool>> condition = c => true;
            if (!string.IsNullOrEmpty(FilterCategory))
                condition = condition.And(c => c.Category.Contains(FilterCategory));

            if (MinAmount != null)
                condition = condition.And(c => c.SalesOrderRowAmount >= MinAmount);

            if (MaxAmount != null)
                condition = condition.And(c => c.SalesOrderRowAmount <= MaxAmount);
            return condition;
        }

        #endregion

        private readonly TweakedObservableCollection<ChartDataDto> _salesSumary =
            new TweakedObservableCollection<ChartDataDto>();


        [UIHint(UIHints.DisplayChart, "",
            UIHints.ChartControlParameters.ChartType, UIHints.ChartControlParameters.ChartTypePie,
            UIHints.ChartControlParameters.ChartCategoryProperty, "Area",
            UIHints.ChartControlParameters.ChartValueProperty, "TotalSales")]
        public ObservableCollection<ChartDataDto> SalesSumary
        {
            get { return _salesSumary; }
        }

        protected override void OnActivated()
        {
            _salesListSource = new List<DataDto>();

            #region fill query source (could be from database...)

            int i;
            for (i = 0; i < _rnd.Next(1, 10000); i++)
            {
                _salesListSource.Add(new DataDto
                                         {
                                             CreatedAt = DateTime.Now,
                                             Category = "North",
                                             SalesOrderRowAmount = _rnd.Next(1, 100)
                                         });
            }
            for (i = 0; i < _rnd.Next(1, 10000); i++)
            {
                _salesListSource.Add(new DataDto
                                         {
                                             CreatedAt = DateTime.Now,
                                             Category = "East",
                                             SalesOrderRowAmount = _rnd.Next(1, 100)
                                         });
            }
            for (i = 0; i < _rnd.Next(1, 10000); i++)
            {
                _salesListSource.Add(new DataDto
                                         {
                                             CreatedAt = DateTime.Now,
                                             Category = "West",
                                             SalesOrderRowAmount = _rnd.Next(1, 100)
                                         });
            }
            for (i = 0; i < _rnd.Next(1, 10000); i++)
            {
                _salesListSource.Add(new DataDto
                                         {
                                             CreatedAt = DateTime.Now,
                                             Category = "South",
                                             SalesOrderRowAmount = _rnd.Next(1, 100)
                                         });
            }

            #endregion

            IOrderedQueryable<DataDto> query = _salesListSource.AsQueryable().OrderBy(o => o.CreatedAt);
            _salesList = new PagedQueryable<DataDto>(query, 10);

            _salesList.Reloaded += SalesListReloaded;

            base.OnActivated();
        }

        protected override void OnDeactivated()
        {
            _salesList.Reloaded -= SalesListReloaded;


            _salesSumary.Clear();
            base.OnDeactivated();
        }

        private void SalesListReloaded(object sender, EventArgs e)
        {
            _salesSumary.Clear();

            IQueryable<ChartDataDto> chartDataQuery = from i in SalesList.GetFilteredQuery()
                                                      group i by i.Category
                                                      into gr
                                                      select new ChartDataDto
                                                                 {
                                                                     Area = gr.Key,
                                                                     TotalSales = gr.Sum(s => s.SalesOrderRowAmount)
                                                                 };

            _salesSumary.AddRange(chartDataQuery.ToList());
        }

        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (_salesList != null)
                _salesList.Filter = BuildFilter();

            base.OnPropertyChanged(propertyName, before, after);
        }

        public static void LoadSalesSumaryChart(ChartingForm chartingForm)
        {
            BusinessApplication.Instance.OpenModel(() => new ChartQueryableRiport());
        }

        #region Nested type: DataDto

        public class DataDto : NotifyPropertyChanged
        {
            public DateTime CreatedAt { get; set; }
            public string Category { get; set; }
            public double SalesOrderRowAmount { get; set; }
        }

        #endregion
    }
}