using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using CodeStock.App.ViewModels.Support;
using CodeStock.Data;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls;
using Phone.Common.IOC;
using Phone.Common.Navigation;

namespace CodeStock.App.ViewModels
{
    public class MiscMenuViewModel : AppViewModelBase
    {
        public MiscMenuViewModel(INavigationService navigationService) : base(registerCoreData:true)
        {
            this.Items = new ObservableCollection<MenuItem>
            {
                new MenuItem {Text = "About", SubText = "Basic application information", Action = About},
                new MenuItem {Text = "App Settings", SubText = "Allow running under lock and more", Action = GoToAppSettings},
                new MenuItem {Text = "App Website", SubText = "Info on this app, its author, WP7 and more", Action = VisitAppWebsite},
                new MenuItem { Text = "Clear Cache", SubText = "Sessions, speakers, and schedule", Action = ClearCache },
                new MenuItem {Text = "Codestock.org", SubText = "Visit the official website", Action = VisitCodeStock},

//#if DEBUG
                new MenuItem {Text = "Diagnostic Log", SubText = "View diagnostic log", Action = ViewDiagnostics},
//#endif
                new MenuItem {Text = "Refresh Data", SubText = "Reloads session and speaker data if not expired in cache", Action = RefreshData}
            };

            _navigationService = navigationService;
        }

        private readonly INavigationService _navigationService;

        private ObservableCollection<MenuItem> _items;
 
        public ObservableCollection<MenuItem> Items
        {
            get { return _items; }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    RaisePropertyChanged(() => Items);
                }

            }
        }

        private void About()
        {
            // WP7 restrictions around system.reflection
            var parts = Assembly.GetExecutingAssembly().FullName.Split(",".ToCharArray());
            var appVersion = string.Join(",", parts, 0, 2);

            var sb = new StringBuilder();
            sb.AppendFormat("{0}{1}{1}", appVersion, Environment.NewLine);
            sb.AppendLine("App to support CodeStock conference. Conference website: http://codestock.org");
            sb.AppendLine();
            sb.AppendLine("By Geoff Hudik. http://geoffhudik.com or http://twitter.com/thnk2wn");
            MessageBox(sb.ToString(), "About");
        }

        private void ClearCache()
        {
            CacheUtility.Clear();
            MessageBox("Cache cleared. You may have to refresh data or restart the app for this to fully take effect.", "Done");
        }

        private void VisitCodeStock()
        {
            _navigationService.NavigateTo(Uris.WebBrowser(), "http://codestock.org");
        }

        private void GoToAppSettings()
        {
            _navigationService.NavigateTo(Uris.Settings());
        }

        private void VisitAppWebsite()
        {
            _navigationService.NavigateTo(Uris.WebBrowser(), "http://geoffhudik.com/codestock-wp7");
        }

        public ICommand MenuItemChosenCommand
        {
            get { return new RelayCommand<GestureEventArgs>(MenuItemChosen); }
        }

        private bool IsRefreshInProgress { get; set; }

        private static void MenuItemChosen(GestureEventArgs e)
        {
            var elem = (FrameworkElement)e.OriginalSource;
            var item = elem.DataContext as MenuItem;

            if (null == item) return;
            item.Action();
        }

        private void RefreshData()
        {
            IsRefreshInProgress = true;
            this.SetBusy(true, "Refreshing data...");

            // don't clear the cache first
            ThreadPool.QueueUserWorkItem(delegate
            {
                SafeDispatch(() => IoC.Get<ICoreData>().LoadData());
            });
        }

        protected override void OnCoreDataLoaded(ICoreData coreData)
        {
            if (this.IsRefreshInProgress)
            {
                this.IsRefreshInProgress = false;
                this.SetBusy(false);
            }
        }

        private void ViewDiagnostics()
        {
            _navigationService.NavigateTo(Uris.Diagnostics());
        }
        
    }

    public class MenuItem
    {
        public string Text { get; set; }
        public string SubText { get; set; }
        public Action Action { get; set; }
    }
}
