using CMS.DataEngine;
using CMS.OnlineForms;
using CMS.SiteProvider;
using DeleteBoilerplate.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface IBizFormRepository
    {
        IList<BizFormInfo> GetSiteForms();

        IList<OptionModel> GetSiteFormNames();

        BizFormInfo GetFormInfo(string formName);

        ObjectQuery<BizFormItem> GetQueryFormItem(int formClassId);
    }

    public class BizFormRepository : IBizFormRepository
    {
        public IList<BizFormInfo> GetSiteForms()
        {
            var siteForms = BizFormInfoProvider
               .GetBizForms()
               .Columns("FormSiteID", "FormName", "FormDisplayName", "FormDevelopmentModel")
               .WhereEquals("FormDevelopmentModel", 1)
               .WhereEquals("FormSiteID", SiteContext.CurrentSiteID)
               .ToList();

            return siteForms;
        }

        public IList<OptionModel> GetSiteFormNames()
        {
            var siteFormNames = this.GetSiteForms()
                .Select(x => new OptionModel
                {
                    Name = x.FormDisplayName,
                    Value = x.FormName
                }).ToList();

            return siteFormNames;
        }

        public BizFormInfo GetFormInfo(string formName)
        {
            var formInfo = BizFormInfoProvider.GetBizFormInfo(formName, SiteContext.CurrentSiteID);
            return formInfo;
        }

        public ObjectQuery<BizFormItem> GetQueryFormItem(int formClassId)
        {
            var formClassName = DataClassInfoProvider.GetClassName(formClassId);

            var queryFormItem = BizFormItemProvider.GetItems(formClassName);
            return queryFormItem;
        }
    }
}
