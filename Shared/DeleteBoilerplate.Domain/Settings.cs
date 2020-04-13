using System;
using CMS.Base;
using CMS.Helpers;
using DeleteBoilerplate.Domain.Helpers;

namespace DeleteBoilerplate.Domain
{
    public static class Settings
    {
        /// <summary>
        /// Use this setting instead of HttpContext.Current.Kentico().Preview().Enabled
        /// If something wrong - check that ApplicationType setting exist in Web.config of Web App
        /// Expected value for CMS solution - "CMS", for MVC solution - "MVC"
        /// </summary>
        public static bool PreviewEnabled => 
            string.Equals(SettingsHelper.AppSettings["ApplicationType"], "MVC", StringComparison.OrdinalIgnoreCase)
            && VirtualContext.IsPreviewLinkInitialized;

        public static class Global
        {
            public static string DefaultImage => SiteSettingsHelper.GetSettingValue("DeleteBoilerplate_DefaultImage") ?? string.Empty;

            public static string DefaultImageAlt => SiteSettingsHelper.GetSettingValue("DeleteBoilerplate_DefaultImageAlt") ?? string.Empty;
        }

        public static class Taxonomy
        {
            public static string SearchIndex => SiteSettingsHelper.GetSettingValue("DeleteBoilerplate_SearchIndex") ?? string.Empty;
        }

        public static class Cache
        {
            public static int RepositoryCacheItemDuration => SiteSettingsHelper.GetSettingIntValue("DeleteBoilerplate_Cache_RepositoryCacheItemDuration");
        }

        public static class Navigation
        {
            public static string HeaderNavigationPath => SiteSettingsHelper.GetSettingValue("DeleteBoilerplate_Content_Navigation_HeaderNavigationPath");

            public static string FooterNavigationPath => SiteSettingsHelper.GetSettingValue("DeleteBoilerplate_Content_Navigation_FooterNavigationPath");

            public static string CompanyLogoImageUrl => SiteSettingsHelper.GetSettingValue("DeleteBoilerplate_Content_Navigation_CompanyLogoImageUrl");

            public static string CompanyLogoLink => SiteSettingsHelper.GetSettingValue("DeleteBoilerplate_Content_Navigation_CompanyLogoLink");
        }

        public static class Pages
        {
            public static string NotFoundPagePath => SiteSettingsHelper.GetSettingValue("DeleteBoilerplate_Content_Pages_NotFoundPagePath");
        }

        public static class Integrations
        {
            public static class Google
            {
                public static string GoogleReCaptchaSecretKey => SiteSettingsHelper.GetSettingValue("DeleteBoilerplate_Integrations_Google_GoogleReCaptchaSecretKey");
            }
        }
    }
}