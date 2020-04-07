using System;
using AutoMapper;
using CMS.ContactManagement;
using CMS.DataProtection;
using LightInject;
using DeleteBoilerplate.Account.Infrastructure;
using DeleteBoilerplate.Account.Models;

namespace DeleteBoilerplate.Account.Services
{
    public class ConsentService : IConsentService
    {
        [Inject]
        protected IConsentAgreementService ConsentAgreementService { get; set; }

        [Inject]
        protected IMapper Mapper { get; set; }


        public (bool Status, string ErrorMessage) UpdateAgreement(bool forAgree, int? consentId, AppUser user) =>
            forAgree ? Agree(consentId, user) : Revoke(consentId, user);

        public (bool Status, string ErrorMessage) Agree(int? consentId, AppUser user)
        {
            var consent = consentId.HasValue ? ConsentInfoProvider.GetConsentInfo(consentId.Value) : null;
            if (consent is null)
                return (false, "Consent with such id does not exist");
            (bool revoked, string errorMessage) = ApplyToConsent(consent, user, ConsentAgreementService.Agree);
            return (!revoked, errorMessage);
        }

        public (bool Status, string ErrorMessage) Revoke(int? consentId, AppUser user)
        {
            var consent = consentId.HasValue ? ConsentInfoProvider.GetConsentInfo(consentId.Value) : null;
            if (consent is null)
                return (false, "Consent with such id does not exist");
            return ApplyToConsent(consent, user, ConsentAgreementService.Revoke);
        }

        public T ModelWithConsent<T>(string consentName, string culture)
            where T : ConsentAgreementViewModel, new()
        {
            var model = new T();
            ContactInfo Strategy() => ContactManagementContext.CurrentContact;
            InitializeConsent(ref model, Strategy, consentName, culture);
            return model;
        }

        public void InitializeConsent<T>(ref T model, string email, string consentName, string culture)
            where T : ConsentAgreementViewModel
        {
            ContactInfo Strategy() => ContactManagementContext.CurrentContact ?? ContactInfoProvider.GetContactInfo(email);
            InitializeConsent(ref model, Strategy, consentName, culture);
        }

        public void InitializeConsent<T>(ref T model, Func<ContactInfo> getContactStrategy, string consentName, string culture)
            where T : ConsentAgreementViewModel
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var consent = ConsentInfoProvider.GetConsentInfo(consentName);
            if (consent is null) return;
            var contact = getContactStrategy();
            model.ConsentId = consent.ConsentID;
            model.ConsentShortText = consent.GetConsentText(culture)?.ShortText;
            model.ConsentIsAgreed = contact != null && ConsentAgreementService.IsAgreed(contact, consent);
        }

        private (bool Revoked, string ErrorMessage) ApplyToConsent(ConsentInfo consent, AppUser user, 
            Func<ContactInfo, ConsentInfo, ConsentAgreementInfo> apply)
        {
            var contact = Mapper.Map(user, GetOrCreateContact(user?.Email));
            ContactInfoProvider.SetContactInfo(contact);
            var agreement = apply(contact, consent);
            return (agreement.ConsentAgreementRevoked, string.Empty);
        }

        private static ContactInfo GetOrCreateContact(string email) => 
            ContactManagementContext.CurrentContact ?? ContactInfoProvider.GetContactInfo(email) ?? new ContactInfo();
    }
}