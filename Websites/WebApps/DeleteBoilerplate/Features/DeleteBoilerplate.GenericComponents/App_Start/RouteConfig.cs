using DeleteBoilerplate.GenericComponents.Constants;
using System.Web.Mvc;
using System.Web.Routing;

namespace DeleteBoilerplate.GenericComponents
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: RouteNames.GetTweets,
                url: "api/tweet/get",
                defaults: new { controller = "TwitterWidget", action = "GetTweets" });
        }
    }
}