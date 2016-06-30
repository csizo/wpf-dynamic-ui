using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Csizmazia.WpfDynamicUI.WpfDynamicControl.ReflectiveWpfTools
{
    public class IsNotEmptyStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (string) value;
            return !string.IsNullOrEmpty(str);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("IsNotEmptyStringConverter can only be used OneWay.");
        }

        #endregion
    }

    public class EmptyStringToInvisibleConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (string) value;
            return !string.IsNullOrEmpty(str) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("EmptyStringToInvisibleConverter can only be used OneWay.");
        }

        #endregion
    }
}