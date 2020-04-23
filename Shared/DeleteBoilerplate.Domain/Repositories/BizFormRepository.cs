using CMS.OnlineForms;
using CMS.SiteProvider;
using DeleteBoilerplate.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface IBizFormRepository
    {
        IList<OptionModel> GetSiteFormNames();

        IList<BizFormInfo> GetSiteForms();
    }

    public class BizFormRepository : IBizFormRepository
    {
        public IList<BizFormInfo> GetSiteForms()
        {
            return BizFormInfoProvider
                .GetBizForms()
                .Columns("FormSiteID", "FormName", "FormDisplayName", "FormDevelopmentModel")
                .WhereEquals("FormDevelopmentModel", 1)
                .WhereEquals("FormSiteID", SiteContext.CurrentSiteID)
                .ToList();
        }

        public IList<OptionModel> GetSiteFormNames()
        {
            return this.GetSiteForms()
                .Select(x => new OptionModel{
                    Name = x.FormDisplayName,
                    Value = x.FormName
                }).ToList();
        }
    }
}
