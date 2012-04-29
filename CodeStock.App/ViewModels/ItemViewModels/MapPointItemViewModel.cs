using System.Device.Location;
using CodeStock.Data.Model;

namespace CodeStock.App.ViewModels.ItemViewModels
{
    public class MapPointItemViewModel : AppViewModelBase
    {
        public MapPointItemViewModel(MapPoint point)
        {
            this.Label = point.Label;
            this.Location = new GeoCoordinate(point.Latitude, point.Longitude);
            this.Description = point.Description;
            this.BackgroundColor = point.BackgroundColor;
            this.ForegroundColor = point.ForegroundColor;
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
    }
}
