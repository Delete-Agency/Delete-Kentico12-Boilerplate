using CMS.Core;
using CMS.DataProtection;
using LightInject;
using DeleteBoilerplate.Account.Services;

namespace DeleteBoilerplate.Account.DI
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterScoped<IAuthService, AuthService>();
            //serviceRegistry.RegisterScoped<IConsentService, ConsentService>();
            //serviceRegistry.Register(factory => Service.Resolve<IConsentAgreementService>());
        }
    }
}