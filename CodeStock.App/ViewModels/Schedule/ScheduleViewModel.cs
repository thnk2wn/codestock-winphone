using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Support;
using CodeStock.Data;
using GalaSoft.MvvmLight.Command;
using Phone.Common.Extensions.System.Collections.Generic_;
using Phone.Common.IO;
using Phone.Common.IOC;
using Phone.Common.Navigation;
using Phone.Common.Threading;

namespace CodeStock.App.ViewModels.Schedule
{
    /// <summary>
    /// Schedule view model (My Schedule / Favorities / Right Now)
    /// </summary>
    /// <remarks>
    ///     Represents 3 views in one; each presents the same session data in the same manner.
    ///     The datasource and inputs simply change for each.
    ///     Although there are similarities, should probably break this up into child view models
    ///     or otherwise help separate if this class gets too big / too many concerns.
    /// </remarks>
    public class ScheduleViewModel : AppViewModelBase
    {
        private const string MySchedule = "Schedule (codestock.org)";
        private const string Favorites = "Favorites";
        private const string Time = "Right about now";
        private const string StartingSoon = "Starting soon";
        private const string Upcoming = "Upcoming";

        private Dictionary<string, ScheduleChildViewModel> ChildViewModels { get; set; }

        public ScheduleViewModel() : base(registerCoreData:true)
        {
            SetBusy(true, "Do or do not... there is no try.");

            this.ChildViewModels = new Dictionary<string, ScheduleChildViewModel>
            {
                {MySchedule, IoC.Get<MyScheduleViewModel>()},
                {Favorites, new MyFavoritesViewModel()},
                {Time, new RightAboutNowViewModel()},
                {StartingSoon, new StartingSoonViewModel()},
                {Upcoming, new UpcomingViewModel()}
            };

            this.ScheduleViews = new List<string>(this.ChildViewModels.Keys);
            this.ChildViewModels.Values.ForEach(l => l.SetViewModel(this));
            _childViewModel = this.ChildViewModels[MySchedule];

            //WithCoreDataPresent(Resume);

            this.InitialActivation = true;
        }

        private void Resume(ICoreData coreData)
        {
            this.AllSessions = coreData.Sessions;
            HasCoreDataLoaded = true;
            QueueLoadData();
        }

        private IEnumerable<string> _scheduleViews;

        public IEnumerable<string> ScheduleViews
        {
            get { return _scheduleViews; }
            set
            {
                if (_scheduleViews != value)
                {
                    _scheduleViews = value;
                    RaisePropertyChanged(() => ScheduleViews);
                }
            }
        }
        
        public ICommand ViewChangedCommand
        {
            get { return new RelayCommand<SelectionChangedEventArgs>(ViewChanged); }
        }

        private void ViewChanged(SelectionChangedEventArgs e)
        {
            if (0 == e.AddedItems.Count) return;

            // give the UI a chance to finish the listpicker closing transition
            // not the best way to do this really
            DispatchTimerUtil.OnWithDispatcher(TimeSpan.FromMilliseconds(150),
                () => this.ScheduleView = e.AddedItems[0].ToString());
        }

        private bool HasCoreDataLoaded { get; set; }

        protected override void OnCoreDataLoaded(ICoreData coreData)
        {
            this.HasCoreDataLoaded = true;
            this.AllSessions = coreData.Sessions;
            CheckAllFinished();
        }

        public bool InitialActivation { get; private set; }

        protected override void OnActivated()
        {
            // for favorites and "now" views we should reload / refresh upon back navigation or a panorama item switch
            var loader = this.ChildViewModels[this.ScheduleView];
            if (loader.IsRefreshNeeded)
                QueueLoadData();

            this.InitialActivation = false;
        }

        public List<SessionItemViewModel> AllSessions { get; private set; }

        private bool CanLoadData()
        {
            return null != _childViewModel && _childViewModel.CanLoadData();
        }

        private IEnumerable<int> _sessionIds;

        private IEnumerable<int> SessionIds
        {
            get { return _sessionIds; }
            set
            {
                if (_sessionIds != value)
                {
                    _sessionIds = value;
                    CheckAllFinished();
                }
            }
        }

        public void SetResult(IEnumerable<int> sessionIds, CompletedEventArgs e = null)
        {
            this.Error = (null == e || null == e.Error) ? null : new ErrorItemViewModel(e);
            this.SessionIds = sessionIds;

            //this.SetBusy(false); // no
        }

        private bool IsAllDone
        {
            get
            {
                var allDone = (this.HasCoreDataLoaded && this.SessionIds != null);
                return allDone;
            }
        }

        private void CheckAllFinished()
        {
            if (!IsAllDone) return;

            if (null != this.SessionIds && this.SessionIds.Any())
                SetBusy(true, "Displaying...");

            QueueSafeDispatch( () =>
            {
                var temp = new ObservableCollection<SessionItemViewModel>();

                if (this.SessionIds != null)
                {
                    this.SessionIds.ForEach(sessionId =>
                    {
                        var session = this.AllSessions.Where(s => s.SessionId == sessionId).FirstOrDefault();

                        if (null != session)
                            temp.Add(session);
                    });
                }
                //else 
                    //LogInstance.l


                this.MySessions = temp.OrderBy(s=> s.StartTime).ThenBy(s=> s.Title).ToObservableCollection();
                this.LastLoadTime = DateTime.Now;
                this.NoSessions = (null == this.MySessions || 0 == this.MySessions.Count);

                SetBusy(false);
            });
        }

        public DateTime? LastLoadTime { get; private set; }

        private void ClearData()
        {
            this.SessionIds = null; // not a new list
            this.MySessions = new ObservableCollection<SessionItemViewModel>();
            this.NotFoundText = string.Empty;
            this.NoSessions = false;
        }

        private void QueueLoadData()
        {
            if (IsInDesignMode)
            {
                LoadData();
                return;
            }
            
            if (CanLoadData())
                SetBusy(true, string.Format("Loading {0}...", this.ScheduleView));
            else
            {
                SetBusy(false);
            }

            ThreadPool.QueueUserWorkItem(delegate
            {
                SafeDispatch( LoadData);
            });
        }

        private void LoadData()
        {
            ClearData();

            if (ChildViewModels.ContainsKey(this.ScheduleView))
                _childViewModel.Load();

            // can't turn off busy here
        }

        private string _scheduleView = MySchedule;

        public string ScheduleView
        {
            get { return _scheduleView; }
            set
            {
                if (_scheduleView != value)
                {
                    _scheduleView = value;
                    RaisePropertyChanged(() => ScheduleView);
                    
                    IsMyScheduleView = (MySchedule == _scheduleView);
                    _childViewModel = !string.IsNullOrEmpty(_scheduleView) ? this.ChildViewModels[_scheduleView] : null;
                    QueueLoadData();
                }
            }
        }

        private ScheduleChildViewModel _childViewModel;

        private bool _isMyScheduleView = true;

        public bool IsMyScheduleView
        {
            get { return _isMyScheduleView; }
            set
            {
                if (_isMyScheduleView != value)
                {
                    _isMyScheduleView = value;
                    RaisePropertyChanged(() => IsMyScheduleView);
                }
            }
        }

        private ObservableCollection<SessionItemViewModel> _mySessions;

        public ObservableCollection<SessionItemViewModel> MySessions
        {
            get { return _mySessions; }
            set
            {
                if (_mySessions != value)
                {
                    _mySessions = value;
                    RaisePropertyChanged(() => MySessions);
                    NoSessions = (null == value || !value.Any());
                }
            }
        }

        private bool _noSessions; // = true;

        public bool NoSessions
        {
            get { return _noSessions; }
            set
            {
                if (_noSessions != value)
                {
                    _noSessions = value;
                    RaisePropertyChanged(() => NoSessions);
                }
            }
        }

        private string _notFoundText = "No sessions retrieved.";

        public string NotFoundText
        {
            get { return _notFoundText; }

            set
            {
                if (_notFoundText != value)
                {
                    _notFoundText = value;
                    RaisePropertyChanged(() => NotFoundText);
                }
            }
        }

        public ICommand SessionSelectedCommand
        {
            get { return new RelayCommand<SelectionChangedEventArgs>(SessionSelected); }
        }

        private static void SessionSelected(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;

            var session = (SessionItemViewModel)e.AddedItems[0];

            var navigationService = IoC.Get<INavigationService>();
            navigationService.NavigateTo(Uris.Session(session));

            return;
        }

        public ICommand RefreshCommand
        {
            get { return new RelayCommand(RefreshData); }
        }

        private void RefreshData()
        {
            _childViewModel.BeforeRefresh();
            QueueLoadData();
        }

        public MyScheduleViewModel MyScheduleViewModel
        {
            get
            {
                return (MyScheduleViewModel)this.ChildViewModels[MySchedule];
            }
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

        public void Backup(IDataStorage store)
        {
            store.Backup(() => this.ScheduleView, this.ScheduleView);
            store.Backup(() => this.IsMyScheduleView, this.IsMyScheduleView);
            store.Backup(() => this.NotFoundText, this.NotFoundText);
            store.Backup(() => this.SessionIds, this.SessionIds);
        }

        public void Restore(IDataStorage store)
        {
            // only want to restore if this is a tombstone scenario
            if (IoC.Get<IApp>().IsNewApp)
            {
                return;
            }

            _scheduleView = store.Restore(() => this.ScheduleView, MySchedule);
            _isMyScheduleView = store.Restore(() => this.IsMyScheduleView, true);
            _notFoundText = store.Restore(() => this.NotFoundText, _notFoundText);
            _sessionIds = store.Restore(() => this.SessionIds, null);
            _childViewModel = this.ChildViewModels[_scheduleView];
            WithCoreDataPresent(Resume);
        }

        public void ChildBusyChanged(bool isBusy, string busyText = "")
        {
            this.SetBusy(isBusy, busyText);
        }
    }
}
