using System;
using CMS;
using CMS.Core;
using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMSApp.Old_App_Code.CustomUniGridTransformations;
using DeleteBoilerplate.Domain.Models.CmsClasses.SitemapRule;

[assembly: RegisterModule(typeof(CustomUniGridTransformationModule))]

namespace CMSApp.Old_App_Code.CustomUniGridTransformations
{
    public class CustomUniGridTransformationModule : Module
    {
        public CustomUniGridTransformationModule()
            : base("CustomUniGridTransformations")
        {
        }

        public CustomUniGridTransformationModule(ModuleMetadata metadata, bool isInstallable = false) : base(metadata, isInstallable)
        {
        }

        public CustomUniGridTransformationModule(string moduleName, bool isInstallable = false) : base(moduleName, isInstallable)
        {
        }

        protected override void OnInit()
        {

            base.OnInit();

            UniGridTransformations.Global.RegisterTransformation("#sitemapruletype", GetSitemapRuleType);
            UniGridTransformations.Global.RegisterTransformation("#siteid", GetSiteNameById);
        }

        private static object GetSitemapRuleType(object parameter)
        {
            SitemapRuleType sitemapRule = (SitemapRuleType)Enum.ToObject(typeof(SitemapRuleType), parameter);
            return Enum.GetName(typeof(SitemapRuleType), sitemapRule);
        }

        private static object GetSiteNameById(object parameter)
        {
            var isValid = Int32.TryParse(parameter.ToString(), out int siteId);
            if (isValid && siteId > 0)
            {
                var siteName = SiteInfoProvider.GetSiteName(siteId);
                return siteName;
            }

            return "(none)";
        }
    }
}
