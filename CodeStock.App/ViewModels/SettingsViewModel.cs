using System;
using CodeStock.App.Core;
using CodeStock.App.Messaging;
using GalaSoft.MvvmLight.Messaging;
using Phone.Common.IOC;

namespace CodeStock.App.ViewModels
{
    public class SettingsViewModel : AppViewModelBase
    {
        public SettingsViewModel(AppSettings settings)
        {
            _settings = settings;
            this.SessionCacheDuration = settings.SessionCacheDuration;
            this.SpeakerCacheDuration = settings.SpeakerCacheDuration;
            this.ScheduleCacheDuration = settings.ScheduleCacheDuration;
            this.MainBackgroundImageOn = settings.BackgroundImageOn;
            _initialized = true;
        }

        private readonly bool _initialized;
        private readonly AppSettings _settings;

        public bool AllowRunUnderLock
        {
            get { return ApplicationIdleModeHelper.Current.RunsUnderLock; }

            set
            {
                if (ApplicationIdleModeHelper.Current.RunsUnderLock != value)
                {
                    if (HandledLockChange())
                        RaisePropertyChanged(() => AllowRunUnderLock);
                }
            }
        }

        private bool? _mainBackgroundImageOn;

        public bool? MainBackgroundImageOn
        {
            get { return _mainBackgroundImageOn; }

            set
            {
                if (_mainBackgroundImageOn != value)
                {
                    if (!_initialized || ConfirmBackgroundChange())
                    {
                        _mainBackgroundImageOn = value;

                        if (null != value)
                            _settings.BackgroundImageOn = value.Value;

                        RaisePropertyChanged(() => MainBackgroundImageOn);

                        if (_initialized)
                            Messenger.Default.Send(new BackgroundChangedMessage());
                    }
                }
            }
        }

        private bool ConfirmBackgroundChange()
        {
            // if current is off we are turning on
            if (_mainBackgroundImageOn.HasValue && !_mainBackgroundImageOn.Value)
                return true;

            var isLightTheme = !IoC.Get<IApp>().IsDarkTheme;
            if (!isLightTheme) return true;

            var msg = "Turning off the background image can help with readability in outdoor conditions and improve startup time. "
                + "However currently it results in choppier scrolling due to a theming issue with the Light theme in use." + Environment.NewLine + Environment.NewLine
                + "Click OK to continue";

            var value = MessageBoxOKCancel(msg, "");

            // need bound control to switch back
            if (!value)
                RaisePropertyChanged(() => MainBackgroundImageOn);

            return value;
        }

        private bool HandledLockChange()
        {
            // toggle the value of runs under lock without changing source value
            var runUnderLock = !ApplicationIdleModeHelper.Current.RunsUnderLock;

            if (runUnderLock)
            {
                // only warn the user once
                if (!ApplicationIdleModeHelper.Current.HasUserAgreedToRunUnderLock)
                {
                    var msg = "No network or background work will be performed under lock (that isn't already in progress). " +
                        "However some extra battery could still be consumed while not using the application." + Environment.NewLine + Environment.NewLine +
                        "Enabling will allow immediate resume without normal, slower tombstone resume." + Environment.NewLine + Environment.NewLine + 
                        "Click OK to enable.";

                    if (!MessageBoxOKCancel(msg, ""))
                        return false;

                    ApplicationIdleModeHelper.Current.HasUserAgreedToRunUnderLock = true;
                }
            }

            // update source run under lock setting
            ApplicationIdleModeHelper.Current.RunsUnderLock = runUnderLock;

            if (ApplicationIdleModeHelper.Current.IsRestartRequired)
            {
                MessageBox("This change will take effect the next time you start the application.", "Delayed Effect");
            }

            return true;
        }

        private double? _sessionCacheDuration;

        public double? SessionCacheDuration
        {
            get { return _sessionCacheDuration; }

            set
            {
                if (_sessionCacheDuration != value)
                {
                    _sessionCacheDuration = value;

                    if (null != value)
                        _settings.SessionCacheDuration = value.Value;
                    RaisePropertyChanged(() => SessionCacheDuration);
                }
            }
        }

        private double? _speakerCacheDuration;

        public double? SpeakerCacheDuration
        {
            get { return _speakerCacheDuration; }

            set
            {
                if (_speakerCacheDuration != value)
                {
                    _speakerCacheDuration = value;

                    if (null != value)
                        _settings.SpeakerCacheDuration = value.Value;

                    RaisePropertyChanged(() => SpeakerCacheDuration);
                }
            }
        }

        private double? _scheduleCacheDuration;

        public double? ScheduleCacheDuration
        {
            get { return _scheduleCacheDuration; }

            set
            {
                if (_scheduleCacheDuration != value)
                {
                    _scheduleCacheDuration = value;

                    if (null != value)
                        _settings.ScheduleCacheDuration = value.Value;

                    RaisePropertyChanged(() => ScheduleCacheDuration);
                }
            }
        }
    }
}
