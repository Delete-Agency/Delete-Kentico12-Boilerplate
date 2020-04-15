using System;
using System.Data;
using System.Runtime.Serialization;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using DeleteBoilerplate;

[assembly: RegisterObjectType(typeof(CompanyMemberInfo), CompanyMemberInfo.OBJECT_TYPE)]

namespace DeleteBoilerplate
{
    /// <summary>
    /// Data container class for <see cref="CompanyMemberInfo"/>.
    /// </summary>
    [Serializable]
    public partial class CompanyMemberInfo : AbstractInfo<CompanyMemberInfo>
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "deleteboilerplate.companymember";


        /// <summary>
        /// Type information.
        /// </summary>
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(CompanyMemberInfoProvider), OBJECT_TYPE, "DeleteBoilerplate.CompanyMember", "CompanyMemberID", "CompanyMemberLastModified", "CompanyMemberGuid", "FullName", null, null, null, null, null)
        {
            ModuleName = "DeleteBoilerplate",
            TouchCacheDependencies = true,
        };


        /// <summary>
        /// Company member ID.
        /// </summary>
        [DatabaseField]
        public virtual int CompanyMemberID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("CompanyMemberID"), 0);
            }
            set
            {
                SetValue("CompanyMemberID", value);
            }
        }


        /// <summary>
        /// Full name.
        /// </summary>
        [DatabaseField]
        public virtual string FullName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("FullName"), String.Empty);
            }
            set
            {
                SetValue("FullName", value);
            }
        }


        /// <summary>
        /// Team.
        /// </summary>
        [DatabaseField]
        public virtual string Team
        {
            get
            {
                return ValidationHelper.GetString(GetValue("Team"), String.Empty);
            }
            set
            {
                SetValue("Team", value);
            }
        }


        /// <summary>
        /// Company member guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid CompanyMemberGuid
        {
            get
            {
                return ValidationHelper.GetGuid(GetValue("CompanyMemberGuid"), Guid.Empty);
            }
            set
            {
                SetValue("CompanyMemberGuid", value);
            }
        }


        /// <summary>
        /// Company member last modified.
        /// </summary>
        [DatabaseField]
        public virtual DateTime CompanyMemberLastModified
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("CompanyMemberLastModified"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("CompanyMemberLastModified", value);
            }
        }


        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            CompanyMemberInfoProvider.DeleteCompanyMemberInfo(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            CompanyMemberInfoProvider.SetCompanyMemberInfo(this);
        }


        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected CompanyMemberInfo(SerializationInfo info, StreamingContext context)
            : base(info, context, TYPEINFO)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="CompanyMemberInfo"/> class.
        /// </summary>
        public CompanyMemberInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="CompanyMemberInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public CompanyMemberInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}