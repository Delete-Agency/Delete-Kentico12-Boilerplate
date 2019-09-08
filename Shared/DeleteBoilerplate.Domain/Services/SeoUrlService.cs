using System.Collections.Generic;
using System.Linq;
using CMS.DocumentEngine;
using DeleteBoilerplate.Domain.Extensions;
using DeleteBoilerplate.Domain.Helpers;

namespace DeleteBoilerplate.Domain.Services
{
    public interface ISeoUrlService
    {
        IList<TreeNode> GetAllDocumentsBySeoUrl(string seoUrl);
    }

    public class SeoUrlService : ISeoUrlService
    {
        public IList<TreeNode> GetAllDocumentsBySeoUrl(string seoUrl)
        {
            var columns = new[] { Constants.DynamicRouting.SeoUrlFieldName, "DocumentID", "DocumentNamePath", "DocumentCulture" };

            var publishedDocuments = RoutingQueryHelper.GetNodeBySeoUrlQuery(seoUrl, columns)
                .AddVersionsParameters(false)
                .ToList();

            var unpublishedDocuments = RoutingQueryHelper.GetNodeBySeoUrlQuery(seoUrl, columns)
                .AddVersionsParameters(true)
                .ToList();

            return publishedDocuments.Union(unpublishedDocuments).ToList();
        }
    }
}