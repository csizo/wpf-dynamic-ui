using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Csizmazia.Collections;
using Csizmazia.Tracing;
using Csizmazia.WpfDynamicUI.Properties;

namespace Csizmazia.WpfDynamicUI.Collections
{
    /// <summary>
    /// PagedQueryable is providing a bindable and paged extension over an IQueryable query instance
    /// <para>
    /// PagedQueryable is compatible with in-memory AsQueryable() queries and Entity Framework queries</para>
    /// </summary>
    /// <typeparam name="T">Type of the queryed object</typeparam>
    public class PagedQueryable<T> : IPagedQueryable, INotifyPropertyChanged, ISupportInitialize
    {
        private static readonly Tracer<PagedQueryable<T>> Tracer = Tracer<PagedQueryable<T>>.Instance;

        private readonly TweakedObservableCollection<T> _items = new TweakedObservableCollection<T>();
        private readonly IOrderedQueryable<T> _query;
        private readonly TweakedObservableCollection<T> _selectedItems = new TweakedObservableCollection<T>();
        private int _currentPage = 1;
        private Expression<Func<T, bool>> _filter;
        private bool _isFreezed;
        private int _pageCount;
        private int _pageSize = 20;
        private int _queryCount;
        private string _sortColumn;
        private SortDirection _sortDirection;

        public PagedQueryable(IOrderedQueryable<T> query, bool autoLoad = true)
        {
            using (new ConstructorTracer<PagedQueryable<T>>())
            {
                if (query == null) throw new ArgumentNullException("query");

                Tracer.Verbose(() => "Adding event handler to SelectedItems.CollectionChanged");
                _selectedItems.CollectionChanged += SelectedItemsCollectionChanged;

                Tracer.Verbose(() => "setting to freezed");

                _isFreezed = true;

                _query = query;


                Tracer.Verbose(() => "setting to non-freezed");
                _isFreezed = false;

                Tracer.Verbose(() => "checking for autoload");
                if (autoLoad)
                {
                    Tracer.Verbose(() => "resetting");
                    Reset();
                }
            }
        }

        public PagedQueryable(IOrderedQueryable<T> query, Expression<Func<T, bool>> condition, bool autoLoad = true)
        {
            using (new ConstructorTracer<PagedQueryable<T>>())
            {
                if (query == null) throw new ArgumentNullException("query");
                if (condition == null) throw new ArgumentNullException("condition");

                Tracer.Verbose(() => "Adding event handler to SelectedItems.CollectionChanged");
                _selectedItems.CollectionChanged += SelectedItemsCollectionChanged;


                Tracer.Verbose(() => "setting to freezed");
                _isFreezed = true;

                _query = query;
                Filter = condition;

                Tracer.Verbose(() => "setting to nonfreezed");
                _isFreezed = false;

                Tracer.Verbose(() => "checking for autoload");
                if (autoLoad)
                {
                    Tracer.Verbose(() => "resetting");
                    Reset();
                }
            }
        }

        public PagedQueryable(IOrderedQueryable<T> query, Expression<Func<T, bool>> condition, int pageSize,
                              bool autoLoad = true)
        {
            using (new ConstructorTracer<PagedQueryable<T>>())
            {
                if (query == null) throw new ArgumentNullException("query");
                if (condition == null) throw new ArgumentNullException("condition");
                if (pageSize < 0)
                    throw new ArgumentOutOfRangeException("pageSize", "pageSize cannot be less than zero");

                Tracer.Verbose(() => "Adding event handler to SelectedItems.CollectionChanged");
                _selectedItems.CollectionChanged += SelectedItemsCollectionChanged;


                Tracer.Verbose(() => "setting to freezed");
                _isFreezed = true;

                _query = query;
                Filter = condition;
                _pageSize = pageSize;

                Tracer.Verbose(() => "setting to nonfreezed");
                _isFreezed = false;

                Tracer.Verbose(() => "checking autoload");
                if (autoLoad)
                {
                    Tracer.Verbose(() => "resetting");
                    Reset();
                }
            }
        }

        public PagedQueryable(IOrderedQueryable<T> query, int pageSize, bool autoLoad = true)
        {
            using (new ConstructorTracer<PagedQueryable<T>>())
            {
                if (query == null) throw new ArgumentNullException("query");
                if (pageSize < 0)
                    throw new ArgumentOutOfRangeException("pageSize", "pageSize cannot be less than zero");

                Tracer.Verbose(() => "Adding event handler to SelectedItems.CollectionChanged");
                _selectedItems.CollectionChanged += SelectedItemsCollectionChanged;


                Tracer.Verbose(() => "setting to freezed");
                _isFreezed = true;

                _query = query;
                _pageSize = pageSize;

                Tracer.Verbose(() => "setting to nonfreezed");
                _isFreezed = false;

                Tracer.Verbose(() => "checking for autoload");
                if (autoLoad)
                {
                    Tracer.Verbose(() => "resetting");
                    Reset();
                }
            }
        }

        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (_pageSize != value)
                {
                    Tracer.Verbose(() => "setting pagesize");
                    _pageSize = value;

                    Tracer.Verbose(() => "resetting");
                    Reset();
                    OnPropertyChanged("PageSize");
                }
            }
        }

        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (CurrentPage != value)
                {
                    Tracer.Verbose(() => "setting currentpage");
                    _currentPage = value;

                    Tracer.Verbose(() => "reloading");
                    Reload();
                    OnPropertyChanged("CurrentPage");
                }
            }
        }

        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public int PageCount
        {
            get { return _pageCount; }
            private set
            {
                if (_pageCount != value)
                {
                    _pageCount = value;
                    OnPropertyChanged("PageCount");
                }
            }
        }

#if DEBUG
        [Display(AutoGenerateField = false)]
        public bool IsBreakingOnQuery { get; set; }
#endif

        public int QueryCount
        {
            get { return _queryCount; }
            private set
            {
                if (_queryCount != value)
                {
                    _queryCount = value;
                    OnPropertyChanged("QueryCount");
                }
            }
        }

        public Expression<Func<T, bool>> Filter
        {
            get { return _filter; }
            set
            {
                if (_filter != value)
                {
                    Tracer.Verbose(() => "setting filter");

                    _filter = value;

                    Tracer.Verbose(() => "reloading");
                    Reload();

                    OnPropertyChanged("Filter");
                }
            }
        }

        [UIHint("DataGrid")]
        public TweakedObservableCollection<T> Items
        {
            get { return _items; }
        }

        public TweakedObservableCollection<T> SelectedItems
        {
            get { return _selectedItems; }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IPagedQueryable Members

        [Display(AutoGenerateField = false)]
        public bool CanMoveFirst
        {
            get { return CurrentPage > 1; }
        }

        [Display(AutoGenerateField = false)]
        public bool CanMovePrevious
        {
            get { return CurrentPage > 1; }
        }

        [Display(AutoGenerateField = false)]
        public bool CanMoveNext
        {
            get { return CurrentPage < PageCount; }
        }

        [Display(AutoGenerateField = false)]
        public bool CanMoveLast
        {
            get { return CurrentPage < PageCount; }
        }

        [Display(AutoGenerateField = false)]
        public string SortColumn
        {
            get { return _sortColumn; }
            set
            {
                if (_sortColumn != value)
                {
                    _sortColumn = value;
                    OnPropertyChanged("SortColumn");
                }
            }
        }

        [Display(AutoGenerateField = false)]
        public SortDirection SortDirection
        {
            get { return _sortDirection; }
            set
            {
                if (_sortDirection != value)
                {
                    _sortDirection = value;
                    OnPropertyChanged("SortDirection");
                }
            }
        }

        [Display(AutoGenerateField = false)]
        public void BeginInit()
        {
            using (new MethodTracer<PagedQueryable<T>>("BeginInit"))
            {
                Tracer.Verbose(() => "setting PagedQueryable to freezed");
                _isFreezed = true;
            }
        }

        [Display(AutoGenerateField = false)]
        public void EndInit()
        {
            using (new MethodTracer<PagedQueryable<T>>("EndInit"))
            {
                if (!_isFreezed)
                    throw new InvalidOperationException();


                Tracer.Verbose(() => "setting PagedQueryable to non freezed");
                _isFreezed = false;

                Tracer.Verbose(() => "reloading");
                Reload();
            }
        }

        [Display(AutoGenerateField = false)]
        public void ChangeSelection(IList removedItems, IList addedItems)
        {
            using (new MethodTracer<PagedQueryable<T>>("ChangeSelection"))
            {
                SelectedItems.ChangingItems(removedItems.OfType<T>(), addedItems.OfType<T>());
            }
        }

        [Display(AutoGenerateField = false, ResourceType = typeof(Resources))]
        public void MoveFirst()
        {
            using (new MethodTracer<PagedQueryable<T>>("MoveFirst"))
            {
                Tracer.Info(() => "moving to first page");
                CurrentPage = 1;
            }
        }
        [Display(AutoGenerateField = false, ResourceType = typeof(Resources))]
        public void MovePrevious()
        {
            using (new MethodTracer<PagedQueryable<T>>("MovePrevious"))
            {
                Tracer.Info(() => "move previous page");
                if (CurrentPage > 1)
                {
                    Tracer.Verbose(() => "moving to previous page");
                    CurrentPage = CurrentPage - 1;
                }
            }
        }
        [Display(AutoGenerateField = false, ResourceType = typeof(Resources))]
        public void MoveNext()
        {
            using (new MethodTracer<PagedQueryable<T>>("MoveNext"))
            {
                Tracer.Info(() => "move next page");

                if (CurrentPage < PageCount)
                {
                    Tracer.Verbose(() => "moving to next page");
                    CurrentPage = CurrentPage + 1;
                }
            }
        }
        [Display(AutoGenerateField = false, ResourceType = typeof(Resources))]
        public void MoveLast()
        {
            using (new MethodTracer<PagedQueryable<T>>("MoveLast"))
            {
                Tracer.Info(() => "move last page");

                CurrentPage = PageCount;
            }
        }

        #endregion

        public event EventHandler SelectedItemsChanged;

        protected virtual void SelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SelectedItemsChanged != null)
                SelectedItemsChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Reset the PagedQueryable
        /// </summary>
        [Display(AutoGenerateField = false, ResourceType = typeof(Resources))]
        public void Reset()
        {
            using (new MethodTracer<PagedQueryable<T>>("Reset"))
            {
                Tracer.Info(() => "Reseting PagedQueryable");

                if (_isFreezed)
                {
                    Tracer.Verbose(() => "PagedQueryable is freezed");
                    return;
                }


                Tracer.Verbose(() => "set PagedQueryable freezed");

                Tracer.Verbose(() => "set currentPage to 1");
                _currentPage = 1;

                Tracer.Verbose(() => "raising CurrentPage property changed event");
                OnPropertyChanged("CurrentPage");


                Reload();
            }
        }

        /// <summary>
        /// Reloads the PagedQueryable
        /// </summary>
        [Display(AutoGenerateField = false, ResourceType = typeof(Resources))]
        public void Reload()
        {
#if DEBUG

            if (IsBreakingOnQuery)
            {
                //breaking on query
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
                else
                    System.Diagnostics.Debugger.Launch();

            }
#endif
            using (new MethodTracer<PagedQueryable<T>>("Reload"))
            {
                if (_isFreezed)
                {
                    Tracer.Verbose(() => "PagedQueryable is freezed");
                    return;
                }


                Tracer.Verbose(() => "clear Items");
                Items.Clear();


                int totalPages;
                int queryCount;

                Tracer.Verbose(() => "getting paged items");
                List<T> items = _query.ApplyFilter(Filter).ApplyOrder(SortColumn, SortDirection).ApplyPaging(
                    CurrentPage, PageSize, out totalPages, out queryCount).ToList();
                Items.AddRange(items);

                Tracer.Verbose(() => "setting PageCount");
                PageCount = totalPages;

                Tracer.Verbose(() => "maximize current page");
                CurrentPage = Math.Min(CurrentPage, PageCount);

                Tracer.Verbose(() => "setting QueryCount");
                QueryCount = queryCount;

                Tracer.Verbose(() => "raising navigation properties changed event");
                OnPropertyChanged("CanMoveFirst");
                OnPropertyChanged("CanMoveLast");
                OnPropertyChanged("CanMoveNext");
                OnPropertyChanged("CanMovePrevious");

                OnReloaded();
            }
        }

        public event EventHandler Reloaded;

        protected virtual void OnReloaded()
        {
            if (Reloaded != null)
                Reloaded(this, EventArgs.Empty);
        }

        [Display(AutoGenerateField = false)]
        public IOrderedQueryable<T> GetFilteredQuery()
        {
            using (new MethodTracer<PagedQueryable<T>>("GetFilteredQuery"))
            {
                return _query.ApplyFilter(Filter).ApplyOrder(SortColumn, SortDirection);
            }
        }

        /// <summary>
        /// returns the original query this PagedQueryable was created with
        /// </summary>
        /// <returns></returns>
        [Display(AutoGenerateField = false)]
        public IOrderedQueryable<T> GetQuery()
        {
            using (new MethodTracer<PagedQueryable<T>>("GetQuery"))
            {
                Tracer.Verbose(() => "getting query this PagedQueryable was created with");
                return _query;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (!_isFreezed && PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}