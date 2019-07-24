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
            // If no document found anyway, then it will always be a match
            var foundDoc = DocumentQueryHelper.GetNodeByAliasPath(EnvironmentHelper.GetUrl(httpContext.Request));
            return (foundDoc != null && (foundDoc.NodeAliasPath != "/" || !IgnoreRootPage));
        }
    }
}