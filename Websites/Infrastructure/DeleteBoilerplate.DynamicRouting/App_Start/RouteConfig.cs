using System.Web.Mvc;
using System.Web.Routing;
using DeleteBoilerplate.DynamicRouting.RequestHandling;

namespace DeleteBoilerplate.DynamicRouting
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // If a normal MVC Route is found and it has priority, it will take it, otherwise it will bypass.
            var route = routes.MapRoute(
                name: "DefaultIfPriority",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { ControlIsPriority = new RouteOverPathPriorityConstraint() }
            );

            // If the Page is found, will handle the routing dynamically
            route = routes.MapRoute(
                name: "CheckByUrl",
                url: "{*url}",
                // Defaults are if it can't find a controller based on the pages
                defaults: new { defaultcontroller = "HttpErrors", defaultaction = "Index" },
                constraints: new { PageFound = new PageFoundConstraint(true) }
            );
            route.RouteHandler = new DynamicRouteHandler();
        }
    }
}