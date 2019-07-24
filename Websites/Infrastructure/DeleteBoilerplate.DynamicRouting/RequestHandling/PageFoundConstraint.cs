using System.Linq;
using System.Web;
using System.Web.Routing;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.DynamicRouting.Helpers;

namespace DeleteBoilerplate.DynamicRouting.RequestHandling
{
    public class PageFoundConstraint : IRouteConstraint
    {
        public PageFoundConstraint(bool ignoreRootPage = true)
        {
            this.IgnoreRootPage = ignoreRootPage;
        }

        public bool IgnoreRootPage;

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var url = EnvironmentHelper.GetUrl(httpContext.Request);
            
            //TODO move it to DocumentQueryHelper, use cache implementation and fix query
            // If no document found anyway, then it will always be a match
            var foundDoc = DocumentHelper.GetDocuments(BasePage.CLASS_NAME)
                .WhereEquals("SeoUrl", url)
                .Or()
                .WhereEquals("NodeAliasPath", url)
                .TopN(1)
                .FirstOrDefault();

            return (foundDoc != null && (foundDoc.NodeAliasPath != "/" || !IgnoreRootPage));
        }
    }
}