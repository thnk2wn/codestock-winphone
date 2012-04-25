namespace Phone.Common.IO
{
    public class FakeSettingsHelper : ISettingsHelper
    {

        #region ISettingsHelper Members

        public bool AutoSave
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public object GetSetting(string key)
        {
            return null;
        }

        public string GetString(string key)
        {
            return null;
        }

        public double? GetDouble(string key)
        {
            return null;
        }

        public int? GetInt(string key)
        {
            return null;
        }

        public bool? GetBool(string key)
        {
            return null;
        }

        public void SetSetting(string key, object value)
        {
            return;
        }

        public void TrySave()
        {
            return;
        }

        #endregion
    }
}
