using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CodeStock.Data.ServiceAccess;
using GalaSoft.MvvmLight.Command;
using Phone.Common.IO;
using Phone.Common.Text;

namespace CodeStock.App.ViewModels.Schedule
{
    public class MyScheduleViewModel : ScheduleChildViewModel
    {
        private const string EmailSetting = "EmailAdress";
        private const string UserIdSetting = "UserId";

        public MyScheduleViewModel(IScheduleService scheduleService, IUserIdLookupService userIdLookupService, 
            ISettingsHelper settingsHelper)
        {
            IsInitializing = true;
            _scheduleService = scheduleService;
            _userIdLookupService = userIdLookupService;
            _settingsHelper = settingsHelper;

            // consider more robust, using isolated storage file, DI/IOC etc. for app settings like this
            if (!IsInDesignMode)
            {
                ReadSettings();
            }

            IsInitializing = false;
        }

        private bool IsInitializing { get; set; }

        private readonly IScheduleService _scheduleService;
        private readonly IUserIdLookupService _userIdLookupService;

        private readonly ISettingsHelper _settingsHelper;

        public override void Load()
        {
            this.NotFoundText = "No sessions found in your schedule. Add sessions at codestock.org";
            if (null == this.UserId || this.UserId <= 0)
            {
                SetBusy(false);
                this.NoSessions = true;
                this.NotFoundText = "Enter a schedule id then tap the screen. Or enter an email address and tap the arrow to lookup a schedule id." 
                    + Environment.NewLine + Environment.NewLine + "This requires personal schedule creation on the codestock.org website.";
                SetResult(null, null);
                return;
            }

            IEnumerable<int> sessionIds = null;

            _scheduleService.AfterCompleted = (e) =>
            {
                if (null != _scheduleService.SessionIds)
                {
                    sessionIds = _scheduleService.SessionIds.ToList();
                    SetBusy(true, "Displaying...");
                }
                else
                {
                    SetBusy(false);
                }

                
                SetResult(sessionIds, e);
            };

            _scheduleService.Load(this.UserId.Value);
        }

        public override bool IsRefreshNeeded
        {
            get { return InitialActivation; }
        }

        public ICommand LookupUserIdCommand
        {
            get { return new RelayCommand(LookupUserId); }
        }

        private void LookupUserId()
        {
            if (string.IsNullOrEmpty(this.EmailAddress))
            {
                MessageBox("Please enter an email address to lookup schedule / user id.", "Required Data");
                return;
            }

            if (!RegexUtilities.IsValidEmail(this.EmailAddress))
            {
                MessageBox("Please enter a valid email address.", "Invalid data");
                return;
            }

            if (_userIdLookupService.IsBusy)
                return;

            _userIdLookupService.AfterCompleted += (e) => SafeDispatch(() =>
            {
                SetBusy(false);

                if (null == e.Error)
                {
                    if (_userIdLookupService.NotFound)
                    {
                        MessageBox(string.Format("No user / schedule id returned for email address '{0}'", this.EmailAddress), "Not Found");
                    }
                    else
                    {
                        this.UserId = _userIdLookupService.UserId;
                    }
                }
            });

            SetBusy(true, "Fetching User / Schedule Id...");
            _userIdLookupService.Lookup(this.EmailAddress);
        }

        private int? _userId;

        public int? UserId
        {
            get { return _userId; }
            set
            {
                if (_userId != value)
                {
                    if (value.HasValue && value > 0)
                        this.UserIdWatermark = string.Empty;
                    else
                        this.UserIdWatermark = "Sch. Id";

                    // if we changed from one user id to another, clear the cache
                    //if (_userId != null)
                    if (!IsInitializing)
                        _scheduleService.ClearCache();

                    _userId = value;
                    RaisePropertyChanged(() => UserId);

                    if (!IsInDesignMode)
                        _settingsHelper.SetSetting(UserIdSetting, value);

                    if (!IsInitializing)
                        RefreshData();
                }
            }
        }

        private string _emailAddress;

        public string EmailAddress
        {
            get { return _emailAddress; }
            set
            {
                if (_emailAddress != value)
                {
                    _emailAddress = value;
                    RaisePropertyChanged(() => EmailAddress);

                    if (!IsInDesignMode)
                        _settingsHelper.SetSetting(EmailSetting, value);
                    //LookupUserId();
                }
            }
        }

        private string _userIdWaterMark = "Sch. Id";

        public string UserIdWatermark
        {
            get { return _userIdWaterMark; }

            set
            {
                if (_userIdWaterMark != value)
                {
                    _userIdWaterMark = value;
                    RaisePropertyChanged(() => UserIdWatermark);
                }
            }
        }

        private void ReadSettings()
        {
            this.EmailAddress = _settingsHelper.GetString(EmailSetting);
            this.UserId = _settingsHelper.GetInt(UserIdSetting);
        }

        public override bool CanLoadData()
        {
            return !(null == this.UserId || this.UserId <= 0);
        }

        public override void BeforeRefresh()
        {
            _scheduleService.ClearCache();
        }
    }
}
