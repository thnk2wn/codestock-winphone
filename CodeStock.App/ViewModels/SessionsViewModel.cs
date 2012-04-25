using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Support;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls;
using Phone.Common.Collections;
using Phone.Common.Diagnostics.Logging;
using Phone.Common.Extensions.System.Collections.Generic_;
using Phone.Common.IOC;
using Phone.Common.Navigation;

namespace CodeStock.App.ViewModels
{
    public class SessionsViewModel : AppViewModelBase
    {
        public SessionsViewModel(INavigationService navigationService) : base(registerCoreData:true)
        {
            this.BusyText = "Waiting for session data...";

            if (IsInDesignMode)
                DesignTimeLoad();
            else
            {
                IsBusy = true;
                _navigationService = navigationService;
                
                // core data is loaded prior to this point. if it is available now we'll use it (i.e. tombstone)
                // otherwise it'll come in on OnCoreDataLoaded()
                WithCoreDataPresent(BindCoreData);
            }
        }

        private readonly INavigationService _navigationService;

        protected override void OnCoreDataLoaded(ICoreData coreData)
        {
            BindCoreData(coreData);
        }

        private void BindCoreData(ICoreData coreData)
        {
            this.BusyText = "Displaying...";

            QueueSafeDispatch(() =>
            {
                LogInstance.LogDebug("Core data present; binding session data");
                this.Items = coreData.Sessions.ToObservableCollection();

                LogInstance.LogDebug("Session data set on ViewModel");
                coreData.SessionsBound = true;
                IsDataBound = true;
                this.Error = ErrorItemViewModel.FromCompletion(coreData.SessionCompletedInfo);
            });
        }

        private bool IsDataBound { get; set; }

        private ObservableCollection<SessionItemViewModel> _items;
        public ObservableCollection<SessionItemViewModel> Items
        {
            get { return _items; }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    RaisePropertyChanged(() => Items);
                    SetGroupedItems();
                }
            }
        }

        public bool IsLoaded
        {
            get
            {
                return (null != this.Items && this.Items.Any());
            }
        }

        private void SetGroupedItems()
        {
            this.GroupedItems =
                from item in this.Items
                group item by item.TrackArea into i
                orderby i.Key
                select new Group<SessionItemViewModel>(i.Key, i);
        }

        private IEnumerable<Group<SessionItemViewModel>> _groupedItems;
        public IEnumerable<Group<SessionItemViewModel>> GroupedItems
        {
            get { return _groupedItems; }
            set
            {
                if (_groupedItems != value)
                {
                    _groupedItems = value;
                    RaisePropertyChanged(() => GroupedItems);
                }
            }
        }

        public ICommand SessionsLoadedCommand
        {
            get { return new RelayCommand(AfterSessionsLoaded); }
        }

        private void AfterSessionsLoaded()
        {
            if (this.IsBusy && IsDataBound)
            {
                // i.e. layoutupdated, bound in UI
                this.BusyText = string.Empty;
                this.IsBusy = false;
            }
        }

        public ICommand SessionSelectedCommand
        {
            get { return new RelayCommand<GestureEventArgs>(SessionSelected); }
        }

        private void SessionSelected(GestureEventArgs e)
        {
            var elem = (FrameworkElement)e.OriginalSource;
            var vm = elem.DataContext as SessionItemViewModel;

            // i.e. group header clicked
            if (null == vm) return;

            _navigationService.NavigateTo(Uris.Session(vm));
        }

        private void DesignTimeLoad()
        {
            var coreData = IoC.Get<ICoreData>();
            OnCoreDataLoaded(coreData);
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
    }
}
