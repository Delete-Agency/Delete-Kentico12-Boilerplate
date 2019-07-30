using CMS.DataEngine;
using CMS.SiteProvider;
using DeleteBoilerplate.Infrastructure.Enums;
using DeleteBoilerplate.Infrastructure.Extensions;

namespace DeleteBoilerplate.Infrastructure.Helpers
{


    public static class SiteSettingsHelper
    {
        public static string GetSettingValue(SiteSetting setting)
        {
            if (setting == SiteSetting.Default)
                return string.Empty;

            return SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.{setting.GetStringValue()}");
        }
    }
}
