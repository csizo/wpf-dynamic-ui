using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Csizmazia.WpfDynamicUI.Collections;

namespace Csizmazia.WpfDynamicUI.DataServices
{
    public abstract class EntityListModel<TContext, TEntity> : RepositoryModel<TContext>
        where TContext : DbContext, new()
        where TEntity : class
    {
        #region Properties

        #region Query

        protected PagedQueryable<TEntity> _query;

        public PagedQueryable<TEntity> Query
        {
            get { return _query; }
        }

        #endregion

        private TEntity _selectedQueryItem;

        [Display(AutoGenerateField = false)]
        public TEntity SelectedQueryItem
        {
            get { return _selectedQueryItem; }
            set
            {
                if (_selectedQueryItem != value)
                {
                    if (value != null)
                    {
                        DbEntityEntry<TEntity> entry = base.Repository.Context.Entry(value);
                        if (entry == null)
                            throw new InvalidOperationException("Cannot set Entity from a different DbDontext");
                    }

                    TEntity before = _selectedQueryItem;
                    _selectedQueryItem = value;
                    OnPropertyChanged("SelectedQueryItem", before, value);
                }
            }
        }

        #endregion

        /*
        CodeGen filters here...
         * 
        */
        //#region codeGen Filters
        //public string FilterValue { get; set; }

        //protected override Expression<Func<T, bool>> BuildFilter()
        //{
        //    Expression<Func<T, bool>> condition = c => true;

        //    if (FilterValue != null)
        //        condition.FilterValue.Contains(Value);

        //    return condition;
        //}

        //#endregion

        //private bool _isEditEnabled;

        //private bool _isNewEnabled;

        //[Display(AutoGenerateField = false)]
        //public bool IsEditEnabled
        //{
        //    get { return _isEditEnabled; }
        //    private set
        //    {
        //        if (_isEditEnabled != value)
        //        {
        //            bool before = _isEditEnabled;
        //            _isEditEnabled = value;
        //            OnPropertyChanged("IsEditEnabled", before, value);
        //        }
        //    }
        //}

        //[Display(AutoGenerateField = false)]
        //public bool IsNewEnabled
        //{
        //    get { return _isNewEnabled; }
        //    set
        //    {
        //        if (_isNewEnabled != value)
        //        {
        //            bool before = _isNewEnabled;
        //            _isNewEnabled = value;
        //            OnPropertyChanged("IsNewEnabled", before, value);
        //        }
        //    }
        //}

        //public void Edit()
        //{
        //}

        //public void New()
        //{
        //}

        /// <summary>
        /// Applies filter on <see cref="Query"/>
        /// </summary>
        protected abstract void ApplyFilter();

        /// <summary>
        /// Provide the PagedQueryable&lt;TEntity&gt; Query 
        /// </summary>
        protected abstract void ProvideQuery();

        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName.StartsWith("Filter"))
            {
                ApplyFilter();
            }

            switch (propertyName)
            {
                case "SelectedQueryItem":
                    //IsEditEnabled = SelectedQueryItem != null;
                    break;
            }

            base.OnPropertyChanged(propertyName, before, after);
        }

        protected override void OnOpened()
        {
            ProvideQuery();

            base.OnOpened();
        }
    }
}