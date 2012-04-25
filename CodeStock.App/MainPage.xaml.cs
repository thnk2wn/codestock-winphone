using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using CodeStock.App.Core;
using CodeStock.App.Messaging;
using CodeStock.App.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Phone.Common.Diagnostics;
using Phone.Common.Diagnostics.Logging;
using Phone.Common.Extensions.Microsoft.Phone.Controls_;
using Phone.Common.IOC;

namespace CodeStock.App
{
    public partial class MainPage //: PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.ViewModel.Activate();
            LoadTransientState();
        }

        private void LoadTransientState()
        {
            var timer = CodeTimer.StartNew();
            LogInstance.LogInfo("Loading transient state");
            this.LoadState(uxPanorma);
            uxSessionsCtl.LoadState();
            uxSpeakersControl.LoadState();
            uxScheduleCtl.LoadState();
            uxTweetsControl.LoadState();
            uxSearchControl.LoadState();
            LogInstance.LogInfo("Transient state restored in {0:#.000} seconds", timer.Stop());
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SaveTransientState();

            if (!_backgroundMsgRegistered)
            {
                Messenger.Default.Register<BackgroundChangedMessage>(this, AfterBackgroundImageSettingChanged);
                _backgroundMsgRegistered = true;
            }
        }

        private static bool _backgroundMsgRegistered;

        private void SaveTransientState()
        {
            var timer = CodeTimer.StartNew();
            LogInstance.LogInfo("Saving transient state");
            // might want to consider doing this only for the current control
            this.SaveState(uxPanorma);
            uxSessionsCtl.SaveState();
            uxSpeakersControl.SaveState();
            uxScheduleCtl.SaveState();
            uxTweetsControl.SaveState();
            uxSearchControl.SaveState();
            LogInstance.LogInfo("Transient state saved in {0:#.##} seconds", timer.Stop());
        }

        private MainViewModel ViewModel
        {
            get { return this.DataContext as MainViewModel; }
        }

        private void PanoramaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            if (DesignerProperties.IsInDesignTool) return;

            var vm = ViewModelFromPanormaItem(e.AddedItems[0]);
            if (null == vm) return;

            this.ViewModel.ChildViewModelChangedCommand.Execute(vm);
        }

        private static AppViewModelBase ViewModelFromPanormaItem(object item)
        {
            var panoItem = (PanoramaItem) item;
            var control = panoItem.Content as UserControl;
            if (null == control) return null;
            var vm = control.DataContext as AppViewModelBase;
            return vm;
        }

        private void PanoramaLoaded(object sender, RoutedEventArgs e)
        {
            // panorama SelectionChanged won't fire for the initial panorma item b/c it hasn't changed
            // but we want to treat the initial load as selection changed to 1st / default item
            if (DesignerProperties.IsInDesignTool) return;

            var panorama = (Panorama)sender;
            var vm = ViewModelFromPanormaItem(panorama.Items[0]);
            this.ViewModel.ChildViewModelChangedCommand.Execute(vm);

            LogInstance.LogInfo("Main panorma loaded");
        }

        private void AfterBackgroundImageSettingChanged(BackgroundChangedMessage m)
        {
            RestoreBackground();
        }

        private void RestoreBackground()
        {
            // pre-Mango background issue with themes when using light theme. hopefully fixed with Mango
            // http://stackoverflow.com/questions/5658951/custom-theme-that-overrides-default-theme-wp7
            
            // both with and without the panorama background image, appears we need to set background to transparent to ensure
            // we don't end up with choppy scrolling. however with the white theme, setting transparent background means we end 
            // up with white text on white background currently. 
            
            var isDarkTheme = IoC.Get<IApp>().IsDarkTheme;
            var backgroundImageOn = IoC.Get<AppSettings>().BackgroundImageOn;

            if (isDarkTheme || backgroundImageOn)
            {
                this.LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
                //this.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                // user is on light theme with background image off. ensure black; if the changed image setting mid session, transparent currently
                this.LayoutRoot.Background = new SolidColorBrush(Colors.Black);
            }
        }

        private bool _backgroundIssueFixed;
        private void uxPanorma_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            // only seems to be a problem for the sessions and speakers control so we delay doing this until we get ready to switch pano items
            if (_backgroundIssueFixed) return;
            RestoreBackground();
            _backgroundIssueFixed = true;
        }
    }
}