using System;
using System.Data;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;

namespace DeleteBoilerplate
{
    /// <summary>
    /// Class providing <see cref="SitemapRuleInfo"/> management.
    /// </summary>
    public partial class SitemapRuleInfoProvider : AbstractInfoProvider<SitemapRuleInfo, SitemapRuleInfoProvider>
    {
        /// <summary>
        /// Creates an instance of <see cref="SitemapRuleInfoProvider"/>.
        /// </summary>
        public SitemapRuleInfoProvider()
            : base(SitemapRuleInfo.TYPEINFO)
        {
        }


        /// <summary>
        /// Returns a query for all the <see cref="SitemapRuleInfo"/> objects.
        /// </summary>
        public static ObjectQuery<SitemapRuleInfo> GetSitemapRules()
        {
            return ProviderObject.GetObjectQuery();
        }


        /// <summary>
        /// Returns <see cref="SitemapRuleInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="SitemapRuleInfo"/> ID.</param>
        public static SitemapRuleInfo GetSitemapRuleInfo(int id)
        {
            return ProviderObject.GetInfoById(id);
        }


        /// <summary>
        /// Returns <see cref="SitemapRuleInfo"/> with specified name.
        /// </summary>
        /// <param name="name"><see cref="SitemapRuleInfo"/> name.</param>
        /// <param name="siteName">Site name.</param>
        public static SitemapRuleInfo GetSitemapRuleInfo(string name, string siteName)
        {
            return ProviderObject.GetInfoByCodeName(name, SiteInfoProvider.GetSiteID(siteName));
        }


        /// <summary>
        /// Sets (updates or inserts) specified <see cref="SitemapRuleInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="SitemapRuleInfo"/> to be set.</param>
        public static void SetSitemapRuleInfo(SitemapRuleInfo infoObj)
        {
            ProviderObject.SetInfo(infoObj);
        }


        /// <summary>
        /// Deletes specified <see cref="SitemapRuleInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="SitemapRuleInfo"/> to be deleted.</param>
        public static void DeleteSitemapRuleInfo(SitemapRuleInfo infoObj)
        {
            ProviderObject.DeleteInfo(infoObj);
        }


        /// <summary>
        /// Deletes <see cref="SitemapRuleInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="SitemapRuleInfo"/> ID.</param>
        public static void DeleteSitemapRuleInfo(int id)
        {
            SitemapRuleInfo infoObj = GetSitemapRuleInfo(id);
            DeleteSitemapRuleInfo(infoObj);
        }
    }
}