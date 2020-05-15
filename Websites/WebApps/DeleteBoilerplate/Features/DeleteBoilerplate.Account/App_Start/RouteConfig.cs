using System.Web.Mvc;
using System.Web.Routing;
using DeleteBoilerplate.Account.Constants;

namespace DeleteBoilerplate.Account
{
    public class RouteConfig
    {
        private static RouteCollection Routes { get; set; }

        public static void RegisterRoutes(RouteCollection routes)
        {
            Routes = routes;

            MapRouteAccount();
        }

        private static void MapRouteAccount()
        {
            Routes.MapRoute(
                name: RouteNames.Account.Login,
                url: "api/account/login",
                defaults: new { controller = "Account", action = "Login" });

            Routes.MapRoute(
                name: RouteNames.Account.Register,
                url: "api/account/register",
                defaults: new { controller = "Account", action = "Register" });

            Routes.MapRoute(
                name: RouteNames.Account.ResetPassword,
                url: "api/account/resetPassword",
                defaults: new {controller = "Account", action = "resetPassword" });

            Routes.MapRoute(
                name: RouteNames.Account.Logout,
                url: "api/account/logout",
                defaults: new { controller = "Account", action = "Logout" });
        }
    }
}