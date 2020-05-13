using CMS;
using CMS.Base;
using CMS.ContactManagement;
using DeleteBoilerplate.Common.ContactProviders;
using DeleteBoilerplate.Common.Extensions;
using DeleteBoilerplate.Common.Helpers;
using System;
using System.Web;

[assembly: RegisterImplementation(typeof(ICurrentContactProvider), typeof(CustomCurrentContactProvider))]
namespace DeleteBoilerplate.Common.ContactProviders
{
    public class CustomCurrentContactProvider : ICurrentContactProvider
    {
        private readonly IContactValidator mContactValidator;
        private readonly IContactPersistentStorage mContactPersistentStorage;
        private readonly ICurrentUserContactProvider mCurrentUserContactProvider;
        private readonly IContactCreator mContactCreator;
        private readonly IContactRelationAssigner mContactRelationAssigner;

        /// <summary>
        /// Instantiates a new instance of <see cref="T:CMS.ContactManagement.DefaultCurrentContactProvider" />.
        /// </summary>
        /// <param name="contactValidator">Provides method for validating contact against the database</param>
        /// <param name="contactPersistentStorage">Provides methods for storing and retrieving contact to/from persistent storage</param>
        /// <param name="currentUserContactProvider">Provides methods for getting <see cref="T:CMS.ContactManagement.ContactInfo" /> for <see cref="T:CMS.Membership.UserInfo" /></param>
        /// <param name="contactCreator">Provides method for creating new contacts</param>
        /// <param name="contactRelationAssigner">Provides method for creating relationship between <see cref="T:CMS.ContactManagement.ContactInfo" /> and other objects</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="contactValidator" /> is <c>null</c> -or-
        /// <paramref name="contactPersistentStorage" /> is <c>null</c> -or-
        /// <paramref name="currentUserContactProvider" /> is <c>null</c> -or-
        /// <paramref name="contactCreator" /> is <c>null</c> -or-
        /// <paramref name="contactRelationAssigner" /> is <c>null</c>
        /// </exception>
        public CustomCurrentContactProvider(IContactValidator contactValidator,
            IContactPersistentStorage contactPersistentStorage, ICurrentUserContactProvider currentUserContactProvider,
            IContactCreator contactCreator, IContactRelationAssigner contactRelationAssigner)
        {
            if (contactValidator == null)
                throw new ArgumentNullException("contactValidator");
            if (contactPersistentStorage == null)
                throw new ArgumentNullException("contactPersistentStorage");
            if (currentUserContactProvider == null)
                throw new ArgumentNullException("currentUserContactProvider");
            if (contactCreator == null)
                throw new ArgumentNullException("contactCreator");
            if (contactRelationAssigner == null)
                throw new ArgumentNullException("contactRelationAssigner");
            this.mContactValidator = contactValidator;
            this.mContactPersistentStorage = contactPersistentStorage;
            this.mCurrentUserContactProvider = currentUserContactProvider;
            this.mContactCreator = contactCreator;
            this.mContactRelationAssigner = contactRelationAssigner;
        }

        /// <summary>
        /// Recognizes a contact currently browsing the live site. A returned contact is always
        /// valid (exists in database, is not merged, etc.) and is automatically assigned to <paramref name="currentUser" /> User (if it isn't a public user).
        /// </summary>
        /// <param name="currentUser">Currently signed in user or public user</param>
        /// <param name="forceUserMatching">If true, current contact is tried to be determined by the user even if it can be found by other ways</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="currentUser" /> is <c>null</c>
        /// </exception>
        /// <returns>Recognized contact</returns>
        public ContactInfo GetExistingContact(IUserInfo currentUser, bool forceUserMatching)
        {
            if (currentUser == null)
                throw new ArgumentNullException("currentUser");
            ContactInfo currentContact =
                this.mContactValidator.ValidateContact(this.mContactPersistentStorage.GetPersistentContact());

            currentContact = GetNewFormSubmissionContact(currentContact);

            if (currentContact == null || forceUserMatching)
                currentContact = this.mContactValidator.ValidateContact(currentContact != null
                    ? this.mCurrentUserContactProvider.GetContactForCurrentUserAndContact(currentUser, currentContact)
                    : this.mCurrentUserContactProvider.GetContactForCurrentUser(currentUser));


            return currentContact;
        }

        /// <summary>
        /// Recognizes a contact currently browsing the live site. If the contact cannot be recognized a new one is created. A returned contact is always
        /// valid (exists in database, is not merged, etc.) and is automatically assigned to <paramref name="currentUser" /> User (if it isn't a public
        /// user).
        /// </summary>
        /// <param name="currentUser">Currently signed in user or public user</param>
        /// <param name="forceUserMatching">If true, current contact is tried to be determined by the user even if it can be found by other ways</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="currentUser" /> is <c>null</c>
        /// </exception>
        /// <returns>Recognized contact or new contact, if it was not possible to recognize contact.</returns>
        public ContactInfo GetCurrentContact(IUserInfo currentUser, bool forceUserMatching)
        {
            if (currentUser == null)
                throw new ArgumentNullException("currentUser");
            ContactInfo contact = this.GetExistingContact(currentUser, forceUserMatching);

            if (contact == null)
            {
                contact = this.mContactCreator.CreateAnonymousContact();
                this.AssignContactToUser(currentUser, contact);
                this.SetCurrentContact(contact);
            }
            return contact;
        }

        private ContactInfo GetNewFormSubmissionContact(ContactInfo contact)
        {
            try
            {
                var httpRequest = HttpContext.Current?.Request;
                if (httpRequest == null)
                    return contact;
                if (httpRequest.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
                    return contact;
                var newEmail = httpRequest["Email"] ?? httpRequest["EmailAddress"];
                if (newEmail.IsNullOrEmpty())
                    return contact;
                var formContact = ContactInfoProvider.GetContactInfo(newEmail);
                if (formContact == null)
                    return null;
                if (contact == null)
                    return formContact;
                return !string.Equals(contact.ContactEmail, formContact.ContactEmail, StringComparison.InvariantCultureIgnoreCase)
                    ? formContact
                    : contact;
            }
            catch (Exception exception)
            {
                LogHelper.LogException("RESOLVE", exception);
                return contact;
            }
        }

        private void AssignContactToUser(IUserInfo user, ContactInfo contact)
        {
            if (user == null || contact == null || user.IsPublic())
                return;
            this.mContactRelationAssigner.Assign(user, contact);
        }

        /// <summary>
        /// Stores information about the current contact into the persistent storage (<see cref="T:CMS.ContactManagement.IContactPersistentStorage" />), so that the next time
        /// (possibly in another request in the same session) <see cref="M:CMS.ContactManagement.ICurrentContactProvider.GetCurrentContact(CMS.Base.IUserInfo,System.Boolean)" /> is called, the stored
        /// contact is returned.
        /// </summary>
        /// <param name="contact">The contact who performed the request</param>
        public void SetCurrentContact(ContactInfo contact)
        {
            if (contact == null)
                throw new ArgumentNullException("contact");
            this.mContactPersistentStorage.SetPersistentContact(contact);
        }
    }
}