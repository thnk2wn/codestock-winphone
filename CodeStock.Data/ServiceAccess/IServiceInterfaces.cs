using System;
using System.Collections.Generic;
using CodeStock.Data.Model;

namespace CodeStock.Data.ServiceAccess
{
    public interface IService
    {
        void Load();

        Action<CompletedEventArgs> AfterCompleted { get; set; }

        TimeSpan CacheDuration { get; set; }

        bool IsDataExpired();

        CompletedEventArgs CompletedInfo { get; }
    }

    public interface IService<T> : IService
    {
        IEnumerable<T> Data { get; }
    }

    public interface ISessionsService : IService<Session>
    {
        void EnsureCached();
    }

    public interface ISpeakersService : IService<Speaker>
    {
        void EnsureCached();
    }

    public interface IUserIdLookupService
    {
        int? UserId { get; }
        bool NotFound { get;  }
        void Lookup(string email);
        Action<CompletedEventArgs> AfterCompleted { get; set; }
        bool IsBusy { get; }
    }

    public interface IScheduleService
    {
        void Load(int scheduleId);
        IEnumerable<int> SessionIds { get; }
        Action<CompletedEventArgs> AfterCompleted { get; set; }
        TimeSpan CacheDuration { get; set; }
        void ClearCache();
    }

    public interface ITwitterSearchService
    {
        void Search(string query);
        TwitterSearchResult Result { get; }
        Action<CompletedEventArgs> AfterCompleted { get; set; }
    }

}
