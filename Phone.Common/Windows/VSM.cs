using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Phone.Common.Windows
{
    public static class VSM
    {
        #region StateName (Attached DependencyProperty)

        public static readonly DependencyProperty StateNameProperty =
            DependencyProperty.RegisterAttached("StateName", typeof(string), typeof(VSM), new PropertyMetadata(new PropertyChangedCallback(OnStateNameChanged)));

        public static void SetStateName(Control o, string value)
        {
            o.SetValue(StateNameProperty, value);
        }

        public static string GetStateName(Control o)
        {
            return (string)o.GetValue(StateNameProperty);
        }

        private static void OnStateNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as Control;
            var value = (string)e.NewValue;

            if (sender != null && !string.IsNullOrEmpty(value))
            {
                if (!DesignerProperties.IsInDesignTool)
                {
                    VisualStateManager.GoToState(sender, value, true);
                }
            }
        }

        #endregion

    }
}
