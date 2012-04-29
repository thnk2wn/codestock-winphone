using System;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.Data;
using CodeStock.Data.ServiceAccess;

namespace CodeStock.App.ViewModels
{
    public class MapsViewModel : AppViewModelBase
    {
        private readonly MapService _mapService;

        public MapsViewModel(IApp app, MapService mapService)
        {
            if (IsInDesignModeStatic) return;

            if (null == app) throw new ArgumentNullException("app", "app cannot be null");
            if (string.IsNullOrWhiteSpace(app.MapsApiKey)) throw new NullReferenceException("map api key must be provided");

            _mapService = mapService;
            _mapService.AfterCompleted = AfterMapDataLoaded;
            _mapsApiKey = app.MapsApiKey;
            this.ZoomLevel = 17;

            QueueSafeDispatch(GetData);
        }

        private void GetData()
        {
            //var json = MapPointDefaults.Serialize();
            _mapService.Load();
        }

        private void AfterMapDataLoaded(CompletedEventArgs e)
        {
            this.Location = new GeoCoordinate(_mapService.ConferenceLocation.Latitude,
                _mapService.ConferenceLocation.Longitude);

            var points = new ObservableCollection<MapPointItemViewModel>();
            _mapService.Data.ToList().ForEach(p=> points.Add(new MapPointItemViewModel(p)));
            this.MapPoints = points;
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
    }
}
