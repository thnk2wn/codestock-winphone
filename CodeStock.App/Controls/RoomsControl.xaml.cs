using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using Phone.Common.Threading;

namespace CodeStock.App.Controls
{
    public partial class RoomsControl //: UserControl
    {
        public RoomsControl()
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

        private void uxRoomList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (uxRoomList.SelectedIndex == -1) return;

            DispatchTimerUtil.On(TimeSpan.FromSeconds(0.3), () => uxRoomList.SelectedIndex = -1);
        }

        
    }
}
