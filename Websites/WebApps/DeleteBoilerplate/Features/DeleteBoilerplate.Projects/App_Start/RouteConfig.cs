using System.Web.Mvc;
using System.Web.Routing;
using DeleteBoilerplate.Projects.Constants;

namespace DeleteBoilerplate.Projects
{
    public class RouteConfig
    {
        private static RouteCollection Routes { get; set; }

        public static void RegisterRoutes(RouteCollection routes)
        {
            Routes = routes;

            MapRouteProjects();
        }

        private static void MapRouteProjects()
        {
            Routes.MapRoute(
                name: RouteNames.Project.SearchByYear,
                url: "projectSearch/{year}",
                defaults: new { controller = "Projects", action = "Search" }
            );

            Routes.MapRoute(
                name: RouteNames.Project.SearchByArea,
                url: "projectSearch/area",
                defaults: new { controller = "Projects", action = "SearchByArea" }
            );
        }
    }
}
