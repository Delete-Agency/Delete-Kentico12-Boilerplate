using System.Web.Mvc;
using System.Web.Routing;

namespace DeleteBoilerplate.CompanyMembers
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "Company Member Index",
                url: "team/{team}/{personalIdentifier}",
                defaults: new { controller = "CompanyMembers", action = "Index" }
            );

            routes.MapRoute(
                name: "Company Member Team",
                url: "team/{team}",
                defaults: new { controller = "CompanyMembers", action = "Team" }
            );
        }
    }
}
