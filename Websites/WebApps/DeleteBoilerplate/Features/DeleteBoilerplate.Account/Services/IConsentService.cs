using System;
using CMS.ContactManagement;
using DeleteBoilerplate.Account.Infrastructure;
using DeleteBoilerplate.Account.Models;

namespace DeleteBoilerplate.Account.Services
{
    public interface IConsentService
    {
        T ModelWithConsent<T>(string consentName, string culture)
            where T : ConsentAgreementViewModel, new();

        void InitializeConsent<T>(ref T model, string email, string consentName, string culture)
            where T : ConsentAgreementViewModel;

        void InitializeConsent<T>(ref T model, Func<ContactInfo> getContactStrategy, string consentName, string culture)
            where T : ConsentAgreementViewModel;

        (bool Status, string ErrorMessage) Agree(int? consentId, AppUser user);

        (bool Status, string ErrorMessage) Revoke(int? consentId, AppUser user);

        (bool Status, string ErrorMessage) UpdateAgreement(bool forAgree, int? consentId, AppUser user);
    }
}