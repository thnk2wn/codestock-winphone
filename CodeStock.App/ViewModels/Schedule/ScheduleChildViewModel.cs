using System;
using System.Collections.Generic;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.Data;

namespace CodeStock.App.ViewModels.Schedule
{
    public abstract class ScheduleChildViewModel : AppViewModelBase
    {
        protected ScheduleChildViewModel()
        {
            return;
        }

        public void SetViewModel(ScheduleViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel;
        }

        private ScheduleViewModel _parentViewModel;

        public abstract void Load();
        public abstract bool IsRefreshNeeded { get; }
        
        public string NotFoundText
        {
            get { return _parentViewModel.NotFoundText; }
            set
            {
                _parentViewModel.NotFoundText = value;
            }
        }

        public bool NoSessions
        {
            get { return _parentViewModel.NoSessions; }
            set
            {
                _parentViewModel.NoSessions = value;
            }
        }

        protected void SetResult(IEnumerable<int> sessionIds, CompletedEventArgs e = null)
        {
            // currently null == not complete which isn't ideal but can change later
            // really this is only for error situations
            if (null == sessionIds)
                sessionIds = new List<int>();

            _parentViewModel.SetResult(sessionIds, e);
        }

        protected IEnumerable<SessionItemViewModel> AllSessions
        {
            get { return _parentViewModel.AllSessions; }
        }

        public DateTime? LastLoadTime
        {
            get { return _parentViewModel.LastLoadTime; }
        }

        public bool InitialActivation
        {
            get { return _parentViewModel.InitialActivation; }
        }

        public virtual bool CanLoadData()
        {
            return true;
        }

        public virtual void BeforeRefresh()
        {
            return;
        }

        protected void RefreshData()
        {
            _parentViewModel.RefreshCommand.Execute(null);
        }

        protected override void SetBusy(bool isBusy)
        {
            base.SetBusy(isBusy);
            _parentViewModel.ChildBusyChanged(isBusy);
        }

        protected override void SetBusy(bool isBusy, string busyText)
        {
            base.SetBusy(isBusy, busyText);
            _parentViewModel.ChildBusyChanged(isBusy, busyText);
        }
    }
}
