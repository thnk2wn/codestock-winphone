using System;
using System.Collections.Generic;
using System.Linq;
using CodeStock.Data.Model;

namespace CodeStock.Data.ServiceAccess
{
    public class SessionsService : CodeStockService<Session>, ISessionsService
    {
        private const string URL = @"http://codestock.org/api/v2.0.svc/AllSessionsJson";
        internal const string SESSION_KEY = "Sessions";

        public SessionsService(TimeSpan cacheDuration) : base(SESSION_KEY, cacheDuration, "AllSessionsJson.txt")
        {
            this.AutoAddToCache = false;
            return;
        }

        public void Load()
        {
            this.Load(URL);
        }

        protected override IEnumerable<Session> PostProcessData(IEnumerable<Session> data)
        {
            return data.OrderBy(s => s.Track).ThenBy(s => s.Title);
        }
    }
}
