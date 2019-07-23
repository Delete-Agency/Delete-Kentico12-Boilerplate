using System.Web;
using System.Web.Optimization;

namespace DeleteBoilerplate.WebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Custom JavaScript files from the ~/Scripts/ directory can be included as well
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/Scripts/site.js"));

            // Custom CSS files from the ~/Content/ directory can be included as well
            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/site.css"));
        }
    }
}