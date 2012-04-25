using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CodeStock.App.ViewModels;
using Phone.Common.Extensions.System.Windows_;
using Phone.Common.IO;
using Phone.Common.Extensions.Microsoft.Phone.Controls_;
using Phone.Common.Threading;

namespace CodeStock.App.Controls
{
    public partial class TweetsControl //: UserControl
    {
        public TweetsControl()
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

        private TwitterSearchViewModel ViewModel
        {
            get { return this.DataContext as TwitterSearchViewModel; }

        }

        public void SaveState()
        {
            this.ViewModel.Backup(new TransientDataStorage());
            var page = this.GetPage();
            page.SaveState(uxTweetsListBox);
        }

        public void LoadState()
        {
            this.ViewModel.Restore(new TransientDataStorage());

            var page = this.GetPage();
            page.LoadState(uxTweetsListBox);
        }

        private void uxTweetsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (uxTweetsListBox.SelectedIndex == -1) return;

            DispatchTimerUtil.On(TimeSpan.FromSeconds(0.3), ()=> uxTweetsListBox.SelectedIndex = -1);
        }

        private void SearchTextKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // dismiss keyboard
                this.Focus();

                this.ViewModel.UserSearchedCommand.Execute(null);
            }
        }
    }
}
