using System.Linq;
using System.Web;
using System.Web.Routing;
using CMS.DocumentEngine;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.Domain.Extensions;
using DeleteBoilerplate.Domain.Helpers;
using DeleteBoilerplate.DynamicRouting.Helpers;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

namespace DeleteBoilerplate.DynamicRouting.RequestHandling
{
    public class PageFoundConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase context, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (!context.Items.Contains(Constants.DynamicRouting.ContextItemDocumentId))
            {
                // Get the classname based on the URL
                var foundNode = RoutingQueryHelper
                    .GetNodeBySeoUrlQuery(EnvironmentHelper.GetUrl(context.Request))
                    .TopN(1)
                    .AddVersionsParameters(context.Kentico().Preview().Enabled)
                    .FirstOrDefault();

                context.Items.Add(Constants.DynamicRouting.ContextItemDocumentId, foundNode?.DocumentID);
                context.Items.Add(Constants.DynamicRouting.ContextItemClassName, foundNode?.ClassName);

                return foundNode != default(TreeNode);
            }

            return context.Items[Constants.DynamicRouting.ContextItemDocumentId] != null;
        }
    }
}