using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Support;
using CodeStock.Data;
using CodeStock.Data.ServiceAccess;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls;
using Phone.Common.IO;
using Phone.Common.Navigation;
using GestureEventArgs = Microsoft.Phone.Controls.GestureEventArgs;

namespace CodeStock.App.ViewModels
{
    public class TwitterSearchViewModel : AppViewModelBase
    {
        private const int AutoRefreshSeconds = 120;
        private const string DefaultSearchText = "CodeStock";

        public TwitterSearchViewModel(ITwitterSearchService twitterSearchService, INavigationService navigationService)
        {
            _twitterSearchService = twitterSearchService;
            _twitterSearchService.AfterCompleted = SearchComplete;

            _navigationService = navigationService;

            this.Invalidated = true;

            if (IsInDesignMode)
                Load();
        }

        private readonly ITwitterSearchService _twitterSearchService;
        private readonly INavigationService _navigationService;

        private ObservableCollection<TweetItemViewModel> _tweetItems;

        public ObservableCollection<TweetItemViewModel> TweetItems
        {
            get { return _tweetItems; }
            set
            {
                if (_tweetItems != value)
                {
                    _tweetItems = value;
                    RaisePropertyChanged(() => TweetItems);
                }
            }
        }

        private string _searchText = DefaultSearchText;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    RaisePropertyChanged(() => SearchText);
                    this.Invalidated = true;
                }
            }
        }

        private bool Invalidated { get; set; }
        private DateTime? LastLoaded { get; set; }

        public void Load()
        {
            if (!this.Invalidated && null != this.LastLoaded && (DateTime.Now - this.LastLoaded).Value.TotalSeconds <= AutoRefreshSeconds)
                return;

            this.SetBusy(true, "Searching twitter...");
            QueueSafeDispatch( ()=> _twitterSearchService.Search(this.SearchText));
        }

        private void SearchComplete(CompletedEventArgs e)
        {
            var result = _twitterSearchService.Result;
            var items = new ObservableCollection<TweetItemViewModel>();

            if (result != null && result.Tweets != null)
                result.Tweets.ForEach(t => items.Add(TweetItemViewModel.CreateTweetItem(t)));

            this.TweetItems = items;
            this.Invalidated = false;
            this.LastLoaded = DateTime.Now;
            this.SetBusy(false);

            this.Error = (null == e.Error) ? null : new ErrorItemViewModel(e);
        }

        private ErrorItemViewModel _error;

        public ErrorItemViewModel Error
        {
            get { return _error; }

            set
            {
                if (_error != value)
                {
                    _error = value;
                    RaisePropertyChanged(() => Error);
                }
            }
        }


        protected override void OnActivated()
        {
            if (!WasRestored)
                this.Load();
        }

        public ICommand UserSearchedCommand
        {
            get { return new RelayCommand(UserSearched); }
        }

        private void UserSearched()
        {
            if (this.IsBusy)
                return;

            if (string.IsNullOrEmpty(this.SearchText) || 0 == this.SearchText.Trim().Length)
            {
                this.MessageBox("Please enter something to search for.", "Required Data");
                return;
            }

            this.Invalidated = true;
            this.Load();
        }

        public ICommand TweetSelectedCommand
        {
            get { return new RelayCommand<GestureEventArgs>(TweetSelected); }
        }

        private void TweetSelected(GestureEventArgs e)
        {
            var elem = (FrameworkElement)e.OriginalSource;
            var vm = elem.DataContext as TweetItemViewModel;

            if (null == vm) return;
            _navigationService.NavigateTo(Uris.Tweet(vm));
        }

        public void Backup(IDataStorage store)
        {
            store.Backup(() => this.SearchText, this.SearchText);
            store.Backup(() => this.TweetItems, this.TweetItems);
        }

        public void Restore(IDataStorage store)
        {
            this.SearchText = store.Restore(() => this.SearchText, DefaultSearchText);
            this.TweetItems = store.Restore(() => this.TweetItems, new ObservableCollection<TweetItemViewModel>());
            this.WasRestored = (this.TweetItems.Any());
        }

        private bool WasRestored { get; set; }

        public const string StateKey = "TwitterSearchViewModel.TweetItems";

        public ICommand NewTweetCommand
        {
            get { return new RelayCommand(NewTweet); }
        }

        private void NewTweet()
        {
            const string twitterUrlFormat = "http://twitter.com/share?&text={0}";
            const string text = ""; //" #CodeStock";
            var url = string.Format(twitterUrlFormat, text);
            _navigationService.NavigateTo(Uris.WebBrowser(), url);
        }
    }
}
