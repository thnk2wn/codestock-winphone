using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Phone.Common.Converters
{
    public class NullObjectToVisibiltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // string = "" is debatable; perhaps another StringSetConverter later
            return (value == null || (value.GetType() == typeof(string) && string.Empty == value.ToString())) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
