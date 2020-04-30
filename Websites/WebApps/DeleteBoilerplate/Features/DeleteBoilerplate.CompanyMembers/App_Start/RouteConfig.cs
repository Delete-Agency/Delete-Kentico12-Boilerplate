using System.Web.Mvc;
using System.Web.Routing;
using DeleteBoilerplate.CompanyMembers.Constants;

namespace DeleteBoilerplate.CompanyMembers
{
    public class RouteConfig
    {
        private static RouteCollection Routes { get; set; }

        public static void RegisterRoutes(RouteCollection routes)
        {
            Routes = routes;

            MapRouteCompanyMembers();
        }

        private static void MapRouteCompanyMembers()
        {
            Routes.MapRoute(
                name: RouteNames.CompanyMembers.Index,
                url: "team/{team}/{personalIdentifier}",
                defaults: new { controller = "CompanyMembers", action = "Index" }
            );

            Routes.MapRoute(
                name: RouteNames.CompanyMembers.Team,
                url: "team/{team}",
                defaults: new { controller = "CompanyMembers", action = "Team" }
            );
        }
    }
}
