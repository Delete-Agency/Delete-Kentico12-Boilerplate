using CMS.DataEngine;
using CMS.SiteProvider;

namespace DeleteBoilerplate.Domain.Helpers
{
    public static class SiteSettingsHelper
    {
        public static string GetSettingValue(string settingKey)
        {
            if (string.IsNullOrWhiteSpace(settingKey))
                return string.Empty;

            return SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.{settingKey}");
        }

        public static int GetSettingIntValue(string settingKey)
        {
            if (string.IsNullOrWhiteSpace(settingKey))
                return 0;

            return SettingsKeyInfoProvider.GetIntValue($"{SiteContext.CurrentSiteName}.{settingKey}");
        }

        public static bool GetSettingBoolValue(string settingKey)
        {
            if (string.IsNullOrWhiteSpace(settingKey))
                return false;

            var value = SettingsKeyInfoProvider.GetBoolValue($"{SiteContext.CurrentSiteName}.{settingKey}");
            return value;
        }
    }
}
