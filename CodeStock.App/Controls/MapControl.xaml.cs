using System.ComponentModel;
using System.Windows.Media;

namespace CodeStock.App.Controls
{
    public partial class MapControl //: UserControl
    {
        public MapControl()
        {
            InitializeComponent();
            SetBackground();
        }

        private void SetBackground()
        {
            if (this.Parent != null || !DesignerProperties.IsInDesignTool)
                this.LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetBackground();
        }
    }
}
