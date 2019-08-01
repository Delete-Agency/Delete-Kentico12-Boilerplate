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
    }
}
