using System.Collections.Generic;
using DeleteBoilerplate.AzureSearch.Models.CompanyMember;
using DeleteBoilerplate.AzureSearch.Services.Base;
using DeleteBoilerplate.Common.Extensions;
using DeleteBoilerplate.Domain;

namespace DeleteBoilerplate.AzureSearch.Services.CompanyMember
{
    public interface ICompanyMemberAzureSearchService : IAzureSearchService<CompanyMemberAzureSearchResultItem, CompanyMemberAzureSearchArgs>
    {
    }

    public class CompanyMemberAzureSearchService : BaseAzureSearchService<CompanyMemberAzureSearchResultItem, CompanyMemberAzureSearchArgs>, ICompanyMemberAzureSearchService
    {
        public CompanyMemberAzureSearchService() : base(Constants.SearchIndexes.CompanyMember)
        {
        }

        protected override IList<string> GetFilterQueryInternal(CompanyMemberAzureSearchArgs args)
        {
            var queries = new List<string>();

            if (!args.Team.IsNullOrWhiteSpace())
                queries.Add($"team eq '{args.Team}'");

            return queries;
        }

        protected override IList<string> GetOrderQuery(CompanyMemberAzureSearchArgs args)
        {
            var queries = new List<string>();
            return queries;
        }
    }
}
