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
using CMS.OnlineForms.Types;
using CMS.OnlineForms;

[assembly: RegisterBizForm(ContactItem.CLASS_NAME, typeof(ContactItem))]

namespace CMS.OnlineForms.Types
{
	/// <summary>
	/// Represents a content item of type ContactItem.
	/// </summary>
	public partial class ContactItem : BizFormItem
	{
		#region "Constants and variables"

		/// <summary>
		/// The name of the data class.
		/// </summary>
		public const string CLASS_NAME = "BizForm.Contact";


		/// <summary>
		/// The instance of the class that provides extended API for working with ContactItem fields.
		/// </summary>
		private readonly ContactItemFields mFields;

		#endregion


		#region "Properties"

		/// <summary>
		/// First Name.
		/// </summary>
		[DatabaseField]
		public string FirstName
		{
			get
			{
				return ValidationHelper.GetString(GetValue("FirstName"), @"");
			}
			set
			{
				SetValue("FirstName", value);
			}
		}


		/// <summary>
		/// Last Name.
		/// </summary>
		[DatabaseField]
		public string LastName
		{
			get
			{
				return ValidationHelper.GetString(GetValue("LastName"), @"");
			}
			set
			{
				SetValue("LastName", value);
			}
		}


		/// <summary>
		/// Email.
		/// </summary>
		[DatabaseField]
		public string Email
		{
			get
			{
				return ValidationHelper.GetString(GetValue("Email"), @"");
			}
			set
			{
				SetValue("Email", value);
			}
		}


		/// <summary>
		/// Telephone.
		/// </summary>
		[DatabaseField]
		public string Telephone
		{
			get
			{
				return ValidationHelper.GetString(GetValue("Telephone"), @"");
			}
			set
			{
				SetValue("Telephone", value);
			}
		}


		/// <summary>
		/// Message.
		/// </summary>
		[DatabaseField]
		public string Message
		{
			get
			{
				return ValidationHelper.GetString(GetValue("Message"), @"");
			}
			set
			{
				SetValue("Message", value);
			}
		}


		/// <summary>
		/// IsConsented.
		/// </summary>
		[DatabaseField]
		public bool IsConsented
		{
			get
			{
				return ValidationHelper.GetBoolean(GetValue("IsConsented"), false);
			}
			set
			{
				SetValue("IsConsented", value);
			}
		}


		/// <summary>
		/// Gets an object that provides extended API for working with ContactItem fields.
		/// </summary>
		[RegisterProperty]
		public ContactItemFields Fields
		{
			get
			{
				return mFields;
			}
		}


		/// <summary>
		/// Provides extended API for working with ContactItem fields.
		/// </summary>
		[RegisterAllProperties]
		public partial class ContactItemFields : AbstractHierarchicalObject<ContactItemFields>
		{
			/// <summary>
			/// The content item of type ContactItem that is a target of the extended API.
			/// </summary>
			private readonly ContactItem mInstance;


			/// <summary>
			/// Initializes a new instance of the <see cref="ContactItemFields" /> class with the specified content item of type ContactItem.
			/// </summary>
			/// <param name="instance">The content item of type ContactItem that is a target of the extended API.</param>
			public ContactItemFields(ContactItem instance)
			{
				mInstance = instance;
			}


			/// <summary>
			/// First Name.
			/// </summary>
			public string FirstName
			{
				get
				{
					return mInstance.FirstName;
				}
				set
				{
					mInstance.FirstName = value;
				}
			}


			/// <summary>
			/// Last Name.
			/// </summary>
			public string LastName
			{
				get
				{
					return mInstance.LastName;
				}
				set
				{
					mInstance.LastName = value;
				}
			}


			/// <summary>
			/// Email.
			/// </summary>
			public string Email
			{
				get
				{
					return mInstance.Email;
				}
				set
				{
					mInstance.Email = value;
				}
			}


			/// <summary>
			/// Telephone.
			/// </summary>
			public string Telephone
			{
				get
				{
					return mInstance.Telephone;
				}
				set
				{
					mInstance.Telephone = value;
				}
			}


			/// <summary>
			/// Message.
			/// </summary>
			public string Message
			{
				get
				{
					return mInstance.Message;
				}
				set
				{
					mInstance.Message = value;
				}
			}


			/// <summary>
			/// IsConsented.
			/// </summary>
			public bool IsConsented
			{
				get
				{
					return mInstance.IsConsented;
				}
				set
				{
					mInstance.IsConsented = value;
				}
			}
		}

		#endregion


		#region "Constructors"

		/// <summary>
		/// Initializes a new instance of the <see cref="ContactItem" /> class.
		/// </summary>
		public ContactItem() : base(CLASS_NAME)
		{
			mFields = new ContactItemFields(this);
		}

		#endregion
	}
}