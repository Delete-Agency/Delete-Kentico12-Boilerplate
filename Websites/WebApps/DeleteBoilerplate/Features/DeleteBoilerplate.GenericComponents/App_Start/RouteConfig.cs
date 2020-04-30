using DeleteBoilerplate.GenericComponents.Constants;
using System.Web.Mvc;
using System.Web.Routing;

namespace DeleteBoilerplate.GenericComponents
{
    public class RouteConfig
    {
        private static RouteCollection Routes { get; set; }

        public static void RegisterRoutes(RouteCollection routes)
        {
            Routes = routes;

            MapRouteTwitterWidget();

            MapRouteBizForm();

            MapRouteContactForm();
        }

        private static void MapRouteTwitterWidget()
        {
            Routes.MapRoute(
                name: RouteNames.Twitter.GetTweets,
                url: "api/tweet/get",
                defaults: new { controller = "TwitterWidget", action = "GetTweets" });
        }

        private static void MapRouteBizForm()
        {
            Routes.MapRoute(
                name: RouteNames.BizForm.Submit,
                url: "api/bizForm/submit",
                defaults: new { controller = "BizForm", action = "Submit" });
        }

        private static void MapRouteContactForm()
        {
            Routes.MapRoute(
                name: RouteNames.ContactForm.Submit,
                url: "api/contactForm/submit",
                defaults: new { controller = "ContactForm", action = "Submit" });
        }
    }
}