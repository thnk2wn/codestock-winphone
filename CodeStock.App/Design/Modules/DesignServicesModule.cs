using CodeStock.App.Design.Services;
using CodeStock.Data.ServiceAccess;
using Ninject.Modules;
using Phone.Common.Navigation;

namespace CodeStock.App.Design.Modules
{
    public class DesignServicesModule : NinjectModule
    {

        public override void Load()
        {
            Bind<ISessionsService>().To<DesignSessionService>();
            Bind<ISpeakersService>().To<DesignSpeakersService>();
            Bind<IUserIdLookupService>().To<DesignUserIdLookupService>();
            Bind<IScheduleService>().To<DesignScheduleService>();
            Bind<ITwitterSearchService>().To<DesignTwitterSearchService>();

            Bind<INavigationService>().To<NavigationService>();
        }
    }
}
