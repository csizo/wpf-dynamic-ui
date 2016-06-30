using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Csizmazia.Discovering;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.CustomControls;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ReflectiveWpfTools;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ControlProviders.PropertyControlProviders
{
    public class ItemsControlProvider : InheritedPropertyControlProvider
    {
        public override Control ProvideControl(PropertyInfo propertyInfo, object viewModel)
        {
            var uiHintAttribute = propertyInfo.GetAttribute<UIHintAttribute>();

            Control control;
            if (uiHintAttribute == null)
                uiHintAttribute = new UIHintAttribute("");

            switch (uiHintAttribute.UIHint)
            {
                case UIHints.DisplayDataGrid:
                    control = ProvideGridControl(propertyInfo, viewModel);
                    break;
                case UIHints.DisplayChart:
                    control = ProvideChartControl(propertyInfo, viewModel, uiHintAttribute);
                    break;
                case UIHints.DisplayMap:
                    control = ProvideGMapControl(propertyInfo, viewModel);
                    break;
                default:
                    control = ProvideComboBoxControl(propertyInfo, viewModel, uiHintAttribute);
                    break;
            }


            string selectedItemBindingPath = null;
            if (uiHintAttribute.ControlParameters.ContainsKey(UIHints.ItemsControlParameters.SelectedItemBindingPath))
                selectedItemBindingPath = (string)uiHintAttribute.ControlParameters[UIHints.ItemsControlParameters.SelectedItemBindingPath];

            if (selectedItemBindingPath != null)
            {
                //setting selected item binding to selection property
                Binding selectedItemBinding = new Binding(selectedItemBindingPath);
                control.SetBinding(Selector.SelectedItemProperty, selectedItemBinding);
            }
            else
                if (propertyInfo.HasPropertyForSelectedItem())//check for selected item
                {
                    PropertyInfo property = propertyInfo.GetPropertyForSelectedItem();

                    //setting selected item binding to selection property
                    Binding selectedItemBinding = property.GetBinding();
                    control.SetBinding(Selector.SelectedItemProperty, selectedItemBinding);
                }


            return control;
        }

        internal static ComboBox ProvideComboBoxControl(PropertyInfo propertyInfo, object viewModel, UIHintAttribute uiHintAttribute)
        {
            Binding binding = propertyInfo.GetBinding();


            string displayMemberPath = null;
            if (uiHintAttribute.ControlParameters.ContainsKey(UIHints.ComboBoxControlParameters.DisplayMemberPath))
                displayMemberPath = (string)uiHintAttribute.ControlParameters[UIHints.ComboBoxControlParameters.DisplayMemberPath];

            string selectedItemBindingPath = null;
            if (uiHintAttribute.ControlParameters.ContainsKey(UIHints.ItemsControlParameters.SelectedItemBindingPath))
                selectedItemBindingPath = (string)uiHintAttribute.ControlParameters[UIHints.ItemsControlParameters.SelectedItemBindingPath];

            if (displayMemberPath == null)
            {
                //try find DisplayChart Value Property
                if (viewModel.GetInstanceProperties().Any(p => p.Name == "Name"))
                    displayMemberPath = "Name";
                else if (viewModel.GetInstanceProperties().Any(p => p.Name == "Description"))
                    displayMemberPath = "Description";
            }


            var control = new ComboBox();
            control.SetBinding(ItemsControl.ItemsSourceProperty, binding);

            control.SetValue(ItemsControl.DisplayMemberPathProperty, displayMemberPath);


            control.IsEnabled = selectedItemBindingPath != null || propertyInfo.HasPropertyForSelectedItem();
            //control.IsEnabled = propertyInfo.IsControlEnabled();


            return control;
        }

        internal static DataGrid ProvideGridControl(PropertyInfo propertyInfo, object viewModel)
        {
            Binding binding = propertyInfo.GetBinding();


            var control = new DataGrid();
            control.SetBinding(ItemsControl.ItemsSourceProperty, binding);

            //building DisplayDataGrid columns
            foreach (
                PropertyInfo instanceProperty in
                    propertyInfo.PropertyType.GetGenericArguments().First().GetPropertiesForUserInterface())
            {
                Binding columnBinding = instanceProperty.GetBinding();
                if (instanceProperty.PropertyType == typeof(string))
                    columnBinding.Converter = new StringToShortStringConverter();

                if (instanceProperty.PropertyType != typeof(string) && instanceProperty.PropertyType.IsIEnumerable())
                {
                    continue;
                    ////support for icollection members

                }
                if (instanceProperty.PropertyType != typeof(string) && instanceProperty.PropertyType.IsClass)
                {
                    var bindingPath = string.Format("{0}.{1}", instanceProperty.Name,
                                                    instanceProperty.GetDisplayBindingPath());
                    columnBinding = new Binding(bindingPath);
                    //creating column
                    var column = new DataGridTextColumn
                                     {
                                         Header = instanceProperty.GetDisplayShortName(),
                                         Binding = columnBinding,
                                     };
                    control.Columns.Add(column);
                }
                else
                {
                    //creating column
                    var column = new DataGridTextColumn
                                     {
                                         Header = instanceProperty.GetDisplayShortName(),
                                         Binding = columnBinding,
                                     };
                    control.Columns.Add(column);
                }
            }

            control.SetValue(DataGrid.AutoGenerateColumnsProperty, false);

            control.IsReadOnly = true;

            return control;
        }

        internal static DynamicGMapControl ProvideGMapControl(PropertyInfo propertyInfo, object viewModel)
        {
            var control = new DynamicGMapControl();

            control.SetBinding(FrameworkElement.DataContextProperty, propertyInfo.GetBinding());

            return control;
        }

        #region Chart

        internal static Chart ProvideChartControl(PropertyInfo propertyInfo, object viewModel,
                                                  UIHintAttribute uiHintAttribute)
        {
            var control = new Chart();

            //check if chart is one series or multi series chart...
            //build seriescontrol for each dataseries


            var independentValuePath =
                (string)uiHintAttribute.ControlParameters[UIHints.ChartControlParameters.ChartCategoryProperty];
            if (independentValuePath == null)
            {
                //try find Category Property
            }


            var dependentValuePath =
                (string)uiHintAttribute.ControlParameters[UIHints.ChartControlParameters.ChartValueProperty];
            if (dependentValuePath == null)
            {
                //try find DisplayChart Value Property
            }


            control.Series.Add(ProvideSeries(propertyInfo, uiHintAttribute, dependentValuePath, independentValuePath));


            control.SetValue(Chart.TitleProperty, propertyInfo.GetDisplayName());

            control.Unloaded += (o, e) =>
                                    {
                                        var chart = o as Chart;
                                        if (chart == null)
                                            return;

                                        BindingOperations.ClearAllBindings(chart);

                                        chart.Series.Clear();
                                    };

            return control;
        }

        private static ISeries ProvideSeries(PropertyInfo propertyInfo, UIHintAttribute uiHintAttribute,
                                             string dependentValuePath, string independentValuePath)
        {
            string chartType = string.Empty;
            if (uiHintAttribute.ControlParameters.ContainsKey(UIHints.ChartControlParameters.ChartType))
                chartType = (string)uiHintAttribute.ControlParameters[UIHints.ChartControlParameters.ChartType];


            switch (chartType)
            {
                case UIHints.ChartControlParameters.ChartTypeArea:
                    return ProvideAreaSeries(propertyInfo, dependentValuePath, independentValuePath);


                case UIHints.ChartControlParameters.ChartTypeBar:
                    return ProvideBarSeries(propertyInfo, dependentValuePath, independentValuePath);

                case UIHints.ChartControlParameters.ChartTypeBubble:
                    return ProvideBubbleSeries(propertyInfo, dependentValuePath, independentValuePath);

                case UIHints.ChartControlParameters.ChartTypeLine:
                    return ProvideLineSeries(propertyInfo, dependentValuePath, independentValuePath);

                case UIHints.ChartControlParameters.ChartTypePie:
                    return ProvidePieSeries(propertyInfo, dependentValuePath, independentValuePath);

                case UIHints.ChartControlParameters.ChartTypeScatter:
                    return ProvideScatterSeries(propertyInfo, dependentValuePath, independentValuePath);
                default:
                    return ProvideAreaSeries(propertyInfo, dependentValuePath, independentValuePath);
            }
        }

        private static AreaSeries ProvideAreaSeries(PropertyInfo propertyInfo, string dependentValuePath,
                                                    string independentValuePath)
        {
            var series = new AreaSeries();
            series.SetBinding(DataPointSeries.ItemsSourceProperty, propertyInfo.GetBinding());
            series.IndependentValuePath = independentValuePath;
            series.DependentValuePath = dependentValuePath;

            series.Unloaded += (o, e) =>
                                   {
                                       var control = o as AreaSeries;
                                       if (control == null)
                                           return;

                                       BindingOperations.ClearAllBindings(control);
                                   };
            return series;
        }

        private static BarSeries ProvideBarSeries(PropertyInfo propertyInfo, string dependentValuePath,
                                                  string independentValuePath)
        {
            var series = new BarSeries();
            series.SetBinding(DataPointSeries.ItemsSourceProperty, propertyInfo.GetBinding());
            series.IndependentValuePath = independentValuePath;
            series.DependentValuePath = dependentValuePath;

            series.Unloaded += (o, e) =>
                                   {
                                       var control = o as BarSeries;
                                       if (control == null)
                                           return;

                                       BindingOperations.ClearAllBindings(control);
                                   };

            return series;
        }

        private static BubbleSeries ProvideBubbleSeries(PropertyInfo propertyInfo, string dependentValuePath,
                                                        string independentValuePath)
        {
            var series = new BubbleSeries();
            series.SetBinding(DataPointSeries.ItemsSourceProperty, propertyInfo.GetBinding());
            series.IndependentValuePath = independentValuePath;
            series.DependentValuePath = dependentValuePath;

            series.Unloaded += (o, e) =>
                                   {
                                       var control = o as BubbleSeries;
                                       if (control == null)
                                           return;

                                       BindingOperations.ClearAllBindings(control);
                                   };
            return series;
        }


        private static LineSeries ProvideLineSeries(PropertyInfo propertyInfo, string dependentValuePath,
                                                    string independentValuePath)
        {
            var series = new LineSeries();
            series.SetBinding(DataPointSeries.ItemsSourceProperty, propertyInfo.GetBinding());
            series.IndependentValuePath = independentValuePath;
            series.DependentValuePath = dependentValuePath;

            series.Unloaded += (o, e) =>
                                   {
                                       var control = o as LineSeries;
                                       if (control == null)
                                           return;

                                       BindingOperations.ClearAllBindings(control);
                                   };

            return series;
        }

        private static PieSeries ProvidePieSeries(PropertyInfo propertyInfo, string dependentValuePath,
                                                  string independentValuePath)
        {
            var series = new PieSeries();
            series.SetBinding(DataPointSeries.ItemsSourceProperty, propertyInfo.GetBinding());
            series.IndependentValuePath = independentValuePath;
            series.DependentValuePath = dependentValuePath;

            series.Unloaded += (o, e) =>
                                   {
                                       var control = o as PieSeries;
                                       if (control == null)
                                           return;

                                       BindingOperations.ClearAllBindings(control);
                                   };

            return series;
        }

        private static ScatterSeries ProvideScatterSeries(PropertyInfo propertyInfo, string dependentValuePath,
                                                          string independentValuePath)
        {
            var series = new ScatterSeries();
            series.SetBinding(DataPointSeries.ItemsSourceProperty, propertyInfo.GetBinding());
            series.IndependentValuePath = independentValuePath;
            series.DependentValuePath = dependentValuePath;

            series.Unloaded += (o, e) =>
                                   {
                                       var control = o as ScatterSeries;
                                       if (control == null)
                                           return;

                                       BindingOperations.ClearAllBindings(control);
                                   };

            return series;
        }

        #endregion
    }
}