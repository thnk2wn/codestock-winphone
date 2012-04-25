using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using CodeStock.App.ViewModels.Schedule;
using Phone.Common.Extensions.System.Windows_;
using Phone.Common.IO;

namespace CodeStock.App.Controls
{
    public partial class ScheduleControl //: UserControl
    {
        public ScheduleControl()
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

        private const string ScrollPos = "ScrollPos";

        public void SaveState()
        {
            this.ViewModel.Backup(new TransientDataStorage());
            
            var page = this.GetPage();
            var sv = uxSessionsListBox.GetScrollViewer();
            page.State[ScrollPos] = sv.VerticalOffset;
        }

        private double? ScrollOffset { get; set; }

        public void LoadState()
        {
            this.ViewModel.Restore(new TransientDataStorage());
            var page = this.GetPage();
            if (page.State.ContainsKey(ScrollPos))
            {
                this.ScrollOffset = Convert.ToDouble(page.State[ScrollPos]);
            }
        }

        private ScheduleViewModel ViewModel
        {
            get { return this.DataContext as ScheduleViewModel; }
            
        }

        private void uxSessionsListBox_LayoutUpdated(object sender, EventArgs e)
        {
            if (!this.ScrollOffset.HasValue) return;
            if (null == this.ViewModel.MySessions || !this.ViewModel.MySessions.Any()) return;

            uxSessionsListBox.GetScrollViewer().ScrollToVerticalOffset(this.ScrollOffset.Value);
            this.ScrollOffset = null;
        }
    }
}
