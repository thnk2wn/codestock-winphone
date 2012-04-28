using CodeStock.App.Core;
using CodeStock.App.ViewModels;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Schedule;
using CodeStock.App.ViewModels.Support;
using Ninject.Modules;
using Phone.Common.Diagnostics.Logging;
using Phone.Common.IO;
using Phone.Common.IOC;
using Phone.Common.Net;
using Phone.Common.Tasks;
using Phone.Common.Windows;

namespace CodeStock.App.IOC
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            BindViewModels();

            Bind<IHtmlDecoder>().To<HtmlDecoder>();
            Bind<ICoreData>().To<CoreData>().InSingletonScope();
            Bind<IMessageBoxService>().To<MessageBoxService>();
            Bind<ITaskInvoker>().To<TaskInvoker>();
            Bind<IApp>().ToMethod(m=> ((IApp) App.Current));
            Bind<ISettingsHelper>().ToMethod(c => new SettingsHelper { AutoSave = false });
            Bind<AppSettings>().ToSelf();
            
            SetupLogging();
        }

        private void SetupLogging()
        {
            Bind<ILogManager>().ToMethod(c =>
            {
                var ls = new LoggingService("CodeStock")
                             {
                                 AutoPersist = LogAutoPersist.None,
                                 MessageFormat = "{0:HH:mm:ss.ffff} - {1}"
                             };

#if DEBUG
                ls.DebugOut = true;
                ls.Level = LogLevel.Debug;
#else
                ls.DebugOut = false;
                ls.Level = LogLevel.Info;
#endif

                ls.Enable();
                ls.Clear();
                return ls;
            }).InSingletonScope();

            Bind<ILog>().ToMethod(c => IoC.Get<ILogManager>());
        }

        private void BindViewModels()
        {
            Bind<MainViewModel>().ToSelf().InSingletonScope();

            Bind<SessionsViewModel>().ToSelf();
            Bind<SpeakersViewModel>().ToSelf();
            Bind<RoomsViewModel>().ToSelf();

            Bind<ScheduleViewModel>().ToSelf();
            Bind<MyScheduleViewModel>().ToSelf();

            Bind<MiscMenuViewModel>().ToSelf();
            Bind<DiagnosticViewModel>().ToSelf();
            Bind<TwitterSearchViewModel>().ToSelf();
            Bind<WebBrowserViewModel>().ToSelf();
            Bind<SearchViewModel>().ToSelf();
            Bind<SettingsViewModel>().ToSelf();
            Bind<MapsViewModel>().ToSelf();

            Bind<ISessionItemViewModel>().To<SessionItemViewModel>();
            Bind<ISpeakerItemViewModel>().To<SpeakerItemViewModel>();
            Bind<IRoomItemViewModel>().To<RoomItemViewModel>();
            
        }
    }
}
