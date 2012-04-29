using System.Collections.Generic;
using System.Linq;
using Serialization;

namespace CodeStock.Data.Model
{
    public class MapPoint
    {
        public int Ordinal { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string BackgroundColor { get; set; }
        public string ForegroundColor { get; set; }
        public string Address { get; set; }
    }

    [Serializer(typeof(IEnumerable<MapPoint>))]
    public sealed class MapPointSerializer : ISerializeObject
    {
        public object[] Serialize(object target)
        {
            var tmp = (IEnumerable<MapPoint>)target;
            return new object[] { tmp.ToList() };
        }

        public object Deserialize(object[] data)
        {
            var tmp = new List<MapPoint>();

            var items = (List<MapPoint>)data[0];
            items.ForEach(tmp.Add);

            return tmp;
        }
    }
}
