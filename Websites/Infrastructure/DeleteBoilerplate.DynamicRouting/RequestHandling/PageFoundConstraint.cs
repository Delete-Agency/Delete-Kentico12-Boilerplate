using System;
using System.Linq;
using System.Web;
using System.Web.Routing;
using CMS.DocumentEngine;
using CMS.Helpers;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.Domain.Extensions;
using DeleteBoilerplate.Domain.Helpers;
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
                var url = $"/{ValidationHelper.GetString(values[Constants.DynamicRouting.RoutingUrlParameter], String.Empty)}";

                // Get the classname based on the URL
                var foundNode = RoutingQueryHelper
                    .GetNodeBySeoUrlQuery(url)
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