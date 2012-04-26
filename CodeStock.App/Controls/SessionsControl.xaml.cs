using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using CodeStock.App.ViewModels;
using CodeStock.App.ViewModels.ItemViewModels;
using Phone.Common.Extensions.System.Windows_;
using Phone.Common.Extensions.System_;

namespace CodeStock.App.Controls
{
    public partial class SessionsControl //: UserControl
    {
        private const string FirstItemInView = "FirstItemInView";

        public SessionsControl()
        {
            InitializeComponent();
            SetBackground();
        }

        private void SetBackground()
        {
            // in control designer we have solid color brush so we can see UX at design time but runtime we want transparent
            if (this.Parent != null || !DesignerProperties.IsInDesignTool)
                this.SessionsLayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
        }

        public void SaveState()
        {
            var page = this.GetPage();
            
            // virtualized nature of longListSelector makes saving scroll position more difficult

            var firstItem = uxSessionsList.GetItemsInView().FirstOrDefault() as SessionItemViewModel;
            if (null != firstItem)
            {
                page.State[FirstItemInView] = firstItem.SessionId;
            }
        }

        private int? SessionIdToScrollTo { get; set; }

        public void LoadState()
        {
            var page = this.GetPage();
            
            if (page.State.ContainsKey(FirstItemInView))
            {
                // items not set at this point, save id and do it later
                this.SessionIdToScrollTo = Convert.ToInt32(page.State[FirstItemInView]);
                //CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
        }

        private void uxSessionsList_LayoutUpdated(object sender, EventArgs e)
        {
            RestoreScrollPosition();
        }

        private void RestoreScrollPosition()
        {
            if (this.SessionIdToScrollTo == null) return;
            var vm = this.DataContext.As<SessionsViewModel>();
            if (vm.Items == null || !vm.Items.Any() || vm.IsBusySafe()) return;

            var session = vm.Items.FirstOrDefault(s => s.SessionId == this.SessionIdToScrollTo);

            if (null != session)
            {
                uxSessionsList.ScrollTo(session);
                // critical to null this otherwise recursion 
                this.SessionIdToScrollTo = null;
            }
        }
    }
}
