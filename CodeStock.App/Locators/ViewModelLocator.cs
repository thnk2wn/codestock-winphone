using CodeStock.App.IOC;
using CodeStock.App.ViewModels;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Schedule;
using Phone.Common.IOC;

namespace CodeStock.App.Locators
{
    public class ViewModelLocator
    {
       
        static ViewModelLocator()
        {
            ModuleLoader.Load();
        }

        public MainViewModel Main
        {
            get
            {
                return IoC.Get<MainViewModel>();
            }
        }

        public SessionsViewModel Sessions
        {
            get
            {
                return IoC.Get<SessionsViewModel>();
            }
        }

        public SpeakersViewModel Speakers
        {
            get
            {
                return IoC.Get<SpeakersViewModel>();
            }
        }

        public ISessionItemViewModel SessionItem
        {
            get
            {
                return IoC.Get<ISessionItemViewModel>();
            }
        }

        public RoomsViewModel Rooms
        {
            get
            {
                return IoC.Get<RoomsViewModel>();
            }
        }

        public ScheduleViewModel Schedule
        {
            get
            {
                return IoC.Get<ScheduleViewModel>();
            }
        }

        public ISpeakerItemViewModel SpeakerItem
        {
            get { return IoC.Get<ISpeakerItemViewModel>(); }
        }

        public IRoomItemViewModel RoomItem
        {
            get
            {
                return IoC.Get<IRoomItemViewModel>();
            }
        }

        public MiscMenuViewModel MiscMenu
        {
            get
            {
                return IoC.Get<MiscMenuViewModel>();
            }
        }

        public DiagnosticViewModel DiagnosticLog
        {
            get { return IoC.Get<DiagnosticViewModel>(); }
        }

        public TwitterSearchViewModel TwitterSearch
        {
            get { return IoC.Get<TwitterSearchViewModel>(); }
        }

        public TweetItemViewModel TweetItem
        {
            get { return IoC.Get<TweetItemViewModel>(); }
        }

        public WebBrowserViewModel WebBrowser
        {
            get { return IoC.Get<WebBrowserViewModel>(); }
        }

        public SearchViewModel Search
        {
            get { return IoC.Get<SearchViewModel>(); }
        }

        public SettingsViewModel Settings
        {
            get { return IoC.Get<SettingsViewModel>(); }
        }
        
    }
}