using System.Web.Mvc;
using System.Web.Routing;
using DeleteBoilerplate.DynamicRouting.RequestHandling;

namespace DeleteBoilerplate.DynamicRouting
{
    public class RouteConfig
    {
        public static void RegisterDynamicRoutes(RouteCollection routes)
        {
            // If the Page is found by URL, will handle the routing dynamically
            var route = routes.MapRoute(
                name: "CheckByUrl",
                url: "{*url}",
                // Defaults are if it can't find a controller based on the pages
                defaults: new {defaultcontroller = "HttpErrors", defaultaction = "Index"},
                constraints: new {PageFound = new PageFoundConstraint()}
            );
            route.RouteHandler = new DynamicRouteHandler();
        }
    }
}