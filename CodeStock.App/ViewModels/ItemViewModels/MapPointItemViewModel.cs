using System.Device.Location;
using CodeStock.Data.Model;

namespace CodeStock.App.ViewModels.ItemViewModels
{
    public class MapPointItemViewModel : AppViewModelBase
    {
        private readonly MapsViewModel _parent;

        public MapPointItemViewModel(MapPoint point, MapsViewModel parent)
        {
            _parent = parent;
            this.Label = point.Label;
            this.Location = new GeoCoordinate(point.Latitude, point.Longitude);
            this.Description = point.Description;
            this.BackgroundColor = point.BackgroundColor;
            this.ForegroundColor = point.ForegroundColor;
            this.Address = point.Address;
        }

        private GeoCoordinate _location;

        public MapsViewModel Parent
        {
            get { return _parent; }
        }

        public GeoCoordinate Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    RaisePropertyChanged(()=> Location);
                }
            }
        }

        private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                if (_label != value)
                {
                    _label = value;
                    RaisePropertyChanged(()=> Label);
                }
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged(() => Description);
                }
            }
        }

        private string _backgroundColor;
        public string BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                if (_backgroundColor != value)
                {
                    _backgroundColor = value;
                    RaisePropertyChanged(()=> BackgroundColor);
                }
            }
        }

        private string _foregroundColor;
        public string ForegroundColor
        {
            get { return _foregroundColor; }
            set
            {
                if (_foregroundColor != value)
                {
                    _foregroundColor = value;
                    RaisePropertyChanged(()=> ForegroundColor);
                }
            }
        }


        private string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                if (_address != value)
                {
                    _address = value;
                    RaisePropertyChanged(() => Address);
                }
            }
        }
    }
}
