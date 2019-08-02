using System.Web.Mvc;
using System.Web.Routing;
using DeleteBoilerplate.Infrastructure.Helpers;
using Kentico.Web.Mvc;

namespace DeleteBoilerplate.Infrastructure.Routing
{
    public class CustomRoutesHelper
    {
        public static void RegisterFeaturesRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Maps routes to Kentico HTTP handlers and features enabled in ApplicationConfig.cs
            // Always map the Kentico routes before adding other routes. Issues may occur if Kentico URLs are matched by a general route, for example images might not be displayed on pages
            routes.Kentico().MapRoutes();

            var assemblies = AssemblyHelper.GetDiscoverableAssemblyAssemblies();
            foreach (var assembly in assemblies)
            {
                var rcType = assembly.GetType($"{assembly.GetName().Name}.RouteConfig");
                if (rcType == null) continue;
                var registerRoutesMethod = rcType.GetMethod("RegisterRoutes");
                if (registerRoutesMethod == null) continue;
                registerRoutesMethod.Invoke(null, new object[] { routes });
            }
        }
    }
}
