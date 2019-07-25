using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using DeleteBoilerplate.DynamicRouting;
using DeleteBoilerplate.DynamicRouting.Attributes;
using Kentico.Web.Mvc;

namespace DeleteBoilerplate.WebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Maps routes to Kentico HTTP handlers and features enabled in ApplicationConfig.cs
            // Always map the Kentico routes before adding other routes. Issues may occur if Kentico URLs are matched by a general route, for example images might not be displayed on pages
            routes.Kentico().MapRoutes();

            DynamicRouting.RouteConfig.RegisterRoutes(routes, PageTypeRoutingConfig.RoutingDictionary);

            RegisterFeaturesRoutes(routes);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
        
        private static void RegisterFeaturesRoutes(RouteCollection routes)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x =>
                x.CustomAttributes.Any(ca => ca.AttributeType == typeof(CustomRoutesAttribute)));
            foreach (var assembly in assemblies)
            {
                var rcType = assembly.GetType($"{assembly.GetName().Name}.RouteConfig");
                if (rcType==null) continue;
                var registerRoutesMethod = rcType.GetMethod("RegisterRoutes");
                if (registerRoutesMethod == null) continue;
                registerRoutesMethod.Invoke(null, new object[] {routes});
            }
        }
    }
}
