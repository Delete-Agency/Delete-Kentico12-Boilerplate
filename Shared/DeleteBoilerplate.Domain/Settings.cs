using DeleteBoilerplate.Domain.Helpers;

namespace DeleteBoilerplate.Domain
{
    public static class Settings
    {
        public static class Global
        {
            public static string DefaultImage => SiteSettingsHelper.GetSettingValue("DeleteBoilerplate_DefaultImage") ?? string.Empty;

            public static string DefaultImageAlt => SiteSettingsHelper.GetSettingValue("DeleteBoilerplate_DefaultImageAlt") ?? string.Empty;
        }

        public static class Taxonomy
        {
            public static string SearchIndex => SiteSettingsHelper.GetSettingValue("DeleteBoilerplate_SearchIndex") ?? string.Empty;
        }
    }
}