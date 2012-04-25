using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Phone.Common.Converters
{
    public class IntFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (null != value) ? value.ToString() : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var strVal = value.ToString();

                if (string.IsNullOrEmpty(strVal))
                {
                    return (int?)null;
                }

                return int.Parse(strVal);
            }

            catch (Exception exc)
            {
                Debug.WriteLine(exc);
                return null;

            }
        }
    }
}
