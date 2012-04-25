using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Support;
using GalaSoft.MvvmLight.Command;
using Phone.Common.Extensions.System.Collections.Generic_;
using Phone.Common.Extensions.System_;
using Phone.Common.IO;
using Phone.Common.IOC;
using Phone.Common.Navigation;

namespace CodeStock.App.ViewModels
{
    public class SearchViewModel : AppViewModelBase
    {
        public SearchViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        private readonly INavigationService _navigationService;

        private string _searchText;

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

        public void Search()
        {
            var coreData = IoC.TryGet<ICoreData>();
            if (null == coreData || !coreData.IsLoaded)
            {
                MessageBox("Core data is not yet loaded; please try again in a bit.", "Not ready");
                return;
            }

            // e
            if (coreData.IsUnavailableFromError && (null == coreData.Sessions || !coreData.Sessions.Any()) && (null == coreData.Speakers || !coreData.Speakers.Any()) )
            {
                MessageBox("Search is unavailable due to an error loading core session and speaker data.", "Not Available");
                return;
            }

            var qry = this.SearchText;

            if (string.IsNullOrEmpty(qry) || qry.Trim().Length < 3)
            {
                MessageBox("Please enter at least 3 characters to search by.", "Required Data");
                return;
            }

            QueueSearch(coreData, qry);
        }

        private void QueueSearch(ICoreData coreData, string qry)
        {
            SetBusy(true, "Searching...");

            QueueSafeDispatch( () =>
            {
                var items = new ObservableCollection<SearchItemViewModel>();

                qry = qry.ToLower();

                coreData.Sessions.Where(s =>
                    s.Title.ToLower().Contains(qry) ||
                    s.Abstract.ToLower().Contains(qry) ||
                    s.Technology.ToLower().Contains(qry) ||
                    s.TrackArea.ToLower().Contains(qry) ||
                    (s.Speaker != null && s.Speaker.Name.ToLower().Contains(qry))
                    ).ForEach(s => items.Add(SearchItemViewModel.Create(s)));

                coreData.Speakers.Where(sp =>
                    sp.Name.ToLower().Contains(qry) ||
                    (sp.TwitterId != null && sp.TwitterId.ToLower().Contains(qry)) ||
                    (sp.Bio != null && sp.Bio.ToLower().Contains(qry)) ||
                    (sp.Website != null && sp.Website.ToLower().Contains(qry)) ||
                    (sp.Company != null && sp.Company.ToLower().Contains(qry))
                    ).ForEach(sp => items.Add(SearchItemViewModel.Create(sp)));

                this.Items = items;
                SetBusy(false);
            });
        }



        private bool Invalidated { get; set; }

        private ObservableCollection<SearchItemViewModel> _items;

        public ObservableCollection<SearchItemViewModel> Items
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

        public ICommand ItemSelectedCommand
        {
            get { return new RelayCommand<SelectionChangedEventArgs>(ItemSelected); }
        }

        private void ItemSelected(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;

            var item = (SearchItemViewModel) e.AddedItems[0];

            if (item.ViewModel is SessionItemViewModel)
                _navigationService.NavigateTo(Uris.Session(item.ViewModel.As<SessionItemViewModel>()));
            else if (item.ViewModel is SpeakerItemViewModel)
                _navigationService.NavigateTo(Uris.Speaker(item.ViewModel.As<SpeakerItemViewModel>()));

            return;
        }

        public void Backup(IDataStorage store)
        {
            store.Backup(() => this.SearchText, this.SearchText);
            store.Backup(() => this.Items, this.Items);
        }

        public void Restore(IDataStorage store)
        {
            this.SearchText = store.Restore(() => this.SearchText, string.Empty);
            this.Items = store.Restore(() => this.Items, new ObservableCollection<SearchItemViewModel>());
        }
    }
}
