using System.Web.Mvc;
using System.Web.Routing;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.DynamicRouting.Helpers;
using DeleteBoilerplate.DynamicRouting.RequestHandling;

namespace DeleteBoilerplate.DynamicRouting
{
    public class DynamicRouteConfig
    {
        public static void RegisterDynamicRoutes(RouteCollection routes)
        {
            PageTypeRoutingHelper.Initialize();

            // If the Page is found by URL, will handle the routing dynamically
            var route = routes.MapRoute(
                name: "CheckByUrl",
                url: $"{{*{Constants.DynamicRouting.RoutingUrlParameter}}}",
                // Defaults are if it can't find a controller based on the pages
                defaults: new {defaultcontroller = "HttpErrors", defaultaction = "Index"},
                constraints: new {PageFound = new PageFoundConstraint()}
            );
            route.RouteHandler = new DynamicRouteHandler();
        }
    }
}