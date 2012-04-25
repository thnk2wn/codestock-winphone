using System;
using CodeStock.App.Core;
using CodeStock.Data.LocalAccess;
using CodeStock.Data.ServiceAccess;
using Ninject.Modules;
using Phone.Common.IOC;
using Phone.Common.Navigation;

namespace CodeStock.App.IOC
{
    public class ServicesModule : NinjectModule
    {
        public override void Load()
        {
            //var days = App.ConferenceCountdown.TotalDays;
            var settings = IoC.Get<AppSettings>();

            var tsSession = TimeSpan.FromHours(settings.SessionCacheDuration);
            var tsSpeaker = TimeSpan.FromHours(settings.SpeakerCacheDuration);
            var tsSchedule = TimeSpan.FromHours(settings.ScheduleCacheDuration);

            Bind<ISessionsService>().ToMethod((c) => new SessionsService(tsSession));
            Bind<ISpeakersService>().ToMethod((c) => new SpeakersService(tsSpeaker));

            Bind<IUserIdLookupService>().To<UserIdLookupService>();
            Bind<IScheduleService>().ToMethod((c) => new ScheduleService(tsSchedule));

            Bind<ITwitterSearchService>().To<TwitterSearchService>();

            Bind<INavigationService>().To<NavigationService>();

            Bind<FavoriteSessions>().ToMethod((c) =>
            {
                var s = new FavoriteSessions();
                s.Load();
                return s;
            }).InSingletonScope();
        }
    }
}
