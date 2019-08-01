﻿using CMS.DataEngine;
using CMS.Localization;
using CultureInfo = System.Globalization.CultureInfo;


namespace DeleteBoilerplate.Domain
{
    public static class Constants
    {
        public static class Cultures
        {
            private static CultureInfo _default;

            public static CultureInfo Default
            {
                get
                {
                    if (_default == null)
                    {
                        var defaultKenticoCulture =
                            CultureInfoProvider.GetCultureInfo(
                                SettingsKeyInfoProvider.GetValue("CMSDefaultCultureCode"));
                        _default = CultureInfo.GetCultureInfo(defaultKenticoCulture.CultureCode);
                    }

                    return _default;
                }
            }
        }

        public static class Taxonomy
        {
            public const string SearchFieldNamePrefix = "Taxonomy";
            public const string ParentSearchFieldNamePrefix = SearchFieldNamePrefix + "Parent";

        }
    }
}