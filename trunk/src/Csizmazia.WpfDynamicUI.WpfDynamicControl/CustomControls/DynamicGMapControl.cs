using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Csizmazia.Collections;
using Csizmazia.Discovering;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.CustomControls
{
    public class DynamicGMapControl : Control
    {
        public static readonly DependencyProperty MarkerLatitudePathProperty =
            DependencyProperty.Register("MarkerLatitudePath", typeof(string), typeof(DynamicGMapControl),
                                        new PropertyMetadata(default(string)));

        public static readonly DependencyProperty MarkerLongitudePathProperty =
            DependencyProperty.Register("MarkerLongitudePath", typeof(string), typeof(DynamicGMapControl),
                                        new PropertyMetadata(default(string)));

        public static readonly DependencyProperty MarkerToolTipPathProperty =
            DependencyProperty.Register("MarkerToolTipPath", typeof(string), typeof(DynamicGMapControl),
                                        new PropertyMetadata(default(string)));

        private PropertyInfo MarkerLatitudePropertyInfo;
        private PropertyInfo MarkerLongitudePropertyInfo;
        private PropertyInfo MarkerToolTipPropertyInfo;
        private TweakedObservableCollection<object> Markers = new TweakedObservableCollection<object>();

        private Button buttonZoomIn;
        private Button buttonZoomOut;
        private GMapControl gMapControl;
        private Slider sliderZoom;

        static DynamicGMapControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicGMapControl),
                                                     new FrameworkPropertyMetadata(typeof(DynamicGMapControl)));
        }


        public DynamicGMapControl()
        {
            DataContextChanged += DynamicGMapControl_DataContextChanged;
        }

        public string MarkerLatitudePath
        {
            get { return (string)GetValue(MarkerLatitudePathProperty); }
            set { SetValue(MarkerLatitudePathProperty, value); }
        }

        public string MarkerLongitudePath
        {
            get { return (string)GetValue(MarkerLongitudePathProperty); }
            set { SetValue(MarkerLongitudePathProperty, value); }
        }

        public string MarkerToolTipPath
        {
            get { return (string)GetValue(MarkerToolTipPathProperty); }
            set { SetValue(MarkerToolTipPathProperty, value); }
        }

        private void DynamicGMapControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                var notifyCollectionChanged = e.OldValue as INotifyCollectionChanged;
                if (notifyCollectionChanged != null)
                {
                    notifyCollectionChanged.CollectionChanged -= DataContextCollectionChanged;
                }
            }

            if (e.NewValue != null)
            {
                var notifyCollectionChanged = e.NewValue as INotifyCollectionChanged;
                if (notifyCollectionChanged != null)
                {
                    notifyCollectionChanged.CollectionChanged += DataContextCollectionChanged;
                }
            }

            ReflectMarkerPropertyInfo();

            BuildGMapControl(gMapControl);
        }

        private void ReflectMarkerPropertyInfo()
        {
            if (DataContext == null)
            {
                MarkerLatitudePropertyInfo = null;
                MarkerLongitudePropertyInfo = null;
                MarkerToolTipPropertyInfo = null;

                return;
            }

            if (DataContext is IEnumerable == false)
                return;

            Type itemType = DataContext.GetType().GetCollectionItemType();

            MarkerLatitudePropertyInfo = string.IsNullOrEmpty(MarkerLatitudePath)
                                             ? itemType.GetInstanceProperties().FirstOrDefault(
                                                 pi => pi.Name.StartsWith("Latitude"))
                                             : itemType.GetInstanceProperties().FirstOrDefault(
                                                 pi => pi.Name.StartsWith(MarkerLatitudePath));

            MarkerLongitudePropertyInfo = string.IsNullOrEmpty(MarkerLongitudePath)
                                              ? itemType.GetInstanceProperties().FirstOrDefault(
                                                  pi => pi.Name.StartsWith("Longitude"))
                                              : itemType.GetInstanceProperties().FirstOrDefault(
                                                  pi => pi.Name.StartsWith(MarkerLongitudePath));

            MarkerToolTipPropertyInfo = string.IsNullOrEmpty(MarkerToolTipPath)
                                            ? itemType.GetInstanceProperties().FirstOrDefault(
                                                pi => pi.Name.StartsWith("Description"))
                                            : itemType.GetInstanceProperties().FirstOrDefault(
                                                pi => pi.Name.StartsWith(MarkerToolTipPath));
        }

        private void InitializeMarkers(IEnumerable<object> removedItems, IEnumerable<object> addedItems)
        {
            if (gMapControl == null)
                return;

            List<GMapMarker> gMapMarkers = gMapControl.Markers.Where(m => removedItems.Contains(m.Tag)).ToList();
            foreach (GMapMarker gMapMarker in gMapMarkers)
            {
                gMapControl.Markers.Remove(gMapMarker);
            }

            InitializeMarkers(addedItems);
        }

        private void InitializeMarkers(IEnumerable<object> addedItems)
        {
            if (gMapControl == null)
                return;

            foreach (object addedItem in addedItems)
            {
                double lat = Convert.ToDouble(MarkerLatitudePropertyInfo.GetValue(addedItem, null));
                double lng = Convert.ToDouble(MarkerLongitudePropertyInfo.GetValue(addedItem, null));
                var title = (string)MarkerToolTipPropertyInfo.GetValue(addedItem, null);

                var point = new PointLatLng(lat, lng);
                var gmapMarker = new GMapMarker(point) { Tag = addedItem };

                var markerControl = new DynamicGMapMarkerControl(gmapMarker);
                markerControl.SetValue(DynamicGMapMarkerControl.TitleProperty, title);
                gmapMarker.Shape = markerControl;


                gMapControl.Markers.Add(gmapMarker);
            }


            gMapControl.ZoomAndCenterMarkers(null);
        }

        private void DataContextCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IEnumerable<object> removedItems = new object[] { };
            IEnumerable<object> addedItems = new object[] { };

            if (e.NewItems != null)
                addedItems = e.NewItems.OfType<object>();

            if (e.OldItems != null)
                removedItems = e.OldItems.OfType<object>();

            InitializeMarkers(removedItems, addedItems);
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            gMapControl = GetTemplateChild("PART_GMapControl") as GMapControl;
            buttonZoomIn = GetTemplateChild("PART_ButtonZoomIn") as Button;
            buttonZoomOut = GetTemplateChild("PART_ButtonZoomOut") as Button;
            sliderZoom = GetTemplateChild("PART_SliderZoom") as Slider;

            BuildControl();
        }

        private void BuildControl()
        {
            BuildGMapControl(gMapControl);

            BuildSliderZoom(sliderZoom);
            BuildButtonZoomOut(buttonZoomOut);
            BuildButtonZoomIn(buttonZoomIn);
        }

        private void BuildGMapControl(GMapControl gmapControl)
        {
            if (gmapControl == null)
                return;

            if (DataContext == null)
            {
                gmapControl.Markers.Clear();
                return;
            }

            gmapControl.MapProvider = OpenStreetMapProvider.Instance;
            gmapControl.DragButton = MouseButton.Left;

            var addedItems = DataContext.As<IEnumerable<object>>();
            InitializeMarkers(addedItems);

            gmapControl.ZoomAndCenterMarkers(null);

            //build context menu
            var contextMenu = new ContextMenu();
            var menuItem = new MenuItem { Header = "Maps" };

            foreach (GMapProvider gMapProvider in GMapProviders.List)
            {
                var childMenuItem = new MenuItem { Header = gMapProvider.Name, Tag = gMapProvider };
                childMenuItem.Click += (o, e) =>
                                           {
                                               var mi = o as MenuItem;
                                               if (mi == null)
                                                   return;

                                               //setting the MapProvider
                                               var gmapProvider = (GMapProvider)mi.Tag;
                                               gmapControl.MapProvider = gmapProvider;
                                               gmapControl.MinZoom = gmapProvider.MinZoom;
                                               gmapControl.MaxZoom = gmapProvider.MaxZoom.HasValue
                                                                         ? gmapProvider.MaxZoom.Value
                                                                         : 24;
                                           };
                menuItem.Items.Add(childMenuItem);
            }


            contextMenu.Items.Add(menuItem);

            SetValue(ContextMenuProperty, contextMenu);
        }

        private static void BuildSliderZoom(Slider slider)
        {
            if (slider == null)
                return;

            slider.SetValue(RangeBase.ValueProperty, 9.0);
        }

        private void BuildButtonZoomOut(Button button)
        {
            if (button == null)
                return;

            button.Click += (o, e) =>
                                {
                                    if (gMapControl != null)
                                        gMapControl.Zoom = Math.Max(gMapControl.Zoom - 1.0, gMapControl.MinZoom);
                                };
            button.Unloaded += (o, e) =>
                                   {
                                       var btn = o as Button;
                                       if (btn == null)
                                           return;

                                       BindingOperations.ClearAllBindings(btn);
                                   };
        }

        private void BuildButtonZoomIn(Button button)
        {
            if (button == null)
                return;

            button.Click += (o, e) =>
                                {
                                    if (gMapControl != null)
                                        gMapControl.Zoom = Math.Min(gMapControl.Zoom + 1.0, gMapControl.MaxZoom);
                                };
            button.Unloaded += (o, e) =>
                                   {
                                       var btn = o as Button;
                                       if (btn == null)
                                           return;

                                       BindingOperations.ClearAllBindings(btn);
                                   };
        }
    }
}