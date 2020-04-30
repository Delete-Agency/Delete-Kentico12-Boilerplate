using System.Web.Routing;

namespace DeleteBoilerplate.Forms
{
    public class RouteConfig
    {
        private static RouteCollection Routes { get; set; }

        public static void RegisterRoutes(RouteCollection routes)
        {
            Routes = routes;
        }
    }
}
