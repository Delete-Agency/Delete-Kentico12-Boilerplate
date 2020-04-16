using DeleteBoilerplate.AzureSearch.Services.CompanyMember;
using LightInject;

namespace DeleteBoilerplate.AzureSearch.DI
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterScoped<ICompanyMemberAzureSearchService, CompanyMemberAzureSearchService>();
        }
    }
}
