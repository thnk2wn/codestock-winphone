using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Controls;

namespace CodeStock.App.Core
{
    public class ApplicationIdleModeHelper : INotifyPropertyChanged
    {
        private ApplicationIdleModeHelper()
        {
        }

        static ApplicationIdleModeHelper current;

        public static ApplicationIdleModeHelper Current
        {
            get
            {
                if (null == current)
                {
                    current = new ApplicationIdleModeHelper();
                }
                Debug.Assert(current != null);

                return current;
            }
        }


        public void Initialize()
        {
            Debug.WriteLine("initialized " + PhoneApplicationService.Current.StartupMode);


            bool fromStorage = false;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("RunsUnderLock", out fromStorage))
            {
                if (fromStorage != (PhoneApplicationService.Current.ApplicationIdleDetectionMode == IdleDetectionMode.Disabled))
                {
                    RunsUnderLock = fromStorage;
                    Debug.WriteLine("Did not match");
                }
                else
                {
                    Debug.WriteLine("Matched");
                    runsUnderLock = fromStorage;
                }
            }

            bool hasPrompted = false;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("HasUserAgreedToRunUnderLock", out hasPrompted))
            {
                HasUserAgreedToRunUnderLock = hasPrompted;
            }


        }


        #region private members
        bool runsUnderLock;
        bool isRunningUnderLock;
        bool hasUserAgreedToRunUnderLock;
        bool isRestartRequired;

        #endregion

        #region public properties
        public bool RunsUnderLock
        {
            get
            {
                return runsUnderLock;
            }
            set
            {
                if (value != runsUnderLock)
                {
                    runsUnderLock = value;


                    if (runsUnderLock)
                    {
                        PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
                        var rootframe = App.Current.RootVisual as PhoneApplicationFrame;

                        Debug.Assert(rootframe != null, "assumes RootVisual has been set");
                        if (rootframe != null)
                        {
                            rootframe.Obscured += rootframe_Obscured;
                            rootframe.Unobscured += rootframe_Unobscured;
                        }

                    }
                    else
                    {
                        IsRestartRequired = true;
                        // we can not set it; we have to restart app ...
                        //  PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Enabled ; 
                        var eh = RestartRequired;
                        if (eh != null)
                            eh(this, new EventArgs());
                    }

                    SaveSetting("RunsUnderLock", runsUnderLock);
                    OnPropertyChanged("RunsUnderLock");
                }
            }
        }


        public bool IsRestartRequired
        {
            get
            {
                return isRestartRequired;
            }
            private set
            {
                if (value != isRestartRequired)
                {
                    isRestartRequired = value;
                    OnPropertyChanged("IsRestartRequired");
                }
            }
        }

        public bool HasUserAgreedToRunUnderLock
        {
            get
            {
                return hasUserAgreedToRunUnderLock;
            }
            set
            {
                if (value != hasUserAgreedToRunUnderLock)
                {
                    hasUserAgreedToRunUnderLock = value;
                    SaveSetting("HasUserAgreedToRunUnderLock", hasUserAgreedToRunUnderLock);
                    OnPropertyChanged("HasUserAgreedToRunUnderLock");
                }
            }
        }

        public void SaveSetting(string key, object value)
        {

            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                IsolatedStorageSettings.ApplicationSettings[key] = value;
            }
            else
                IsolatedStorageSettings.ApplicationSettings.Add(key, value);

            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        void rootframe_Unobscured(object sender, EventArgs e)
        {
            IsRunningUnderLock = false;

        }

        void rootframe_Obscured(object sender, ObscuredEventArgs e)
        {
            if (e.IsLocked)
            {
                IsRunningUnderLock = e.IsLocked;
            }
        }

        public bool IsRunningUnderLock
        {
            get
            {
                return isRunningUnderLock;
            }
            private set
            {
                if (value != isRunningUnderLock)
                {
                    isRunningUnderLock = value;
                    OnPropertyChanged("IsRunningUnderLock");

                    if (isRunningUnderLock)
                    {
                        EventHandler eh = Locked;
                        if (eh != null)
                            eh(this, new EventArgs());
                    }
                    else
                    {
                        EventHandler eh = UnLocked;
                        if (eh != null)
                            eh(this, new EventArgs());
                    }

                }

            }
        }

        #endregion

        #region events

        public event EventHandler Locked;
        public event EventHandler UnLocked;
        public event EventHandler RestartRequired;

        #endregion


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler ph = PropertyChanged;
            if (ph != null)
                ph(this, new PropertyChangedEventArgs(propertyName));

        }

        #endregion



    }
}
