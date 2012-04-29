using System.Collections.Generic;
using CodeStock.Data.Model;
using Newtonsoft.Json;

namespace CodeStock.Data
{
    public static class MapPointDefaults
    {
        private static readonly List<MapPoint> _data = new List<MapPoint>
        {
            Create(35.962393, -83.921241, "Conference", "UT Conference Center", "600 Henley Street", "Green"),
            Create(35.962481, -83.920170, "Hilton", "Official hotel. Starbucks, Orange Martini.", "501 West Church Avenue", "Navy", "White"),
            Create(35.96275, -83.919754, "Locust St Parking", "Locust Street Parking Garage", "500 Locust Street", "Orange", "White"),
            Create(35.965294, -83.919647, "Market Square", "Market Square - several restauraunts, bars and shops.", "1 Market Square SW"),
            Create(35.971017, -83.917469, "Barleys", "Barley's Taproom and Pizzeria; lots of beer on tap", "200 East Jackson Avenue"),
            Create(35.965336, -83.918262, "Downtown Grill & Brewery", "Local beer and great food", "424 South Gay Street"),
            Create(35.969585, -83.918237, "Crown & Goose", "London style gastropub", "123 South Central Street")
        };

        private static int _ordinal = 0;

        private static MapPoint Create(double latitude, double longitude, string label, 
            string description, string address, string backgroundColor = "Yellow", string foregroundColor = "Black")
        {
            return new MapPoint
                       {
                           Ordinal = ++_ordinal, 
                           Latitude = latitude, 
                           Longitude = longitude,
                           Label = label,
                           BackgroundColor = backgroundColor,
                           ForegroundColor = foregroundColor,
                           Description = description,
                           Address = address
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
