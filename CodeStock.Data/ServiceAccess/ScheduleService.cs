using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Phone.Common.Diagnostics.Logging;
using Phone.Common.IO;
using Phone.Common.Net;

namespace CodeStock.Data.ServiceAccess
{
    public class ScheduleService : WebServiceBase, IScheduleService
    {
        private const string URL = "http://codestock.org/api/v2.0.svc/GetScheduleJson?ScheduleID={0}";
        internal const string CacheKey = "Schedule";

        public ScheduleService(TimeSpan cacheDuration)
        {
            this.CacheDuration = cacheDuration;
        }

        private CacheInfo _cacheInfo;

        public void Load(int scheduleId)
        {
            _cacheInfo = Cache.Current.Info(CacheKey);

            LogInstance.LogInfo("Schedule cache state is {0}, duration is {1} hrs", _cacheInfo.State, this.CacheDuration.TotalHours);

            if (_cacheInfo.State == CacheStates.Exists)
            {
                LoadFromCache();
                return;
            }

            var url = string.Format(URL, scheduleId);
            MakeRequest(url);
        }

        private void LoadFromCache(bool expiredOkay = false)
        {
            this.SessionIds = Cache.Current.Get<IEnumerable<int>>(_cacheInfo, expiredOkay);
            OnAfterCompleted(new CompletedEventArgs(null));
        }

        public IEnumerable<int> SessionIds { get; private set; }

        public TimeSpan CacheDuration { get; set; }

        protected override void AfterRequestCompleted(string result)
        {
            var root = JsonConvert.DeserializeObject<ScheduleResult>(result);
            this.SessionIds = root.d;

            if (this.SessionIds != null && this.SessionIds.Any())
            {
                Cache.Current.Add(_cacheInfo, Cache.NoAbsoluteExpiration, this.CacheDuration, this.SessionIds.ToList());
            }

            OnAfterCompleted(new CompletedEventArgs());
        }

        protected override void AfterRequestFailure(RequestFailure failure)
        {
            // if we have something in cache, load that, even if expired; better than no data at all
            if (_cacheInfo.State != CacheStates.NotFound)
            {
                LoadFromCache(expiredOkay:true);
            }

            // let base log error, raise completed
            base.AfterRequestFailure(failure);
        }

        public void ClearCache()
        {
            if (Cache.Current.Contains(CacheKey))
                Cache.Current.Remove(CacheKey);
        }
    }

    public class ScheduleResult
    {
        public IList<int> d { get; set; }
    }
}
