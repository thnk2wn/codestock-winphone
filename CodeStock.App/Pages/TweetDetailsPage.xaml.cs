using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Navigation;
using CodeStock.App.ViewModels;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Support;
using Phone.Common.IO;
using Phone.Common.IOC;
using Phone.Common.Navigation;

namespace CodeStock.App.Pages
{
    public partial class TweetDetailsPage //: PhoneApplicationPage
    {
        public TweetDetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (DesignerProperties.IsInDesignTool) return;

            const string key = TwitterSearchViewModel.StateKey;
            var store = new TransientDataStorage();
            var items = store.Restore<ObservableCollection<TweetItemViewModel>>(key);

            if (null == items || !items.Any())
                throw new NullReferenceException("Failed to find tweet items in store with key " + key);

            var arg = this.NavigationContext.QueryString["TweetId"];
            var tweetId = Convert.ToInt64(arg);

            this.ViewModel = items.Where(i => i.TweetId == tweetId).FirstOrDefault();
        }

        private TweetItemViewModel ViewModel
        {
            get { return this.LayoutRoot.DataContext as TweetItemViewModel; }

            set
            {
                this.LayoutRoot.DataContext = value;
            }
        }

        private void TweetBrowserActionTouch(object sender, System.Windows.RoutedEventArgs e)
        {
            IoC.Get<INavigationService>().NavigateTo(Uris.WebBrowser(), sender.ToString());
        }

        private void uxMentionButton_Click(object sender, EventArgs e)
        {
            this.ViewModel.MentionUserCommand.Execute(null);
        }
    }
}