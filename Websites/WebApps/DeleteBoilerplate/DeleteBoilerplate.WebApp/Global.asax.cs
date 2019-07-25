using System.Web.Optimization;
using System.Web.Routing;
using Kentico.Web.Mvc;

namespace DeleteBoilerplate.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Enables and configures selected Kentico ASP.NET MVC integration features
            ApplicationConfig.RegisterFeatures(ApplicationBuilder.Current);

            PageTypeRoutingConfig.CollectRoutingDefinitions();

            // Registers routes including system routes for enabled features
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //ToDo: will we really use bundles?
            // Registers enabled bundles
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DIConfig.Bootstrap();
        }
    }
}
