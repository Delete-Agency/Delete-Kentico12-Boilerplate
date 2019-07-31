using Kentico.Web.Mvc;

namespace DeleteBoilerplate.OutputCache
{
    public static class OutputCacheKeyOptionsExtensions
    {
        public static IOutputCacheKeyOptions VarByAssetsCookie(this IOutputCacheKeyOptions options)
        {
            options.AddCacheKey(new AssetsOutputCacheKey());
            return options;
        }
    }
}