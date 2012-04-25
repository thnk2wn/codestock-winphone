using System;
using System.Collections.Generic;
using System.Linq;
using CodeStock.Data.Model;

namespace CodeStock.Data.ServiceAccess
{
    public class SpeakersService : CodeStockService<Speaker>, ISpeakersService
    {
        private const string SpeakersUrl = "http://codestock.org/api/v2.0.svc/AllSpeakersJson";
        internal const string SpeakersCacheKey = "Speakers";

        public SpeakersService(TimeSpan cacheDuration)
            : base(SpeakersCacheKey, cacheDuration, "AllSpeakersJson.txt")
        {
            this.AutoAddToCache = false;
            return;
        }

        public void Load()
        {
            this.Load(SpeakersUrl);
        }

        protected override IEnumerable<Speaker> PostProcessData(IEnumerable<Speaker> data)
        {
            return data.OrderBy(s => s.Name);
        }
    }
}
