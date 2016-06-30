using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Csizmazia.Tracing;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Csizmazia.Discovering;
using Csizmazia.WpfDynamicUI.MefComposition;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ModelDiscovery;
using Csizmazia.WpfDynamicUI.WpfDynamicControl.ReflectiveWpfTools;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl
{
    /// <summary>
    /// Defines module menu display type
    /// </summary>
    public enum ModuleMenuType
    {
        /// <summary>
        /// Trivial Module menu displays all menu items in a stackpanel
        /// </summary>
        Trivial = 0,

        /// <summary>
        /// Grouped Module menu display all menu items in an Accrodion panel grouped by Category
        /// </summary>
        Accordion = 1
    }
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Csizmazia.WpfDynamicUI.DynamicModelControl"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Csizmazia.WpfDynamicUI.WpfDynamicControl;assembly=Csizmazia.WpfDynamicUI.DynamicModelControl"
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
    [TemplatePart(Name = "PART_ModuleMenuAccordionPanel", Type = typeof(Accordion))]
    [TemplatePart(Name = "PART_FilterPanel", Type = typeof(StackPanel))]
    public class DynamicModelControl : Control
    {
        public static readonly DependencyProperty ModuleMenuTypeProperty =
            DependencyProperty.Register("ModuleMenuType", typeof(ModuleMenuType), typeof(DynamicModelControl), new PropertyMetadata(ModuleMenuType.Trivial));

        public ModuleMenuType ModuleMenuType
        {
            get { return (ModuleMenuType)GetValue(ModuleMenuTypeProperty); }
            set { SetValue(ModuleMenuTypeProperty, value); }
        }
        private static readonly Tracer<DynamicModelControl> Tracer = Tracer<DynamicModelControl>.Instance;

        public static readonly DependencyProperty DisplayMenuProperty =
            DependencyProperty.Register("DisplayMenu", typeof(bool), typeof(DynamicModelControl),
                                        new PropertyMetadata(false, DisplayMenuChanged));

        public static readonly DependencyProperty DisplayContextActionPanelProperty =
            DependencyProperty.Register("DisplayContextActionPanel", typeof(bool), typeof(DynamicModelControl),
                                        new PropertyMetadata(true, DisplayContextMenuChanged));


        public static readonly DependencyProperty ShowUsageHelpProperty =
            DependencyProperty.Register("ShowUsageHelp", typeof(bool), typeof(DynamicModelControl),
                                        new PropertyMetadata(true));

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
        private Accordion _moduleMenuAccordionPanel;

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

        public bool ShowUsageHelp
        {
            get { return (bool)GetValue(ShowUsageHelpProperty); }
            set { SetValue(ShowUsageHelpProperty, value); }
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
            _moduleMenuAccordionPanel = GetTemplateChild("PART_ModuleMenuAccordionPanel") as Accordion;
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
                    BuildModuleMenu(_moduleMenuPanel, _moduleMenuAccordionPanel);
                }


                //fill contextual properties (details trivialPanel)
                BuildDetailsPanel(_detailsPanel);

                //checking if context actions trivialPanel is enabled
                if (DisplayContextActionPanel)
                {
                    //build contextual actions (context trivialPanel)
                    BuildContextActionPanel(_contextMenuPanel);
                }


                //building filter trivialPanel
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
                                      string serialized = _detailsPanel.ToXaml();
                                      Clipboard.SetText(serialized);

                                      BusinessApplication.Instance.ShowPopup(
                                          "Control Xaml definition is saved in Clipboard!");
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
                label.SetValue(ContentControl.ContentProperty, Properties.Resources.NoDesignMenuAvailable);
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
                label.SetValue(ContentControl.ContentProperty, Properties.Resources.NoMenuAvailable);
                panel.Children.Add(label);
            }
        }

        private void BuildModuleMenu(Panel trivialPanel, Accordion accordionPanel)
        {
            var controls = new object[] { trivialPanel, accordionPanel };

            if (controls.Any(c => c == null))
                return;

            //clear previous menu panels
            trivialPanel.Children.Clear();
            if (accordionPanel.Items.Count > 0)
                accordionPanel.Items.Clear();

            //check for design mode
            if (DesignModeCheck.IsInDesignModeStatic)
            {
                //MEF in design time is disabled
                var label = new Label();
                label.SetValue(ContentControl.ContentProperty, Properties.Resources.NoDesignMenuAvailable);
                trivialPanel.Children.Add(label);

                //disable accordion trivialPanel
                accordionPanel.SetValue(VisibilityProperty, Visibility.Collapsed);

                return;
            }


            //switch on ModuleMenuType
            switch (ModuleMenuType)
            {
                case ModuleMenuType.Trivial:
                    //call for Trivial menu
                    BuildModuleMenuTrivial(trivialPanel, accordionPanel);

                    break;
                case ModuleMenuType.Accordion:
                    //call for Accordion menu
                    BuildModuleMenuAccordion(trivialPanel, accordionPanel);


                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void BuildModuleMenuAccordion(Panel trivialPanel, Accordion accordionPanel)
        {
            bool haveAccordionMenuItem = false;

            var accordionMenuItems = new Dictionary<string, StackPanel>();

            //collect manuItems
            foreach (var model in MefExportedModels.Select(m => m.Value))
            {
                foreach (var staticMethod in model.GetType().GetStaticMethodsForUserInterfaceAction(DataContext)
                    .Where(mi => mi.GetMethodParameters().Length == 1).ToArray())
                {
                    var displayAttribute = staticMethod.GetAttribute(() => new DisplayAttribute { GroupName = Properties.Resources.ModuleMenuOther });
                    var groupName = displayAttribute.GetGroupName() ?? Properties.Resources.ModuleMenuOther;

                    var stackPanel = accordionMenuItems.TryGetValue(groupName, () => new StackPanel());

                    //add control to container
                    stackPanel.Children.Add(staticMethod.GetControl(DataContext));

                    haveAccordionMenuItem = true;
                }
            }


            if (accordionMenuItems.Keys.Count > 1)//check for keys count...
            {
                //build menuItems
                foreach (var accordionMenuItemKey in accordionMenuItems.Keys)
                {
                    //create scollviewer
                    var scrollViewer = new ScrollViewer
                                           {
                                               VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                               Content = accordionMenuItems[accordionMenuItemKey]
                                           };

                    //create accordion item
                    var accordionItem = new AccordionItem
                                            {
                                                Header = accordionMenuItemKey,
                                                Content = scrollViewer
                                            };

                    //add accordion item
                    accordionPanel.Items.Add(accordionItem);

                }
                if (!haveAccordionMenuItem)
                {
                    var label = new Label();
                    label.SetValue(ContentControl.ContentProperty, Properties.Resources.NoMenuAvailable);

                    var accordionItem = new AccordionItem { Header = "", Content = label };


                    accordionPanel.Items.Add(accordionItem);
                }

                //collapse trivialPanel
                trivialPanel.SetValue(VisibilityProperty, Visibility.Collapsed);
            }
            else//one key only
            {
                //add to trivial menu
                foreach (var accordionMenuItemKey in accordionMenuItems.Keys)
                {
                    trivialPanel.Children.Add(accordionMenuItems[accordionMenuItemKey]);

                }

                accordionPanel.SetValue(VisibilityProperty, Visibility.Collapsed);
            }


        }

        private void BuildModuleMenuTrivial(Panel trivialPanel, Accordion accordionPanel)
        {
            bool haveTrivialMenuItem = false;
            foreach (IModel model in MefExportedModels.Select(m => m.Value))
            {
                foreach (
                    MethodInfo staticMethod in
                        model.GetType().GetStaticMethodsForUserInterfaceAction(DataContext).Where(
                            mi => mi.GetMethodParameters().Length == 1).ToArray())
                {
                    trivialPanel.Children.Add(staticMethod.GetControl(DataContext));
                    haveTrivialMenuItem = true;
                }
            }

            if (!haveTrivialMenuItem)
            {
                var label = new Label();
                label.SetValue(ContentControl.ContentProperty, Properties.Resources.NoMenuAvailable);
                trivialPanel.Children.Add(label);
            }

            //collapse accordionPanel
            accordionPanel.SetValue(VisibilityProperty, Visibility.Collapsed);
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
                label.SetValue(ContentControl.ContentProperty, Properties.Resources.NoMenuAvailable);
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

            //clearing trivialPanel items
            panel.Children.Clear();
            //clearing trivialPanel row definitions
            panel.RowDefinitions.Clear();

            //changed stackpanel to Grid
            foreach (PropertyInfo propertyInfo in DataContext.GetType().GetPropertiesForUserInterface())
            {
                //add label
                panel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
                Control label = propertyInfo.GetLabel();
                Grid.SetRow(label, panel.RowDefinitions.Count - 1);
                panel.Children.Add(label);

                panel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
                Control control = propertyInfo.GetControl(DataContext);
                Grid.SetRow(control, panel.RowDefinitions.Count - 1);
                panel.Children.Add(control);
            }

            //BuildDynamicXamlContextMenu(trivialPanel);
        }
    }
}