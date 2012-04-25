using System.Windows;
using System.Windows.Media;

namespace Phone.Common.Windows
{
    public static class ThemeHelper
    {
        //static method to check if the phone is in darktheme mode
        public static bool IsDarkTheme()
        {
            //var themeColor = (Color)Application.Current.Resources["PhoneForegroundColor"];
            var isDarkTheme = (Visibility.Visible == (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"]);
            return isDarkTheme;
        }
    }

}
