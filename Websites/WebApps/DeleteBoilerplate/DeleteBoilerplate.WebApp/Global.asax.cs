using System;
using System.Web;
using System.Web.Routing;
using DeleteBoilerplate.DynamicRouting;
using DeleteBoilerplate.DynamicRouting.Helpers;
using DeleteBoilerplate.Infrastructure;
using DeleteBoilerplate.Infrastructure.Configuration;
using DeleteBoilerplate.Infrastructure.Routing;
using DeleteBoilerplate.OutputCache;
using Kentico.OnlineMarketing.Web.Mvc;
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

            // Registers routes including system routes for enabled features
            CustomRoutesHelper.RegisterFeaturesRoutes(RouteTable.Routes);

            // Register dynamic routes at the end
            DynamicRouteConfig.RegisterDynamicRoutes(RouteTable.Routes);

            //DoTo: Customization of Kentico forms is turned off
            //CustomizationKenticoForm();
        }

        private static void CustomizationKenticoForm()
        {
            KenticoFormMarkupInjection.RegisterEventHandlers();
            KenticoFormBuilderStaticMarkupConfiguration.SetGlobalRenderingConfigurations();
        }

        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            var options = OutputCacheKeyHelper.CreateOptions().VarByAssetsCookie();

            switch (arg)
            {
                case OutputCacheConsts.VarByCustom.Default:
                    //options.VaryByHost().VaryByBrowser().VaryByUser();
                    break;

                case OutputCacheConsts.VarByCustom.OnlineMarketing:
                    options
                        .VaryByCookieLevel()
                        .VaryByPersona()
                        .VaryByABTestVariant();
                    break;
            }

            string cacheKey = OutputCacheKeyHelper.GetVaryByCustomString(context, arg, options);

            if (!String.IsNullOrEmpty(cacheKey))
            {
                return cacheKey;
            }

            // Calls the base implementation if the provided custom string does not match any predefined configurations
            return base.GetVaryByCustomString(context, arg);
        }

    }
}
