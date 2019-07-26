using System.Linq;
using System.Web;
using System.Web.Routing;
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

            var foundDoc = DocumentQueryHelper.GetNodeByAliasPathOrSeoUrlQuery(url).FirstOrDefault();

            return (foundDoc != null && (foundDoc.NodeAliasPath != "/" || !IgnoreRootPage));
        }
    }
}