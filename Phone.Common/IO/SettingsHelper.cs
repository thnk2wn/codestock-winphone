using System;
using System.IO.IsolatedStorage;
using System.Threading;
using Phone.Common.Diagnostics.Logging;

namespace Phone.Common.IO
{
    public class SettingsHelper : ISettingsHelper
    {
        private static IsolatedStorageSettings IsoSettings
        {
            get
            {
                try
                {
                    return IsolatedStorageSettings.ApplicationSettings;
                }
                catch
                {
                    return IsolatedStorageSettings.ApplicationSettings;
                }
            }
        }

        #region ISettings Members

        public bool AutoSave {get; set;}

        public object GetSetting(string key)
        {
            return !IsoSettings.Contains(key) ? null : IsoSettings[key];
        }

        public string GetString(string key)
        {
            var value = GetSetting(key);
            return (null != value) ? value.ToString() : null;
        }

        public double? GetDouble(string key)
        {
            var value = GetSetting(key);
            return (null != value && string.Empty != value.ToString()) ? Convert.ToDouble(value) : (double?)null;
        }

        public int? GetInt(string key)
        {
            var value = GetSetting(key);
            return (null != value && string.Empty != value.ToString()) ? Convert.ToInt32(value) : (int?)null;
        }

        public bool? GetBool(string key)
        {
            var value = GetSetting(key);
            return (null != value && string.Empty != value.ToString()) ? Convert.ToBoolean(value) : (bool?)null;
        }

        public void SetSetting(string key, object value)
        {
            IsoSettings[key] = value;

            if (AutoSave)
                TrySave();
        }

        public void TrySave()
        {
            ThreadPool.QueueUserWorkItem(func =>
            {
                try
                {
                    //may throw InvalidOperationException exception if user is in process of changing settings and navigating away from page
                    Thread.Sleep(200);
                    IsoSettings.Save();
                }
                catch (Exception ex)
                {
                    LogInstance.LogError("Error saving settings: {0}", ex);
                }
            });
        }

        #endregion ISettings Members
    }
}
