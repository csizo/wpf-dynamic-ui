using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Csizmazia.Collections;
using Csizmazia.Discovering;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.PropertyControlProviders;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.CustomControls
{
    public class DynamicPagerControl : Control
    {
        private ContentControl Reload;
        private TextBlock currentPage;
        private ContentControl dynamicGrid;
        private ContentControl moveFirst;
        private ContentControl moveLast;
        private ContentControl moveNext;
        private ContentControl movePrevious;
        private TextBlock pageCount;
        private ContentControl pageSize;

        static DynamicPagerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicPagerControl),
                                                     new FrameworkPropertyMetadata(typeof(DynamicPagerControl)));
        }

        public DynamicPagerControl()
        {
            DataContextChanged += DynamicPagerControlDataContextChanged;
        }

        internal Binding SelectedItemBinding { get; set; }

        private void DynamicPagerControlDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //on datacontext has been changed

            //building control
            BuildControl();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            dynamicGrid = GetTemplateChild("PART_DynamicGrid") as ContentControl;
            moveFirst = GetTemplateChild("PART_MoveFirst") as ContentControl;
            moveLast = GetTemplateChild("PART_MoveLast") as ContentControl;
            moveNext = GetTemplateChild("PART_MoveNext") as ContentControl;
            movePrevious = GetTemplateChild("PART_MovePrevious") as ContentControl;
            Reload = GetTemplateChild("PART_Reload") as ContentControl;

            pageSize = GetTemplateChild("PART_PageSize") as ContentControl;

            currentPage = GetTemplateChild("PART_CurrentPage") as TextBlock;
            pageCount = GetTemplateChild("PART_PageCount") as TextBlock;

            BuildControl();
        }

        private void BuildControl()
        {
            if (DataContext == null)
                return;

            BuildDynamicGrid(dynamicGrid);

            BuildMoveFirst(moveFirst);
            BuildMoveLast(moveLast);
            BuildMoveNext(moveNext);
            BuildMovePrevious(movePrevious);
            BuildReload(Reload);

            BuildPageSize(pageSize);

            BuildCurrentPage(currentPage);
            BuildPageCount(pageCount);
        }

        private void BuildMoveFirst(ContentControl first)
        {
            if (first == null)
                return;

            MethodInfo moveFirstMethod = DataContext.GetInstanceMethods().FirstOrDefault(pi => pi.Name == "MoveFirst");
            Control buttonControl = moveFirstMethod.GetControl(DataContext);

            first.SetValue(ContentControl.ContentProperty, buttonControl);


        }

        private void BuildMoveLast(ContentControl last)
        {
            if (last == null)
                return;

            MethodInfo moveLastMethod = DataContext.GetInstanceMethods().FirstOrDefault(pi => pi.Name == "MoveLast");
            Control buttonControl = moveLastMethod.GetControl(DataContext);

            last.SetValue(ContentControl.ContentProperty, buttonControl);
        }

        private void BuildMoveNext(ContentControl next)
        {
            if (next == null)
                return;

            MethodInfo moveNextMethod = DataContext.GetInstanceMethods().FirstOrDefault(pi => pi.Name == "MoveNext");
            Control buttonControl = moveNextMethod.GetControl(DataContext);

            next.SetValue(ContentControl.ContentProperty, buttonControl);
        }

        private void BuildMovePrevious(ContentControl previous)
        {
            if (previous == null)
                return;

            MethodInfo movePreviousMethod =
                DataContext.GetInstanceMethods().FirstOrDefault(pi => pi.Name == "MovePrevious");
            Control buttonControl = movePreviousMethod.GetControl(DataContext);

            previous.SetValue(ContentControl.ContentProperty, buttonControl);
        }


        private void BuildPageSize(ContentControl size)
        {
            if (size == null)
                return;

            PropertyInfo pageSizeProperty =
                DataContext.GetInstanceProperties().FirstOrDefault(pi => pi.Name == "PageSize");
            Control control = pageSizeProperty.GetControl(DataContext);

            size.SetValue(ContentControl.ContentProperty, control);
        }

        private void BuildPageCount(TextBlock count)
        {
            if (count == null)
                return;

            PropertyInfo pageCountProperty =
                DataContext.GetInstanceProperties().FirstOrDefault(pi => pi.Name == "PageCount");

            count.SetBinding(TextBlock.TextProperty, pageCountProperty.GetBinding());
        }

        private void BuildCurrentPage(TextBlock page)
        {
            if (page == null)
                return;

            PropertyInfo currentPageProperty =
                DataContext.GetInstanceProperties().FirstOrDefault(pi => pi.Name == "CurrentPage");

            page.SetBinding(TextBlock.TextProperty, currentPageProperty.GetBinding());
        }

        private void BuildReload(ContentControl reload)
        {
            if (reload == null)
                return;

            MethodInfo reloadMethod = DataContext.GetInstanceMethods().FirstOrDefault(pi => pi.Name == "Reload");
            Control buttonControl = reloadMethod.GetControl(DataContext);

            reload.SetValue(ContentControl.ContentProperty, buttonControl);
        }

        private void BuildDynamicGrid(ContentControl grid)
        {
            if (grid == null)
                return;


            PropertyInfo itemsProperty = DataContext.GetInstanceProperties().FirstOrDefault(pi => pi.Name == "Items");
            DataGrid gridControl = ItemsControlProvider.ProvideGridControl(itemsProperty, DataContext);

            if (SelectedItemBinding != null)
                gridControl.SetBinding(Selector.SelectedItemProperty, SelectedItemBinding);


            //set datagrid queryable sorting
            gridControl.Sorting += (o, e) =>
                                       {
                                           var dc = DataContext as IPagedQueryable;
                                           if (dc == null)
                                               return;

                                           if (e.Column.SortDirection.HasValue == false ||
                                               e.Column.SortDirection.Value == ListSortDirection.Descending)
                                           {
                                               dc.BeginInit();
                                               dc.SortColumn = e.Column.SortMemberPath;
                                               dc.SortDirection = (SortDirection)ListSortDirection.Ascending;
                                               dc.EndInit();
                                               e.Column.SortDirection = ListSortDirection.Ascending;
                                           }
                                           else
                                           {
                                               dc.BeginInit();
                                               dc.SortColumn = e.Column.SortMemberPath;
                                               dc.SortDirection = (SortDirection)ListSortDirection.Descending;
                                               dc.EndInit();
                                               e.Column.SortDirection = ListSortDirection.Descending;
                                           }
                                           e.Handled = true;
                                       };


            gridControl.SelectionChanged += (o, e) =>
                                                {
                                                    var gc = o as DataGrid;
                                                    if (gc == null)
                                                        return;

                                                    var dc = gc.DataContext as IPagedQueryable;
                                                    if (dc == null)
                                                        return;

                                                    dc.ChangeSelection(e.RemovedItems, e.AddedItems);
                                                };

            gridControl.MouseDown += (o, e) =>
                                         {
                                             var gc = o as DataGrid;
                                             if (gc == null)
                                                 return;

                                             var dc = gc.DataContext as IPagedQueryable;
                                             if (dc == null)
                                                 return;

                                             if (e.ChangedButton == MouseButton.XButton1)
                                             {
                                                 if (dc.CanMovePrevious)
                                                     dc.MovePrevious();
                                             }
                                             if (e.ChangedButton == MouseButton.XButton2)
                                             {
                                                 if (dc.CanMoveNext)
                                                     dc.MoveNext();
                                             }
                                         };

            gridControl.Unloaded += (o, e) =>
                                        {
                                            var control = o as DataGrid;
                                            if (control == null)
                                                return;

                                            BindingOperations.ClearAllBindings(control);
                                        };

            grid.SetValue(ContentControl.ContentProperty, gridControl);
        }
    }
}