using System.Linq;
using System.Web;
using System.Web.Routing;
using CMS.DocumentEngine;
using DeleteBoilerplate.DynamicRouting.Extensions;
using DeleteBoilerplate.DynamicRouting.Helpers;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

namespace DeleteBoilerplate.DynamicRouting.RequestHandling
{
    public class PageFoundConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase context, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (!context.Items.Contains("ContextItemDocumentId"))
            {
                // Get the classname based on the URL
                var foundNode = RoutingQueryHelper
                    .GetNodeBySeoUrlQuery(EnvironmentHelper.GetUrl(context.Request))
                    .AddVersionsParameters(context.Kentico().Preview().Enabled)
                    .FirstOrDefault();

                context.Items.Add("ContextItemDocumentId", foundNode?.DocumentID);
                context.Items.Add("ContextItemClassName", foundNode?.ClassName);

                return foundNode != default(TreeNode);
            }

            return context.Items["ContextItemDocumentId"] != null;
        }
    }
}