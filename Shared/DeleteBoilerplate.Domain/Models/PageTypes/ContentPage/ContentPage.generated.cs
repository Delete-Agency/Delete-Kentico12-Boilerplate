﻿//--------------------------------------------------------------------------------------------------
// <auto-generated>
//
//     This code was generated by code generator tool.
//
//     To customize the code use your own partial class. For more info about how to use and customize
//     the generated code see the documentation at http://docs.kentico.com.
//
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using CMS;
using CMS.Base;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using CMS.DocumentEngine;

[assembly: RegisterDocumentType(ContentPage.CLASS_NAME, typeof(ContentPage))]

namespace CMS.DocumentEngine.Types.DeleteBoilerplate
{
    /// <summary>
    /// Represents a content item of type ContentPage.
    /// </summary>
    public partial class ContentPage : TreeNode
    {
        #region "Constants and variables"

        /// <summary>
        /// The name of the data class.
        /// </summary>
        public const string CLASS_NAME = "DeleteBoilerplate.ContentPage";


        /// <summary>
        /// The instance of the class that provides extended API for working with ContentPage fields.
        /// </summary>
        private readonly ContentPageFields mFields;

        #endregion


        #region "Properties"

        /// <summary>
        /// ContentPageID.
        /// </summary>
        [DatabaseIDField]
        public int ContentPageID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("ContentPageID"), 0);
            }
            set
            {
                SetValue("ContentPageID", value);
            }
        }


        /// <summary>
        /// Name.
        /// </summary>
        [DatabaseField]
        public string Name
        {
            get
            {
                return ValidationHelper.GetString(GetValue("Name"), @"");
            }
            set
            {
                SetValue("Name", value);
            }
        }


        /// <summary>
        /// Seo Url.
        /// </summary>
        [DatabaseField]
        public string SeoUrl
        {
            get
            {
                return ValidationHelper.GetString(GetValue("SeoUrl"), @"");
            }
            set
            {
                SetValue("SeoUrl", value);
            }
        }


        /// <summary>
        /// Taxonomy.
        /// </summary>
        [DatabaseField]
        public string Taxonomy
        {
            get
            {
                return ValidationHelper.GetString(GetValue("Taxonomy"), @"");
            }
            set
            {
                SetValue("Taxonomy", value);
            }
        }


        /// <summary>
        /// Gets an object that provides extended API for working with ContentPage fields.
        /// </summary>
        [RegisterProperty]
        public ContentPageFields Fields
        {
            get
            {
                return mFields;
            }
        }


        /// <summary>
        /// Provides extended API for working with ContentPage fields.
        /// </summary>
        [RegisterAllProperties]
        public partial class ContentPageFields : AbstractHierarchicalObject<ContentPageFields>
        {
            /// <summary>
            /// The content item of type ContentPage that is a target of the extended API.
            /// </summary>
            private readonly ContentPage mInstance;


            /// <summary>
            /// Initializes a new instance of the <see cref="ContentPageFields" /> class with the specified content item of type ContentPage.
            /// </summary>
            /// <param name="instance">The content item of type ContentPage that is a target of the extended API.</param>
            public ContentPageFields(ContentPage instance)
            {
                mInstance = instance;
            }


            /// <summary>
            /// ContentPageID.
            /// </summary>
            public int ID
            {
                get
                {
                    return mInstance.ContentPageID;
                }
                set
                {
                    mInstance.ContentPageID = value;
                }
            }


            /// <summary>
            /// Name.
            /// </summary>
            public string Name
            {
                get
                {
                    return mInstance.Name;
                }
                set
                {
                    mInstance.Name = value;
                }
            }


            /// <summary>
            /// Seo Url.
            /// </summary>
            public string SeoUrl
            {
                get
                {
                    return mInstance.SeoUrl;
                }
                set
                {
                    mInstance.SeoUrl = value;
                }
            }


            /// <summary>
            /// Taxonomy.
            /// </summary>
            public string Taxonomy
            {
                get
                {
                    return mInstance.Taxonomy;
                }
                set
                {
                    mInstance.Taxonomy = value;
                }
            }
        }

        #endregion


        #region "Constructors"

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPage" /> class.
        /// </summary>
        public ContentPage() : base(CLASS_NAME)
        {
            mFields = new ContentPageFields(this);
        }

        #endregion
    }
}
