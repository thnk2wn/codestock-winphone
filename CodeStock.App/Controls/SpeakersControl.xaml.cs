using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using Phone.Common.Extensions.Microsoft.Phone.Controls_;
using Phone.Common.Extensions.System.Windows_;
using Phone.Common.Threading;

namespace CodeStock.App.Controls
{
    public partial class SpeakersControl //: UserControl
    {
        public SpeakersControl()
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

        private void DeferredLoadListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            DispatchTimerUtil.On(TimeSpan.FromSeconds(0.3), () => uxSpeakerListBox.SelectedIndex = -1);
        }

        public void SaveState()
        {
            var page = this.GetPage();
            page.SaveState(uxSpeakerListBox);
        }

        public void LoadState()
        {
            var page = this.GetPage();
            page.LoadState(uxSpeakerListBox);
        }
        
    }
}
