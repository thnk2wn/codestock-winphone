using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using CodeStock.App.Core;
using CodeStock.App.IOC;
using CodeStock.App.Messaging;
using CodeStock.App.Pages;
using CodeStock.App.ViewModels.Support;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Phone.Common.Diagnostics.Logging;
using Phone.Common.IOC;
using Phone.Common.Threading;
using Phone.Common.Windows;

namespace CodeStock.App
{
    public partial class App : IApp //: Application
    {
        #region App Properties
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        public static DateTime ConferenceStart { get; private set; }
        public bool IsDarkTheme { get; private set; }

        public static TimeSpan ConferenceCountdown
        {
            get
            {
                var tsCountdown = ConferenceStart - DateTime.Now;
                return tsCountdown;
            }
        }
        #endregion App Properties

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Show graphics profiling information while debugging.
#if DEBUG

            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are being GPU accelerated with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;
            }
#endif

            // important: theme detection must happen here before we initialize anything and load our own app resources
            // which override the default ones and would give us false positives on theme detection
            IsDarkTheme = ThemeHelper.IsDarkTheme();

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            ThemeOverride();

            ConferenceStart = new DateTime(2012, 6, 15);
        }

        private static void ThemeOverride()
        {
            ThemeManager.ToDarkTheme();
            SetColors();
        }

        private static void SetColors()
        {
            var limeGreen = Color.FromArgb(255, 140, 191, 38); //#90ee90
            ((SolidColorBrush) Current.Resources["PhoneAccentBrush"]).Color = limeGreen;
        }

        public DateTime StartupTime { get; private set; }

        private bool _isNewApp = true;
        public bool IsNewApp { get { return _isNewApp; } }

        public string MapsApiKey
        {
            get
            {
                var intern = (Internal) this.Resources["Internal"];
                return intern.MapsApiKey;
            }
        }

        #region Private Methdos

        private void SetupApp(string eventName)
        {
            this.StartupTime = DateTime.Now;

            // ensure DI/IOC is setup first
            ModuleLoader.Load();

            LogInstance.LogInfo("Setting up application ({0})", eventName);

            LogInstance.LogInfo("Ensuring core data is loaded");
            var coreData = IoC.Get<ICoreData>();
            coreData.EnsureLoaded();

            Messenger.Default.Register<ErrorMessage>(this, AfterErrorMessageReceived);
        }

        private static void AfterErrorMessageReceived(ErrorMessage errorMessage)
        {
            DispatchUtil.SafeDispatch(() =>
            {
                var friendlyMsg = !string.IsNullOrEmpty(errorMessage.FriendlyError) ? errorMessage.FriendlyError : "Aw snap. An error occurred.";
                var errorWin = new ErrorWindow(errorMessage.Error, friendlyMsg)
                {
                    Title = errorMessage.Title
                };
                errorWin.Show();
            });
        }

        #endregion Private Methods

        #region App Events
        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            SetupApp("Launching");
        }
        

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            _isNewApp = false;
            SetupApp("Activated");
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Ensure that required application state is persisted here.
            LogInstance.LogInfo("Application is being deactivated");
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            LogInstance.LogInfo("Application is closing");
        }

        // Code to execute if a navigation fails
        private static void RootFrameNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            var msg = string.Format("Failed to navigate to '{0}'. Error was: {1}", e.Uri, e.Exception);
            LogInstance.LogError(msg);

            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
            
            MessageBox.Show(msg, "Sorry", MessageBoxButton.OK);
        }

        // Code to execute on Unhandled Exceptions
        private static void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                LogInstance.LogCritical("Unhandled exception: {0}", e.ExceptionObject);

                if (Debugger.IsAttached)
                {
                    // An unhandled exception has occurred; break into the debugger
                    Debugger.Break();
                }

                // don't navigate to an error view as that is awkward and doesn't play well with navigation
                // don't show a MessageBox either as that isn't customizable

                DispatchUtil.SafeDispatch(() =>
                {
                    var errorWin = new ErrorWindow(e.ExceptionObject,
                    "An unexpected error has occurred. Please click Send to report the error details.") { Title = "Unexpected Error" };
                    errorWin.Show();
                });

                e.Handled = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                e.Handled = false;
                //Quit();
            }
        }
        #endregion App Events

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrameNavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            ApplicationIdleModeHelper.Current.Initialize();

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}