using System.Collections.ObjectModel;
using System.Linq;
using CodeStock.App.Core;
using CodeStock.App.Design.Services;
using CodeStock.App.Design.ViewModels;
using CodeStock.App.ViewModels;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Schedule;
using CodeStock.App.ViewModels.Support;
using Ninject.Activation;
using Ninject.Modules;
using Phone.Common.Diagnostics.Logging;
using Phone.Common.IO;
using Phone.Common.IOC;
using Phone.Common.Net;

namespace CodeStock.App.Design.Modules
{
    public class DesignModule : NinjectModule
    {

        public override void Load()
        {
            Bind<IHtmlDecoder>().To<DesignHtmlDecoder>();

            // it appears at design time that InSingletonScope() isn't working, at least in cases
            //Bind<CoreData>().ToSelf().InSingletonScope();
            Bind<ICoreData>().To<DesignCoreData>().InSingletonScope();

            BindViewModels();

            Bind<ILogManager>().To<NullLoggingService>();
            Bind<ILog>().To<DebugLog>();

            Bind<ISettingsHelper>().To<FakeSettingsHelper>();
            Bind<AppSettings>().ToSelf();
        }

        private void BindViewModels()
        {
            Bind<ISessionItemViewModel>().To<DesignSessionItemViewModel>();
            Bind<ISpeakerItemViewModel>().To<DesignSpeakerItemViewModel>();
            Bind<IRoomItemViewModel>().To<DesignRoomItemViewModel>();

            Bind<TweetItemViewModel>().ToMethod(x =>
            {
                var svc = new DesignTwitterSearchService();
                svc.Search("does not matter");
                return TweetItemViewModel.CreateTweetItem(svc.Result.Tweets.FirstOrDefault());
            });

            Bind<MainViewModel>().ToSelf();
            Bind<SessionsViewModel>().ToSelf();
            Bind<SpeakersViewModel>().ToSelf();
            Bind<RoomsViewModel>().ToSelf();

            Bind<ScheduleViewModel>().ToSelf();
            Bind<MyScheduleViewModel>().ToSelf();

            Bind<DiagnosticViewModel>().ToSelf();
            Bind<WebBrowserViewModel>().ToSelf();
            Bind<SettingsViewModel>().ToSelf();

            Bind<SearchViewModel>().ToMethod(CreateSearchViewModel);
        }

        private static SearchViewModel CreateSearchViewModel(IContext context)
        {
            var session = IoC.Get<ISessionItemViewModel>();
            var speaker = IoC.Get<ISpeakerItemViewModel>();

            var vm = new SearchViewModel(null)
            {
                Items = new ObservableCollection<SearchItemViewModel>(),
                SearchText = "Random"
            };

            vm.Items.Add(SearchItemViewModel.Create(session));
            vm.Items.Add(SearchItemViewModel.Create(speaker));
            return vm;
        }
    }
}
