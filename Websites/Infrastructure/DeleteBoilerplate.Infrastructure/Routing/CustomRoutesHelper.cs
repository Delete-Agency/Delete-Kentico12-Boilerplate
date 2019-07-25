using System.Web.Routing;
using DeleteBoilerplate.Infrastructure.Helpers;

namespace DeleteBoilerplate.Infrastructure.Routing
{
    public class CustomRoutesHelper
    {
        public static void RegisterFeaturesRoutes(RouteCollection routes)
        {
            var assemblies = AssemblyHelper.GetDiscoverableAsseblyAssemblies();
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
