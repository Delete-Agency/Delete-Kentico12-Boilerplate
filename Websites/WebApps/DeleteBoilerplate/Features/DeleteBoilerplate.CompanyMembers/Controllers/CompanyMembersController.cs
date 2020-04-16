using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DeleteBoilerplate.AzureSearch.Models.CompanyMember;
using DeleteBoilerplate.AzureSearch.Services.CompanyMember;
using DeleteBoilerplate.CompanyMembers.Models;
using DeleteBoilerplate.Domain.Repositories;
using DeleteBoilerplate.DynamicRouting.Controllers;
using LightInject;

namespace DeleteBoilerplate.CompanyMembers.Controllers
{
    public class CompanyMembersController : BaseCustomObjectController
    {
        [Inject]
        protected ICompanyMemberRepository CompanyMemberRepository { get; set; }

        [Inject]
        protected ICompanyMemberAzureSearchService CompanyMemberAzureSearchService { get; set; }

        public ActionResult Index(string personalIdentifier)
        {
            var companyMember = this.CompanyMemberRepository.GetByPersonalIdentifier(personalIdentifier);
            var model = this.Mapper.Map<CompanyMemberViewModel>(companyMember);

            return View(model);
        }

        public ActionResult Team(string team)
        {
            var args = new CompanyMemberAzureSearchArgs
            {
                Page = 1,
                PageSize = 25,
                Team = team
            };

            IList<CompanyMemberViewModel> companyMembers = null;

            var companyMembersSearchResult = this.CompanyMemberAzureSearchService.Find(args);

            if (companyMembersSearchResult?.Items?.Any() == true)
                companyMembers = this.Mapper.Map<IList<CompanyMemberViewModel>>(companyMembersSearchResult.Items);

            return View(companyMembers);
        }
    }
}