using System.Web.Routing;
using DeleteBoilerplate.DynamicRouting.Config;
using DeleteBoilerplate.Infrastructure;
using Kentico.Web.Mvc;

namespace DeleteBoilerplate.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Enables and configures selected Kentico ASP.NET MVC integration features
            ApplicationConfig.RegisterFeatures(ApplicationBuilder.Current);

            DIConfig.Bootstrap();

            PageTypeRoutingConfig.Initialize();

            // Registers routes including system routes for enabled features
            RouteConfig.RegisterRoutes(RouteTable.Routes);

        }

    }
}
