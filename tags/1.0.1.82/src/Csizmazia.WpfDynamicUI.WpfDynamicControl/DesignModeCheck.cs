using System.ComponentModel;
using System.Windows;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl
{
    internal static class DesignModeCheck
    {
        private static bool? _isInDesignMode;

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend
        /// or Visual Studio).
        /// </summary>
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
#if SILVERLIGHT
            _isInDesignMode = DesignerProperties.IsInDesignTool;
#else
                    DependencyProperty prop = DesignerProperties.IsInDesignModeProperty;
                    _isInDesignMode
                        = (bool) DependencyPropertyDescriptor.FromProperty(prop, typeof (FrameworkElement))
                                     .Metadata.DefaultValue;
#endif
                }

                return _isInDesignMode.Value;
            }
        }
    }
}