using System;
using System.Data;
using System.Runtime.Serialization;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using DeleteBoilerplate;

[assembly: RegisterObjectType(typeof(SitemapRuleInfo), SitemapRuleInfo.OBJECT_TYPE)]

namespace DeleteBoilerplate
{
    /// <summary>
    /// Data container class for <see cref="SitemapRuleInfo"/>.
    /// </summary>
    [Serializable]
    public partial class SitemapRuleInfo : AbstractInfo<SitemapRuleInfo>
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "deleteboilerplate.sitemaprule";


        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(SitemapRuleInfoProvider), OBJECT_TYPE, "DeleteBoilerplate.SitemapRule", "SitemapRuleID", "SitemapRuleLastModified", "SitemapRuleGuid", "FileName", null, null, null, null, null)
        {
            ModuleName = "DeleteBoilerplate",
            TouchCacheDependencies = true,
        };


        /// <summary>
        /// Sitemap rule ID.
        /// </summary>
        [DatabaseField]
        public virtual int SitemapRuleID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("SitemapRuleID"), 0);
            }
            set
            {
                SetValue("SitemapRuleID", value);
            }
        }


        /// <summary>
        /// File name.
        /// </summary>
        [DatabaseField]
        public virtual string FileName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("FileName"), String.Empty);
            }
            set
            {
                SetValue("FileName", value, String.Empty);
            }
        }


        /// <summary>
        /// Rule type.
        /// </summary>
        [DatabaseField]
        public virtual int RuleType
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("RuleType"), 0);
            }
            set
            {
                SetValue("RuleType", value, 0);
            }
        }


        /// <summary>
        /// Container page.
        /// </summary>
        [DatabaseField]
        public virtual string ContainerPage
        {
            get
            {
                return ValidationHelper.GetString(GetValue("ContainerPage"), String.Empty);
            }
            set
            {
                SetValue("ContainerPage", value, String.Empty);
            }
        }


        /// <summary>
        /// Custom class.
        /// </summary>
        [DatabaseField]
        public virtual string CustomClass
        {
            get
            {
                return ValidationHelper.GetString(GetValue("CustomClass"), String.Empty);
            }
            set
            {
                SetValue("CustomClass", value, String.Empty);
            }
        }


        /// <summary>
        /// Sitemap rule guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid SitemapRuleGuid
        {
            get
            {
                return ValidationHelper.GetGuid(GetValue("SitemapRuleGuid"), Guid.Empty);
            }
            set
            {
                SetValue("SitemapRuleGuid", value);
            }
        }


        /// <summary>
        /// Sitemap rule last modified.
        /// </summary>
        [DatabaseField]
        public virtual DateTime SitemapRuleLastModified
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("SitemapRuleLastModified"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("SitemapRuleLastModified", value);
            }
        }


        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            SitemapRuleInfoProvider.DeleteSitemapRuleInfo(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            SitemapRuleInfoProvider.SetSitemapRuleInfo(this);
        }


        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected SitemapRuleInfo(SerializationInfo info, StreamingContext context)
            : base(info, context, TYPEINFO)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="SitemapRuleInfo"/> class.
        /// </summary>
        public SitemapRuleInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="SitemapRuleInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public SitemapRuleInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}