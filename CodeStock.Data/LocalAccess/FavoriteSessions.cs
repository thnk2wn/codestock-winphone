using System.Collections.Generic;
using System.Diagnostics;
using Phone.Common.IO;

namespace CodeStock.Data.LocalAccess
{
    public class FavoriteSessions
    {
        private const string CacheKey = "FavoriteSessions";

        public FavoriteSessions()
        {
            this.SessionIds = new List<int>();
            Debug.WriteLine("FavoriteSessions ctor");
        }

        public List<int> SessionIds { get; private set; }

        public bool IsFavorite(int sessionId)
        {
            return (null != this.SessionIds && this.SessionIds.Contains(sessionId));
        }

        public void SetFavorite(int sessionId)
        {
            if (null != this.SessionIds && !this.SessionIds.Contains(sessionId))
            {
                this.SessionIds.Add(sessionId);
                Save();
            }
        }

        public void RemoveFavorite(int sessionId)
        {
            if (null != this.SessionIds && this.SessionIds.Contains(sessionId))
            {
                this.SessionIds.Remove(sessionId);
                Save();
            }
        }

        public void ToggleFavorite(int sessionId)
        {
            if (!IsFavorite(sessionId))
                SetFavorite(sessionId);
            else
                RemoveFavorite(sessionId);
        }

        public void Load()
        {
            if (Cache.Current.Contains(CacheKey))
            {
                this.SessionIds = Cache.Current.Get<List<int>>(CacheKey);
                HasLoaded = true;
            }
        }

        public bool HasLoaded { get; private set; }

        public void Save()
        {
            Cache.Current.Add(CacheKey, this.SessionIds, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration);
        }
    }
}
