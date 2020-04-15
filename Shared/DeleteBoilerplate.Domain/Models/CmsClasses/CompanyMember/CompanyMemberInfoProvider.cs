using System;
using System.Data;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;

namespace DeleteBoilerplate
{
    /// <summary>
    /// Class providing <see cref="CompanyMemberInfo"/> management.
    /// </summary>
    public partial class CompanyMemberInfoProvider : AbstractInfoProvider<CompanyMemberInfo, CompanyMemberInfoProvider>
    {
        /// <summary>
        /// Creates an instance of <see cref="CompanyMemberInfoProvider"/>.
        /// </summary>
        public CompanyMemberInfoProvider()
            : base(CompanyMemberInfo.TYPEINFO)
        {
        }


        /// <summary>
        /// Returns a query for all the <see cref="CompanyMemberInfo"/> objects.
        /// </summary>
        public static ObjectQuery<CompanyMemberInfo> GetCompanyMembers()
        {
            return ProviderObject.GetObjectQuery();
        }


        /// <summary>
        /// Returns <see cref="CompanyMemberInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="CompanyMemberInfo"/> ID.</param>
        public static CompanyMemberInfo GetCompanyMemberInfo(int id)
        {
            return ProviderObject.GetInfoById(id);
        }


        /// <summary>
        /// Returns <see cref="CompanyMemberInfo"/> with specified name.
        /// </summary>
        /// <param name="name"><see cref="CompanyMemberInfo"/> name.</param>
        public static CompanyMemberInfo GetCompanyMemberInfo(string name)
        {
            return ProviderObject.GetInfoByCodeName(name);
        }


        /// <summary>
        /// Sets (updates or inserts) specified <see cref="CompanyMemberInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="CompanyMemberInfo"/> to be set.</param>
        public static void SetCompanyMemberInfo(CompanyMemberInfo infoObj)
        {
            ProviderObject.SetInfo(infoObj);
        }


        /// <summary>
        /// Deletes specified <see cref="CompanyMemberInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="CompanyMemberInfo"/> to be deleted.</param>
        public static void DeleteCompanyMemberInfo(CompanyMemberInfo infoObj)
        {
            ProviderObject.DeleteInfo(infoObj);
        }


        /// <summary>
        /// Deletes <see cref="CompanyMemberInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="CompanyMemberInfo"/> ID.</param>
        public static void DeleteCompanyMemberInfo(int id)
        {
            CompanyMemberInfo infoObj = GetCompanyMemberInfo(id);
            DeleteCompanyMemberInfo(infoObj);
        }
    }
}