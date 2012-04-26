using Phone.Common.IO;

namespace CodeStock.App.Core
{
    public class AppSettings
    {
        public AppSettings(ISettingsHelper settingsHelper)
        {
            _settingsHelper = settingsHelper;
        }

        private readonly ISettingsHelper _settingsHelper;

        public double SessionCacheDuration
        {
            get
            {
                var value = _settingsHelper.GetDouble("SessionCacheDuration") ?? 2;
                return value;
            }
            set
            {
                _settingsHelper.SetSetting("SessionCacheDuration", value);
            }
        }

        public double SpeakerCacheDuration
        {
            get
            {
                var value = _settingsHelper.GetDouble("SpeakerCacheDuration") ?? 4;
                return value;
            }
            set
            {
                _settingsHelper.SetSetting("SpeakerCacheDuration", value);
            }
        }

        public double ScheduleCacheDuration
        {
            get
            {
                var value = _settingsHelper.GetDouble("ScheduleCacheDuration") ?? 1;
                return value;
            }
            set
            {
                _settingsHelper.SetSetting("ScheduleCacheDuration", value);
            }
        }

        public bool BackgroundImageOn
        {
            get
            {
                var value = _settingsHelper.GetBool("BackgroundImageOn") ?? true;
                return value;
            }
            set
            {
                _settingsHelper.SetSetting("BackgroundImageOn", value);
            }
        }

        public string BackgroundImageUrl
        {
            get
            {
                var imageUrl = this.BackgroundImageOn ? "Images/Cosmic.jpg" : null;
                return imageUrl;
            }
        }
    }
}
