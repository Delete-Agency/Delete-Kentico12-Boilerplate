using System.Web.Mvc;
using System.Web.Routing;
using DeleteBoilerplate.WebApp.Constants;

namespace DeleteBoilerplate.WebApp
{
    public class RouteConfig
    {
        private static RouteCollection Routes { get; set; }

        public static void RegisterRoutes(RouteCollection routes)
        {
            Routes = routes;

            MapRouteCookies();
        }

        private static void MapRouteCookies()
        {
            Routes.MapRoute(
                name: RouteNames.Cookies.Index,
                url: "cookies",
                defaults: new { controller = "Cookies", action = "Cookies" });

            Routes.MapRoute(
                name: RouteNames.Cookies.AcceptAllCookies,
                url: "api/cookies/accept",
                defaults: new { controller = "Cookies", action = "AcceptAllCookies" });

            Routes.MapRoute(
                name: RouteNames.Cookies.SetCookieLevel,
                url: "api/cookies/set",
                defaults: new { controller = "Cookies", action = "SetCookieLevel" });
        }
    }
}