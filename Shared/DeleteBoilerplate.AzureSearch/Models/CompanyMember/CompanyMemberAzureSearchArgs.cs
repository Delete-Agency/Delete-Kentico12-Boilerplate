using DeleteBoilerplate.AzureSearch.Models.Base;

namespace DeleteBoilerplate.AzureSearch.Models.CompanyMember
{
    public class CompanyMemberAzureSearchArgs : BaseAzureSearchArgs
    {
        public string Team { get; set; }
    }
}
