using System;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.Data;
using CodeStock.Data.ServiceAccess;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using Phone.Common.Threading;
using Phone.Common.Windows;

namespace CodeStock.App.ViewModels
{
    public class MapsViewModel : AppViewModelBase
    {
        private readonly MapService _mapService; // should do an IMapService at some point
        private readonly IMessageBoxService _messageBoxService;

        public MapsViewModel(IApp app, MapService mapService, IMessageBoxService messageBoxService)
        {
            if (IsInDesignModeStatic) return;

            if (null == app) throw new ArgumentNullException("app", "app cannot be null");
            if (string.IsNullOrWhiteSpace(app.MapsApiKey)) throw new NullReferenceException("map api key must be provided");

            _mapService = mapService;
            _messageBoxService = messageBoxService;
            _mapService.AfterCompleted = AfterMapDataLoaded;
            _mapsApiKey = app.MapsApiKey;
            this.ZoomLevel = 17;

            QueueSafeDispatch(GetData);
        }

        private void GetData()
        {
            //var json = MapPointDefaults.Serialize(); // for creating initial data or recreating all
            _mapService.Load();
        }

        private void AfterMapDataLoaded(CompletedEventArgs e)
        {
            this.Location = new GeoCoordinate(_mapService.ConferenceLocation.Latitude,
                _mapService.ConferenceLocation.Longitude);
            
            var points = new ObservableCollection<MapPointItemViewModel>();
            _mapService.Data.OrderBy(x=> x.Label).ToList().ForEach(p=> points.Add(new MapPointItemViewModel(p, this)));

            this.MapPoints = points;
            this.SelectedJumpToPoint = MapPoints.Single(x => x.Label == _mapService.ConferenceLocation.Label);
        }

        private readonly string _mapsApiKey;

        public string MapsApiKey
        {
            get { return _mapsApiKey; }
        }

        private ObservableCollection<MapPointItemViewModel> _mapPoints;
        public ObservableCollection<MapPointItemViewModel> MapPoints
        {
            get { return _mapPoints; }
            set
            {
                if (_mapPoints != value)
                {
                    _mapPoints = value;
                    RaisePropertyChanged(()=> MapPoints);
                }
            }
        }

        private GeoCoordinate _location;
        public GeoCoordinate Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    RaisePropertyChanged("Location");
                }
            }
        }

        private double _zoomLevel;

        public double ZoomLevel
        {
            get { return _zoomLevel; }
            set
            {
                if (_zoomLevel != value)
                {
                    _zoomLevel = value;
                    RaisePropertyChanged("ZoomLevel");
                }
            }
        }

        public ICommand JumpToCommand
        {
            get { return new RelayCommand<SelectionChangedEventArgs>(JumpTo); }
        }

        private void JumpTo(SelectionChangedEventArgs e)
        {
            if (0 == e.AddedItems.Count) return;

            // give the UI a chance to finish the listpicker closing transition
            // not the best way to do this really
            DispatchTimerUtil.OnWithDispatcher(TimeSpan.FromMilliseconds(150),
                () =>
                {
                    var p = (MapPointItemViewModel) e.AddedItems[0];
                    this.SelectedJumpToPoint = this.MapPoints.Single(x => x.Label == p.Label);
                    this.Location = this.SelectedJumpToPoint.Location;
                });
        }

        private MapPointItemViewModel _selectedJumpToPoint;

        public MapPointItemViewModel SelectedJumpToPoint
        {
            get { return _selectedJumpToPoint; }
            set
            {
                if (_selectedJumpToPoint != value)
                {
                    _selectedJumpToPoint = value;
                    RaisePropertyChanged(()=> SelectedJumpToPoint);
                }
            }
        }

        public ICommand PushPinSelectedCommand
        {
            get { return new RelayCommand<MouseButtonEventArgs>(PushPinSelected); }
        }

        private void PushPinSelected(MouseButtonEventArgs e)
        {
            var label = ((TextBlock) e.OriginalSource).Text;
            var p = this.MapPoints.Single(x => x.Label == label);
            var msg = string.Format("{0}{1}{1}{2}{1}{1}Get directions to this location?", p.Description, Environment.NewLine, p.Address);
            
            var launchMapsApp = _messageBoxService.ShowOkCancel(msg, p.Label);
            if (!launchMapsApp) return;

            // this should be behind an interface w/IOC
            var task = new BingMapsDirectionsTask {End = new LabeledMapLocation(p.Label, p.Location)};
            task.Show();
        }

    }
}
