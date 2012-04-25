using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Phone.Common.Extensions.System.Windows_
{
    public static class DependencyObjectExtensions
    {
        public static ScrollViewer GetScrollViewer(this DependencyObject parent)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childCount; i++)
            {
                var elt = VisualTreeHelper.GetChild(parent, i);
                if (elt is ScrollViewer) return (ScrollViewer)elt;
                var result = GetScrollViewer(elt);
                if (result != null) return result;
            }
            return null;
        }
    }
}
