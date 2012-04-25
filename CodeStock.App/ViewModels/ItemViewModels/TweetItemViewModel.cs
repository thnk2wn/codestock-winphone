using System;
using System.Linq;
using System.Windows.Input;
using CodeStock.App.ViewModels.Support;
using CodeStock.Data.Model;
using GalaSoft.MvvmLight.Command;
using Phone.Common.IOC;
using Phone.Common.Navigation;
using Phone.Common.Net;
using Phone.Common.Text;

namespace CodeStock.App.ViewModels.ItemViewModels
{
    public class TweetItemViewModel : AppViewModelBase
    {

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    RaisePropertyChanged(() => Text);
                }
            }
        }

        private string _profileImageUrl = "/Images/unisex-48x48.png";

        public string ProfileImageUrl
        {
            get { return _profileImageUrl; }

            set
            {
                if (_profileImageUrl != value)
                {
                    _profileImageUrl = value;
                    RaisePropertyChanged(() => ProfileImageUrl);
                }
            }
        }

        private string _fromUser;

        public string FromUser
        {
            get { return _fromUser; }

            set
            {
                if (_fromUser != value)
                {
                    _fromUser = value;
                    RaisePropertyChanged(() => FromUser);
                }
            }
        }

        private string _timeAgoText;

        public string TimeAgoText
        {
            get { return _timeAgoText; }

            set
            {
                if (_timeAgoText != value)
                {
                    _timeAgoText = value;
                    RaisePropertyChanged(() => TimeAgoText);
                }
            }
        }

        private string _client;

        public string Client
        {
            get { return _client; }

            set
            {
                if (_client != value)
                {
                    _client = value;
                    RaisePropertyChanged(() => Client);
                }
            }
        }

        private string _clientUrl;

        public string ClientUrl
        {
            get { return _clientUrl; }

            set
            {
                if (_clientUrl != value)
                {
                    _clientUrl = value;
                    RaisePropertyChanged(() => ClientUrl);
                }
            }
        }

        private long _tweetId;

        public long TweetId
        {
            get { return _tweetId; }

            set
            {
                if (_tweetId != value)
                {
                    _tweetId = value;
                    RaisePropertyChanged(() => TweetId);
                }
            }
        }

        private string _dateTimeText;

        public string DateTimeText
        {
            get { return _dateTimeText; }

            set
            {
                if (_dateTimeText != value)
                {
                    _dateTimeText = value;
                    RaisePropertyChanged(() => DateTimeText);
                }
            }
        }

        public static TweetItemViewModel CreateTweetItem(Tweet t)
        {
            var decoder = IoC.Get<IHtmlDecoder>();
            var vm = new TweetItemViewModel
            {
                Text = decoder.Decode(t.Text),
                ProfileImageUrl = t.ProfileImageUrl,
                FromUser = t.FromUser,
                TweetId =  t.Id
            };

            if (t.CreatedAt.HasValue)
            {
                var ts = DateTime.Now - t.CreatedAt.Value;
                if (ts.TotalSeconds < 1)
                    vm.TimeAgoText = "Just now";
                else if (ts.TotalMinutes < 1)
                    vm.TimeAgoText = string.Format("{0:##} secs", ts.TotalSeconds);
                else if (ts.TotalHours < 1)
                    vm.TimeAgoText = string.Format("{0:##} mins", ts.TotalMinutes);
                else if (ts.TotalDays < 1)
                    vm.TimeAgoText = string.Format("{0:##} hrs", ts.TotalHours);
                else
                    vm.TimeAgoText = string.Format("{0:##} days", ts.TotalDays);

                vm.DateTimeText = t.CreatedAt.Value.ToString("G");
            }

            if (!string.IsNullOrEmpty(t.Source))
            {
                var src = t.Source.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"");
                var link = LinkFinder.Find(src).FirstOrDefault();

                if (null != link)
                {
                    vm.ClientUrl = link.Href;
                    vm.Client = link.Text;
                }
            }


            return vm;
        }

        public ICommand TwitterUserCommand
        {
            get { return new RelayCommand(GoToTwitterUser); }
        }

        private void GoToTwitterUser()
        {
            var url = string.Format("http://twitter.com/{0}", this.FromUser);
            IoC.Get<INavigationService>().NavigateTo(Uris.WebBrowser(), url);
        }

        public ICommand MentionUserCommand
        {
            get { return new RelayCommand(MentionUser); }
        }

        private void MentionUser()
        {
            const string twitterUrlFormat = "http://twitter.com/share?&text={0}";
            var text = "@" + this.FromUser + " ";
            var url = string.Format(twitterUrlFormat, text);
            IoC.Get<INavigationService>().NavigateTo(Uris.WebBrowser(), url);
        }

    }
}
