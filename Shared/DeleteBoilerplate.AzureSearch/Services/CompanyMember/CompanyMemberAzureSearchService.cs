using System.Collections.Generic;
using DeleteBoilerplate.AzureSearch.Models.CompanyMember;
using DeleteBoilerplate.AzureSearch.Services.Base;

namespace DeleteBoilerplate.AzureSearch.Services.CompanyMember
{
    public interface ICompanyMemberAzureSearchService : IAzureSearchService<CompanyMemberAzureSearchResultItem, CompanyMemberAzureSearchArgs>
    {
    }

    public class CompanyMemberAzureSearchService : BaseAzureSearchService<CompanyMemberAzureSearchResultItem, CompanyMemberAzureSearchArgs>, ICompanyMemberAzureSearchService
    {
        // TODO place valid index name
        public CompanyMemberAzureSearchService() : base("")
        {
        }

        protected override IList<string> GetFilterQueryInternal(CompanyMemberAzureSearchArgs args)
        {
            throw new System.NotImplementedException();
        }

        protected override IList<string> GetOrderQuery(CompanyMemberAzureSearchArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
}
