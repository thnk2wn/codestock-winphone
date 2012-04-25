using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace CodeStock.App.Controls
{
    public partial class MiscMenuControl : UserControl
    {
        public MiscMenuControl()
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
    }
}
