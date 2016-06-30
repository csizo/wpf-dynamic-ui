using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Csizmazia.Discovering;
using Csizmazia.Tracing;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Csizmazia.WpfDynamicUI.MefComposition;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ReflectiveWpfTools;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Csizmazia.WpfDynamicUI.WpfDynamicControl.Detail"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Csizmazia.WpfDynamicUI.WpfDynamicControl.Detail;assembly=Csizmazia.WpfDynamicUI.WpfDynamicControl.Detail"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:DynamicModelControl/>
    ///
    /// </summary>
    [TemplatePart(Name = "PART_DetailsPanel", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_ContextMenuPanel", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_ApplicationMenuPanel", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_ModuleMenuPanel", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_FilterPanel", Type = typeof(StackPanel))]
    public class DynamicModelControl : Control
    {
        private static readonly Tracer<DynamicModelControl> Tracer = Tracer<DynamicModelControl>.Instance;

        public static readonly DependencyProperty DisplayMenuProperty =
            DependencyProperty.Register("DisplayMenu", typeof(bool), typeof(DynamicModelControl),
                                        new PropertyMetadata(false, DisplayMenuChanged));

        public static readonly DependencyProperty DisplayContextActionPanelProperty =
            DependencyProperty.Register("DisplayContextActionPanel", typeof(bool), typeof(DynamicModelControl),
                                        new PropertyMetadata(true, DisplayContextMenuChanged));



        public static readonly DependencyProperty ShowUsageHelpProperty =
            DependencyProperty.Register("ShowUsageHelp", typeof(bool), typeof(DynamicModelControl), new PropertyMetadata(true));

        public bool ShowUsageHelp
        {
            get { return (bool)GetValue(ShowUsageHelpProperty); }
            set { SetValue(ShowUsageHelpProperty, value); }
        }
        public static readonly DependencyProperty DisplayFilterProperty
            = DependencyProperty.Register("DisplayFilter",
                                                                                                      typeof(bool),
                                                                                                      typeof(DynamicModelControl),
                                                                                                      new PropertyMetadata(true));

        private static readonly DependencyPropertyKey RenderTimePropertyKey =
            DependencyProperty.RegisterReadOnly("RenderTime", typeof(TimeSpan), typeof(DynamicModelControl),
                                                new PropertyMetadata(TimeSpan.Zero));

        public static readonly DependencyProperty RenderTimeProperty = RenderTimePropertyKey.DependencyProperty;
        private readonly List<Lazy<IModel>> _mefExportedModels = new List<Lazy<IModel>>();


        private Panel _applicationMenuPanel;
        private Panel _contextMenuPanel;
        private Grid _detailsPanel;
        private Panel _filterPanel;
        private Panel _moduleMenuPanel;

        static DynamicModelControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicModelControl),
                                                     new FrameworkPropertyMetadata(typeof(DynamicModelControl)));
        }

        public DynamicModelControl()
        {
            using (new ConstructorTracer<DynamicModelControl>())
            {
                //registering for Datacontextchanged event
                DataContextChanged += DynamicModelControlDataContextChanged;

                Unloaded += DynamicModelControlUnloaded;

            }
        }

        public bool DisplayMenu
        {
            get { return (bool)GetValue(DisplayMenuProperty); }
            set { SetValue(DisplayMenuProperty, value); }
        }

        public bool DisplayContextActionPanel
        {
            get { return (bool)GetValue(DisplayContextActionPanelProperty); }
            set { SetValue(DisplayContextActionPanelProperty, value); }
        }

        public bool DisplayFilter
        {
            get { return (bool)GetValue(DisplayFilterProperty); }
            set { SetValue(DisplayFilterProperty, value); }
        }

        public TimeSpan RenderTime
        {
            get { return (TimeSpan)GetValue(RenderTimeProperty); }
            private set { SetValue(RenderTimePropertyKey, value); }
        }

        private IEnumerable<Lazy<IModel>> MefExportedModels
        {
            get
            {
                if (_mefExportedModels.Count == 0)
                {
                    _mefExportedModels.AddRange(ImportModels());
                }
                return _mefExportedModels;
            }
        }

        private IEnumerable<Lazy<IModel>> ImportModels()
        {
            //get the current active exported models
            return Container.Instance.GetExports<IModel>();
        }

        private void DynamicModelControlUnloaded(object sender, RoutedEventArgs e)
        {
            ReleaseExports();
        }

        private void ReleaseExports()
        {
            //release all previously exported models
            Container.Instance.ReleaseExports(_mefExportedModels);
            _mefExportedModels.Clear();
        }

        private static void DisplayMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DynamicModelControl)d;
            control.BuildControl();
        }

        private static void DisplayContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DynamicModelControl)d;
            control.BuildControl();
        }


        public override void OnApplyTemplate()
        {
            //onApplyingTemplate
            base.OnApplyTemplate();

            //getting Panels
            _detailsPanel = GetTemplateChild("PART_DetailsPanel") as Grid;
            _contextMenuPanel = GetTemplateChild("PART_ContextMenuPanel") as Panel;
            _applicationMenuPanel = GetTemplateChild("PART_ApplicationMenuPanel") as Panel;
            _moduleMenuPanel = GetTemplateChild("PART_ModuleMenuPanel") as Panel;
            _filterPanel = GetTemplateChild("PART_FilterPanel") as Panel;

            //building control
            BuildControl();
        }

        private void DynamicModelControlDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            using (new MethodTracer<DynamicModelControl>("DynamicModelControlDataContextChanged"))
            {
                //on datacontext has been changed

                //building control
                BuildControl();

                ReleaseExports();

            }
        }


        private void BuildControl()
        {

            var sw = new Stopwatch();
            try
            {
                sw.Start();

                if (DataContext == null)
                {
                    return;
                }


                //checking if displaymenu is enabled
                if (DisplayMenu)
                {
                    //build application menu
                    BuildApplicationMenu(_applicationMenuPanel);

                    //build module menu
                    BuildModuleMenu(_moduleMenuPanel);
                }


                //fill contextual properties (details panel)
                BuildDetailsPanel(_detailsPanel);

                //checking if context actions panel is enabled
                if (DisplayContextActionPanel)
                {
                    //build contextual actions (context panel)
                    BuildContextActionPanel(_contextMenuPanel);
                }



                //building filter panel
                BuildFilterPanel(_filterPanel);


                //building context menu
                BuildControlContextMenu();
            }
            finally
            {
                sw.Stop();
                RenderTime = sw.Elapsed;
            }
        }

        private void BuildControlContextMenu()
        {
            if (DesignModeCheck.IsInDesignModeStatic)
            {
                return;
            }

#if DEBUG
            var contextMenu = new ContextMenu();
            var menuItem = new MenuItem { Header = "Export as Xaml" };
            menuItem.Click += (o, e) =>
                                  {

                                      var serialized = _detailsPanel.ToXaml();
                                      Clipboard.SetText(serialized);

                                      BusinessApplication.Instance.ShowPopup("Control Xaml definition is saved in Clipboard!");
                                  };
            contextMenu.Items.Add(menuItem);
            SetValue(ContextMenuProperty, contextMenu);
#endif
        }

        private void BuildApplicationMenu(Panel panel)
        {
            if (panel == null)
                return;

            panel.Children.Clear();

            if (DesignModeCheck.IsInDesignModeStatic)
            {
                //MEF in design time is disabled
                var label = new Label();
                label.SetValue(ContentControl.ContentProperty, "Design mode empty...");
                panel.Children.Add(label);
                return;
            }
            bool haveMenuItem = false;

            foreach (IModel model in MefExportedModels.Select(m => m.Value))
            {
                foreach (
                    MethodInfo staticMethod in
                        model.GetType().GetStaticMethodsForUserInterfaceAction(DataContext).Where(
                            mi => mi.GetMethodParameters().Length == 0).ToArray())
                {
                    panel.Children.Add(staticMethod.GetControl(DataContext));

                    haveMenuItem = true;
                }
            }

            if (!haveMenuItem)
            {
                var label = new Label();
                label.SetValue(ContentControl.ContentProperty, "No Menu available...");
                panel.Children.Add(label);
            }
        }

        private void BuildModuleMenu(Panel panel)
        {
            if (panel == null)
                return;

            panel.Children.Clear();

            if (DesignModeCheck.IsInDesignModeStatic)
            {
                //MEF in design time is disabled
                var label = new Label();
                label.SetValue(ContentControl.ContentProperty, "Design mode empty...");
                panel.Children.Add(label);
                return;
            }

            bool haveMenuItem = false;

            foreach (IModel model in MefExportedModels.Select(m => m.Value))
            {
                foreach (
                    MethodInfo staticMethod in
                        model.GetType().GetStaticMethodsForUserInterfaceAction(DataContext).Where(
                            mi => mi.GetMethodParameters().Length == 1).ToArray())
                {
                    panel.Children.Add(staticMethod.GetControl(DataContext));
                    haveMenuItem = true;
                }
            }

            if (!haveMenuItem)
            {
                var label = new Label();
                label.SetValue(ContentControl.ContentProperty, "No Menu available...");
                panel.Children.Add(label);
            }
        }

        private void BuildContextActionPanel(Panel panel)
        {
            if (panel == null)
                return;

            panel.Children.Clear();


            bool haveMenuItem = false;
            foreach (MethodInfo method in DataContext.GetType().GetMethodsForUserInterfaceAction())
            {
                panel.Children.Add(method.GetControl(DataContext));
                haveMenuItem = true;
            }

            if (!haveMenuItem)
            {
                var label = new Label();
                label.SetValue(ContentControl.ContentProperty, "No Menu available...");
                panel.Children.Add(label);
            }
        }

        private void BuildFilterPanel(Panel panel)
        {
            if (panel == null)
                return;

            panel.Children.Clear();

            bool foundFilter = false;
            foreach (PropertyInfo propertyInfo in DataContext.GetType().GetFilterPropertiesForUserInterface())
            {
                panel.Children.Add(propertyInfo.GetLabel());
                panel.Children.Add(propertyInfo.GetControl(DataContext));

                foundFilter = true;
            }

            //setting display filter
            DisplayFilter = foundFilter;
        }

        private void BuildDetailsPanel(Grid panel)
        {
            if (panel == null)
                return;

            //clearing panel items
            panel.Children.Clear();
            //clearing panel row definitions
            panel.RowDefinitions.Clear();

            //changed stackpanel to Grid
            foreach (PropertyInfo propertyInfo in DataContext.GetType().GetPropertiesForUserInterface())
            {
                //add label
                panel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
                var label = propertyInfo.GetLabel();
                Grid.SetRow(label, panel.RowDefinitions.Count - 1);
                panel.Children.Add(label);

                panel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
                var control = propertyInfo.GetControl(DataContext);
                Grid.SetRow(control, panel.RowDefinitions.Count - 1);
                panel.Children.Add(control);
            }

            //BuildDynamicXamlContextMenu(panel);
        }
    }
}