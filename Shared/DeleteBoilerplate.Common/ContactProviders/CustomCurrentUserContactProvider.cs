using System;
using System.Linq;
using CMS;
using CMS.Base;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.EventLog;
using DeleteBoilerplate.Common.ContactProviders;
using DeleteBoilerplate.Common.Extensions;

[assembly: RegisterImplementation(typeof(ICurrentUserContactProvider), typeof(CustomCurrentUserContactProvider))]
namespace DeleteBoilerplate.Common.ContactProviders
{
    public class CustomCurrentUserContactProvider : ICurrentUserContactProvider
    {
        private readonly IContactMergeService mMergeService;
        private readonly IContactRelationAssigner mContactRelationAssigner;

        public CustomCurrentUserContactProvider(IContactMergeService mergeService, IContactRelationAssigner contactRelationAssigner)
        {
            this.mMergeService = mergeService;
            this.mContactRelationAssigner = contactRelationAssigner;
        }

        /// <summary>
        /// Gets a <see cref="T:CMS.ContactManagement.ContactInfo" /> for the given <paramref name="currentUser" /> when no information about the possible current contact is available.
        /// </summary>
        /// <param name="currentUser">The user the contacts will be obtained for</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="currentUser" />Thrown when <paramref name="currentUser" /> is <c>null</c>.</exception>
        /// <returns>
        /// Contact selected as the current one for the <paramref name="currentUser" />. If the user is public, returns null.
        /// </returns>
        public ContactInfo GetContactForCurrentUser(IUserInfo currentUser)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));
            if (currentUser.IsPublic())
                return (ContactInfo)null;
            return this.GetRelatedContact(currentUser);
        }

        /// <summary>
        /// Gets a <see cref="T:CMS.ContactManagement.ContactInfo" /> for the given <paramref name="currentUser" /> when there is a priori information about the possible current contact available.
        /// </summary>
        /// <param name="currentUser">The user the contact will be obtained for</param>
        /// <param name="currentContact">A possible candidate to be selected as the current contact</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="currentUser" /> or <paramref name="currentContact" /></exception>
        /// <returns>
        /// The contact selected as the current one for the <paramref name="currentUser" />. Note that all other contacts related to the <paramref name="currentUser" /> are merged into this one.
        /// If the user is public, returns null.
        /// </returns>
        public ContactInfo GetContactForCurrentUserAndContact(IUserInfo currentUser, ContactInfo currentContact)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));
            if (currentContact == null)
                throw new ArgumentNullException(nameof(currentContact));
            if (currentUser.IsPublic() || currentUser.Email.IsNullOrEmpty())
                return null;

            try
            {
                var relatedContact = this.GetRelatedContact(currentUser);
                if (relatedContact != null)
                {
                    if (currentContact.ContactEmail.IsNullOrEmpty())
                    {
                        this.mMergeService.MergeContacts(currentContact, relatedContact);
                    }

                    if (!this.ContactHasUserAssigned(currentUser, relatedContact))
                    {
                        this.mContactRelationAssigner.Assign(currentUser, relatedContact);
                    }

                    return relatedContact;
                }
                else
                {
                    if (!currentContact.ContactEmail.IsNullOrEmpty()) return null;

                    currentContact.ContactEmail = currentUser.Email;
                    if (!currentUser.FirstName.IsNullOrEmpty())
                    {
                        currentContact.ContactFirstName = currentUser.FirstName;
                    }
                    if (!currentUser.LastName.IsNullOrEmpty())
                    {
                        currentContact.ContactLastName = currentUser.LastName;
                    }
                    ContactInfoProvider.SetContactInfo(currentContact);
                    this.mContactRelationAssigner.Assign(currentUser, currentContact);

                    return currentContact;
                }
            }
            catch (Exception exception)
            {
                EventLogProvider.LogException(nameof(CustomCurrentUserContactProvider), "RESOLVE", exception);
                return null;
            }
        }

        /// <summary>
        /// Gets all contacts that are related to the given <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user the related contacts are searched for</param>
        /// <returns>A collection of contacts that are related to the <paramref name="user" />.</returns>
        private ContactInfo GetRelatedContact(IUserInfo user)
        {
            try
            {
                return user.Email.IsNullOrEmpty()
                    ? null
                    : ContactInfoProvider.GetContactInfo(user.Email);
            }
            catch (Exception exception)
            {
                EventLogProvider.LogException(nameof(CustomCurrentUserContactProvider), "GET", exception);
                return null;
            }
            
        }

        /// <summary>
        /// Determines whether the given <paramref name="contact" /> has at least one user assigned.
        /// </summary>
        /// <param name="contact">The contact the user assignment will be searched for</param>
        /// <returns>True, if the contact has at least one user assigned; otherwise, false.</returns>
        private bool ContactHasUserAssigned(IUserInfo user, ContactInfo contact)
        {
            return new IDQuery("om.membershipuser", "ContactID", true)
                .WhereEquals("ContactID", contact.ContactID)
                .And()
                .WhereEquals("RelatedID", user.UserID)
                .Any<BaseInfo>();
        }
    }
}
