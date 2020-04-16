using DeleteBoilerplate.AzureSearch.Models.Base;

namespace DeleteBoilerplate.AzureSearch.Models.CompanyMember
{
    public class CompanyMemberAzureSearchResultItem : IAzureSearchResultItem
    {            
        public string FullName { get; set; }

        public string PersonalIdentifier { get; set; }

        public string Team { get; set; }
    }
}
