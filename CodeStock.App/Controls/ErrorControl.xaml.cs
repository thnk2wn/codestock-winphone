using System.ComponentModel;
using System.Windows.Media;
using System.Windows;

namespace CodeStock.App.Controls
{
    public partial class ErrorControl //: UserControl
    {
        public ErrorControl()
        {
            InitializeComponent();
            SetBackground();
        }

        private void SetBackground()
        {
            // in control designer we have solid color brush so we can see UX at design time but runtime we want transparent
            if (this.Parent != null || !DesignerProperties.IsInDesignTool)
                this.LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void CloseClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
