using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Windows.Input;
using CodeStock.App.ViewModels.Support;
using CodeStock.Data.Model;
using GalaSoft.MvvmLight.Command;
using Phone.Common.IOC;
using Phone.Common.Navigation;

namespace CodeStock.App.ViewModels.ItemViewModels
{
    public class SpeakerItemViewModel : AppViewModelBase, ISpeakerItemViewModel
    {
        public const string DefaultImage = "Images/unisex-48x48.png";

        public SpeakerItemViewModel()
        {
            this.PhotoUrl = DefaultImage;

            return;
        }

        public SpeakerItemViewModel(Speaker speaker) : this()
        {
            // some design time cases etc.
            SetSpeaker(speaker);
        }

        private void SetSpeaker(Speaker speaker)
        {
            SpeakerItemMapper.FillViewModel(this, speaker);
        }
        

        private string _bio;
        public string Bio
        {
            get { return _bio; }
            set
            {
                if (_bio != value)
                {
                    _bio = value;
                    RaisePropertyChanged(() => Bio);
                }
            }
        }

        private string _company;

        public string Company
        {
            get { return _company; }
            set
            {
                if (_company != value)
                {
                    _company = value;
                    RaisePropertyChanged(() => Company);
                }
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(() => Name);
                }
            }
        }

        private string _photoUrl;

        public string PhotoUrl
        {
            get { return _photoUrl; }
            set
            {
                if (_photoUrl != value)
                {
                    _photoUrl = value;
                    RaisePropertyChanged(() => PhotoUrl);
                }
            }
        }

        private int _speakerId;

        public int SpeakerId
        {
            get { return _speakerId; }
            set
            {
                if (_speakerId != value)
                {
                    _speakerId = value;
                    RaisePropertyChanged(() => SpeakerId);
                }
            }
        }

        private string _twitterId;

        public string TwitterId
        {
            get { return _twitterId; }
            set
            {
                if (_twitterId != value)
                {
                    _twitterId = value;
                    RaisePropertyChanged(() => TwitterId);
                    this.TwitterUrl = !string.IsNullOrEmpty(_twitterId) ? string.Format("http://twitter.com/{0}", _twitterId) : null;
                }
            }
        }

        private string _twitterUrl;

        public string TwitterUrl
        {
            get { return _twitterUrl; }
            set
            {
                if (_twitterUrl != value)
                {
                    _twitterUrl = value;
                    RaisePropertyChanged(() => TwitterUrl);
                }
            }
        }

        private string _website;

        public string Website
        {
            get { return _website; }
            set
            {
                if (_website != value)
                {
                    _website = value;
                    RaisePropertyChanged(() => Website);
                }
            }
        }

        private ObservableCollection<SessionItemViewModel> _sessions;

        // causes problems serializing this
        [IgnoreDataMember]
        public ObservableCollection<SessionItemViewModel> Sessions
        {
            get
            {
                // this lazy load style is needed to prevent circular reference stack overflow type errors loading data
                // and also to delay load data for performance reasons
                if (null == _sessions || !_sessions.Any())
                {
                    _sessions = IoC.Get<ICoreData>().SessionsForSpeaker(this.SpeakerId);
                    RaisePropertyChanged(() => Sessions);
                }
                
                return _sessions;
            }
            set
            {
                if (_sessions != value)
                {
                    _sessions = value;
                    RaisePropertyChanged(() => Sessions);
                }
            }
        }

        public ICommand SessionSelectedCommand
        {
            get { return new RelayCommand<SelectionChangedEventArgs>(SessionSelected); }
        }

        private void SessionSelected(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;

            var session = (SessionItemViewModel)e.AddedItems[0];

            var navigationService = IoC.Get<INavigationService>();
            navigationService.NavigateTo(Uris.Session(session));
            return;
        }

        public override string ToString()
        {
            var s = string.Format("Speaker [Id: {0}, Name: {1}]", this.SpeakerId, this.Name);
            return s.ToString();
        }

        public ICommand WebsiteCommand
        {
            get { return new RelayCommand(GoToWebsite); }
        }

        private void GoToWebsite()
        {
            IoC.Get<INavigationService>().NavigateTo(Uris.WebBrowser(), this.Website);
        }

        public ICommand TwitterCommand
        {
            get { return new RelayCommand(GoToTwitter); }
        }

        private void GoToTwitter()
        {
            IoC.Get<INavigationService>().NavigateTo(Uris.WebBrowser(), this.TwitterUrl);
        }

        private string _url;

        public string Url
        {
            get { return _url; }

            set
            {
                if (_url != value)
                {
                    _url = value;
                    RaisePropertyChanged(() => Url);
                }
            }
        }

        public ICommand SpeakerUrlCommand
        {
            get { return new RelayCommand(GoToSpeakerUrl); }
        }

        private void GoToSpeakerUrl()
        {
            IoC.Get<INavigationService>().NavigateTo(Uris.WebBrowser(), this.Url);
        }

    }
}
