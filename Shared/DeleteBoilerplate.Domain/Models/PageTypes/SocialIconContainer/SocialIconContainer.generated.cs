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

[assembly: RegisterDocumentType(SocialIconContainer.CLASS_NAME, typeof(SocialIconContainer))]

namespace CMS.DocumentEngine.Types.DeleteBoilerplate
{
    /// <summary>
    /// Represents a content item of type SocialIconContainer.
    /// </summary>
    public partial class SocialIconContainer : TreeNode
    {
        #region "Constants and variables"

        /// <summary>
        /// The name of the data class.
        /// </summary>
        public const string CLASS_NAME = "DeleteBoilerplate.SocialIconContainer";


        /// <summary>
        /// The instance of the class that provides extended API for working with SocialIconContainer fields.
        /// </summary>
        private readonly SocialIconContainerFields mFields;

        #endregion


        #region "Properties"

        /// <summary>
        /// Gets an object that provides extended API for working with SocialIconContainer fields.
        /// </summary>
        [RegisterProperty]
        public SocialIconContainerFields Fields
        {
            get
            {
                return mFields;
            }
        }


        /// <summary>
        /// Provides extended API for working with SocialIconContainer fields.
        /// </summary>
        [RegisterAllProperties]
        public partial class SocialIconContainerFields : AbstractHierarchicalObject<SocialIconContainerFields>
        {
            /// <summary>
            /// The content item of type SocialIconContainer that is a target of the extended API.
            /// </summary>
            private readonly SocialIconContainer mInstance;


            /// <summary>
            /// Initializes a new instance of the <see cref="SocialIconContainerFields" /> class with the specified content item of type SocialIconContainer.
            /// </summary>
            /// <param name="instance">The content item of type SocialIconContainer that is a target of the extended API.</param>
            public SocialIconContainerFields(SocialIconContainer instance)
            {
                mInstance = instance;
            }
        }

        #endregion


        #region "Constructors"

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialIconContainer" /> class.
        /// </summary>
        public SocialIconContainer() : base(CLASS_NAME)
        {
            mFields = new SocialIconContainerFields(this);
        }

        #endregion
    }
}
