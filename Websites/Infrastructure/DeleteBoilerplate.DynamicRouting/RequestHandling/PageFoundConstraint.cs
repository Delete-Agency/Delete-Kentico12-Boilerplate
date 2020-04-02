using System;
using System.Linq;
using System.Web;
using System.Web.Routing;
using CMS.DocumentEngine;
using CMS.Helpers;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.Domain.Extensions;
using DeleteBoilerplate.Domain.Helpers;
using DeleteBoilerplate.Domain.Services;
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
                // Try to get the URL from the routing parameter
                var url = $"/{ValidationHelper.GetString(values[Constants.DynamicRouting.RoutingUrlParameter], String.Empty)}";

                // If the URL is empty - try to distinguish the URL from the request
                if (url.Equals("/", StringComparison.OrdinalIgnoreCase))
                {
                    url = EnvironmentHelper.GetUrl(context.Request);
                }

                if (Settings.PreviewEnabled == false)
                {
                    //Try to get classname based on the URL from SearchIndex
                    var foundNodeInIndex = SearchNodeInIndexService.SearchBySeoUrl(url);
                    if (foundNodeInIndex.IsSuccess)
                    {
                        context.Items.Add(Constants.DynamicRouting.ContextItemDocumentId, foundNodeInIndex.DocumentId);
                        context.Items.Add(Constants.DynamicRouting.ContextItemClassName, foundNodeInIndex.ClassName);
                        return true;
                    }
                }

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