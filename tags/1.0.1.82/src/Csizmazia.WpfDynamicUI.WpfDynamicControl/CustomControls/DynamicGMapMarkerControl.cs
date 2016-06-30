using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using GMap.NET.WindowsPresentation;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.CustomControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Csizmazia.WpfDynamicUI.WpfDynamicControl.GMapControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Csizmazia.WpfDynamicUI.WpfDynamicControl.GMapControls;assembly=Csizmazia.WpfDynamicUI.WpfDynamicControl.GMapControls"
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
    ///     <MyNamespace:MarkerControl/>
    ///
    /// </summary>
    public class DynamicGMapMarkerControl : Control
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof (string), typeof (DynamicGMapMarkerControl),
                                        new PropertyMetadata(default(string)));

        private readonly GMapMarker Marker;


        private Popup Popup;
        private Image icon;

        static DynamicGMapMarkerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (DynamicGMapMarkerControl),
                                                     new FrameworkPropertyMetadata(typeof (DynamicGMapMarkerControl)));
        }


        public DynamicGMapMarkerControl(GMapMarker marker)
        {
            Marker = marker;


            Unloaded += (o, e) =>
                            {
                                var mc = o as DynamicGMapMarkerControl;
                                if (mc == null)
                                    return;

                                mc.Marker.Shape = null;
                                if (mc.icon != null)
                                    mc.icon.Source = null;
                                mc.icon = null;
                                mc.Popup = null;
                            };
            Loaded += (o, e) =>
                          {
                              var mc = o as DynamicGMapMarkerControl;
                              if (mc == null)
                                  return;

                              if (icon.Source.CanFreeze)
                              {
                                  icon.Source.Freeze();
                              }
                          };
            SizeChanged += (o, e) =>
                               {
                                   var mc = o as DynamicGMapMarkerControl;
                                   if (mc == null)
                                       return;


                                   mc.Marker.Offset = new Point(-e.NewSize.Width/2, -e.NewSize.Height);
                               };
            MouseEnter += (o, e) =>
                              {
                                  var mc = o as DynamicGMapMarkerControl;
                                  if (mc == null)
                                      return;

                                  if (mc.Popup == null) return;

                                  mc.Marker.ZIndex += 10000;
                                  mc.Popup.IsOpen = true;
                              };
            MouseLeave += (o, e) =>
                              {
                                  var mc = o as DynamicGMapMarkerControl;
                                  if (mc == null)
                                      return;

                                  if (mc.Popup == null) return;

                                  mc.Marker.ZIndex -= 10000;
                                  mc.Popup.IsOpen = false;
                              };
        }

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            icon = GetTemplateChild("PART_Icon") as Image;
            Popup = GetTemplateChild("PART_Popup") as Popup;
        }
    }
}