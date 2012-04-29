using System.Collections.Generic;
using CodeStock.Data.Model;
using Newtonsoft.Json;

namespace CodeStock.Data
{
    public static class MapPointDefaults
    {
        private static readonly List<MapPoint> _data = new List<MapPoint>
        {
            Create(35.962393, -83.921241, "Conference", "UT Conference Center", "Green"),
            Create(35.96275, -83.919754, "Hilton", "Official hotel"),
            Create(35.96275, -83.919754, "Park", "Locust Street Parking Garage"),
            Create(35.965294, -83.919647, "Mkt Sq", "Market Square"),
            Create(35.971017, -83.917469, "Barleys", "Barley's Taproom and Pizzeria"),
            Create(35.965336, -83.918262, "Downtown Grill & Brewery", "Local beer and great food")
        };

        private static int _ordinal = 0;

        private static MapPoint Create(double latitude, double longitude, string label, 
            string description = null, string backgroundColor = "Yellow", string foregroundColor = "Black")
        {
            return new MapPoint
                       {
                           Ordinal = ++_ordinal, 
                           Latitude = latitude, 
                           Longitude = longitude,
                           Label = label,
                           BackgroundColor = backgroundColor,
                           ForegroundColor = foregroundColor,
                           Description = description
                       };
        }

        public static IEnumerable<MapPoint> Data
        {
            get { return _data; }
        }

        public static string Serialize()
        {
            var model = new ModelBase<MapPoint> {d = _data};
            var result = JsonConvert.SerializeObject(model, Formatting.Indented);
            return result;
        }
    }
}
