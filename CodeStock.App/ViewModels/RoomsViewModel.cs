using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Support;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls;
using Phone.Common.Extensions.System.Collections.Generic_;
using Phone.Common.IOC;
using Phone.Common.Navigation;

namespace CodeStock.App.ViewModels
{
    /// <summary>
    /// ViewModel for the map and list of all rooms which leads to specific room and sessions by room
    /// </summary>
    /// <remarks>
    /// Originally called rooms service but all the data needed for rooms is already available in session data.
    /// </remarks>
    public class RoomsViewModel : AppViewModelBase
    {
        public RoomsViewModel(INavigationService navigationService) : base(registerCoreData:true)
        {
            _navigationService = navigationService;
            WithCoreDataPresent(BindCoreData);
        }

        private readonly INavigationService _navigationService;

        private ObservableCollection<RoomItemViewModel> _items;
        public ObservableCollection<RoomItemViewModel> Items
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


        private void BindCoreData(ICoreData coreData)
        {
            if (IsInDesignMode) return;
            if (this.Items != null && this.Items.Any()) return;

            SafeDispatch(() =>
            {
                // you'd think .Distinct() is what we'd use but nope
                var distinctRoomNames =
                    coreData.Sessions.GroupBy(x => x.Room).Select(x => x.First().Room);
                this.Items = new ObservableCollection<RoomItemViewModel>();
                distinctRoomNames.OrderBy(n=> n).ForEach(roomName => this.Items.Add(new RoomItemViewModel {Name = roomName}));
            });
        }

        protected override void OnCoreDataLoaded(ICoreData coreData)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                BindCoreData(coreData);
            });
        }

        public ICommand RoomSelectedCommand
        {
            get { return new RelayCommand<GestureEventArgs>(RoomSelected); }
        }

        private void RoomSelected(GestureEventArgs e)
        {
            var elem = (FrameworkElement)e.OriginalSource;
            var model = elem.DataContext as RoomItemViewModel;

            if (null == model) return;
            _navigationService.NavigateTo(Uris.Room(model));
        }

        public ICommand MapSelectedCommand
        {
            get { return new RelayCommand(MapSelected); }
        }

        private static void MapSelected()
        {
            // considered using image gesture libary to allow pinch and zoom but had issues with it
            const string largeMapUrl = "http://www.geoffhudik.com/storage/blogs/tech/2011/05/CodeStockRoomMapLarge.png";
            IoC.Get<INavigationService>().NavigateTo(Uris.WebBrowser(), new WebBrowserArgs(largeMapUrl, SupportedPageOrientation.Landscape));
        }

    }
}
