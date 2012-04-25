namespace Phone.Common.IO
{
    public interface ISettingsHelper
    {
        bool AutoSave { get; set; }

        object GetSetting(string key);
        string GetString(string key);
        double? GetDouble(string key);
        int? GetInt(string key);
        bool? GetBool(string key);

        void SetSetting(string key, object value);

        void TrySave();
    }
}
