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

[assembly: RegisterDocumentType(NavigationContainer.CLASS_NAME, typeof(NavigationContainer))]

namespace CMS.DocumentEngine.Types.DeleteBoilerplate
{
    /// <summary>
    /// Represents a content item of type NavigationContainer.
    /// </summary>
    public partial class NavigationContainer : TreeNode
    {
        #region "Constants and variables"

        /// <summary>
        /// The name of the data class.
        /// </summary>
        public const string CLASS_NAME = "DeleteBoilerplate.NavigationContainer";


        /// <summary>
        /// The instance of the class that provides extended API for working with NavigationContainer fields.
        /// </summary>
        private readonly NavigationContainerFields mFields;

        #endregion


        #region "Properties"

        /// <summary>
        /// Gets an object that provides extended API for working with NavigationContainer fields.
        /// </summary>
        [RegisterProperty]
        public NavigationContainerFields Fields
        {
            get
            {
                return mFields;
            }
        }


        /// <summary>
        /// Provides extended API for working with NavigationContainer fields.
        /// </summary>
        [RegisterAllProperties]
        public partial class NavigationContainerFields : AbstractHierarchicalObject<NavigationContainerFields>
        {
            /// <summary>
            /// The content item of type NavigationContainer that is a target of the extended API.
            /// </summary>
            private readonly NavigationContainer mInstance;


            /// <summary>
            /// Initializes a new instance of the <see cref="NavigationContainerFields" /> class with the specified content item of type NavigationContainer.
            /// </summary>
            /// <param name="instance">The content item of type NavigationContainer that is a target of the extended API.</param>
            public NavigationContainerFields(NavigationContainer instance)
            {
                mInstance = instance;
            }
        }

        #endregion


        #region "Constructors"

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationContainer" /> class.
        /// </summary>
        public NavigationContainer() : base(CLASS_NAME)
        {
            mFields = new NavigationContainerFields(this);
        }

        #endregion
    }
}
