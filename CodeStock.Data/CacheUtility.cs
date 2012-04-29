using CodeStock.Data.ServiceAccess;
using Phone.Common.IO;

namespace CodeStock.Data
{
    public class CacheUtility
    {
        public static void Clear()
        {
            RemoveFromCache(ScheduleService.CacheKey);
            RemoveFromCache(SessionsService.SESSION_KEY);
            RemoveFromCache(SpeakersService.SpeakersCacheKey);
            RemoveFromCache(MapService.MapCacheKey);
        }

        private static void RemoveFromCache(string key)
        {
            if (Cache.Current.Contains(key))
                Cache.Current.Remove(key);
        }
    }
}
