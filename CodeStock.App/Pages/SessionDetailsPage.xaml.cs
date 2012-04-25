using System;
using System.ComponentModel;
using System.Windows.Navigation;
using System.Linq;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Support;
using Phone.Common.Diagnostics.Logging;
using Phone.Common.Extensions.Microsoft.Phone.Shell_;
using Phone.Common.IO;
using Phone.Common.IOC;

namespace CodeStock.App.Pages
{
    public partial class SessionDetailsPage //: PhoneApplicationPage
    {
        public SessionDetailsPage()
        {
            InitializeComponent();
            this.LayoutUpdated += SessionDetailsPage_LayoutUpdated;
        }

        private void SessionDetailsPage_LayoutUpdated(object sender, EventArgs e)
        {
            if (!this.ScrollOffset.HasValue) return;

            uxScrollViewer.ScrollToVerticalOffset(this.ScrollOffset.Value);
            this.ScrollOffset = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (DesignerProperties.IsInDesignTool) return;

            var store = new TransientDataStorage();
            var vm =  store.Restore(()=> ViewModel);

            var sessionIdArg = this.NavigationContext.QueryString["SessionId"];
            this.SessionId = Convert.ToInt32(sessionIdArg);

            if (null != vm && vm.SessionId == this.SessionId)
            {
                this.ViewModel = vm;
                this.ScrollOffset = store.Restore<double>(ScrollPos);
                LogInstance.LogInfo("Restored from transient storage: {0}", vm);
                return;
            }
           
            LoadFromCoreData();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var store = new TransientDataStorage();
            store.Backup(() => ViewModel, this.ViewModel);
            store.Backup(ScrollPos, uxScrollViewer.VerticalOffset);
            LogInstance.LogInfo("Backed up to transient storage: {0}", this.ViewModel);
        }

        private int? SessionId { get; set; }
        private double? ScrollOffset { get; set; }
        private const string ScrollPos = "SessionDetails.ScrollPos";

        private void LoadFromCoreData()
        {
            var coreData = IoC.Get<ICoreData>();
            var sessions = coreData.Sessions;
            this.ViewModel = sessions.Where(i => i.SessionId == this.SessionId).FirstOrDefault();
        }

        private void SetupDynamicAppBarItems()
        {
            var vm = this.ViewModel;
            if (null == vm) return;

            var bar = this.ApplicationBar;
            bar.RemoveAllItems();

            var isFav = this.ViewModel.IsFavorite();

            if (!isFav)
            {
                bar.InsertButton(0, "/Images/appbar.favs.addto.rest.png", "Add Favorite", vm.ToggleFavoriteCommand);
                bar.InsertMenuItem(0, "Add Favorite", vm.ToggleFavoriteCommand);
            }
            else
            {
                bar.InsertButton(0, "/Images/appbar.favs.rest.png", "Remove Fav", vm.ToggleFavoriteCommand);
                bar.InsertMenuItem(0, "Remove Favorite", vm.ToggleFavoriteCommand);
            }

            bar.InsertButton(1, "/Images/appbar.share.rest.png", "Share", vm.ShareOnTwitterCommand);
            bar.InsertMenuItem(1, "Share on Twitter", vm.ShareOnTwitterCommand);
        }
        

        private SessionItemViewModel ViewModel
        {
            get { return this.DataContext as SessionItemViewModel; }
            set
            {
                this.DataContext = value;

                if (null != value)
                {
                    SetupDynamicAppBarItems();
                    value.FavoriteChanged += (s, e) => SetupDynamicAppBarItems();
                }
            }
        }

        private void SpeakerLinkClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ViewModel.SpeakerCommand.Execute(null);
        }
    }
}