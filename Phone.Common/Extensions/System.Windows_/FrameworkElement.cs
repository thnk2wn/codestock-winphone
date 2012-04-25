using System.Windows;
using Microsoft.Phone.Controls;

namespace Phone.Common.Extensions.System.Windows_
{
    public static class FrameworkElementExtensions
    {
        public static PhoneApplicationPage GetPage(this FrameworkElement element)
        {
            var p = element.Parent;

            while (!(p is PhoneApplicationPage))
            {
                p = ((FrameworkElement)p).Parent;
            }

            var page = (PhoneApplicationPage)p;
            return page;
        }
    }
}
