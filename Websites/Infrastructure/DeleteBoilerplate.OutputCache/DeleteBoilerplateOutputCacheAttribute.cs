using System.Web.Mvc;

namespace DeleteBoilerplate.OutputCache
{
    public class DeleteBoilerplateOutputCacheAttribute : OutputCacheAttribute
    {
        public DeleteBoilerplateOutputCacheAttribute()
        {
            VaryByCustom = OutputCacheConsts.VarByCustom.Default;
        }
    }
}
