using System.Linq;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface ICompanyMemberRepository : IRepository<CompanyMemberInfo>
    {
        CompanyMemberInfo GetByPersonalIdentifier(string personalIdentifier);
    }

    public class CompanyMemberRepository : ICompanyMemberRepository
    {
        public CompanyMemberInfo GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public CompanyMemberInfo GetByPersonalIdentifier(string personalIdentifier)
        {
            return CompanyMemberInfoProvider
                .GetCompanyMembers()
                .WhereEquals(nameof(CompanyMemberInfo.PersonalIdentifier), personalIdentifier)
                .TopN(1)
                .FirstOrDefault();
        }
    }
}
