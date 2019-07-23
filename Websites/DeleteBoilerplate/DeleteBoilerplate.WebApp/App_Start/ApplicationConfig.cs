using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;

namespace DeleteBoilerplate.WebApp
{
    public class ApplicationConfig
    {
        public static void RegisterFeatures(IApplicationBuilder builder)
        {
            // Enable required Kentico features

            // Uncomment the following to use the Page builder feature
            //builder.UsePreview();
            //builder.UsePageBuilder();
        }
    }
}