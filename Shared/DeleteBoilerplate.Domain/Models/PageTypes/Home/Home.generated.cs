//--------------------------------------------------------------------------------------------------
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

[assembly: RegisterDocumentType(Home.CLASS_NAME, typeof(Home))]

namespace CMS.DocumentEngine.Types.DeleteBoilerplate
{
	/// <summary>
	/// Represents a content item of type Home.
	/// </summary>
	public partial class Home : TreeNode
	{
		#region "Constants and variables"

		/// <summary>
		/// The name of the data class.
		/// </summary>
		public const string CLASS_NAME = "DeleteBoilerplate.Home";


		/// <summary>
		/// The instance of the class that provides extended API for working with Home fields.
		/// </summary>
		private readonly HomeFields mFields;

		#endregion


		#region "Properties"

		/// <summary>
		/// HomeID.
		/// </summary>
		[DatabaseIDField]
		public int HomeID
		{
			get
			{
				return ValidationHelper.GetInteger(GetValue("HomeID"), 0);
			}
			set
			{
				SetValue("HomeID", value);
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
		/// Gets an object that provides extended API for working with Home fields.
		/// </summary>
		[RegisterProperty]
		public HomeFields Fields
		{
			get
			{
				return mFields;
			}
		}


		/// <summary>
		/// Provides extended API for working with Home fields.
		/// </summary>
		[RegisterAllProperties]
		public partial class HomeFields : AbstractHierarchicalObject<HomeFields>
		{
			/// <summary>
			/// The content item of type Home that is a target of the extended API.
			/// </summary>
			private readonly Home mInstance;


			/// <summary>
			/// Initializes a new instance of the <see cref="HomeFields" /> class with the specified content item of type Home.
			/// </summary>
			/// <param name="instance">The content item of type Home that is a target of the extended API.</param>
			public HomeFields(Home instance)
			{
				mInstance = instance;
			}


			/// <summary>
			/// HomeID.
			/// </summary>
			public int ID
			{
				get
				{
					return mInstance.HomeID;
				}
				set
				{
					mInstance.HomeID = value;
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
		/// Initializes a new instance of the <see cref="Home" /> class.
		/// </summary>
		public Home() : base(CLASS_NAME)
		{
			mFields = new HomeFields(this);
		}

		#endregion
	}
}