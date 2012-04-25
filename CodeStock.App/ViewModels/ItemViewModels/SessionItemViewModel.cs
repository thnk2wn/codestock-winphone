using System;
using System.Text;
using System.Windows.Input;
using CodeStock.App.ViewModels.Support;
using CodeStock.Data.LocalAccess;
using CodeStock.Data.Model;
using GalaSoft.MvvmLight.Command;
using Phone.Common.IOC;
using Phone.Common.Navigation;

namespace CodeStock.App.ViewModels.ItemViewModels
{
    //[DataContract]
    public class SessionItemViewModel : AppViewModelBase, ISessionItemViewModel
    {
        private string _title;

        public SessionItemViewModel()
        {
            //this.Speaker = new SpeakerItemViewModel();
            return;
        }

        public SessionItemViewModel(Session session) : this()
        {
            SetSession(session);
        }

        private void SetSession(Session session)
        {
            SessionItemMapper.FillViewModel(this, session);

            // for performance we copy this off and lazy get all later
            //this.Speaker = new SpeakerItemViewModel(session.Speaker);

            this.SpeakerName = null != session.Speaker ? session.Speaker.Name : "Unknown Speaker (Refresh)";

            _session = session;
        }

        private Session _session;

        private string _speakerName;

        // for performance we copy this off
        public string SpeakerName
        {
            get { return _speakerName; }

            set
            {
                if (_speakerName != value)
                {
                    _speakerName = value;
                    RaisePropertyChanged(() => SpeakerName);
                }
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        private string _trackArea;

        public string TrackArea
        {
            get
            {
                return _trackArea;
            }
            set
            {
                if (value != _trackArea)
                {
                    _trackArea = value;
                    RaisePropertyChanged(() => TrackArea);
                }
            }
        }

        private string _technology;

        public string Technology
        {
            get { return _technology; }
            set
            {
                if (_technology != value)
                {
                    _technology = value;
                    RaisePropertyChanged(() => Technology);
                }
            }
        }

        private string _level;

        public string Level
        {
            get { return _level; }
            set
            {
                if (_level != value)
                {
                    _level = value;
                    RaisePropertyChanged(() => Level);
                }
            }
        }

        private string _abstract;

        public string Abstract
        {
            get { return _abstract; }
            set
            {
                if (_abstract != value)
                {
                    _abstract = value;
                    RaisePropertyChanged(() => Abstract);
                }
            }
        }

        private int _sessionId;

        public int SessionId
        {
            get { return _sessionId; }
            set
            {
                if (_sessionId != value)
                {
                    _sessionId = value;
                    RaisePropertyChanged(() => SessionId);
                }
            }
        }

        private string _timeText;

        public string TimeText
        {
            get { return _timeText; }
            set
            {
                if (_timeText != value)
                {
                    _timeText = value;
                    RaisePropertyChanged(() => TimeText);
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

        private string _room;

        public string Room
        {
            get { return _room; }
            set
            {
                if (_room != value)
                {
                    _room = value;
                    RaisePropertyChanged(() => Room);
                }
            }
        }

        private string _roomText;

        public string RoomText
        {
            get { return _roomText; }
            set
            {
                if (_roomText != value)
                {
                    _roomText = value;
                    RaisePropertyChanged(() => RoomText);
                }
            }
        }

        private DateTime _startTime;

        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    RaisePropertyChanged(() => StartTime);
                }
            }
        }

        private DateTime _endTime;

        public DateTime EndTime
        {
            get { return _endTime; }
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                    RaisePropertyChanged(() => EndTime);
                }
            }
        }

        private string _timeAndRoom;

        public string TimeAndRoom
        {
            get { return _timeAndRoom; }

            set
            {
                if (_timeAndRoom != value)
                {
                    _timeAndRoom = value;
                    RaisePropertyChanged(() => TimeAndRoom);
                }
            }
        }

        private string _voteRank;

        public string VoteRank
        {
            get { return _voteRank; }

            set
            {
                if (_voteRank != value)
                {
                    _voteRank = value;
                    RaisePropertyChanged(() => VoteRank);
                }
            }
        }

        private ISpeakerItemViewModel _speakerItemViewModel;

        public ISpeakerItemViewModel Speaker
        {
            get
            {
                if (null == _speakerItemViewModel && null != _session)
                {
                    // lazy init for performance
                    _speakerItemViewModel = new SpeakerItemViewModel(_session.Speaker);
                    RaisePropertyChanged(() => Speaker);
                }

                return _speakerItemViewModel;
            }
            set
            {
                if (_speakerItemViewModel != value)
                {
                    _speakerItemViewModel = value;
                    RaisePropertyChanged(() => Speaker);
                }
            }
        }

        public ICommand ToggleFavoriteCommand
        {
            get { return new RelayCommand(ToggleFavorite); }
        }

        private void ToggleFavorite()
        {
            Favorites().ToggleFavorite(this.SessionId);

            if (null != this.FavoriteChanged)
                this.FavoriteChanged(this, new EventArgs());
        }

        private static FavoriteSessions Favorites()
        {
            return IoC.Get<FavoriteSessions>();
        }

        public bool IsFavorite()
        {
            return Favorites().IsFavorite(this.SessionId);
        }

        // Action cannot be serialized
        //public Action FavoriteChanged { get; set; }
        public event EventHandler FavoriteChanged;

        public ICommand ShareOnTwitterCommand
        {
            get
            {
                return new RelayCommand(ShareOnTwitter);
            }
        }


        private void ShareOnTwitter()
        {
            var who = this.Speaker.TwitterId ?? this.Speaker.Name;
            var twitterUrl = GetTheDamnTwitterUrl(who);

            IoC.Get<INavigationService>().NavigateTo(Uris.WebBrowser(), twitterUrl);
        }

        private string GetTheDamnTwitterUrl(string who)
        {
            // previously tried: http://m.twitter.com/share?url={0}&text={1}, http://twitter.com/intent/tweet?text={0}&url={1}, http://twitter.com/home?status={0} ...

            const string twitterUrlFormat = "http://twitter.com/share?url={0}&text={1}";
            var shareUrl = this.Url;

            // hmm. if we include any hashtag like #CodeStock it gets stripped entirely. Uri.EscapeUriString() is done later
            var status = string.Format("{0} by {1} #CodeStock ", this.Title.TrimEnd(), who);
            var twitterUrl = string.Format(twitterUrlFormat, shareUrl, status);
            return twitterUrl;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Session [Id: {0}, Title: {1}]", this.SessionId, this.Title);
            return sb.ToString();
        }

        public bool HasEnded
        {
            get { return Now() > this.EndTime; }
        }

        public bool HasStarted
        {
            get { return Now() >= this.StartTime; }
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

        public ICommand SessionUrlCommand
        {
            get { return new RelayCommand(GoToSessionUrl); }
        }

        private void GoToSessionUrl()
        {
            IoC.Get<INavigationService>().NavigateTo(Uris.WebBrowser(), this.Url);
        }

        public ICommand SpeakerCommand
        {
            get { return new RelayCommand(GoToSpeaker); }
        }

        private void GoToSpeaker()
        {
            IoC.Get<INavigationService>().NavigateTo(Uris.Speaker(this.Speaker));
        }
       
    }
}