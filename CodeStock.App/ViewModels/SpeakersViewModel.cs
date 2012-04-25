using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Support;
using GalaSoft.MvvmLight.Command;
using Phone.Common.Diagnostics.Logging;
using Phone.Common.Extensions.System.Collections.Generic_;
using Phone.Common.IOC;
using Phone.Common.Navigation;

namespace CodeStock.App.ViewModels
{
    public class SpeakersViewModel : AppViewModelBase
    {
        public SpeakersViewModel(INavigationService navigationService) : base(registerCoreData:true)
        {
            SetBusy(true, "Waiting for speaker data...");

            if (IsInDesignMode)
                DesignTimeLoad();
            else
            {
                _navigationService = navigationService;
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
                LogInstance.LogDebug("Core data present; binding speaker data");
                this.Items = coreData.Speakers.ToObservableCollection();
                //this.BusyText = string.Empty;
                LogInstance.LogDebug("Speaker data set in ViewModel");
                coreData.SpeakersBound = true;
                IsDataBound = true;
                this.Error = ErrorItemViewModel.FromCompletion(coreData.SpeakerCompletedInfo);
            });
        }

        private bool IsDataBound { get; set; }

        private ObservableCollection<SpeakerItemViewModel> _items;
        public ObservableCollection<SpeakerItemViewModel> Items
        {
            get { return _items; }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    RaisePropertyChanged(() => Items);
                }
            }
        }

        private void DesignTimeLoad()
        {
            var coreData = IoC.Get<ICoreData>();
            OnCoreDataLoaded(coreData);
        }

        public ICommand SpeakerSelectedCommand
        {
            get { return new RelayCommand<SelectionChangedEventArgs>(SpeakerSelected); }
        }

        private void SpeakerSelected(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;

            var speaker = (SpeakerItemViewModel)e.AddedItems[0];
            var uri = Uris.Speaker(speaker);
            _navigationService.NavigateTo(uri);

            return;
        }

        public ICommand SpeakersLoadedCommand
        {
            get { return new RelayCommand(AfterSpeakersLoaded); }
        }

        private void AfterSpeakersLoaded()
        {
            if (this.IsBusy && IsDataBound)
            {
                // i.e. layoutupdated, bound in UI
                this.BusyText = string.Empty;
                this.IsBusy = false;
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
    }
}
