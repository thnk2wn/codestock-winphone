using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeStock.Data.Model;
using Newtonsoft.Json;
using Phone.Common.Diagnostics;
using Phone.Common.Diagnostics.Logging;
using Phone.Common.IO;

namespace CodeStock.Data.ServiceAccess
{
    public abstract class CodeStockService<T> : WebServiceBase
    {
        public TimeSpan CacheDuration { get; set; }

        protected string CacheKey { get; set; }
        protected string CacheResourceFile { get; set; }

        protected bool AutoAddToCache { get; set; }
        protected bool EnableResourceLoad { get; set; }

        protected CodeStockService(string cacheKey, TimeSpan cacheDuration, string cacheResourceFile = null)
        {
            this.CacheKey = cacheKey;
            this.CacheDuration = cacheDuration;
            this.CacheResourceFile = cacheResourceFile;
            this.ChildType = this.GetType().Name;
            this.AutoAddToCache = true;
        }

        private string ChildType { get; set; }

        public bool IsDataExpired()
        {
            var info = Cache.Current.Info(CacheKey);
            return (info.State != CacheStates.Exists);
        }

        private DateTime _startTime;
        private CacheInfo _cacheInfo;

        protected void Load(string url)
        {
            _startTime = DateTime.Now;
            _cacheInfo = Cache.Current.Info(CacheKey);

            LogInstance.LogInfo("{0} cache state is {1}, duration is {2} hrs", this.ChildType, _cacheInfo.State, this.CacheDuration.TotalHours);

            if (_cacheInfo.State == CacheStates.Exists)
            {
                LoadDataFromCache();

                if (null != Data)
                {
                    OnAfterCompleted(new CompletedEventArgs());
                    return;
                }
            }

            // not in cache or expired
            MakeRequest(url);
        }

        private void LoadDataFromCache(bool expiredOkay = false)
        {
            LogInstance.LogInfo("{0}: Loading data found in cache; not expired", this.ChildType);
            var timer = CodeTimer.StartNew();
            this.Data = Cache.Current.Get<IEnumerable<T>>(_cacheInfo, expiredOkay);
            LogInstance.LogInfo("{0}: Loaded data found in cache in {1:#.000} seconds", this.ChildType, timer.Stop());
        }

        protected override void AfterRequestCompleted(string result)
        {
            LogInstance.LogInfo("{0}: Service call complete; {1:###,##0} bytes of JSON returned.", this.ChildType, result.Length);

            var root = ParseJson(result);
            this.Data = PostProcessData(root.d);

            OnAfterCompleted(new CompletedEventArgs());
        }

        protected ModelBase<T> ParseJson(string result)
        {
            var json = result.TrimEnd();

            // removed use of JsonSerializerSettings for performance and it was more for unusual error troubleshooting
            var timer = CodeTimer.StartNew();
            var root = JsonConvert.DeserializeObject<ModelBase<T>>(json);
            LogInstance.LogDebug("{0}: Json parsed in {1:#.000} seconds", this.GetType().Name, timer.Stop());
            return root;
        }

        protected virtual IEnumerable<T> PostProcessData(IEnumerable<T> data)
        {
            return data;
        }

        protected virtual void PreProcessError(Exception error)
        {
            // load even if expired
            if (_cacheInfo.State != CacheStates.NotFound)
            {
                LoadDataFromCache(expiredOkay:true);
                OnAfterCompleted(new CompletedEventArgs());
                return;
            }

            if (EnableResourceLoad)
            {
                // in the event of no data cached and an error preventing no network data, attempt 
                // to load data from resource file even through it may be out of date
                LoadDataFromResource();
            }
        }

        private void LoadDataFromResource()
        {
            if (string.IsNullOrEmpty(this.CacheResourceFile)) return;

            LogInstance.LogInfo("{0}: Getting JSON from local resource file", this.ChildType);
            var res = new ResourceHelper();
            var info = res.GetResource("CodeStockData", this.CacheResourceFile);

            if (null == info)
                throw new NullReferenceException(string.Format("Failed to find resource '{0}'", this.CacheResourceFile));

            string json;

            using (var reader = new StreamReader(info.Stream))
            {
                json = reader.ReadToEnd();
            }

            var root = ParseJson(json);
            this.Data = PostProcessData(root.d);
        }


        public IEnumerable<T> Data { get; protected set; }

        protected override void OnAfterCompleted(CompletedEventArgs e)
        {
            if (null == e.Error && this.Data.Any())
            {
                if (_cacheInfo.State != CacheStates.Exists)
                {
                    if (AutoAddToCache)
                        AddToCache();
                    else
                        this.CacheNeeded = true;
                }
            }
            else
            {
                PreProcessError(e.Error);
            }

            var ts = DateTime.Now - _startTime;
            LogInstance.LogInfo("{0} completed in {1:#.000} seconds", this.ChildType, ts.TotalSeconds);

            this.CompletedInfo = e;

            if (null != AfterCompleted)
                AfterCompleted(e);
        }

        private bool CacheNeeded { get; set; }

        public void EnsureCached()
        {
            if (this.CacheNeeded)
            {
                AddToCache();
                this.CacheNeeded = false;
            }
        }

        private void AddToCache()
        {
            // don't do any type of thread work here; that'll be left to caller
            var timer = CodeTimer.StartNew();
            var list = this.Data.ToList();
            //Cache.Current.Add(CacheKey, list, Cache.NoAbsoluteExpiration, this.CacheDuration);
            Cache.Current.Add(_cacheInfo, Cache.NoAbsoluteExpiration, this.CacheDuration, list);
            LogInstance.LogInfo("Added {0} to cache in {1:#.000} seconds", this.GetType().Name, timer.Stop());
        }

        public CompletedEventArgs CompletedInfo { get; protected set; }
    }
}
