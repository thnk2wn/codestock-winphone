using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CodeStock.App.ViewModels;
using Phone.Common.Extensions.Microsoft.Phone.Controls_;
using Phone.Common.Extensions.System.Windows_;
using Phone.Common.Extensions.System_;
using Phone.Common.IO;
using Phone.Common.Threading;

namespace CodeStock.App.Controls
{
    public partial class SearchControl //: UserControl
    {
        public SearchControl()
        {
            InitializeComponent();
            SetBackground();
        }

        private void SearchKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // dismiss keyboard
                this.Focus();

                this.DataContext.As<SearchViewModel>().Search();
            }
        }

        public void SaveState()
        {
            this.DataContext.As<SearchViewModel>().Backup(new TransientDataStorage());
            this.GetPage().SaveState(uxResultsListBox);
        }

        public void LoadState()
        {
            var vm = this.DataContext.As<SearchViewModel>();
            vm.Restore(new TransientDataStorage());
            
            if (vm.Items != null && vm.Items.Any())
            {
                this.GetPage().LoadState(uxResultsListBox);
            }
        }

        private void SetBackground()
        {
            // in control designer we have solid color brush so we can see UX at design time but runtime we want transparent
            if (this.Parent != null || !DesignerProperties.IsInDesignTool)
                this.LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void uxResultsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (uxResultsListBox.SelectedIndex == -1) return;
            DispatchTimerUtil.On(TimeSpan.FromSeconds(0.3), ()=> uxResultsListBox.SelectedIndex = -1);
        }
    }
}
