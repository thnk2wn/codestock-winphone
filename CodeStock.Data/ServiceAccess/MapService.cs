using System;
using System.Collections.Generic;
using CodeStock.Data.Model;
using System.Linq;

namespace CodeStock.Data.ServiceAccess
{
    public class MapService : CodeStockService<MapPoint>
    {
        private const string MapUrl = "http://geoffhudik.com/codestock/map.json";
        internal const string MapCacheKey = "MapPointData";

        public MapService(TimeSpan cacheDuration)
            : base(MapCacheKey, cacheDuration)
        {
        }

        public void Load()
        {
            this.Load(MapUrl);
        }

        protected override IEnumerable<MapPoint> PostProcessData(IEnumerable<MapPoint> data)
        {
            return data.OrderBy(x => x.Ordinal);
        }

        public MapPoint ConferenceLocation
        {
            get { return this.Data.Single(x => "Conference" == x.Label); }
        }
    }
}
