using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Data;
using Phone.Common.IOC;
using Phone.Common.Net;

namespace Phone.Common.Converters
{
    public class UnHtmlConverter : IValueConverter
    {
        //[DepInject]
        public IHtmlDecoder HtmlDecoder { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //if (DesignerProperties.IsInDesignTool) return value;
            if (null == value) return value;

            if (null == HtmlDecoder)
                HtmlDecoder = IoC.Get<IHtmlDecoder>();

            if (targetType == typeof(string) && value.GetType() == typeof(string))
            {
                // From Roy Osherove's Blog
                // http://weblogs.asp.net/rosherove/archive/2003/05/13/6963.aspx

                var result = Regex.Replace((string)value, @"</h2>", string.Concat(Environment.NewLine, Environment.NewLine));
                result = Regex.Replace(result, @"</p>", string.Concat(Environment.NewLine, Environment.NewLine));
                result = Regex.Replace(result, @"<(.|\n)*?>", string.Empty);

                try
                {
                    // at design time, occasional type load exception here with HttpUtility so fake one will be used at design time
                    result = this.HtmlDecoder.Decode(result);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                result = Regex.Replace(result, @"[ ]+", " ");

                // should prob have a property for this maybe:
                result = result.TrimEnd();

                return result;
            }

            // No conversion
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // No Conversion
            return value;
        }
    }
}
