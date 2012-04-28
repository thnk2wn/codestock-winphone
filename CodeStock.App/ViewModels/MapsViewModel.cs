using System;
using System.Device.Location;

namespace CodeStock.App.ViewModels
{
    public class MapsViewModel : AppViewModelBase
    {
        public MapsViewModel(IApp app)
        {
            if (AppViewModelBase.IsInDesignModeStatic) return;

            if (null == app) throw new ArgumentNullException("app", "app cannot be null");
            if (string.IsNullOrWhiteSpace(app.MapsApiKey)) throw new NullReferenceException("map api key must be provided");
            _mapsApiKey = app.MapsApiKey;

            this.ZoomLevel = 17;
            this.Location = new GeoCoordinate(35.962393, -83.921241);
        }

        private readonly string _mapsApiKey;

        public string MapsApiKey
        {
            get { return _mapsApiKey; }
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
