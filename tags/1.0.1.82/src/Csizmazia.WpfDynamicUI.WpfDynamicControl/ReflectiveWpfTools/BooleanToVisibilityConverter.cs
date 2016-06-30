using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ReflectiveWpfTools
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool) value;
            if (parameter == null)
                return boolValue ? Visibility.Visible : Visibility.Collapsed;

            return !boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (Visibility) value;
            if (parameter == null)
                return visibility == Visibility.Visible;

            return visibility != Visibility.Visible;
        }

        #endregion
    }
}